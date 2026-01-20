import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    TouchableOpacity,
    Image,
    TextInput,
    Alert,
    ActivityIndicator,
    SafeAreaView,
    Dimensions,
    Modal,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import * as ImagePicker from 'expo-image-picker';
import { useAuthStore } from '@/stores/authStore';
import { profilesApi, photosApi } from '@/services';
import { InterestDto, PhotoDto } from '@/types';
import FilterModal, { FilterState } from '@/components/FilterModal';
import UniversityAutocomplete from '@/components/UniversityAutocomplete';
import InterestSelector from '@/components/InterestSelector';
import ProfileCompletionBar, { calculateProfileCompletion } from '@/components/ProfileCompletionBar';
import { ProfileHeader, ProfilePhotos, ProfileActions } from '@/components/profile';
import Colors from '@/constants/Colors';
import { InterestItem } from '@/data/interests';


const { width: SCREEN_WIDTH } = Dimensions.get('window');
const PHOTO_SIZE = (SCREEN_WIDTH - 64) / 3;

// Prompt options
const promptOptions = [
    "My biggest study distraction is...",
    "I know the best spot on campus for...",
    "My go-to coffee order is...",
    "On weekends you'll find me...",
    "I'm weirdly good at...",
    "Best class I've ever taken...",
    "My unpopular opinion is...",
    "I geek out about...",
];

// Major / Field of Study options
const majorOptions = [
    'Accounting', 'Actuarial Science', 'Aerospace Engineering', 'Agriculture', 'Anthropology',
    'Architecture', 'Art History', 'Biochemistry', 'Biology', 'Biomedical Engineering',
    'Business Administration', 'Chemical Engineering', 'Chemistry', 'Civil Engineering',
    'Communications', 'Computer Engineering', 'Computer Science', 'Criminology',
    'Data Science', 'Dentistry', 'Economics', 'Education', 'Electrical Engineering',
    'English', 'Environmental Science', 'Film Studies', 'Finance', 'Fine Arts',
    'French', 'Geography', 'Geology', 'Graphic Design', 'Health Sciences', 'History',
    'Human Resources', 'Information Technology', 'International Relations', 'Journalism',
    'Kinesiology', 'Law', 'Linguistics', 'Marketing', 'Mathematics', 'Mechanical Engineering',
    'Media Studies', 'Medicine', 'Music', 'Neuroscience', 'Nursing', 'Nutrition',
    'Pharmacy', 'Philosophy', 'Physics', 'Political Science', 'Psychology', 'Public Health',
    'Social Work', 'Sociology', 'Software Engineering', 'Statistics', 'Theatre',
    'Urban Planning', 'Veterinary Medicine', 'Other'
];

export default function ProfileScreen() {
    const router = useRouter();
    const { user, updateUser, logout } = useAuthStore();

    const [isEditing, setIsEditing] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    const [photos, setPhotos] = useState<PhotoDto[]>([]);
    const [allInterests, setAllInterests] = useState<InterestDto[]>([]);

    // Filter modal
    const [showFilterModal, setShowFilterModal] = useState(false);
    const [filters, setFilters] = useState<FilterState | null>(null);

    // Prompts
    const [prompts, setPrompts] = useState<{ question: string; answer: string }[]>([
        { question: promptOptions[0], answer: '' },
        { question: promptOptions[2], answer: '' },
        { question: promptOptions[3], answer: '' },
    ]);
    const [showPromptPicker, setShowPromptPicker] = useState<number | null>(null);

    // University
    const [showUniversityPicker, setShowUniversityPicker] = useState(false);
    const [selectedUniversity, setSelectedUniversity] = useState(user?.university || '');
    const [customUniversity, setCustomUniversity] = useState('');

    // Major picker
    const [showMajorPicker, setShowMajorPicker] = useState(false);
    const [customMajor, setCustomMajor] = useState('');

    // Edit state
    const [name, setName] = useState(user?.name || '');
    const [bio, setBio] = useState(user?.bio || '');
    const [age, setAge] = useState(user?.age?.toString() || '');
    const [major, setMajor] = useState(user?.major || '');
    const [year, setYear] = useState(user?.year || '');
    const [selectedInterests, setSelectedInterests] = useState<number[]>(
        user?.interests?.map(i => i.id) || []
    );
    const [selectedLookingFor, setSelectedLookingFor] = useState<string[]>([]);

    // Interest tags (new system) - Store full objects to handle custom tags and display
    const [selectedInterestTags, setSelectedInterestTags] = useState<InterestItem[]>([]);

    useEffect(() => {
        fetchData();
    }, []);

    useEffect(() => {
        if (user) {
            setName(user.name);
            setBio(user.bio || '');
            setAge(user.age?.toString() || '');
            setMajor(user.major || '');
            setYear(user.year || '');
            setSelectedUniversity(user.university || '');
            setSelectedInterests(user.interests?.map(i => i.id) || []);
        }
    }, [user]);

    const fetchData = async () => {
        try {
            const [photosData, interestsData] = await Promise.all([
                photosApi.getMyPhotos(),
                profilesApi.getInterests(),
            ]);
            setPhotos(photosData);
            setAllInterests(interestsData);
        } catch (err) {
            console.error('Error fetching profile data:', err);
        }
    };

    const handleSave = async () => {
        setIsSaving(true);
        try {
            await profilesApi.updateMyProfile({
                name,
                bio: bio || undefined,
                age: age ? parseInt(age) : undefined,
                major: major || undefined,
                year: year || undefined,
                university: selectedUniversity || undefined,
                gender: user?.gender,
                preferredGender: user?.preferredGender,
                phoneNumber: user?.phoneNumber,
                instagramHandle: user?.instagramHandle,
                photoUrl: user?.photoUrl,
                // Note: We are not syncing the new string-based interests to the backend yet 
                // as the backend expects numeric IDs. This is a UI demo for the advanced selector.
            });

            const freshProfile = await profilesApi.getMyProfile();
            updateUser(freshProfile);

            setIsEditing(false);
            Alert.alert('Success', 'Profile updated successfully');
        } catch (error) {
            console.error('Error updating profile:', error);
            Alert.alert('Error', 'Failed to update profile');
        } finally {
            setIsSaving(false);
        }
    };

    const handleAddPhoto = async () => {
        const result = await ImagePicker.launchImageLibraryAsync({
            mediaTypes: ImagePicker.MediaTypeOptions.Images,
            allowsEditing: true,
            aspect: [3, 4],
            quality: 0.8,
        });

        if (!result.canceled && result.assets[0]) {
            try {
                const asset = result.assets[0];
                await photosApi.uploadPhoto(
                    asset.uri,
                    asset.mimeType || 'image/jpeg'
                );
                await fetchData();
            } catch (err) {
                console.error('Photo upload error:', err);
                Alert.alert('Error', 'Failed to upload photo');
            }
        }
    };

    const handleDeletePhoto = async (photoId: number) => {
        Alert.alert(
            'Delete Photo',
            'Are you sure you want to delete this photo?',
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Delete',
                    style: 'destructive',
                    onPress: async () => {
                        try {
                            await photosApi.deletePhoto(photoId);
                            await fetchData();
                        } catch (err) {
                            Alert.alert('Error', 'Failed to delete photo');
                        }
                    },
                },
            ]
        );
    };

    const handleLogout = () => {
        Alert.alert(
            'Logout',
            'Are you sure you want to logout?',
            [
                { text: 'Cancel', style: 'cancel' },
                { text: 'Logout', style: 'destructive', onPress: logout },
            ]
        );
    };

    const handleApplyFilters = (newFilters: FilterState) => {
        setFilters(newFilters);
    };

    const mainPhoto = photos.find(p => p.displayOrder === 0) || photos[0];

    if (!user) {
        return (
            <LinearGradient
                colors={Colors.gradients.dark}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <View style={styles.centerContent}>
                        <ActivityIndicator size="large" color="#7C3AED" />
                    </View>
                </SafeAreaView>
            </LinearGradient>
        );
    }

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <SafeAreaView style={styles.safeArea}>
                {/* Header */}
                <View style={styles.header}>
                    <Text style={styles.headerTitle}>User Profile</Text>
                </View>

                <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.content}>
                    {/* Profile Avatar Section - Using extracted component */}
                    <ProfileHeader
                        name={user.name}
                        age={user.age}
                        photoUrl={mainPhoto?.url || user.photoUrl}
                        isVerified={true}
                    />

                    {/* Action Buttons Row - Using extracted component */}
                    <ProfileActions
                        userId={user.id}
                        isEditing={isEditing}
                        onEditToggle={() => setIsEditing(!isEditing)}
                        onFilterPress={() => setShowFilterModal(true)}
                    />

                    {/* Profile Completion Bar */}
                    <ProfileCompletionBar
                        fields={calculateProfileCompletion({
                            name: user.name,
                            bio: user.bio || bio,
                            age: user.age,
                            major: user.major || major,
                            year: user.year || year,
                            university: user.university || selectedUniversity,
                            photoUrl: user.photoUrl,
                            photos,
                            interests: user.interests,
                        })}
                        showDetails={true}
                        onFieldPress={(fieldId) => {
                            // Scroll to the relevant section or enable editing
                            if (!isEditing) setIsEditing(true);
                        }}
                    />

                    {/* University Section */}
                    <View style={styles.section}>

                        <Text style={styles.sectionTitle}>Campus</Text>
                        <TouchableOpacity
                            style={styles.universitySelector}
                            onPress={() => setShowUniversityPicker(true)}
                        >
                            <Ionicons name="school" size={20} color="#7C3AED" />
                            <Text style={styles.universityText}>
                                {selectedUniversity || 'Select your Campus'}
                            </Text>
                            <Ionicons name="chevron-down" size={20} color="rgba(255,255,255,0.5)" />
                        </TouchableOpacity>
                    </View>

                    {/* Academic Year Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Academic Year</Text>
                        <View style={styles.pillsContainer}>
                            {['Freshman', 'Sophomore', 'Junior', 'Senior', 'Grad Student', 'Alumni'].map((yearOption) => (
                                <TouchableOpacity
                                    key={yearOption}
                                    style={[
                                        styles.yearPill,
                                        year === yearOption && styles.yearPillActive,
                                    ]}
                                    onPress={() => setYear(yearOption)}
                                >
                                    <Text style={[
                                        styles.yearPillText,
                                        year === yearOption && styles.yearPillTextActive,
                                    ]}>
                                        {yearOption}
                                    </Text>
                                </TouchableOpacity>
                            ))}
                        </View>
                    </View>

                    {/* Major / Field of Study */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Major / Field of Study</Text>
                        <TouchableOpacity
                            style={styles.universitySelector}
                            onPress={() => setShowMajorPicker(true)}
                        >
                            <Ionicons name="school-outline" size={20} color="#7C3AED" />
                            <Text style={styles.universityText}>
                                {major || 'Select your major'}
                            </Text>
                            <Ionicons name="chevron-down" size={20} color="rgba(255,255,255,0.5)" />
                        </TouchableOpacity>
                    </View>

                    {/* Looking For Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Looking For</Text>
                        <View style={styles.lookingForGrid}>
                            {[
                                { id: 'relationship', label: 'Relationship', icon: 'heart' },
                                { id: 'casual', label: 'Casual', icon: 'flame' },
                                { id: 'study', label: 'Study Buddy', icon: 'book' },
                                { id: 'activity', label: 'Activity Partner', icon: 'bicycle' },
                            ].map((option) => (
                                <TouchableOpacity
                                    key={option.id}
                                    style={[
                                        styles.lookingForItem,
                                        selectedLookingFor.includes(option.id) && styles.lookingForItemActive,
                                    ]}
                                    onPress={() => {
                                        setSelectedLookingFor(prev =>
                                            prev.includes(option.id)
                                                ? prev.filter(i => i !== option.id)
                                                : [...prev, option.id]
                                        );
                                    }}
                                >
                                    <Ionicons
                                        name={option.icon as any}
                                        size={24}
                                        color={selectedLookingFor.includes(option.id) ? '#7C3AED' : 'rgba(255,255,255,0.5)'}
                                    />
                                    <Text style={[
                                        styles.lookingForText,
                                        selectedLookingFor.includes(option.id) && styles.lookingForTextActive,
                                    ]}>
                                        {option.label}
                                    </Text>
                                </TouchableOpacity>
                            ))}
                        </View>
                    </View>

                    {/* Prompts Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>My Prompts</Text>
                        {prompts.map((prompt, index) => (
                            <View key={index} style={styles.promptCard}>
                                <TouchableOpacity
                                    style={styles.promptQuestion}
                                    onPress={() => setShowPromptPicker(index)}
                                >
                                    <Text style={styles.promptQuestionText}>{prompt.question}</Text>
                                    <Ionicons name="chevron-down" size={16} color="rgba(255,255,255,0.5)" />
                                </TouchableOpacity>
                                <TextInput
                                    style={styles.promptAnswer}
                                    value={prompt.answer}
                                    onChangeText={(text) => {
                                        const newPrompts = [...prompts];
                                        newPrompts[index].answer = text;
                                        setPrompts(newPrompts);
                                    }}
                                    placeholder="Your answer..."
                                    placeholderTextColor="rgba(255,255,255,0.3)"
                                    multiline
                                />
                            </View>
                        ))}
                    </View>

                    {/* My Photos Section - Using extracted component */}
                    <ProfilePhotos
                        photos={photos}
                        maxPhotos={6}
                        onPhotosChange={fetchData}
                        editable={true}
                    />

                    {/* Bio Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Bio</Text>
                        {isEditing ? (
                            <TextInput
                                style={styles.bioInput}
                                value={bio}
                                onChangeText={setBio}
                                placeholder="Tell others about yourself..."
                                placeholderTextColor="rgba(255,255,255,0.4)"
                                multiline
                                numberOfLines={4}
                            />
                        ) : (
                            <Text style={styles.bioText}>
                                {bio || 'No bio yet. Tap Edit Profile to add one!'}
                            </Text>
                        )}
                    </View>

                    {/* Interests Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>My Interests</Text>
                        {isEditing ? (
                            <InterestSelector
                                selectedIds={selectedInterestTags.map(i => i.id)}
                                onToggle={(id, label, emoji) => {
                                    if (selectedInterestTags.some(item => item.id === id)) {
                                        setSelectedInterestTags(prev => prev.filter(t => t.id !== id));
                                    } else {
                                        if (selectedInterestTags.length < 5) {
                                            setSelectedInterestTags(prev => [...prev, { id, label, emoji }]);
                                        }
                                    }
                                }}
                                maxSelection={5}
                            />
                        ) : (
                            <View style={styles.interestsGrid}>
                                {selectedInterestTags.length > 0 ? (
                                    selectedInterestTags.map((interest) => (
                                        <View key={interest.id} style={styles.interestTag}>
                                            <Text style={styles.interestEmoji}>{interest.emoji}</Text>
                                            <Text style={styles.interestName}>{interest.label}</Text>
                                        </View>
                                    ))
                                ) : (
                                    <Text style={styles.bioText}>No interests selected.</Text>
                                )}
                            </View>
                        )}
                    </View>

                    {/* Save Button */}
                    {isEditing && (
                        <TouchableOpacity
                            style={styles.saveButton}
                            onPress={handleSave}
                            disabled={isSaving}
                        >
                            <LinearGradient
                                colors={['#7C3AED', '#6D28D9']}
                                style={styles.saveButtonGradient}
                            >
                                {isSaving ? (
                                    <ActivityIndicator color={Colors.white} />
                                ) : (
                                    <Text style={styles.saveButtonText}>Save Changes</Text>
                                )}
                            </LinearGradient>
                        </TouchableOpacity>
                    )}

                    {/* Logout Button */}
                    <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
                        <Ionicons name="log-out-outline" size={20} color="#EF4444" />
                        <Text style={styles.logoutText}>Logout</Text>
                    </TouchableOpacity>

                    <View style={styles.bottomPadding} />
                </ScrollView>

                {/* University Autocomplete */}
                <UniversityAutocomplete
                    visible={showUniversityPicker}
                    onClose={() => setShowUniversityPicker(false)}
                    onSelect={(university) => setSelectedUniversity(university)}
                    currentValue={selectedUniversity}
                />

                {/* Prompt Picker Modal */}
                <Modal
                    visible={showPromptPicker !== null}
                    animationType="slide"
                    transparent
                    onRequestClose={() => setShowPromptPicker(null)}
                >
                    <View style={styles.pickerOverlay}>
                        <View style={styles.pickerModal}>
                            <View style={styles.pickerHeader}>
                                <Text style={styles.pickerTitle}>Choose a Prompt</Text>
                                <TouchableOpacity onPress={() => setShowPromptPicker(null)}>
                                    <Ionicons name="close" size={24} color={Colors.white} />
                                </TouchableOpacity>
                            </View>
                            <ScrollView>
                                {promptOptions.map((option) => (
                                    <TouchableOpacity
                                        key={option}
                                        style={styles.pickerOption}
                                        onPress={() => {
                                            if (showPromptPicker !== null) {
                                                const newPrompts = [...prompts];
                                                newPrompts[showPromptPicker].question = option;
                                                setPrompts(newPrompts);
                                                setShowPromptPicker(null);
                                            }
                                        }}
                                    >
                                        <Text style={styles.pickerOptionText}>{option}</Text>
                                    </TouchableOpacity>
                                ))}
                            </ScrollView>
                        </View>
                    </View>
                </Modal>

                {/* Major Picker Modal */}
                <Modal
                    visible={showMajorPicker}
                    animationType="slide"
                    transparent
                    onRequestClose={() => setShowMajorPicker(false)}
                >
                    <View style={styles.pickerOverlay}>
                        <View style={styles.pickerModal}>
                            <View style={styles.pickerHeader}>
                                <Text style={styles.pickerTitle}>Select Major / Field</Text>
                                <TouchableOpacity onPress={() => setShowMajorPicker(false)}>
                                    <Ionicons name="close" size={24} color={Colors.white} />
                                </TouchableOpacity>
                            </View>
                            <ScrollView>
                                {majorOptions.map((majorOption) => (
                                    <TouchableOpacity
                                        key={majorOption}
                                        style={[
                                            styles.pickerOption,
                                            major === majorOption && styles.pickerOptionActive,
                                        ]}
                                        onPress={() => {
                                            if (majorOption === 'Other') {
                                                // Show text input for custom major
                                                setMajor('');
                                                setCustomMajor('');
                                            } else {
                                                setMajor(majorOption);
                                                setShowMajorPicker(false);
                                            }
                                        }}
                                    >
                                        <Text style={[
                                            styles.pickerOptionText,
                                            major === majorOption && styles.pickerOptionTextActive,
                                        ]}>
                                            {majorOption}
                                        </Text>
                                        {major === majorOption && (
                                            <Ionicons name="checkmark" size={20} color="#7C3AED" />
                                        )}
                                    </TouchableOpacity>
                                ))}
                                {/* Custom Major Input */}
                                <View style={styles.customInputContainer}>
                                    <Text style={styles.customInputLabel}>Or type your major:</Text>
                                    <TextInput
                                        style={styles.customInput}
                                        value={customMajor}
                                        onChangeText={setCustomMajor}
                                        placeholder="Enter your major..."
                                        placeholderTextColor="rgba(255,255,255,0.4)"
                                    />
                                    {customMajor.length > 0 && (
                                        <TouchableOpacity
                                            style={styles.customInputButton}
                                            onPress={() => {
                                                setMajor(customMajor);
                                                setShowMajorPicker(false);
                                            }}
                                        >
                                            <Text style={styles.customInputButtonText}>Use This Major</Text>
                                        </TouchableOpacity>
                                    )}
                                </View>
                            </ScrollView>
                        </View>
                    </View>
                </Modal>

                {/* Filter Modal */}
                <FilterModal
                    visible={showFilterModal}
                    onClose={() => setShowFilterModal(false)}
                    onApply={handleApplyFilters}
                    initialFilters={filters || undefined}
                />
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
    centerContent: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    header: {
        alignItems: 'center',
        paddingVertical: 16,
    },
    headerTitle: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    content: {
        paddingHorizontal: 16,
    },
    avatarSection: {
        alignItems: 'center',
        marginTop: 8,
    },
    avatarRing: {
        width: 120,
        height: 120,
        borderRadius: 60,
        padding: 4,
        justifyContent: 'center',
        alignItems: 'center',
    },
    avatar: {
        width: 112,
        height: 112,
        borderRadius: 56,
        backgroundColor: Colors.dark.surface,
    },
    noAvatar: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    nameSection: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 16,
        gap: 6,
    },
    userName: {
        fontSize: 24,
        fontWeight: '700',
        color: Colors.white,
    },
    verifiedBadge: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 20,
        marginTop: 8,
        gap: 6,
    },
    verifiedText: {
        fontSize: 12,
        fontWeight: '500',
        color: '#22C55E',
    },
    actionsRow: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 20,
        gap: 8,
    },
    viewProfileButton: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        paddingHorizontal: 14,
        paddingVertical: 12,
        borderRadius: 12,
        gap: 6,
        borderWidth: 1,
        borderColor: '#7C3AED',
    },
    viewProfileText: {
        fontSize: 13,
        fontWeight: '600',
        color: Colors.white,
    },
    editButton: {
        flex: 1,
        backgroundColor: 'rgba(255,255,255,0.1)',
        paddingVertical: 12,
        borderRadius: 12,
        alignItems: 'center',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.2)',
    },
    editButtonText: {
        fontSize: 14,
        fontWeight: '600',
        color: Colors.white,
    },
    iconButton: {
        width: 44,
        height: 44,
        borderRadius: 12,
        backgroundColor: 'rgba(255,255,255,0.1)',
        justifyContent: 'center',
        alignItems: 'center',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.2)',
    },
    section: {
        marginTop: 24,
    },
    sectionTitle: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 12,
    },
    universitySelector: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        padding: 16,
        borderRadius: 12,
        gap: 12,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.3)',
    },
    universityText: {
        flex: 1,
        fontSize: 15,
        color: Colors.white,
    },
    promptCard: {
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        padding: 16,
        marginBottom: 12,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.2)',
    },
    promptQuestion: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginBottom: 12,
    },
    promptQuestionText: {
        fontSize: 14,
        fontWeight: '600',
        color: '#7C3AED',
        flex: 1,
    },
    promptAnswer: {
        fontSize: 14,
        color: Colors.white,
        lineHeight: 20,
        minHeight: 40,
    },
    photoGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
    },
    photoSlot: {
        width: PHOTO_SIZE,
        height: PHOTO_SIZE,
        borderRadius: 12,
        overflow: 'hidden',
        backgroundColor: Colors.dark.surface,
    },
    photo: {
        width: '100%',
        height: '100%',
    },
    addPhotoPlaceholder: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        borderWidth: 2,
        borderStyle: 'dashed',
        borderColor: 'rgba(124, 58, 237, 0.3)',
        borderRadius: 12,
    },
    photoDeleteBadge: {
        position: 'absolute',
        top: 6,
        right: 6,
        width: 22,
        height: 22,
        borderRadius: 11,
        backgroundColor: 'rgba(0,0,0,0.6)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    bioText: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.7)',
        lineHeight: 22,
    },
    bioInput: {
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        padding: 16,
        fontSize: 14,
        color: Colors.white,
        textAlignVertical: 'top',
        minHeight: 100,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.3)',
    },
    interestsGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
    },
    interestTag: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        paddingHorizontal: 12,
        paddingVertical: 8,
        borderRadius: 20,
        gap: 6,
    },
    interestEmoji: {
        fontSize: 16,
    },
    interestName: {
        fontSize: 13,
        color: Colors.white,
    },
    saveButton: {
        marginTop: 24,
        borderRadius: 12,
        overflow: 'hidden',
    },
    saveButtonGradient: {
        paddingVertical: 16,
        alignItems: 'center',
    },
    saveButtonText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    logoutButton: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: 24,
        paddingVertical: 16,
        gap: 8,
    },
    logoutText: {
        fontSize: 15,
        fontWeight: '500',
        color: '#EF4444',
    },
    bottomPadding: {
        height: 100,
    },
    pickerOverlay: {
        flex: 1,
        justifyContent: 'flex-end',
        backgroundColor: 'rgba(0,0,0,0.5)',
    },
    pickerModal: {
        backgroundColor: '#0F0A1A',
        borderTopLeftRadius: 24,
        borderTopRightRadius: 24,
        maxHeight: '70%',
    },
    pickerHeader: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 20,
        paddingVertical: 16,
        borderBottomWidth: 1,
        borderBottomColor: 'rgba(255,255,255,0.1)',
    },
    pickerTitle: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.white,
    },
    pickerOption: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 20,
        paddingVertical: 16,
        borderBottomWidth: 1,
        borderBottomColor: 'rgba(255,255,255,0.05)',
    },
    pickerOptionActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.1)',
    },
    pickerOptionText: {
        fontSize: 15,
        color: 'rgba(255,255,255,0.8)',
    },
    pickerOptionTextActive: {
        color: '#7C3AED',
        fontWeight: '600',
    },
    // Campus Filter Edit Styles
    pillsContainer: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
    },
    yearPill: {
        paddingHorizontal: 16,
        paddingVertical: 10,
        borderRadius: 20,
        backgroundColor: Colors.dark.surface,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    yearPillActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    yearPillText: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    yearPillTextActive: {
        color: Colors.white,
        fontWeight: '600',
    },
    majorChipsContainer: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 10,
    },
    majorChip: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: 14,
        paddingVertical: 10,
        borderRadius: 20,
        backgroundColor: Colors.dark.surface,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
        gap: 6,
    },
    majorChipActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    majorChipEmoji: {
        fontSize: 16,
    },
    majorChipText: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    majorChipTextActive: {
        color: Colors.white,
        fontWeight: '600',
    },
    lookingForGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 12,
    },
    lookingForItem: {
        width: '47%',
        backgroundColor: Colors.dark.surface,
        borderRadius: 16,
        paddingVertical: 16,
        alignItems: 'center',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    lookingForItemActive: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    lookingForText: {
        marginTop: 8,
        fontSize: 12,
        color: 'rgba(255,255,255,0.6)',
    },
    lookingForTextActive: {
        color: Colors.white,
        fontWeight: '500',
    },
    // Custom Input Styles for Major Picker
    customInputContainer: {
        padding: 20,
        borderTopWidth: 1,
        borderTopColor: 'rgba(255,255,255,0.1)',
        marginTop: 12,
    },
    customInputLabel: {
        fontSize: 14,
        fontWeight: '500',
        color: 'rgba(255,255,255,0.7)',
        marginBottom: 12,
    },
    customInput: {
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        paddingHorizontal: 16,
        paddingVertical: 14,
        fontSize: 15,
        color: Colors.white,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.3)',
    },
    customInputButton: {
        marginTop: 12,
        backgroundColor: Colors.primary.main,
        borderRadius: 12,
        paddingVertical: 14,
        alignItems: 'center',
    },
    customInputButtonText: {
        fontSize: 15,
        fontWeight: '600',
        color: Colors.white,
    },
});
