import React, { useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    Modal,
    TouchableOpacity,
    TextInput,
    ActivityIndicator,
    Alert,
    KeyboardAvoidingView,
    Platform,
    TouchableWithoutFeedback,
    Keyboard,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';
import { eventsApi } from '@/services/events';

interface EventModalProps {
    visible: boolean;
    onClose: () => void;
    onEventCreated: () => void;
}

const LOCATIONS = [
    'The Daily Grind',
    'Main Library',
    'Student Union',
    'Campus Green',
    'Gym',
    'Dining Hall',
    'Study Hall A',
    'The Quad'
];

const DURATIONS = [
    { label: '30m', value: 30 },
    { label: '1h', value: 60 },
    { label: '2h', value: 120 },
    { label: '3h', value: 180 },
    { label: '5h', value: 300 },
];

export default function EventModal({ visible, onClose, onEventCreated }: EventModalProps) {
    const [title, setTitle] = useState('');
    const [selectedLocation, setSelectedLocation] = useState(LOCATIONS[0]);
    const [selectedDuration, setSelectedDuration] = useState(60);
    const [isLoading, setIsLoading] = useState(false);

    const handleCreate = async () => {
        if (!title.trim()) {
            Alert.alert('Missing Info', 'Please add a title (e.g., "Studying for Bio").');
            return;
        }

        setIsLoading(true);
        try {
            await eventsApi.createEvent(title, selectedLocation, selectedDuration);
            onEventCreated();
            onClose();
            // Reset form
            setTitle('');
            setSelectedLocation(LOCATIONS[0]);
            setSelectedDuration(60);
        } catch (error) {
            Alert.alert('Error', 'Failed to create event. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Modal
            visible={visible}
            animationType="slide"
            transparent={true}
            onRequestClose={onClose}
        >
            <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
                <View style={styles.modalOverlay}>
                    <KeyboardAvoidingView
                        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                        style={styles.keyboardView}
                    >
                        <View style={styles.modalContent}>
                            <View style={styles.header}>
                                <Text style={styles.title}>New Vibe Check 📍</Text>
                                <TouchableOpacity onPress={onClose} style={styles.closeButton}>
                                    <Ionicons name="close" size={24} color={Colors.white} />
                                </TouchableOpacity>
                            </View>

                            <Text style={styles.subtitle}>Where are you headed?</Text>

                            {/* Location Selector */}
                            <View style={styles.tagsContainer}>
                                {LOCATIONS.map((loc) => (
                                    <TouchableOpacity
                                        key={loc}
                                        style={[
                                            styles.tag,
                                            selectedLocation === loc && styles.tagSelected
                                        ]}
                                        onPress={() => setSelectedLocation(loc)}
                                    >
                                        <Text style={[
                                            styles.tagText,
                                            selectedLocation === loc && styles.tagTextSelected
                                        ]}>{loc}</Text>
                                    </TouchableOpacity>
                                ))}
                            </View>

                            {/* Title Input */}
                            <Text style={styles.label}>What are you doing?</Text>
                            <TextInput
                                style={styles.input}
                                placeholder="e.g. Cramming for finals, Coffee break..."
                                placeholderTextColor="rgba(255,255,255,0.4)"
                                value={title}
                                onChangeText={setTitle}
                                maxLength={50}
                            />

                            {/* Duration Selector */}
                            <Text style={styles.label}>How long?</Text>
                            <View style={styles.durationContainer}>
                                {DURATIONS.map((dur) => (
                                    <TouchableOpacity
                                        key={dur.value}
                                        style={[
                                            styles.durationButton,
                                            selectedDuration === dur.value && styles.durationButtonSelected
                                        ]}
                                        onPress={() => setSelectedDuration(dur.value)}
                                    >
                                        <Text style={[
                                            styles.durationText,
                                            selectedDuration === dur.value && styles.durationTextSelected
                                        ]}>{dur.label}</Text>
                                    </TouchableOpacity>
                                ))}
                            </View>

                            {/* Create Button */}
                            <TouchableOpacity
                                style={styles.createButton}
                                onPress={handleCreate}
                                disabled={isLoading}
                            >
                                <LinearGradient
                                    colors={['#7C3AED', '#6D28D9']}
                                    style={styles.gradient}
                                >
                                    {isLoading ? (
                                        <ActivityIndicator color="white" />
                                    ) : (
                                        <Text style={styles.createButtonText}>Post to Campus 🚀</Text>
                                    )}
                                </LinearGradient>
                            </TouchableOpacity>
                        </View>
                    </KeyboardAvoidingView>
                </View>
            </TouchableWithoutFeedback>
        </Modal>
    );
}

const styles = StyleSheet.create({
    modalOverlay: {
        flex: 1,
        backgroundColor: 'rgba(0,0,0,0.7)',
        justifyContent: 'flex-end',
    },
    keyboardView: {
        width: '100%',
    },
    modalContent: {
        backgroundColor: '#1E1B2E',
        borderTopLeftRadius: 24,
        borderTopRightRadius: 24,
        padding: 24,
        paddingBottom: 40,
        minHeight: 500,
    },
    header: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 20,
    },
    title: {
        fontSize: 22,
        fontWeight: '700',
        color: Colors.white,
    },
    closeButton: {
        padding: 4,
        backgroundColor: 'rgba(255,255,255,0.1)',
        borderRadius: 20,
    },
    subtitle: {
        fontSize: 16,
        color: 'rgba(255,255,255,0.6)',
        marginBottom: 12,
    },
    tagsContainer: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
        marginBottom: 24,
    },
    tag: {
        paddingHorizontal: 16,
        paddingVertical: 8,
        borderRadius: 20,
        backgroundColor: 'rgba(255,255,255,0.05)',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    tagSelected: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    tagText: {
        color: 'rgba(255,255,255,0.7)',
        fontSize: 14,
    },
    tagTextSelected: {
        color: '#A78BFA',
        fontWeight: '600',
    },
    label: {
        fontSize: 14,
        fontWeight: '600',
        color: 'rgba(255,255,255,0.8)',
        marginBottom: 8,
    },
    input: {
        backgroundColor: 'rgba(255,255,255,0.05)',
        borderRadius: 12,
        padding: 16,
        color: Colors.white,
        fontSize: 16,
        marginBottom: 24,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    durationContainer: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginBottom: 32,
        gap: 8,
    },
    durationButton: {
        flex: 1,
        alignItems: 'center',
        paddingVertical: 12,
        borderRadius: 12,
        backgroundColor: 'rgba(255,255,255,0.05)',
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.1)',
    },
    durationButtonSelected: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        borderColor: '#7C3AED',
    },
    durationText: {
        color: 'rgba(255,255,255,0.7)',
        fontSize: 14,
        fontWeight: '600',
    },
    durationTextSelected: {
        color: '#A78BFA',
    },
    createButton: {
        width: '100%',
        borderRadius: 16,
        overflow: 'hidden',
    },
    gradient: {
        paddingVertical: 16,
        alignItems: 'center',
    },
    createButtonText: {
        color: Colors.white,
        fontSize: 18,
        fontWeight: '700',
    },
});
