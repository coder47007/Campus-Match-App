import { Stack } from 'expo-router';
import Colors from '@/constants/Colors';

export default function SettingsLayout() {
    return (
        <Stack
            screenOptions={{
                headerStyle: {
                    backgroundColor: Colors.dark.background,
                },
                headerTintColor: Colors.dark.text,
                headerShadowVisible: false,
            }}
        >
            <Stack.Screen
                name="index"
                options={{
                    title: 'Settings',
                }}
            />
            <Stack.Screen
                name="preferences"
                options={{
                    title: 'Preferences',
                }}
            />
            <Stack.Screen
                name="blocked"
                options={{
                    title: 'Blocked Users',
                }}
            />
            <Stack.Screen
                name="change-password"
                options={{
                    title: 'Change Password',
                }}
            />
            <Stack.Screen
                name="delete-account"
                options={{
                    title: 'Delete Account',
                }}
            />
        </Stack>
    );
}
