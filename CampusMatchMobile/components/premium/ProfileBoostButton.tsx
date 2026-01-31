import React, { useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    Modal,
    Animated,
    Easing,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';
import { useSubscriptionStore } from '@/stores/subscriptionStore';

interface ProfileBoostButtonProps {
    onBoostPress: () => Promise<void>;
    onUpgradePress?: () => void;
}

export const ProfileBoostButton: React.FC<ProfileBoostButtonProps> = ({
    onBoostPress,
    onUpgradePress,
}) => {
    const { features, isBoosted } = useSubscriptionStore();
    const [isLoading, setIsLoading] = useState(false);
    const [showSuccessModal, setShowSuccessModal] = useState(false);
    const pulseAnim = new Animated.Value(1);

    const canBoost = features?.profileBoost && !isBoosted;
    const isPremium = features?.profileBoost;

    // Pulse animation for active boost
    React.useEffect(() => {
        if (isBoosted) {
            const animation = Animated.loop(
                Animated.sequence([
                    Animated.timing(pulseAnim, {
                        toValue: 1.1,
                        duration: 800,
                        easing: Easing.inOut(Easing.ease),
                        useNativeDriver: true,
                    }),
                    Animated.timing(pulseAnim, {
                        toValue: 1,
                        duration: 800,
                        easing: Easing.inOut(Easing.ease),
                        useNativeDriver: true,
                    }),
                ])
            );
            animation.start();
            return () => animation.stop();
        }
    }, [isBoosted, pulseAnim]);

    const handlePress = async () => {
        if (!isPremium) {
            onUpgradePress?.();
            return;
        }

        if (!canBoost) return;

        setIsLoading(true);
        try {
            await onBoostPress();
            setShowSuccessModal(true);
            setTimeout(() => setShowSuccessModal(false), 2500);
        } catch (error) {
            console.error('Failed to boost profile:', error);
        } finally {
            setIsLoading(false);
        }
    };

    if (isBoosted) {
        return (
            <Animated.View style={[styles.activeBoostContainer, { transform: [{ scale: pulseAnim }] }] as any}>
                <LinearGradient
                    colors={['#FFD700', '#FFA500']}
                    start={{ x: 0, y: 0 }}
                    end={{ x: 1, y: 1 }}
                    style={styles.activeBoost}
                >
                    <Ionicons name="rocket" size={18} color="white" />
                    <Text style={styles.activeBoostText}>Profile Boosted!</Text>
                    <View style={styles.sparkle}>
                        <Ionicons name="sparkles" size={14} color="white" />
                    </View>
                </LinearGradient>
            </Animated.View>
        );
    }

    return (
        <>
            <TouchableOpacity
                style={[styles.boostButton, !isPremium && styles.lockedButton]}
                onPress={handlePress}
                disabled={isLoading}
                activeOpacity={0.8}
            >
                <LinearGradient
                    colors={isPremium ? ['#7C3AED', '#9D4EDD'] : ['#3D3D5C', '#2D2D44']}
                    start={{ x: 0, y: 0 }}
                    end={{ x: 1, y: 1 }}
                    style={styles.boostGradient}
                >
                    {isLoading ? (
                        <View style={styles.loadingDots}>
                            <Ionicons name="ellipse" size={8} color="white" />
                            <Ionicons name="ellipse" size={8} color="white" />
                            <Ionicons name="ellipse" size={8} color="white" />
                        </View>
                    ) : (
                        <>
                            <Ionicons
                                name={isPremium ? "rocket" : "lock-closed"}
                                size={18}
                                color="white"
                            />
                            <Text style={styles.boostText}>
                                {isPremium ? 'Boost Profile' : 'Boost (Premium)'}
                            </Text>
                        </>
                    )}
                </LinearGradient>
            </TouchableOpacity>

            {/* Success Modal */}
            <Modal visible={showSuccessModal} transparent animationType="fade">
                <View style={styles.modalOverlay}>
                    <View style={styles.successCard}>
                        <LinearGradient
                            colors={['#FFD700', '#FFA500']}
                            style={styles.successGradient}
                        >
                            <Ionicons name="rocket" size={48} color="white" />
                            <Text style={styles.successTitle}>Boosted! 🚀</Text>
                            <Text style={styles.successSubtitle}>
                                Your profile will appear first in discover for the next 30 minutes
                            </Text>
                        </LinearGradient>
                    </View>
                </View>
            </Modal>
        </>
    );
};

const styles = StyleSheet.create({
    boostButton: {
        borderRadius: 12,
        overflow: 'hidden',
        marginVertical: 8,
    },
    lockedButton: {
        opacity: 0.8,
    },
    boostGradient: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 14,
        paddingHorizontal: 20,
        gap: 8,
    },
    boostText: {
        fontSize: 15,
        fontWeight: '600',
        color: 'white',
    },
    loadingDots: {
        flexDirection: 'row',
        gap: 4,
    },
    // Active boost state
    activeBoostContainer: {},
    activeBoost: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 14,
        paddingHorizontal: 20,
        borderRadius: 12,
        gap: 8,
    },
    activeBoostText: {
        fontSize: 15,
        fontWeight: '700',
        color: 'white',
    },
    sparkle: {
        marginLeft: 4,
    },
    // Modal styles
    modalOverlay: {
        flex: 1,
        backgroundColor: 'rgba(0,0,0,0.7)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    successCard: {
        borderRadius: 24,
        overflow: 'hidden',
        width: '80%',
        maxWidth: 300,
    },
    successGradient: {
        alignItems: 'center',
        paddingVertical: 40,
        paddingHorizontal: 24,
    },
    successTitle: {
        fontSize: 24,
        fontWeight: '700',
        color: 'white',
        marginTop: 16,
    },
    successSubtitle: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.9)',
        textAlign: 'center',
        marginTop: 8,
        lineHeight: 20,
    },
});

export default ProfileBoostButton;
