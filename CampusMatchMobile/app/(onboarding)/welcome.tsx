// Onboarding Welcome Screen
import { View, Text, StyleSheet, TouchableOpacity, Image } from 'react-native';
import { useRouter } from 'expo-router';
import { LinearGradient } from 'expo-linear-gradient';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';

export default function WelcomeScreen() {
    const router = useRouter();

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <View style={styles.content}>
                <Ionicons name="heart-circle" size={120} color="#FF6B6B" />

                <Text style={styles.title}>Welcome to CampusMatch</Text>
                <Text style={styles.subtitle}>
                    Find your perfect match on campus
                </Text>

                <View style={styles.features}>
                    <Feature icon="people" text="Meet amazing students" />
                    <Feature icon="chatbubbles" text="Start conversations" />
                    <Feature icon="heart" text="Find your connection" />
                </View>

                <TouchableOpacity
                    style={styles.button}
                    onPress={() => router.push('/(onboarding)/profile-setup' as any)}
                >
                    <LinearGradient
                        colors={['#FF6B6B', '#FF8E53']}
                        style={styles.buttonGradient}
                    >
                        <Text style={styles.buttonText}>Get Started</Text>
                    </LinearGradient>
                </TouchableOpacity>
            </View>
        </LinearGradient>
    );
}

function Feature({ icon, text }: { icon: any; text: string }) {
    return (
        <View style={styles.feature}>
            <Ionicons name={icon} size={24} color="#FF6B6B" />
            <Text style={styles.featureText}>{text}</Text>
        </View>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1 },
    content: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 24,
    },
    title: {
        fontSize: 32,
        fontWeight: 'bold',
        color: Colors.dark.text,
        marginTop: 32,
        textAlign: 'center',
    },
    subtitle: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
        marginTop: 12,
        textAlign: 'center',
    },
    features: {
        marginTop: 48,
        gap: 20,
    },
    feature: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 12,
    },
    featureText: {
        fontSize: 16,
        color: Colors.dark.text,
    },
    button: {
        marginTop: 48,
        width: '100%',
    },
    buttonGradient: {
        paddingVertical: 16,
        borderRadius: 12,
        alignItems: 'center',
    },
    buttonText: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.white,
    },
});
