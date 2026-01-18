import { Redirect } from 'expo-router';
import { useAuthStore } from '@/stores/authStore';
import { View, ActivityIndicator, StyleSheet } from 'react-native';
import Colors from '@/constants/Colors';

export default function Index() {
    const { user, isInitialized } = useAuthStore();

    // Show loading while checking auth
    if (!isInitialized) {
        return (
            <View style={styles.container}>
                <ActivityIndicator size="large" color={Colors.primary.main} />
            </View>
        );
    }

    // Redirect based on auth state
    if (user) {
        return <Redirect href="/(tabs)/discover" />;
    }

    return <Redirect href="/(auth)/login" />;
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: Colors.dark.background,
    },
});
