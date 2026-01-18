import React, { useState, useMemo } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TextInput,
    TouchableOpacity,
    Modal,
    Platform,
    FlatList,
    KeyboardAvoidingView,
    SafeAreaView,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';
import { searchUniversities, University } from '@/data/universities';

interface UniversityAutocompleteProps {
    visible: boolean;
    onClose: () => void;
    onSelect: (university: string) => void;
    currentValue?: string;
}

export default function UniversityAutocomplete({
    visible,
    onClose,
    onSelect,
    currentValue
}: UniversityAutocompleteProps) {
    const [searchQuery, setSearchQuery] = useState('');
    const [customUniversity, setCustomUniversity] = useState('');

    const searchResults = useMemo(() => {
        return searchUniversities(searchQuery, 15);
    }, [searchQuery]);

    const handleSelect = (university: University) => {
        onSelect(`${university.name} (${university.country.slice(0, 2).toUpperCase()})`);
        setSearchQuery('');
        onClose();
    };

    const handleCustomSubmit = () => {
        if (customUniversity.trim().length > 0) {
            onSelect(customUniversity.trim());
            setCustomUniversity('');
            setSearchQuery('');
            onClose();
        }
    };

    const handleClose = () => {
        setSearchQuery('');
        setCustomUniversity('');
        onClose();
    };

    const renderItem = ({ item }: { item: University }) => (
        <TouchableOpacity
            style={styles.resultItem}
            onPress={() => handleSelect(item)}
        >
            <View style={styles.resultIcon}>
                <Ionicons name="school" size={20} color="#7C3AED" />
            </View>
            <View style={styles.resultText}>
                <Text style={styles.universityName}>{item.name}</Text>
                <Text style={styles.countryName}>{item.country}</Text>
            </View>
            <Ionicons name="chevron-forward" size={16} color="rgba(255,255,255,0.3)" />
        </TouchableOpacity>
    );

    return (
        <Modal
            visible={visible}
            animationType="slide"
            presentationStyle="pageSheet"
            onRequestClose={handleClose}
        >
            <SafeAreaView style={styles.container}>
                {/* Sticky Header */}
                <View style={styles.header}>
                    <TouchableOpacity
                        onPress={handleClose}
                        style={styles.closeButton}
                        hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
                    >
                        <Ionicons name="close" size={24} color={Colors.white} />
                    </TouchableOpacity>

                    <View style={styles.searchContainer}>
                        <Ionicons name="search" size={20} color="rgba(255,255,255,0.5)" style={styles.searchIcon} />
                        <TextInput
                            style={styles.searchInput}
                            value={searchQuery}
                            onChangeText={setSearchQuery}
                            placeholder="Find your campus..."
                            placeholderTextColor="rgba(255,255,255,0.4)"
                            autoFocus
                            autoCorrect={false}
                            selectionColor="#3B82F6" // Blinking Blue Cursor
                        />
                        {searchQuery.length > 0 && (
                            <TouchableOpacity
                                onPress={() => setSearchQuery('')}
                                style={styles.clearButton}
                            >
                                <Ionicons name="close-circle" size={18} color="rgba(255,255,255,0.5)" />
                            </TouchableOpacity>
                        )}
                    </View>
                </View>

                {/* Content Area */}
                <KeyboardAvoidingView
                    behavior={Platform.OS === 'ios' ? 'padding' : undefined}
                    style={styles.content}
                >
                    {searchQuery.length >= 2 ? (
                        <FlatList
                            data={searchResults}
                            renderItem={renderItem}
                            keyExtractor={(item, index) => `${item.name}-${index}`}
                            contentContainerStyle={styles.listContent}
                            keyboardShouldPersistTaps="handled"
                            ListEmptyComponent={
                                <View style={styles.emptyState}>
                                    <Text style={styles.emptyTitle}>No universities found</Text>
                                    <Text style={styles.emptySubtitle}>
                                        Don't see your school? Add it below.
                                    </Text>

                                    {/* Custom Input for 'No Results' */}
                                    <View style={styles.customInputContainer}>
                                        <TextInput
                                            style={styles.customInput}
                                            value={customUniversity}
                                            onChangeText={setCustomUniversity}
                                            placeholder="Enter university name manually"
                                            placeholderTextColor="rgba(255,255,255,0.4)"
                                        />
                                        {customUniversity.length > 0 && (
                                            <TouchableOpacity
                                                style={styles.addButton}
                                                onPress={handleCustomSubmit}
                                            >
                                                <Text style={styles.addButtonText}>Add University</Text>
                                            </TouchableOpacity>
                                        )}
                                    </View>
                                </View>
                            }
                        />
                    ) : (
                        <View style={styles.initialState}>
                            <View style={styles.iconContainer}>
                                <Ionicons name="school-outline" size={48} color="rgba(124, 58, 237, 0.5)" />
                            </View>
                            <Text style={styles.initialTitle}>Find Your Campus</Text>
                            <Text style={styles.initialSubtitle}>
                                Search 150+ universities in the US and Canada.{'\n'}
                                Start typing to see results.
                            </Text>

                            {/* Custom Input for 'Initial State' (if they want to skip search) */}
                            <View style={styles.customSection}>
                                <Text style={styles.customLabel}>Can't find your school?</Text>
                                <View style={styles.customInputRow}>
                                    <TextInput
                                        style={styles.customInputSmall}
                                        value={customUniversity}
                                        onChangeText={setCustomUniversity}
                                        placeholder="Type name here..."
                                        placeholderTextColor="rgba(255,255,255,0.4)"
                                    />
                                    {customUniversity.length > 0 && (
                                        <TouchableOpacity
                                            style={styles.addButtonSmall}
                                            onPress={handleCustomSubmit}
                                        >
                                            <Text style={styles.addButtonTextSmall}>Add</Text>
                                        </TouchableOpacity>
                                    )}
                                </View>
                            </View>
                        </View>
                    )}
                </KeyboardAvoidingView>
            </SafeAreaView>
        </Modal>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#0F0A1A',
    },
    header: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: 16,
        paddingVertical: 12,
        backgroundColor: '#0F0A1A', // Sticky header bg
        borderBottomWidth: 1,
        borderBottomColor: 'rgba(255,255,255,0.1)',
        gap: 12,
        zIndex: 10,
    },
    closeButton: {
        padding: 4,
    },
    searchContainer: {
        flex: 1,
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: '#1E1B2E', // Pill-shaped gray container
        borderRadius: 24, // Pill shape
        paddingHorizontal: 12,
        height: 48,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    searchIcon: {
        marginRight: 8,
    },
    searchInput: {
        flex: 1,
        fontSize: 16,
        fontWeight: '600', // Bold text
        color: Colors.white,
        height: '100%',
    },
    clearButton: {
        padding: 4,
    },
    content: {
        flex: 1,
    },
    listContent: {
        paddingBottom: 40,
    },
    resultItem: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingVertical: 16,
        paddingHorizontal: 20,
        borderBottomWidth: 1,
        borderBottomColor: 'rgba(255,255,255,0.05)',
    },
    resultIcon: {
        width: 38,
        height: 38,
        borderRadius: 19,
        backgroundColor: 'rgba(124, 58, 237, 0.15)',
        justifyContent: 'center',
        alignItems: 'center',
        marginRight: 16,
    },
    resultText: {
        flex: 1,
    },
    universityName: {
        fontSize: 16,
        fontWeight: '500',
        color: Colors.white,
        marginBottom: 2,
    },
    countryName: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.5)',
    },
    emptyState: {
        padding: 24,
        alignItems: 'center',
    },
    emptyTitle: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 8,
    },
    emptySubtitle: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.5)',
        marginBottom: 24,
    },
    customInputContainer: {
        width: '100%',
    },
    customInput: {
        backgroundColor: '#1E1B2E',
        borderRadius: 12,
        paddingHorizontal: 16,
        paddingVertical: 16,
        fontSize: 16,
        color: Colors.white,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
        marginBottom: 12,
    },
    addButton: {
        backgroundColor: '#7C3AED',
        borderRadius: 12,
        paddingVertical: 16,
        alignItems: 'center',
    },
    addButtonText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    initialState: {
        flex: 1,
        alignItems: 'center',
        paddingTop: 60,
        paddingHorizontal: 24,
    },
    iconContainer: {
        width: 80,
        height: 80,
        borderRadius: 40,
        backgroundColor: 'rgba(124, 58, 237, 0.1)',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 24,
    },
    initialTitle: {
        fontSize: 22,
        fontWeight: '700',
        color: Colors.white,
        marginBottom: 12,
        textAlign: 'center',
    },
    initialSubtitle: {
        fontSize: 15,
        color: 'rgba(255,255,255,0.5)',
        textAlign: 'center',
        lineHeight: 22,
        marginBottom: 40,
    },
    customSection: {
        width: '100%',
        paddingTop: 24,
        borderTopWidth: 1,
        borderTopColor: 'rgba(255,255,255,0.1)',
    },
    customLabel: {
        fontSize: 14,
        fontWeight: '500',
        color: 'rgba(255,255,255,0.7)',
        marginBottom: 12,
    },
    customInputRow: {
        flexDirection: 'row',
        gap: 12,
    },
    customInputSmall: {
        flex: 1,
        backgroundColor: '#1E1B2E',
        borderRadius: 12,
        paddingHorizontal: 16,
        paddingVertical: 12,
        fontSize: 15,
        color: Colors.white,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    addButtonSmall: {
        backgroundColor: '#7C3AED',
        borderRadius: 12,
        paddingHorizontal: 20,
        justifyContent: 'center',
    },
    addButtonTextSmall: {
        fontSize: 14,
        fontWeight: '600',
        color: Colors.white,
    },
});
