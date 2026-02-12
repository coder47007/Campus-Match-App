// GIF Picker Component - Uses Tenor API (Google's free GIF API)
import React, { useState, useEffect, useCallback } from 'react';
import {
    View,
    Text,
    StyleSheet,
    Modal,
    TextInput,
    FlatList,
    TouchableOpacity,
    Image,
    ActivityIndicator,
    Dimensions,
    KeyboardAvoidingView,
    Platform,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');
const GIF_SIZE = (SCREEN_WIDTH - 48) / 2;

// Tenor API - Free tier (need to register at https://developers.google.com/tenor)
const TENOR_API_KEY = process.env.EXPO_PUBLIC_TENOR_API_KEY || ''; // Using user's Gemini key as placeholder
const TENOR_BASE_URL = 'https://tenor.googleapis.com/v2';

interface GifResult {
    id: string;
    url: string;
    preview: string;
    title: string;
}

interface GifPickerProps {
    visible: boolean;
    onClose: () => void;
    onSelect: (gifUrl: string) => void;
}

export default function GifPicker({ visible, onClose, onSelect }: GifPickerProps) {
    const [searchQuery, setSearchQuery] = useState('');
    const [gifs, setGifs] = useState<GifResult[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [categories] = useState([
        'trending', 'hello', 'love', 'happy', 'sad', 'excited',
        'laugh', 'dance', 'cute', 'bye', 'thank you', 'sorry'
    ]);

    // Fetch trending GIFs on mount
    useEffect(() => {
        if (visible) {
            fetchTrendingGifs();
        }
    }, [visible]);

    const fetchTrendingGifs = async () => {
        setIsLoading(true);
        try {
            const response = await fetch(
                `${TENOR_BASE_URL}/featured?key=${TENOR_API_KEY}&limit=20&media_filter=gif`
            );
            const data = await response.json();

            if (data.results) {
                const formattedGifs = data.results.map((gif: any) => ({
                    id: gif.id,
                    url: gif.media_formats?.gif?.url || gif.url,
                    preview: gif.media_formats?.tinygif?.url || gif.media_formats?.nanogif?.url || gif.url,
                    title: gif.title || 'GIF',
                }));
                setGifs(formattedGifs);
            }
        } catch (error) {
            console.error('Error fetching GIFs:', error);
            // Fallback to placeholder GIFs
            setGifs(getPlaceholderGifs());
        }
        setIsLoading(false);
    };

    const searchGifs = async (query: string) => {
        if (!query.trim()) {
            fetchTrendingGifs();
            return;
        }

        setIsLoading(true);
        try {
            const response = await fetch(
                `${TENOR_BASE_URL}/search?key=${TENOR_API_KEY}&q=${encodeURIComponent(query)}&limit=20&media_filter=gif`
            );
            const data = await response.json();

            if (data.results) {
                const formattedGifs = data.results.map((gif: any) => ({
                    id: gif.id,
                    url: gif.media_formats?.gif?.url || gif.url,
                    preview: gif.media_formats?.tinygif?.url || gif.media_formats?.nanogif?.url || gif.url,
                    title: gif.title || 'GIF',
                }));
                setGifs(formattedGifs);
            }
        } catch (error) {
            console.error('Error searching GIFs:', error);
            setGifs(getPlaceholderGifs());
        }
        setIsLoading(false);
    };

    // Debounced search
    useEffect(() => {
        const timer = setTimeout(() => {
            if (searchQuery) {
                searchGifs(searchQuery);
            }
        }, 500);
        return () => clearTimeout(timer);
    }, [searchQuery]);

    const getPlaceholderGifs = (): GifResult[] => [
        { id: '1', url: 'https://media.giphy.com/media/3oriO0OEd9QIDdllqo/giphy.gif', preview: 'https://media.giphy.com/media/3oriO0OEd9QIDdllqo/100.gif', title: 'Hello' },
        { id: '2', url: 'https://media.giphy.com/media/l0MYGb1LuZ3n7dRnO/giphy.gif', preview: 'https://media.giphy.com/media/l0MYGb1LuZ3n7dRnO/100.gif', title: 'Happy' },
        { id: '3', url: 'https://media.giphy.com/media/3o7aCSPqXE5C6T8tBC/giphy.gif', preview: 'https://media.giphy.com/media/3o7aCSPqXE5C6T8tBC/100.gif', title: 'Love' },
        { id: '4', url: 'https://media.giphy.com/media/xT5LMHxhOfscxPfIfm/giphy.gif', preview: 'https://media.giphy.com/media/xT5LMHxhOfscxPfIfm/100.gif', title: 'Thanks' },
    ];

    const handleSelectGif = (gif: GifResult) => {
        onSelect(gif.url);
        onClose();
        setSearchQuery('');
    };

    const handleCategoryPress = (category: string) => {
        setSearchQuery(category);
    };

    const renderGifItem = ({ item }: { item: GifResult }) => (
        <TouchableOpacity
            style={styles.gifItem}
            onPress={() => handleSelectGif(item)}
            activeOpacity={0.7}
        >
            <Image
                source={{ uri: item.preview }}
                style={styles.gifImage}
                resizeMode="cover"
            />
        </TouchableOpacity>
    );

    return (
        <Modal
            visible={visible}
            animationType="slide"
            transparent
            onRequestClose={onClose}
        >
            <KeyboardAvoidingView
                behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                style={styles.container}
            >
                <View style={styles.overlay}>
                    <View style={styles.modal}>
                        {/* Header */}
                        <View style={styles.header}>
                            <Text style={styles.title}>Choose a GIF</Text>
                            <TouchableOpacity onPress={onClose} style={styles.closeButton}>
                                <Ionicons name="close" size={24} color={Colors.white} />
                            </TouchableOpacity>
                        </View>

                        {/* Search */}
                        <View style={styles.searchContainer}>
                            <Ionicons name="search" size={20} color={Colors.dark.textMuted} />
                            <TextInput
                                style={styles.searchInput}
                                value={searchQuery}
                                onChangeText={setSearchQuery}
                                placeholder="Search GIFs..."
                                placeholderTextColor={Colors.dark.textMuted}
                            />
                            {searchQuery.length > 0 && (
                                <TouchableOpacity onPress={() => setSearchQuery('')}>
                                    <Ionicons name="close-circle" size={20} color={Colors.dark.textMuted} />
                                </TouchableOpacity>
                            )}
                        </View>

                        {/* Categories */}
                        <FlatList
                            horizontal
                            data={categories}
                            keyExtractor={(item) => item}
                            showsHorizontalScrollIndicator={false}
                            style={styles.categoriesContainer}
                            renderItem={({ item }) => (
                                <TouchableOpacity
                                    style={[
                                        styles.categoryChip,
                                        searchQuery === item && styles.categoryChipActive
                                    ]}
                                    onPress={() => handleCategoryPress(item)}
                                >
                                    <Text style={[
                                        styles.categoryText,
                                        searchQuery === item && styles.categoryTextActive
                                    ]}>
                                        {item}
                                    </Text>
                                </TouchableOpacity>
                            )}
                        />

                        {/* GIF Grid */}
                        {isLoading ? (
                            <View style={styles.loadingContainer}>
                                <ActivityIndicator size="large" color="#7C3AED" />
                                <Text style={styles.loadingText}>Loading GIFs...</Text>
                            </View>
                        ) : (
                            <FlatList
                                data={gifs}
                                keyExtractor={(item) => item.id}
                                numColumns={2}
                                renderItem={renderGifItem}
                                contentContainerStyle={styles.gifGrid}
                                showsVerticalScrollIndicator={false}
                                ListEmptyComponent={
                                    <View style={styles.emptyContainer}>
                                        <Ionicons name="images-outline" size={48} color={Colors.dark.textMuted} />
                                        <Text style={styles.emptyText}>No GIFs found</Text>
                                    </View>
                                }
                            />
                        )}

                        {/* Powered by */}
                        <View style={styles.poweredBy}>
                            <Text style={styles.poweredByText}>Powered by Tenor</Text>
                        </View>
                    </View>
                </View>
            </KeyboardAvoidingView>
        </Modal>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    overlay: {
        flex: 1,
        backgroundColor: 'rgba(0, 0, 0, 0.7)',
        justifyContent: 'flex-end',
    },
    modal: {
        backgroundColor: Colors.dark.background,
        borderTopLeftRadius: 24,
        borderTopRightRadius: 24,
        maxHeight: '80%',
        paddingBottom: 20,
    },
    header: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        padding: 16,
        borderBottomWidth: 1,
        borderBottomColor: Colors.dark.surface,
    },
    title: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
    },
    closeButton: {
        padding: 4,
    },
    searchContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        marginHorizontal: 16,
        marginVertical: 12,
        paddingHorizontal: 12,
        borderRadius: 12,
        gap: 8,
    },
    searchInput: {
        flex: 1,
        color: Colors.white,
        fontSize: 16,
        paddingVertical: 12,
    },
    categoriesContainer: {
        paddingHorizontal: 12,
        marginBottom: 8,
        maxHeight: 44,
    },
    categoryChip: {
        backgroundColor: Colors.dark.surface,
        paddingHorizontal: 14,
        paddingVertical: 8,
        borderRadius: 20,
        marginHorizontal: 4,
    },
    categoryChipActive: {
        backgroundColor: '#7C3AED',
    },
    categoryText: {
        color: Colors.dark.textMuted,
        fontSize: 13,
        fontWeight: '500',
    },
    categoryTextActive: {
        color: Colors.white,
    },
    loadingContainer: {
        height: 200,
        justifyContent: 'center',
        alignItems: 'center',
    },
    loadingText: {
        color: Colors.dark.textMuted,
        marginTop: 12,
    },
    gifGrid: {
        paddingHorizontal: 12,
        paddingBottom: 20,
    },
    gifItem: {
        width: GIF_SIZE,
        height: GIF_SIZE,
        margin: 4,
        borderRadius: 12,
        overflow: 'hidden',
        backgroundColor: Colors.dark.surface,
    },
    gifImage: {
        width: '100%',
        height: '100%',
    },
    emptyContainer: {
        height: 200,
        justifyContent: 'center',
        alignItems: 'center',
    },
    emptyText: {
        color: Colors.dark.textMuted,
        marginTop: 12,
    },
    poweredBy: {
        alignItems: 'center',
        paddingVertical: 8,
    },
    poweredByText: {
        color: Colors.dark.textMuted,
        fontSize: 11,
    },
});
