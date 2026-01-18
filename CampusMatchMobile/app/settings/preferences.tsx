import React, { useEffect, useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    Switch,
    ActivityIndicator,
    Alert,
} from 'react-native';
import Slider from '@react-native-community/slider';
import { useSettingsStore } from '@/stores/settingsStore';
import Colors from '@/constants/Colors';
import { MIN_AGE, MAX_AGE, MIN_DISTANCE, MAX_DISTANCE } from '@/constants/config';

export default function PreferencesScreen() {
    const { settings, isLoading, isSaving, fetchSettings, updateSettings } = useSettingsStore();

    const [minAge, setMinAge] = useState(MIN_AGE);
    const [maxAge, setMaxAge] = useState(MAX_AGE);
    const [maxDistance, setMaxDistance] = useState(50);
    const [showOnlineStatus, setShowOnlineStatus] = useState(true);
    const [notifyOnMatch, setNotifyOnMatch] = useState(true);
    const [notifyOnMessage, setNotifyOnMessage] = useState(true);
    const [notifyOnSuperLike, setNotifyOnSuperLike] = useState(true);
    const [isProfileHidden, setIsProfileHidden] = useState(false);

    useEffect(() => {
        fetchSettings();
    }, []);

    useEffect(() => {
        if (settings) {
            setMinAge(settings.minAgePreference);
            setMaxAge(settings.maxAgePreference);
            setMaxDistance(settings.maxDistancePreference);
            setShowOnlineStatus(settings.showOnlineStatus);
            setNotifyOnMatch(settings.notifyOnMatch);
            setNotifyOnMessage(settings.notifyOnMessage);
            setNotifyOnSuperLike(settings.notifyOnSuperLike);
            setIsProfileHidden(settings.isProfileHidden);
        }
    }, [settings]);

    const handleSave = async (key: string, value: number | boolean) => {
        try {
            await updateSettings({ [key]: value });
        } catch (err) {
            Alert.alert('Error', 'Failed to save setting');
        }
    };

    if (isLoading) {
        return (
            <View style={[styles.container, styles.centerContent]}>
                <ActivityIndicator size="large" color={Colors.primary.main} />
            </View>
        );
    }

    return (
        <ScrollView style={styles.container} showsVerticalScrollIndicator={false}>
            {/* Age Range */}
            <View style={styles.section}>
                <Text style={styles.sectionTitle}>Age Range</Text>
                <View style={styles.sliderContainer}>
                    <View style={styles.sliderLabels}>
                        <Text style={styles.sliderLabel}>Min: {minAge}</Text>
                        <Text style={styles.sliderLabel}>Max: {maxAge}</Text>
                    </View>
                    <Text style={styles.sliderSubtitle}>Minimum Age</Text>
                    <Slider
                        style={styles.slider}
                        minimumValue={MIN_AGE}
                        maximumValue={MAX_AGE}
                        step={1}
                        value={minAge}
                        onValueChange={setMinAge}
                        onSlidingComplete={(value) => {
                            if (value > maxAge) {
                                setMaxAge(value);
                                handleSave('maxAgePreference', value);
                            }
                            handleSave('minAgePreference', value);
                        }}
                        minimumTrackTintColor={Colors.primary.main}
                        maximumTrackTintColor={Colors.dark.border}
                        thumbTintColor={Colors.primary.main}
                    />
                    <Text style={styles.sliderSubtitle}>Maximum Age</Text>
                    <Slider
                        style={styles.slider}
                        minimumValue={MIN_AGE}
                        maximumValue={MAX_AGE}
                        step={1}
                        value={maxAge}
                        onValueChange={setMaxAge}
                        onSlidingComplete={(value) => {
                            if (value < minAge) {
                                setMinAge(value);
                                handleSave('minAgePreference', value);
                            }
                            handleSave('maxAgePreference', value);
                        }}
                        minimumTrackTintColor={Colors.primary.main}
                        maximumTrackTintColor={Colors.dark.border}
                        thumbTintColor={Colors.primary.main}
                    />
                </View>
            </View>

            {/* Distance */}
            <View style={styles.section}>
                <Text style={styles.sectionTitle}>Maximum Distance</Text>
                <View style={styles.sliderContainer}>
                    <Text style={styles.sliderLabel}>{maxDistance} miles</Text>
                    <Slider
                        style={styles.slider}
                        minimumValue={MIN_DISTANCE}
                        maximumValue={MAX_DISTANCE}
                        step={1}
                        value={maxDistance}
                        onValueChange={setMaxDistance}
                        onSlidingComplete={(value) => handleSave('maxDistancePreference', value)}
                        minimumTrackTintColor={Colors.primary.main}
                        maximumTrackTintColor={Colors.dark.border}
                        thumbTintColor={Colors.primary.main}
                    />
                </View>
            </View>

            {/* Privacy */}
            <View style={styles.section}>
                <Text style={styles.sectionTitle}>Privacy</Text>

                <View style={styles.settingRow}>
                    <View style={styles.settingInfo}>
                        <Text style={styles.settingTitle}>Show Online Status</Text>
                        <Text style={styles.settingSubtitle}>Let others see when you're active</Text>
                    </View>
                    <Switch
                        value={showOnlineStatus}
                        onValueChange={(value) => {
                            setShowOnlineStatus(value);
                            handleSave('showOnlineStatus', value);
                        }}
                        trackColor={{ false: Colors.dark.border, true: Colors.primary.main }}
                        thumbColor={Colors.white}
                    />
                </View>

                <View style={styles.settingRow}>
                    <View style={styles.settingInfo}>
                        <Text style={styles.settingTitle}>Hide Profile</Text>
                        <Text style={styles.settingSubtitle}>Your profile won't appear in discover</Text>
                    </View>
                    <Switch
                        value={isProfileHidden}
                        onValueChange={(value) => {
                            setIsProfileHidden(value);
                            handleSave('isProfileHidden', value);
                        }}
                        trackColor={{ false: Colors.dark.border, true: Colors.primary.main }}
                        thumbColor={Colors.white}
                    />
                </View>
            </View>

            {/* Notifications */}
            <View style={styles.section}>
                <Text style={styles.sectionTitle}>Notifications</Text>

                <View style={styles.settingRow}>
                    <View style={styles.settingInfo}>
                        <Text style={styles.settingTitle}>New Matches</Text>
                    </View>
                    <Switch
                        value={notifyOnMatch}
                        onValueChange={(value) => {
                            setNotifyOnMatch(value);
                            handleSave('notifyOnMatch', value);
                        }}
                        trackColor={{ false: Colors.dark.border, true: Colors.primary.main }}
                        thumbColor={Colors.white}
                    />
                </View>

                <View style={styles.settingRow}>
                    <View style={styles.settingInfo}>
                        <Text style={styles.settingTitle}>Messages</Text>
                    </View>
                    <Switch
                        value={notifyOnMessage}
                        onValueChange={(value) => {
                            setNotifyOnMessage(value);
                            handleSave('notifyOnMessage', value);
                        }}
                        trackColor={{ false: Colors.dark.border, true: Colors.primary.main }}
                        thumbColor={Colors.white}
                    />
                </View>

                <View style={styles.settingRow}>
                    <View style={styles.settingInfo}>
                        <Text style={styles.settingTitle}>Super Likes</Text>
                    </View>
                    <Switch
                        value={notifyOnSuperLike}
                        onValueChange={(value) => {
                            setNotifyOnSuperLike(value);
                            handleSave('notifyOnSuperLike', value);
                        }}
                        trackColor={{ false: Colors.dark.border, true: Colors.primary.main }}
                        thumbColor={Colors.white}
                    />
                </View>
            </View>

            <View style={styles.bottomPadding} />
        </ScrollView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: Colors.dark.background,
    },
    centerContent: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    section: {
        marginTop: 24,
        paddingHorizontal: 16,
    },
    sectionTitle: {
        fontSize: 13,
        fontWeight: '600',
        color: Colors.dark.textSecondary,
        textTransform: 'uppercase',
        letterSpacing: 1,
        marginBottom: 12,
    },
    sliderContainer: {
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        padding: 16,
    },
    sliderLabels: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginBottom: 8,
    },
    sliderLabel: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.dark.text,
    },
    sliderSubtitle: {
        fontSize: 12,
        color: Colors.dark.textMuted,
        marginTop: 8,
        marginBottom: 4,
    },
    slider: {
        width: '100%',
        height: 40,
    },
    settingRow: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        backgroundColor: Colors.dark.surface,
        padding: 16,
        borderRadius: 12,
        marginBottom: 8,
    },
    settingInfo: {
        flex: 1,
        marginRight: 12,
    },
    settingTitle: {
        fontSize: 16,
        color: Colors.dark.text,
    },
    settingSubtitle: {
        fontSize: 13,
        color: Colors.dark.textMuted,
        marginTop: 2,
    },
    bottomPadding: {
        height: 40,
    },
});
