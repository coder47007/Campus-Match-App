import React, { useEffect } from 'react';
import {
    View,
    Text,
    Image,
    StyleSheet,
    TouchableOpacity,
    Dimensions,
    Modal,
} from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import Animated, {
    useAnimatedStyle,
    useSharedValue,
    withSpring,
    withSequence,
    withDelay,
    withTiming,
} from 'react-native-reanimated';
import { Ionicons } from '@expo/vector-icons';
import { MatchDto } from '@/types';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');

interface MatchPopupProps {
    visible: boolean;
    match: MatchDto | null;
    currentUserPhoto?: string;
    onClose: () => void;
    onSendMessage: () => void;
}

export default function MatchPopup({
    visible,
    match,
    currentUserPhoto,
    onClose,
    onSendMessage,
}: MatchPopupProps) {
    const scale = useSharedValue(0.5);
    const opacity = useSharedValue(0);
    const heartScale = useSharedValue(0);
    const leftPhotoX = useSharedValue(-100);
    const rightPhotoX = useSharedValue(100);

    useEffect(() => {
        if (visible) {
            scale.value = withSpring(1, { damping: 12 });
            opacity.value = withTiming(1, { duration: 300 });
            heartScale.value = withSequence(
                withDelay(200, withSpring(1.3)),
                withSpring(1)
            );
            leftPhotoX.value = withDelay(100, withSpring(0, { damping: 15 }));
            rightPhotoX.value = withDelay(100, withSpring(0, { damping: 15 }));
        } else {
            scale.value = 0.5;
            opacity.value = 0;
            heartScale.value = 0;
            leftPhotoX.value = -100;
            rightPhotoX.value = 100;
        }
    }, [visible]);

    const containerStyle = useAnimatedStyle(() => ({
        transform: [{ scale: scale.value }],
        opacity: opacity.value,
    }));

    const heartStyle = useAnimatedStyle(() => ({
        transform: [{ scale: heartScale.value }],
    }));

    const leftPhotoStyle = useAnimatedStyle(() => ({
        transform: [{ translateX: leftPhotoX.value }],
    }));

    const rightPhotoStyle = useAnimatedStyle(() => ({
        transform: [{ translateX: rightPhotoX.value }],
    }));

    if (!match) return null;

    return (
        <Modal
            visible={visible}
            transparent
            animationType="fade"
            onRequestClose={onClose}
        >
            <View style={styles.overlay}>
                <LinearGradient
                    colors={['rgba(155, 89, 182, 0.95)', 'rgba(142, 68, 173, 0.95)', 'rgba(108, 52, 131, 0.95)']}
                    style={styles.gradient}
                >
                    <Animated.View style={[styles.container, containerStyle]}>
                        {/* It's a Match title */}
                        <Text style={styles.title}>It's a Match!</Text>
                        <Text style={styles.subtitle}>
                            You and {match.otherStudentName} liked each other
                        </Text>

                        {/* Photos */}
                        <View style={styles.photosContainer}>
                            <Animated.View style={[styles.photoWrapper, leftPhotoStyle]}>
                                {currentUserPhoto ? (
                                    <Image
                                        source={{ uri: currentUserPhoto }}
                                        style={styles.photo}
                                        resizeMode="cover"
                                    />
                                ) : (
                                    <View style={[styles.photo, styles.noPhoto]}>
                                        <Ionicons name="person" size={40} color={Colors.white} />
                                    </View>
                                )}
                            </Animated.View>

                            <Animated.View style={[styles.heartContainer, heartStyle]}>
                                <Ionicons name="heart" size={50} color={Colors.white} />
                            </Animated.View>

                            <Animated.View style={[styles.photoWrapper, rightPhotoStyle]}>
                                {match.otherStudentPhotoUrl ? (
                                    <Image
                                        source={{ uri: match.otherStudentPhotoUrl }}
                                        style={styles.photo}
                                        resizeMode="cover"
                                    />
                                ) : (
                                    <View style={[styles.photo, styles.noPhoto]}>
                                        <Ionicons name="person" size={40} color={Colors.white} />
                                    </View>
                                )}
                            </Animated.View>
                        </View>

                        {/* Action buttons */}
                        <View style={styles.buttons}>
                            <TouchableOpacity style={styles.messageButton} onPress={onSendMessage}>
                                <Ionicons name="chatbubble" size={20} color={Colors.primary.main} />
                                <Text style={styles.messageButtonText}>Send a Message</Text>
                            </TouchableOpacity>

                            <TouchableOpacity style={styles.keepSwipingButton} onPress={onClose}>
                                <Text style={styles.keepSwipingText}>Keep Swiping</Text>
                            </TouchableOpacity>
                        </View>
                    </Animated.View>
                </LinearGradient>
            </View>
        </Modal>
    );
}

const styles = StyleSheet.create({
    overlay: {
        flex: 1,
    },
    gradient: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    container: {
        alignItems: 'center',
        paddingHorizontal: 40,
    },
    title: {
        fontSize: 40,
        fontWeight: 'bold',
        color: Colors.white,
        textAlign: 'center',
        marginBottom: 8,
        textShadowColor: 'rgba(0,0,0,0.2)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 4,
    },
    subtitle: {
        fontSize: 16,
        color: Colors.white,
        textAlign: 'center',
        marginBottom: 40,
        opacity: 0.9,
    },
    photosContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        marginBottom: 50,
    },
    photoWrapper: {
        width: 120,
        height: 120,
        borderRadius: 60,
        overflow: 'hidden',
        borderWidth: 4,
        borderColor: Colors.white,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.3,
        shadowRadius: 8,
        elevation: 8,
    },
    photo: {
        width: '100%',
        height: '100%',
    },
    noPhoto: {
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
    heartContainer: {
        marginHorizontal: -15,
        zIndex: 1,
        backgroundColor: Colors.primary.main,
        width: 70,
        height: 70,
        borderRadius: 35,
        justifyContent: 'center',
        alignItems: 'center',
        borderWidth: 3,
        borderColor: Colors.white,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.3,
        shadowRadius: 8,
        elevation: 8,
    },
    buttons: {
        width: SCREEN_WIDTH - 80,
        gap: 12,
    },
    messageButton: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: Colors.white,
        paddingVertical: 16,
        borderRadius: 30,
        gap: 8,
    },
    messageButtonText: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.primary.main,
    },
    keepSwipingButton: {
        alignItems: 'center',
        paddingVertical: 16,
    },
    keepSwipingText: {
        fontSize: 16,
        color: Colors.white,
        fontWeight: '500',
    },
});
