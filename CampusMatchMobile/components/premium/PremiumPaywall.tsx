import React from 'react';
import {
    View,
    Text,
    StyleSheet,
    Modal,
    TouchableOpacity,
    SafeAreaView,
    ScrollView,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';
import { useSubscriptionStore } from '@/stores/subscriptionStore';

interface PremiumPaywallProps {
    visible: boolean;
    onClose: () => void;
    feature: 'likes' | 'rewind' | 'filters' | 'swipes' | 'boost' | 'general';
    onUpgrade?: () => void;
}

const featureMessages = {
    likes: {
        title: 'See Who Likes You',
        description: 'Upgrade to see everyone who has liked your profile and match instantly!',
        icon: 'heart',
    },
    rewind: {
        title: 'Rewind Your Last Swipe',
        description: 'Made a mistake? Go back and swipe again on profiles you passed.',
        icon: 'arrow-undo',
    },
    filters: {
        title: 'Advanced Filters',
        description: 'Filter by major, year, interests, and distance to find your perfect match!',
        icon: 'filter',
    },
    swipes: {
        title: 'Unlimited Swipes',
        description: "You've run out of swipes! Upgrade to swipe without limits.",
        icon: 'infinite',
    },
    boost: {
        title: 'Unlimited Boosts',
        description: 'Boost your profile anytime to get seen by more people!',
        icon: 'rocket',
    },
    general: {
        title: 'Unlock Premium Features',
        description: 'Get the full CampusMatch experience with Premium!',
        icon: 'diamond',
    },
};

const premiumFeatures = [
    { icon: 'infinite', label: 'Unlimited Swipes' },
    { icon: 'heart', label: 'See Who Likes You' },
    { icon: 'arrow-undo', label: 'Rewind Last Swipe' },
    { icon: 'globe', label: 'Cross-Campus Matching' },
    { icon: 'location', label: 'Distance Filter (up to 200km)' },
    { icon: 'filter', label: 'Advanced Filters' },
    { icon: 'rocket', label: 'Unlimited Boosts' },
    { icon: 'checkmark-done', label: 'Read Receipts' },
    { icon: 'ellipsis-horizontal', label: 'Typing Indicators' },
    { icon: 'ribbon', label: 'Premium Badge' },
    { icon: 'ban', label: 'No Ads' },
];

export const PremiumPaywall: React.FC<PremiumPaywallProps> = ({
    visible,
    onClose,
    feature,
    onUpgrade,
}) => {
    const { upgrade, isLoading } = useSubscriptionStore();
    const featureInfo = featureMessages[feature];

    const handleUpgrade = async () => {
        const success = await upgrade('premium');
        if (success) {
            onUpgrade?.();
            onClose();
        }
    };

    return (
        <Modal
            visible={visible}
            animationType="slide"
            presentationStyle="pageSheet"
            onRequestClose={onClose}
        >
            <SafeAreaView style={styles.container}>
                <TouchableOpacity style={styles.closeButton} onPress={onClose}>
                    <Ionicons name="close" size={28} color={Colors.dark.text} />
                </TouchableOpacity>

                <ScrollView
                    contentContainerStyle={styles.content}
                    showsVerticalScrollIndicator={false}
                >
                    {/* Hero Section */}
                    <LinearGradient
                        colors={['#FF6B6B', '#FF8E53', '#FFC93C']}
                        start={{ x: 0, y: 0 }}
                        end={{ x: 1, y: 1 }}
                        style={styles.heroGradient}
                    >
                        <Ionicons
                            name={featureInfo.icon as any}
                            size={60}
                            color="white"
                        />
                    </LinearGradient>

                    <Text style={styles.title}>{featureInfo.title}</Text>
                    <Text style={styles.description}>{featureInfo.description}</Text>

                    {/* Features List */}
                    <View style={styles.featuresContainer}>
                        <Text style={styles.featuresTitle}>CampusMatch+ includes:</Text>
                        {premiumFeatures.map((item, index) => (
                            <View key={index} style={styles.featureRow}>
                                <View style={styles.featureIcon}>
                                    <Ionicons
                                        name={item.icon as any}
                                        size={20}
                                        color={Colors.primary.main}
                                    />
                                </View>
                                <Text style={styles.featureLabel}>{item.label}</Text>
                            </View>
                        ))}
                    </View>

                    {/* Pricing */}
                    <View style={styles.pricingContainer}>
                        <Text style={styles.price}>$14.99</Text>
                        <Text style={styles.pricePeriod}>/month</Text>
                    </View>

                    {/* CTA Button */}
                    <TouchableOpacity
                        style={styles.upgradeButton}
                        onPress={handleUpgrade}
                        disabled={isLoading}
                    >
                        <LinearGradient
                            colors={['#FF6B6B', '#FF8E53']}
                            start={{ x: 0, y: 0 }}
                            end={{ x: 1, y: 0 }}
                            style={styles.upgradeGradient}
                        >
                            <Text style={styles.upgradeButtonText}>
                                {isLoading ? 'Processing...' : 'Upgrade to Premium'}
                            </Text>
                        </LinearGradient>
                    </TouchableOpacity>

                    <Text style={styles.disclaimer}>
                        Cancel anytime. Subscription auto-renews monthly.
                    </Text>
                </ScrollView>
            </SafeAreaView>
        </Modal>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: Colors.dark.background,
    },
    closeButton: {
        position: 'absolute',
        top: 16,
        right: 16,
        zIndex: 10,
        padding: 8,
    },
    content: {
        alignItems: 'center',
        padding: 24,
        paddingTop: 60,
    },
    heroGradient: {
        width: 120,
        height: 120,
        borderRadius: 60,
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 24,
    },
    title: {
        fontSize: 28,
        fontWeight: '700',
        color: Colors.dark.text,
        textAlign: 'center',
        marginBottom: 12,
    },
    description: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        lineHeight: 24,
        marginBottom: 32,
        paddingHorizontal: 16,
    },
    featuresContainer: {
        width: '100%',
        backgroundColor: Colors.dark.card,
        borderRadius: 16,
        padding: 20,
        marginBottom: 24,
    },
    featuresTitle: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.dark.text,
        marginBottom: 16,
    },
    featureRow: {
        flexDirection: 'row',
        alignItems: 'center',
        marginBottom: 12,
    },
    featureIcon: {
        width: 36,
        height: 36,
        borderRadius: 18,
        backgroundColor: Colors.primary.light,
        justifyContent: 'center',
        alignItems: 'center',
        marginRight: 12,
    },
    featureLabel: {
        fontSize: 15,
        color: Colors.dark.text,
        flex: 1,
    },
    pricingContainer: {
        flexDirection: 'row',
        alignItems: 'baseline',
        marginBottom: 24,
    },
    price: {
        fontSize: 42,
        fontWeight: '800',
        color: Colors.dark.text,
    },
    pricePeriod: {
        fontSize: 18,
        color: Colors.dark.textSecondary,
        marginLeft: 4,
    },
    upgradeButton: {
        width: '100%',
        marginBottom: 16,
    },
    upgradeGradient: {
        paddingVertical: 18,
        borderRadius: 30,
        alignItems: 'center',
    },
    upgradeButtonText: {
        fontSize: 18,
        fontWeight: '700',
        color: 'white',
    },
    disclaimer: {
        fontSize: 12,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
    },
});

export default PremiumPaywall;
