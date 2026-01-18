import React from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    SafeAreaView,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { useThemeStore, ThemeMode } from '@/stores/themeStore';
import Colors from '@/constants/Colors';
import { LinearGradient } from 'expo-linear-gradient';

interface ThemeOptionProps {
    mode: ThemeMode;
    title: string;
    subtitle: string;
    icon: keyof typeof Ionicons.glyphMap;
    isSelected: boolean;
    onPress: () => void;
}

function ThemeOption({ mode, title, subtitle, icon, isSelected, onPress }: ThemeOptionProps) {
    return (
        <TouchableOpacity
            style={[styles.option, isSelected && styles.optionSelected]}
            onPress={onPress}
        >
            <View style={[styles.iconContainer, isSelected && styles.iconContainerSelected]}>
                <Ionicons
                    name={icon}
                    size={24}
                    color={isSelected ? Colors.white : Colors.dark.textSecondary}
                />
            </View>
            <View style={styles.optionContent}>
                <Text style={[styles.optionTitle, isSelected && styles.optionTitleSelected]}>
                    {title}
                </Text>
                <Text style={styles.optionSubtitle}>{subtitle}</Text>
            </View>
            {isSelected && (
                <Ionicons name="checkmark-circle" size={24} color={Colors.primary.main} />
            )}
        </TouchableOpacity>
    );
}

export default function AppearanceScreen() {
    const { themeMode, setThemeMode } = useThemeStore();

    const themeOptions: { mode: ThemeMode; title: string; subtitle: string; icon: keyof typeof Ionicons.glyphMap }[] = [
        {
            mode: 'system',
            title: 'System',
            subtitle: 'Match your device settings',
            icon: 'phone-portrait-outline',
        },
        {
            mode: 'dark',
            title: 'Dark',
            subtitle: 'Always use dark mode',
            icon: 'moon-outline',
        },
        {
            mode: 'light',
            title: 'Light',
            subtitle: 'Always use light mode',
            icon: 'sunny-outline',
        },
    ];

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <SafeAreaView style={styles.safeArea}>
                <View style={styles.content}>
                    <Text style={styles.sectionTitle}>Theme</Text>
                    <Text style={styles.sectionSubtitle}>
                        Choose how CampusMatch looks to you
                    </Text>

                    <View style={styles.optionsContainer}>
                        {themeOptions.map((option) => (
                            <ThemeOption
                                key={option.mode}
                                mode={option.mode}
                                title={option.title}
                                subtitle={option.subtitle}
                                icon={option.icon}
                                isSelected={themeMode === option.mode}
                                onPress={() => setThemeMode(option.mode)}
                            />
                        ))}
                    </View>

                    <View style={styles.infoBox}>
                        <Ionicons name="information-circle-outline" size={20} color={Colors.dark.textSecondary} />
                        <Text style={styles.infoText}>
                            System mode will automatically switch between light and dark based on your device settings.
                        </Text>
                    </View>
                </View>
            </SafeAreaView>
        </LinearGradient>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    safeArea: {
        flex: 1,
    },
    content: {
        flex: 1,
        padding: 20,
    },
    sectionTitle: {
        fontSize: 28,
        fontWeight: 'bold',
        color: Colors.dark.text,
        marginBottom: 8,
    },
    sectionSubtitle: {
        fontSize: 15,
        color: Colors.dark.textSecondary,
        marginBottom: 24,
    },
    optionsContainer: {
        gap: 12,
    },
    option: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        padding: 16,
        borderRadius: 16,
        borderWidth: 2,
        borderColor: 'transparent',
    },
    optionSelected: {
        borderColor: Colors.primary.main,
        backgroundColor: 'rgba(255, 64, 129, 0.1)',
    },
    iconContainer: {
        width: 44,
        height: 44,
        borderRadius: 12,
        backgroundColor: Colors.dark.card,
        justifyContent: 'center',
        alignItems: 'center',
        marginRight: 14,
    },
    iconContainerSelected: {
        backgroundColor: Colors.primary.main,
    },
    optionContent: {
        flex: 1,
    },
    optionTitle: {
        fontSize: 17,
        fontWeight: '600',
        color: Colors.dark.text,
        marginBottom: 2,
    },
    optionTitleSelected: {
        color: Colors.primary.main,
    },
    optionSubtitle: {
        fontSize: 13,
        color: Colors.dark.textMuted,
    },
    infoBox: {
        flexDirection: 'row',
        alignItems: 'flex-start',
        backgroundColor: Colors.dark.surface,
        padding: 16,
        borderRadius: 12,
        marginTop: 24,
        gap: 10,
    },
    infoText: {
        flex: 1,
        fontSize: 13,
        color: Colors.dark.textSecondary,
        lineHeight: 18,
    },
});
