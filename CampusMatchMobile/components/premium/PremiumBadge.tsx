import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import { Ionicons } from '@expo/vector-icons';

interface PremiumBadgeProps {
    size?: 'small' | 'medium' | 'large';
    showLabel?: boolean;
}

export const PremiumBadge: React.FC<PremiumBadgeProps> = ({
    size = 'small',
    showLabel = false
}) => {
    const dimensions = {
        small: { badge: 20, icon: 12 },
        medium: { badge: 28, icon: 16 },
        large: { badge: 40, icon: 24 },
    };

    const { badge, icon } = dimensions[size];

    return (
        <View style={styles.container}>
            <LinearGradient
                colors={['#FFD700', '#FFA500', '#FF8C00']}
                start={{ x: 0, y: 0 }}
                end={{ x: 1, y: 1 }}
                style={[
                    styles.badge,
                    { width: badge, height: badge, borderRadius: badge / 2 }
                ]}
            >
                <Ionicons name="diamond" size={icon} color="white" />
            </LinearGradient>
            {showLabel && (
                <Text style={styles.label}>Premium</Text>
            )}
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        alignItems: 'center',
    },
    badge: {
        justifyContent: 'center',
        alignItems: 'center',
        shadowColor: '#FFD700',
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.4,
        shadowRadius: 4,
        elevation: 4,
    },
    label: {
        fontSize: 12,
        fontWeight: '600',
        color: '#FFD700',
        marginLeft: 6,
    },
});

export default PremiumBadge;
