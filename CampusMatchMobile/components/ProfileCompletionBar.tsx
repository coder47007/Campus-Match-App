// Profile completion progress bar component
import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Animated, {
    useAnimatedStyle,
    withSpring,
    useSharedValue,
    withTiming,
} from 'react-native-reanimated';
import Colors from '@/constants/Colors';

export interface ProfileField {
    id: string;
    label: string;
    isComplete: boolean;
    icon: keyof typeof Ionicons.glyphMap;
}

interface ProfileCompletionBarProps {
    fields: ProfileField[];
    onFieldPress?: (fieldId: string) => void;
    showDetails?: boolean;
}

export function calculateProfileCompletion(profile: {
    name?: string;
    bio?: string;
    age?: number;
    major?: string;
    year?: string;
    university?: string;
    photoUrl?: string;
    photos?: any[];
    interests?: any[];
}): ProfileField[] {
    return [
        {
            id: 'photo',
            label: 'Profile Photo',
            isComplete: !!(profile.photoUrl || (profile.photos && profile.photos.length > 0)),
            icon: 'camera-outline',
        },
        {
            id: 'name',
            label: 'Name',
            isComplete: !!profile.name && profile.name.length > 0,
            icon: 'person-outline',
        },
        {
            id: 'age',
            label: 'Age',
            isComplete: !!profile.age,
            icon: 'calendar-outline',
        },
        {
            id: 'university',
            label: 'University',
            isComplete: !!profile.university && profile.university.length > 0,
            icon: 'school-outline',
        },
        {
            id: 'major',
            label: 'Major',
            isComplete: !!profile.major && profile.major.length > 0,
            icon: 'book-outline',
        },
        {
            id: 'year',
            label: 'Year',
            isComplete: !!profile.year && profile.year.length > 0,
            icon: 'ribbon-outline',
        },
        {
            id: 'bio',
            label: 'Bio',
            isComplete: !!profile.bio && profile.bio.length > 10,
            icon: 'document-text-outline',
        },
        {
            id: 'interests',
            label: 'Interests',
            isComplete: !!(profile.interests && profile.interests.length >= 3),
            icon: 'heart-outline',
        },
    ];
}

export default function ProfileCompletionBar({
    fields,
    onFieldPress,
    showDetails = false,
}: ProfileCompletionBarProps) {
    const completedCount = fields.filter(f => f.isComplete).length;
    const totalCount = fields.length;
    const percentage = Math.round((completedCount / totalCount) * 100);

    const progressWidth = useSharedValue(0);

    React.useEffect(() => {
        progressWidth.value = withSpring(percentage, {
            damping: 15,
            stiffness: 100,
        });
    }, [percentage]);

    const progressStyle = useAnimatedStyle(() => ({
        width: `${progressWidth.value}%`,
    }));

    const getProgressColor = () => {
        if (percentage >= 80) return ['#22C55E', '#16A34A'];
        if (percentage >= 50) return ['#FBBF24', '#F59E0B'];
        return ['#EF4444', '#DC2626'];
    };

    const getStatusText = () => {
        if (percentage === 100) return 'Profile Complete! 🎉';
        if (percentage >= 80) return 'Almost there!';
        if (percentage >= 50) return 'Looking good!';
        return 'Complete your profile';
    };

    const incompleteFields = fields.filter(f => !f.isComplete);

    return (
        <View style={styles.container}>
            {/* Header */}
            <View style={styles.header}>
                <View style={styles.headerLeft}>
                    <Text style={styles.title}>{getStatusText()}</Text>
                    <Text style={styles.subtitle}>
                        {completedCount}/{totalCount} completed
                    </Text>
                </View>
                <View style={styles.percentageContainer}>
                    <Text style={[
                        styles.percentage,
                        percentage >= 80 && styles.percentageComplete
                    ]}>
                        {percentage}%
                    </Text>
                </View>
            </View>

            {/* Progress Bar */}
            <View style={styles.progressContainer}>
                <View style={styles.progressBackground}>
                    <Animated.View style={[styles.progressFill, progressStyle]}>
                        <LinearGradient
                            colors={getProgressColor() as [string, string]}
                            start={{ x: 0, y: 0 }}
                            end={{ x: 1, y: 0 }}
                            style={styles.progressGradient}
                        />
                    </Animated.View>
                </View>
            </View>

            {/* Incomplete Fields */}
            {showDetails && incompleteFields.length > 0 && (
                <View style={styles.incompleteSection}>
                    <Text style={styles.incompleteTitle}>Add to boost your profile:</Text>
                    <View style={styles.incompleteList}>
                        {incompleteFields.slice(0, 3).map((field) => (
                            <TouchableOpacity
                                key={field.id}
                                style={styles.incompleteItem}
                                onPress={() => onFieldPress?.(field.id)}
                            >
                                <View style={styles.incompleteIcon}>
                                    <Ionicons name={field.icon} size={16} color={Colors.primary.main} />
                                </View>
                                <Text style={styles.incompleteLabel}>{field.label}</Text>
                                <Ionicons name="add-circle" size={18} color={Colors.primary.main} />
                            </TouchableOpacity>
                        ))}
                    </View>
                </View>
            )}

            {/* Completion Badge */}
            {percentage === 100 && (
                <View style={styles.completeBadge}>
                    <Ionicons name="checkmark-circle" size={20} color="#22C55E" />
                    <Text style={styles.completeBadgeText}>Profile fully optimized</Text>
                </View>
            )}
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        backgroundColor: Colors.dark.surface,
        borderRadius: 16,
        padding: 16,
        marginBottom: 16,
    },
    header: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 12,
    },
    headerLeft: {
        flex: 1,
    },
    title: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 2,
    },
    subtitle: {
        fontSize: 13,
        color: Colors.dark.textSecondary,
    },
    percentageContainer: {
        backgroundColor: 'rgba(124, 58, 237, 0.15)',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 20,
    },
    percentage: {
        fontSize: 14,
        fontWeight: '700',
        color: Colors.primary.main,
    },
    percentageComplete: {
        color: '#22C55E',
    },
    progressContainer: {
        marginBottom: 8,
    },
    progressBackground: {
        height: 8,
        backgroundColor: 'rgba(255, 255, 255, 0.1)',
        borderRadius: 4,
        overflow: 'hidden',
    },
    progressFill: {
        height: '100%',
        borderRadius: 4,
        overflow: 'hidden',
    },
    progressGradient: {
        flex: 1,
    },
    incompleteSection: {
        marginTop: 12,
        paddingTop: 12,
        borderTopWidth: 1,
        borderTopColor: 'rgba(255, 255, 255, 0.1)',
    },
    incompleteTitle: {
        fontSize: 12,
        fontWeight: '500',
        color: Colors.dark.textMuted,
        marginBottom: 8,
    },
    incompleteList: {
        gap: 6,
    },
    incompleteItem: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(124, 58, 237, 0.1)',
        paddingHorizontal: 12,
        paddingVertical: 8,
        borderRadius: 8,
        gap: 8,
    },
    incompleteIcon: {
        width: 28,
        height: 28,
        borderRadius: 14,
        backgroundColor: 'rgba(124, 58, 237, 0.15)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    incompleteLabel: {
        flex: 1,
        fontSize: 14,
        color: Colors.white,
    },
    completeBadge: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        paddingVertical: 10,
        borderRadius: 8,
        marginTop: 12,
        gap: 8,
    },
    completeBadgeText: {
        fontSize: 14,
        fontWeight: '500',
        color: '#22C55E',
    },
});
