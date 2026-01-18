import React, { useState, useCallback, useMemo } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    ScrollView,
    Modal,
    Dimensions,
    Switch,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Animated, {
    useSharedValue,
    useAnimatedStyle,
    withSpring,
    withTiming,
} from 'react-native-reanimated';
import Slider from '@react-native-community/slider';
import Colors from '@/constants/Colors';

const { height: SCREEN_HEIGHT } = Dimensions.get('window');

interface FilterModalProps {
    visible: boolean;
    onClose: () => void;
    onApply: (filters: FilterState) => void;
    initialFilters?: FilterState;
}

export interface FilterState {
    distance: number;
    ageMin: number;
    ageMax: number;
    showMe: 'men' | 'women' | 'everyone';
    academicYears: string[];
    majors: string[];
    lookingFor: string[];
    verifiedOnly: boolean;
    greekLife: boolean;
}

const defaultFilters: FilterState = {
    distance: 25,
    ageMin: 18,
    ageMax: 30,
    showMe: 'everyone',
    academicYears: [],
    majors: [],
    lookingFor: [],
    verifiedOnly: false,
    greekLife: false,
};

const academicYearOptions = ['Freshman', 'Sophomore', 'Junior', 'Senior', 'Grad Student', 'Alumni'];
const majorOptions = [
    { id: 'arts', label: 'Arts', emoji: '🎨' },
    { id: 'stem', label: 'STEM', emoji: '🧬' },
    { id: 'business', label: 'Business', emoji: '💼' },
    { id: 'law', label: 'Law', emoji: '⚖️' },
    { id: 'med', label: 'Med', emoji: '🏥' },
    { id: 'undecided', label: 'Undecided', emoji: '🤷' },
];
const lookingForOptions = [
    { id: 'relationship', label: 'Relationship', icon: 'heart' },
    { id: 'casual', label: 'Casual', icon: 'flame' },
    { id: 'study', label: 'Study Buddy', icon: 'book' },
    { id: 'activity', label: 'Activity Partner', icon: 'bicycle' },
];

export default function FilterModal({ visible, onClose, onApply, initialFilters }: FilterModalProps) {
    const [filters, setFilters] = useState<FilterState>(initialFilters || defaultFilters);

    const hasActiveFilters = useMemo(() => {
        return filters.distance !== 25 ||
            filters.ageMin !== 18 ||
            filters.ageMax !== 30 ||
            filters.showMe !== 'everyone' ||
            filters.academicYears.length > 0 ||
            filters.majors.length > 0 ||
            filters.lookingFor.length > 0;
    }, [filters]);

    const toggleArrayItem = (array: string[], item: string) => {
        return array.includes(item)
            ? array.filter(i => i !== item)
            : [...array, item];
    };

    const handleReset = () => {
        setFilters(defaultFilters);
    };

    const handleApply = () => {
        onApply(filters);
        onClose();
    };

    return (
        <Modal
            visible={visible}
            animationType="slide"
            transparent
            onRequestClose={onClose}
        >
            <View style={styles.overlay}>
                <TouchableOpacity style={styles.backdrop} onPress={onClose} />
                <View style={styles.modalContainer}>
                    {/* Handle Bar */}
                    <View style={styles.handleBar} />

                    {/* Header */}
                    <View style={styles.header}>
                        <Text style={styles.headerTitle}>Filter Preferences</Text>
                        <TouchableOpacity onPress={onClose}>
                            <Ionicons name="close" size={24} color={Colors.white} />
                        </TouchableOpacity>
                    </View>

                    <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.content}>
                        {/* Section A: Essentials */}
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>📍 Essentials</Text>

                            {/* Distance Slider */}
                            <View style={styles.filterItem}>
                                <View style={styles.filterHeader}>
                                    <Text style={styles.filterLabel}>Distance</Text>
                                    <Text style={styles.filterValue}>
                                        Within {filters.distance} {filters.distance === 1 ? 'mile' : 'miles'}
                                    </Text>
                                </View>
                                <Slider
                                    style={styles.slider}
                                    minimumValue={1}
                                    maximumValue={50}
                                    step={1}
                                    value={filters.distance}
                                    onValueChange={(val) => setFilters({ ...filters, distance: val })}
                                    minimumTrackTintColor="#7C3AED"
                                    maximumTrackTintColor="#3D3D5C"
                                    thumbTintColor="#7C3AED"
                                />
                            </View>

                            {/* Age Range */}
                            <View style={styles.filterItem}>
                                <View style={styles.filterHeader}>
                                    <Text style={styles.filterLabel}>Age Range</Text>
                                    <Text style={styles.filterValue}>
                                        {filters.ageMin} - {filters.ageMax}{filters.ageMax >= 30 ? '+' : ''}
                                    </Text>
                                </View>
                                <View style={styles.ageSliders}>
                                    <Slider
                                        style={styles.slider}
                                        minimumValue={18}
                                        maximumValue={30}
                                        step={1}
                                        value={filters.ageMin}
                                        onValueChange={(val) => setFilters({
                                            ...filters,
                                            ageMin: Math.min(val, filters.ageMax - 1)
                                        })}
                                        minimumTrackTintColor="#7C3AED"
                                        maximumTrackTintColor="#3D3D5C"
                                        thumbTintColor="#7C3AED"
                                    />
                                    <Slider
                                        style={styles.slider}
                                        minimumValue={18}
                                        maximumValue={30}
                                        step={1}
                                        value={filters.ageMax}
                                        onValueChange={(val) => setFilters({
                                            ...filters,
                                            ageMax: Math.max(val, filters.ageMin + 1)
                                        })}
                                        minimumTrackTintColor="#7C3AED"
                                        maximumTrackTintColor="#3D3D5C"
                                        thumbTintColor="#7C3AED"
                                    />
                                </View>
                            </View>

                            {/* Show Me */}
                            <View style={styles.filterItem}>
                                <Text style={styles.filterLabel}>Show Me</Text>
                                <View style={styles.segmentedControl}>
                                    {(['men', 'women', 'everyone'] as const).map((option) => (
                                        <TouchableOpacity
                                            key={option}
                                            style={[
                                                styles.segmentButton,
                                                filters.showMe === option && styles.segmentButtonActive,
                                            ]}
                                            onPress={() => setFilters({ ...filters, showMe: option })}
                                        >
                                            <Text style={[
                                                styles.segmentText,
                                                filters.showMe === option && styles.segmentTextActive,
                                            ]}>
                                                {option.charAt(0).toUpperCase() + option.slice(1)}
                                            </Text>
                                        </TouchableOpacity>
                                    ))}
                                </View>
                            </View>
                        </View>

                        {/* Section B: Campus Logic */}
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>🎓 Campus Filters</Text>

                            {/* Academic Year */}
                            <View style={styles.filterItem}>
                                <Text style={styles.filterLabel}>Academic Year</Text>
                                <View style={styles.pillsContainer}>
                                    {academicYearOptions.map((year) => (
                                        <TouchableOpacity
                                            key={year}
                                            style={[
                                                styles.pill,
                                                filters.academicYears.includes(year) && styles.pillActive,
                                            ]}
                                            onPress={() => setFilters({
                                                ...filters,
                                                academicYears: toggleArrayItem(filters.academicYears, year),
                                            })}
                                        >
                                            <Text style={[
                                                styles.pillText,
                                                filters.academicYears.includes(year) && styles.pillTextActive,
                                            ]}>
                                                {year}
                                            </Text>
                                        </TouchableOpacity>
                                    ))}
                                </View>
                            </View>

                            {/* Major / Field of Study */}
                            <View style={styles.filterItem}>
                                <Text style={styles.filterLabel}>Major / Field of Study</Text>
                                <ScrollView horizontal showsHorizontalScrollIndicator={false}>
                                    <View style={styles.chipsContainer}>
                                        {majorOptions.map((major) => (
                                            <TouchableOpacity
                                                key={major.id}
                                                style={[
                                                    styles.chip,
                                                    filters.majors.includes(major.id) && styles.chipActive,
                                                ]}
                                                onPress={() => setFilters({
                                                    ...filters,
                                                    majors: toggleArrayItem(filters.majors, major.id),
                                                })}
                                            >
                                                <Text style={styles.chipEmoji}>{major.emoji}</Text>
                                                <Text style={[
                                                    styles.chipText,
                                                    filters.majors.includes(major.id) && styles.chipTextActive,
                                                ]}>
                                                    {major.label}
                                                </Text>
                                            </TouchableOpacity>
                                        ))}
                                    </View>
                                </ScrollView>
                            </View>
                        </View>

                        {/* Section C: Intent & Vibe */}
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>✨ Looking For</Text>
                            <View style={styles.lookingForGrid}>
                                {lookingForOptions.map((option) => (
                                    <TouchableOpacity
                                        key={option.id}
                                        style={[
                                            styles.lookingForItem,
                                            filters.lookingFor.includes(option.id) && styles.lookingForItemActive,
                                        ]}
                                        onPress={() => setFilters({
                                            ...filters,
                                            lookingFor: toggleArrayItem(filters.lookingFor, option.id),
                                        })}
                                    >
                                        <Ionicons
                                            name={option.icon as any}
                                            size={28}
                                            color={filters.lookingFor.includes(option.id) ? '#7C3AED' : 'rgba(255,255,255,0.5)'}
                                        />
                                        <Text style={[
                                            styles.lookingForText,
                                            filters.lookingFor.includes(option.id) && styles.lookingForTextActive,
                                        ]}>
                                            {option.label}
                                        </Text>
                                    </TouchableOpacity>
                                ))}
                            </View>
                        </View>

                        {/* Premium Filters */}
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>👑 Premium Filters</Text>

                            <View style={styles.premiumItem}>
                                <View style={styles.premiumInfo}>
                                    <View style={styles.premiumLabelRow}>
                                        <Text style={styles.filterLabel}>Verified Profiles Only</Text>
                                        <Ionicons name="lock-closed" size={14} color="#FFD700" />
                                    </View>
                                    <Text style={styles.premiumSubtext}>
                                        See only students with verified .edu emails
                                    </Text>
                                </View>
                                <Switch
                                    value={filters.verifiedOnly}
                                    onValueChange={() => { }}
                                    disabled
                                    trackColor={{ false: '#3D3D5C', true: '#7C3AED' }}
                                    thumbColor={Colors.white}
                                />
                            </View>

                            <View style={styles.premiumItem}>
                                <View style={styles.premiumInfo}>
                                    <View style={styles.premiumLabelRow}>
                                        <Text style={styles.filterLabel}>Greek Life</Text>
                                        <Ionicons name="lock-closed" size={14} color="#FFD700" />
                                    </View>
                                    <Text style={styles.premiumSubtext}>
                                        Filter by Sorority/Fraternity members
                                    </Text>
                                </View>
                                <Switch
                                    value={filters.greekLife}
                                    onValueChange={() => { }}
                                    disabled
                                    trackColor={{ false: '#3D3D5C', true: '#7C3AED' }}
                                    thumbColor={Colors.white}
                                />
                            </View>
                        </View>
                    </ScrollView>

                    {/* Footer */}
                    <View style={styles.footer}>
                        <TouchableOpacity style={styles.resetButton} onPress={handleReset}>
                            <Text style={styles.resetText}>Clear All</Text>
                        </TouchableOpacity>
                        <TouchableOpacity style={styles.applyButton} onPress={handleApply}>
                            <LinearGradient
                                colors={['#7C3AED', '#6D28D9']}
                                style={styles.applyGradient}
                            >
                                <Text style={styles.applyText}>Show Matches</Text>
                            </LinearGradient>
                        </TouchableOpacity>
                    </View>
                </View>
            </View>
        </Modal>
    );
}

const styles = StyleSheet.create({
    overlay: {
        flex: 1,
        justifyContent: 'flex-end',
    },
    backdrop: {
        ...StyleSheet.absoluteFillObject,
        backgroundColor: 'rgba(0,0,0,0.5)',
    },
    modalContainer: {
        height: SCREEN_HEIGHT * 0.85,
        backgroundColor: '#0F0A1A',
        borderTopLeftRadius: 24,
        borderTopRightRadius: 24,
    },
    handleBar: {
        width: 40,
        height: 4,
        backgroundColor: 'rgba(255,255,255,0.3)',
        borderRadius: 2,
        alignSelf: 'center',
        marginTop: 12,
    },
    header: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 20,
        paddingVertical: 16,
        borderBottomWidth: 1,
        borderBottomColor: 'rgba(255,255,255,0.1)',
    },
    headerTitle: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
    },
    content: {
        paddingHorizontal: 20,
        paddingBottom: 100,
    },
    section: {
        marginTop: 24,
    },
    sectionTitle: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 16,
    },
    filterItem: {
        marginBottom: 20,
    },
    filterHeader: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 8,
    },
    filterLabel: {
        fontSize: 14,
        fontWeight: '500',
        color: Colors.white,
    },
    filterValue: {
        fontSize: 14,
        color: '#7C3AED',
        fontWeight: '600',
    },
    slider: {
        width: '100%',
        height: 40,
    },
    ageSliders: {
        gap: -8,
    },
    segmentedControl: {
        flexDirection: 'row',
        backgroundColor: '#1A1025',
        borderRadius: 12,
        padding: 4,
        marginTop: 8,
    },
    segmentButton: {
        flex: 1,
        paddingVertical: 12,
        alignItems: 'center',
        borderRadius: 10,
    },
    segmentButtonActive: {
        backgroundColor: '#7C3AED',
    },
    segmentText: {
        fontSize: 14,
        fontWeight: '500',
        color: 'rgba(255,255,255,0.6)',
    },
    segmentTextActive: {
        color: Colors.white,
    },
    pillsContainer: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
        marginTop: 12,
    },
    pill: {
        paddingHorizontal: 16,
        paddingVertical: 8,
        borderRadius: 20,
        backgroundColor: '#1A1025',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    pillActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    pillText: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    pillTextActive: {
        color: Colors.white,
    },
    chipsContainer: {
        flexDirection: 'row',
        gap: 10,
        marginTop: 12,
    },
    chip: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: 14,
        paddingVertical: 10,
        borderRadius: 20,
        backgroundColor: '#1A1025',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
        gap: 6,
    },
    chipActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    chipEmoji: {
        fontSize: 16,
    },
    chipText: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    chipTextActive: {
        color: Colors.white,
    },
    lookingForGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 12,
    },
    lookingForItem: {
        width: '47%',
        backgroundColor: '#1A1025',
        borderRadius: 16,
        paddingVertical: 20,
        alignItems: 'center',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    lookingForItemActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    lookingForText: {
        marginTop: 8,
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    lookingForTextActive: {
        color: Colors.white,
    },
    premiumItem: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        backgroundColor: '#1A1025',
        borderRadius: 12,
        padding: 16,
        marginBottom: 12,
        opacity: 0.7,
    },
    premiumInfo: {
        flex: 1,
    },
    premiumLabelRow: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 6,
    },
    premiumSubtext: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.4)',
        marginTop: 4,
    },
    footer: {
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 20,
        paddingVertical: 16,
        paddingBottom: 34,
        backgroundColor: '#0F0A1A',
        borderTopWidth: 1,
        borderTopColor: 'rgba(255,255,255,0.1)',
    },
    resetButton: {
        paddingVertical: 12,
        paddingHorizontal: 16,
    },
    resetText: {
        fontSize: 15,
        color: 'rgba(255,255,255,0.7)',
    },
    applyButton: {
        flex: 1,
        marginLeft: 16,
        borderRadius: 12,
        overflow: 'hidden',
    },
    applyGradient: {
        paddingVertical: 14,
        alignItems: 'center',
    },
    applyText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
});
