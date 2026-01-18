import React, { useState, useMemo } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    TextInput,
    ScrollView,
    LayoutAnimation,
    Platform,
    UIManager,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';
import { interestCategories, searchInterests, InterestItem } from '@/data/interests';
import * as Haptics from 'expo-haptics';

if (Platform.OS === 'android' && UIManager.setLayoutAnimationEnabledExperimental) {
    UIManager.setLayoutAnimationEnabledExperimental(true);
}

interface InterestSelectorProps {
    selectedIds: string[];
    onToggle: (id: string, label: string, emoji: string) => void;
    maxSelection?: number;
}

export default function InterestSelector({
    selectedIds,
    onToggle,
    maxSelection = 5
}: InterestSelectorProps) {
    const [searchQuery, setSearchQuery] = useState('');
    const [customInterest, setCustomInterest] = useState('');
    const [isAddingCustom, setIsAddingCustom] = useState(false);

    // Filter results based on search
    const filteredCategories = useMemo(() => {
        if (!searchQuery) return interestCategories;

        const results = searchInterests(searchQuery);
        if (results.length === 0) return [];

        // Return a flat list wrapped in a single category for search results
        return [{
            id: 'search_results',
            title: 'Search Results',
            items: results
        }];
    }, [searchQuery]);

    const handleToggle = (item: InterestItem) => {
        // Haptic feedback
        Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);

        // Check if reached max limit when selecting (not unselecting)
        if (!selectedIds.includes(item.id) && selectedIds.length >= maxSelection) {
            // Shake animation or visual feedback could go here
            Haptics.notificationAsync(Haptics.NotificationFeedbackType.Warning);
            return;
        }

        onToggle(item.id, item.label, item.emoji);
    };

    const handleAddCustom = () => {
        if (customInterest.trim().length > 0) {
            const customId = `custom_${Date.now()}`;
            // Provide a default emoji for custom interests or let user pick (simplified here)
            onToggle(customId, customInterest.trim(), '✨');
            setCustomInterest('');
            setIsAddingCustom(false);
            Haptics.notificationAsync(Haptics.NotificationFeedbackType.Success);
        }
    };

    return (
        <View style={styles.container}>
            {/* Search Bar */}
            <View style={styles.searchContainer}>
                <Ionicons name="search" size={18} color="rgba(255,255,255,0.4)" style={styles.searchIcon} />
                <TextInput
                    style={styles.searchInput}
                    value={searchQuery}
                    onChangeText={(text) => {
                        LayoutAnimation.configureNext(LayoutAnimation.Presets.easeInEaseOut);
                        setSearchQuery(text);
                    }}
                    placeholder={`Search interests (e.g. 'Swimming')...`}
                    placeholderTextColor="rgba(255,255,255,0.4)"
                />
                {searchQuery.length > 0 && (
                    <TouchableOpacity onPress={() => setSearchQuery('')}>
                        <Ionicons name="close-circle" size={18} color="rgba(255,255,255,0.4)" />
                    </TouchableOpacity>
                )}
            </View>

            {/* Selection Count */}
            <View style={styles.counterContainer}>
                <Text style={styles.counterText}>
                    Selected: <Text style={styles.counterHighlight}>{selectedIds.length}/{maxSelection}</Text>
                </Text>
                {selectedIds.length >= maxSelection && (
                    <Text style={styles.maxReachedText}>Max reached!</Text>
                )}
            </View>

            {/* Interest Cloud */}
            <ScrollView
                style={styles.cloudScrollView}
                showsVerticalScrollIndicator={false}
                nestedScrollEnabled={true}
            >
                {filteredCategories.map((category) => (
                    <View key={category.id} style={styles.categoryContainer}>
                        {!searchQuery && (
                            <Text style={styles.categoryTitle}>{category.title}</Text>
                        )}
                        <View style={styles.chipsCloud}>
                            {category.items.map((item) => {
                                const isSelected = selectedIds.includes(item.id);
                                return (
                                    <TouchableOpacity
                                        key={item.id}
                                        onPress={() => handleToggle(item)}
                                        activeOpacity={0.7}
                                    >
                                        {isSelected ? (
                                            <LinearGradient
                                                colors={['#7C3AED', '#6D28D9']}
                                                style={[styles.chip, styles.chipSelected]}
                                            >
                                                <Text style={styles.chipEmoji}>{item.emoji}</Text>
                                                <Text style={styles.chipLabelSelected}>{item.label}</Text>
                                                <Ionicons name="close" size={14} color={Colors.white} style={styles.chipCloseIcon} />
                                            </LinearGradient>
                                        ) : (
                                            <View style={styles.chip}>
                                                <Text style={styles.chipEmoji}>{item.emoji}</Text>
                                                <Text style={styles.chipLabel}>{item.label}</Text>
                                            </View>
                                        )}
                                    </TouchableOpacity>
                                );
                            })}
                        </View>
                    </View>
                ))}

                {/* Add Custom Button */}
                {!searchQuery && (
                    <View style={styles.customSection}>
                        {isAddingCustom ? (
                            <View style={styles.addCustomContainer}>
                                <TextInput
                                    style={styles.customInput}
                                    value={customInterest}
                                    onChangeText={setCustomInterest}
                                    placeholder="Enter custom interest..."
                                    placeholderTextColor="rgba(255,255,255,0.4)"
                                    autoFocus
                                />
                                <TouchableOpacity
                                    style={styles.addCustomButtonConfirm}
                                    onPress={handleAddCustom}
                                >
                                    <Text style={styles.addCustomButtonText}>Add</Text>
                                </TouchableOpacity>
                                <TouchableOpacity
                                    onPress={() => setIsAddingCustom(false)}
                                    style={styles.cancelCustomButton}
                                >
                                    <Ionicons name="close" size={20} color="rgba(255,255,255,0.5)" />
                                </TouchableOpacity>
                            </View>
                        ) : (
                            <TouchableOpacity
                                style={styles.addCustomButton}
                                onPress={() => {
                                    LayoutAnimation.configureNext(LayoutAnimation.Presets.easeInEaseOut);
                                    setIsAddingCustom(true);
                                }}
                            >
                                <Ionicons name="add" size={18} color="#7C3AED" />
                                <Text style={styles.addCustomText}>Add Custom Interest</Text>
                            </TouchableOpacity>
                        )}
                    </View>
                )}

                <View style={{ height: 40 }} />
            </ScrollView>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    searchContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: '#1A1025',
        borderRadius: 12,
        paddingHorizontal: 12,
        paddingVertical: 10,
        marginBottom: 16,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    searchIcon: {
        marginRight: 8,
    },
    searchInput: {
        flex: 1,
        fontSize: 15,
        color: Colors.white,
    },
    counterContainer: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 12,
        paddingHorizontal: 4,
    },
    counterText: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    counterHighlight: {
        color: '#7C3AED',
        fontWeight: '700',
    },
    maxReachedText: {
        fontSize: 12,
        color: '#EF4444',
        fontWeight: '600',
    },
    cloudScrollView: {
        maxHeight: 400,
    },
    categoryContainer: {
        marginBottom: 24,
    },
    categoryTitle: {
        fontSize: 14,
        fontWeight: '600',
        color: 'rgba(255,255,255,0.4)',
        marginBottom: 12,
        textTransform: 'uppercase',
        letterSpacing: 1,
        marginLeft: 4,
    },
    chipsCloud: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
    },
    chip: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: '#1A1025',
        borderRadius: 20, // Full rounded pill
        paddingVertical: 8,
        paddingHorizontal: 16,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
        gap: 6,
    },
    chipSelected: {
        borderWidth: 0,
        borderColor: 'transparent',
    },
    chipEmoji: {
        fontSize: 16,
    },
    chipLabel: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.8)',
    },
    chipLabelSelected: {
        fontSize: 13,
        color: Colors.white,
        fontWeight: '600',
    },
    chipCloseIcon: {
        marginLeft: 2,
    },
    customSection: {
        marginTop: 8,
        marginBottom: 20,
    },
    addCustomButton: {
        flexDirection: 'row',
        alignItems: 'center',
        alignSelf: 'flex-start',
        paddingVertical: 10,
        paddingHorizontal: 16,
        borderRadius: 20,
        borderWidth: 1,
        borderColor: '#7C3AED',
        borderStyle: 'dashed',
        gap: 6,
    },
    addCustomText: {
        fontSize: 14,
        color: '#7C3AED',
        fontWeight: '500',
    },
    addCustomContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 8,
    },
    customInput: {
        flex: 1,
        backgroundColor: '#1A1025',
        borderRadius: 12,
        paddingHorizontal: 16,
        paddingVertical: 10,
        fontSize: 14,
        color: Colors.white,
        borderWidth: 1,
        borderColor: '#7C3AED',
    },
    addCustomButtonConfirm: {
        backgroundColor: '#7C3AED',
        paddingVertical: 10,
        paddingHorizontal: 16,
        borderRadius: 12,
    },
    addCustomButtonText: {
        color: Colors.white,
        fontWeight: '600',
        fontSize: 14,
    },
    cancelCustomButton: {
        padding: 8,
    },
});
