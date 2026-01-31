import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    TouchableOpacity,
    Image,
    Alert,
    ActivityIndicator,
    SafeAreaView,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import * as ImagePicker from 'expo-image-picker';
import Colors from '@/constants/Colors';
import { useAuthStore } from '@/stores/authStore';
import { photosApi } from '@/services/photos';
import TextInput from '@/components/ui/TextInput';
import Button from '@/components/ui/Button';
import { profilesApi } from '@/services';

const MIN_PHOTOS = 3;

export default function ProfileSetupScreen() {
    const router = useRouter();
    const { user, refreshProfile, updateUser } = useAuthStore();

    const [photos, setPhotos] = useState<{ id?: number; url: string; isLocal?: boolean }[]>([]);
    const [bio, setBio] = useState(user?.bio || '');
    const [major, setMajor] = useState(user?.major || '');
    const [year, setYear] = useState(user?.year || '');
    const [university, setUniversity] = useState(user?.university || '');
    const [isUploading, setIsUploading] = useState(false);
    const [isSaving, setIsSaving] = useState(false);

    // Load existing photos
    useEffect(() => {
        if (user?.photos) {
            setPhotos(user.photos.map(p => ({ id: p.id, url: p.url })));
        }
    }, [user]);

    const pickImage = async () => {
        const permissionResult = await ImagePicker.requestMediaLibraryPermissionsAsync();

        if (!permissionResult.granted) {
            Alert.alert('Permission Required', 'Please allow access to your photo library.');
            return;
        }

        const result = await ImagePicker.launchImageLibraryAsync({
            mediaTypes: ImagePicker.MediaTypeOptions.Images,
            allowsEditing: true,
            aspect: [3, 4],
            quality: 0.8,
        });

        if (!result.canceled && result.assets[0]) {
            await uploadPhoto(result.assets[0].uri);
        }
    };

    const uploadPhoto = async (uri: string) => {
        setIsUploading(true);
        try {
            const response = await photosApi.uploadPhoto(uri);
            setPhotos(prev => [...prev, { id: response.id, url: response.url }]);
            await refreshProfile();
        } catch (error) {
            Alert.alert('Upload Failed', 'Could not upload your photo. Please try again.');
        } finally {
            setIsUploading(false);
        }
    };

    const removePhoto = async (index: number) => {
        const photo = photos[index];
        if (photo.id) {
            try {
                await photosApi.deletePhoto(photo.id);
                setPhotos(prev => prev.filter((_, i) => i !== index));
                await refreshProfile();
            } catch (error) {
                Alert.alert('Error', 'Could not remove photo.');
            }
        } else {
            setPhotos(prev => prev.filter((_, i) => i !== index));
        }
    };

    const handleComplete = async () => {
        if (photos.length < MIN_PHOTOS) {
            Alert.alert('More Photos Needed', `Please upload at least ${MIN_PHOTOS} photos to continue.`);
            return;
        }

        setIsSaving(true);
        try {
            // Save profile info
            const profileData = await profilesApi.updateMyProfile({
                name: user?.name || '',
                bio: bio.trim() || undefined,
                major: major.trim() || undefined,
                year: year.trim() || undefined,
                university: university.trim() || undefined,
            });

            updateUser(profileData);
            router.replace('/(tabs)/discover');
        } catch (error) {
            Alert.alert('Error', 'Could not save your profile. Please try again.');
        } finally {
            setIsSaving(false);
        }
    };

    const photosNeeded = Math.max(0, MIN_PHOTOS - photos.length);
    const canContinue = photos.length >= MIN_PHOTOS;

    return (
        <SafeAreaView style={styles.container}>
            <ScrollView
                contentContainerStyle={styles.scrollContent}
                showsVerticalScrollIndicator={false}
            >
                {/* Header */}
                <View style={styles.header}>
                    <Image
                        source={require('@/assets/images/campus-match-logo.png')}
                        style={styles.logo}
                        resizeMode="contain"
                    />
                    <Text style={styles.title}>Complete Your Profile</Text>
                    <Text style={styles.subtitle}>
                        Add at least {MIN_PHOTOS} photos to start matching
                    </Text>
                </View>

                {/* Photos Section */}
                <View style={styles.section}>
                    <View style={styles.sectionHeader}>
                        <Text style={styles.sectionTitle}>Your Photos</Text>
                        <Text style={[
                            styles.photoCount,
                            canContinue ? styles.photoCountComplete : styles.photoCountIncomplete
                        ]}>
                            {photos.length}/{MIN_PHOTOS} minimum
                        </Text>
                    </View>

                    <View style={styles.photosGrid}>
                        {[0, 1, 2, 3, 4, 5].map((index) => {
                            const photo = photos[index];
                            return (
                                <TouchableOpacity
                                    key={index}
                                    style={[
                                        styles.photoSlot,
                                        index < MIN_PHOTOS && !photo && styles.requiredSlot,
                                    ]}
                                    onPress={() => photo ? removePhoto(index) : pickImage()}
                                    disabled={isUploading}
                                >
                                    {photo ? (
                                        <>
                                            <Image source={{ uri: photo.url }} style={styles.photo} />
                                            <View style={styles.removeButton}>
                                                <Ionicons name="close-circle" size={24} color="#FF4444" />
                                            </View>
                                        </>
                                    ) : (
                                        <View style={styles.addPhotoContent}>
                                            {isUploading && index === photos.length ? (
                                                <ActivityIndicator color={Colors.primary.main} />
                                            ) : (
                                                <>
                                                    <Ionicons
                                                        name="add"
                                                        size={32}
                                                        color={index < MIN_PHOTOS ? Colors.primary.main : Colors.dark.textMuted}
                                                    />
                                                    {index < MIN_PHOTOS && (
                                                        <Text style={styles.requiredText}>Required</Text>
                                                    )}
                                                </>
                                            )}
                                        </View>
                                    )}
                                </TouchableOpacity>
                            );
                        })}
                    </View>

                    {photosNeeded > 0 && (
                        <View style={styles.warningBanner}>
                            <Ionicons name="camera" size={20} color="#FFB800" />
                            <Text style={styles.warningText}>
                                Add {photosNeeded} more photo{photosNeeded > 1 ? 's' : ''} to continue
                            </Text>
                        </View>
                    )}
                </View>

                {/* Profile Info Section */}
                <View style={styles.section}>
                    <Text style={styles.sectionTitle}>About You (Optional)</Text>

                    <TextInput
                        label="Bio"
                        placeholder="Tell others about yourself..."
                        value={bio}
                        onChangeText={setBio}
                        multiline
                        numberOfLines={3}
                    />

                    <TextInput
                        label="Major / Field of Study"
                        placeholder="e.g. Computer Science"
                        value={major}
                        onChangeText={setMajor}
                        leftIcon="school-outline"
                    />

                    <TextInput
                        label="Year"
                        placeholder="e.g. Junior, 3rd Year"
                        value={year}
                        onChangeText={setYear}
                        leftIcon="calendar-outline"
                    />

                    <TextInput
                        label="University"
                        placeholder="e.g. State University"
                        value={university}
                        onChangeText={setUniversity}
                        leftIcon="business-outline"
                    />
                </View>

                {/* Continue Button */}
                <View style={styles.buttonContainer}>
                    <Button
                        variant={canContinue ? "primary" : "outline"}
                        size="large"
                        fullWidth
                        onPress={handleComplete}
                        loading={isSaving}
                        disabled={!canContinue || isSaving}
                    >
                        {canContinue ? 'Start Matching' : `Add ${photosNeeded} More Photo${photosNeeded > 1 ? 's' : ''}`}
                    </Button>
                </View>
            </ScrollView>
        </SafeAreaView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: Colors.dark.background,
    },
    scrollContent: {
        padding: 20,
        paddingBottom: 40,
    },
    header: {
        alignItems: 'center',
        marginBottom: 32,
        marginTop: 20,
    },
    logo: {
        width: 80,
        height: 80,
        borderRadius: 40,
        marginBottom: 16,
    },
    title: {
        fontSize: 28,
        fontWeight: '700',
        color: Colors.white,
        marginBottom: 8,
    },
    subtitle: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
    },
    section: {
        marginBottom: 24,
    },
    sectionHeader: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 16,
    },
    sectionTitle: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 12,
    },
    photoCount: {
        fontSize: 14,
        fontWeight: '600',
    },
    photoCountComplete: {
        color: Colors.semantic.success,
    },
    photoCountIncomplete: {
        color: '#FFB800',
    },
    photosGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 12,
    },
    photoSlot: {
        width: '31%',
        aspectRatio: 3 / 4,
        borderRadius: 12,
        backgroundColor: Colors.dark.surface,
        overflow: 'hidden',
        borderWidth: 2,
        borderColor: Colors.dark.border,
        borderStyle: 'dashed',
    },
    requiredSlot: {
        borderColor: Colors.primary.main,
    },
    photo: {
        width: '100%',
        height: '100%',
    },
    removeButton: {
        position: 'absolute',
        top: 4,
        right: 4,
        backgroundColor: 'white',
        borderRadius: 12,
    },
    addPhotoContent: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    requiredText: {
        fontSize: 10,
        color: Colors.primary.main,
        marginTop: 4,
    },
    warningBanner: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(255, 184, 0, 0.1)',
        borderRadius: 12,
        padding: 12,
        marginTop: 16,
        gap: 8,
    },
    warningText: {
        color: '#FFB800',
        fontSize: 14,
        fontWeight: '500',
    },
    buttonContainer: {
        marginTop: 16,
    },
});
