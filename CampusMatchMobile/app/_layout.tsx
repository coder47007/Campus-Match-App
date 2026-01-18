import FontAwesome from '@expo/vector-icons/FontAwesome';
import { DarkTheme, DefaultTheme, ThemeProvider } from '@react-navigation/native';
import { useFonts } from 'expo-font';
import { Stack, useRouter, useSegments } from 'expo-router';
import * as SplashScreen from 'expo-splash-screen';
import { useEffect, useState } from 'react';
import { StatusBar } from 'expo-status-bar';
import 'react-native-reanimated';
import { GestureHandlerRootView } from 'react-native-gesture-handler';

import { useColorScheme } from '@/components/useColorScheme';
import { useAuthStore } from '@/stores/authStore';
import { useThemeStore } from '@/stores/themeStore';
import { notificationService } from '@/services/notifications';
import { apiService } from '@/services/api';
import Colors from '@/constants/Colors';


export {
  ErrorBoundary,
} from 'expo-router';

export const unstable_settings = {
  initialRouteName: 'index',
};

// Prevent the splash screen from auto-hiding before asset loading is complete.
SplashScreen.preventAutoHideAsync();

export default function RootLayout() {
  const [loaded, error] = useFonts({
    SpaceMono: require('../assets/fonts/SpaceMono-Regular.ttf'),
    ...FontAwesome.font,
  });

  useEffect(() => {
    if (error) throw error;
  }, [error]);

  useEffect(() => {
    if (loaded) {
      SplashScreen.hideAsync();
    }
  }, [loaded]);

  if (!loaded) {
    return null;
  }

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <RootLayoutNav />
    </GestureHandlerRootView>
  );
}

function RootLayoutNav() {
  const systemColorScheme = useColorScheme();
  const router = useRouter();
  const segments = useSegments();

  const { user, isInitialized, loadStoredAuth } = useAuthStore();
  const { effectiveTheme, loadStoredTheme, isLoading: themeLoading } = useThemeStore();
  const [authChecked, setAuthChecked] = useState(false);

  // Load stored theme and authentication on app start
  useEffect(() => {
    const initialize = async () => {
      await loadStoredTheme();
      await loadStoredAuth();
      setAuthChecked(true);
    };
    initialize();
  }, []);

  // Handle authentication-based routing
  useEffect(() => {
    if (!authChecked || !isInitialized || themeLoading) return;

    const inAuthGroup = segments[0] === '(auth)';

    if (!user && !inAuthGroup) {
      // Redirect to login if not authenticated
      router.replace('/(auth)/login');
    } else if (user && inAuthGroup) {
      // Redirect to main app if authenticated
      router.replace('/(tabs)/discover');
    }
  }, [user, segments, authChecked, isInitialized, themeLoading]);

  // Show nothing while checking auth and loading theme
  if (!authChecked || !isInitialized || themeLoading) {
    return null;
  }

  // Use effectiveTheme from themeStore (respects user preference)
  const isDark = effectiveTheme === 'dark';

  return (
    <ThemeProvider value={isDark ? DarkTheme : DefaultTheme}>
      <StatusBar style={isDark ? 'light' : 'dark'} />

      <Stack screenOptions={{ headerShown: false }}>
        <Stack.Screen name="index" options={{ headerShown: false }} />
        <Stack.Screen name="(auth)" options={{ headerShown: false }} />
        <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
        <Stack.Screen
          name="chat/[matchId]"
          options={{
            headerShown: true,
            presentation: 'card',
          }}
        />
        <Stack.Screen
          name="profile/[userId]"
          options={{
            headerShown: true,
            presentation: 'modal',
          }}
        />
        <Stack.Screen
          name="settings"
          options={{
            headerShown: false,
          }}
        />
      </Stack>
    </ThemeProvider>
  );
}
