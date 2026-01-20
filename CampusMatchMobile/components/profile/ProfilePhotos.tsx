// Profile Photo Grid Component - Displays and manages user photos
import React from 'react';
import { View, Text, Image, TouchableOpacity, StyleSheet, Dimensions, Alert } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import * as ImagePicker from 'expo-image-picker';
import { photosApi } from '@/services';
import { PhotoDto } from '@/types';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');
const PHOTO_SIZE = (SCREEN_WIDTH - 64) / 3;

interface ProfilePhotosProps {
    photos: PhotoDto[];
    maxPhotos?: number;
    onPhotosChange?: () => void;
    editable?: boolean;
}

export default function ProfilePhotos({
    photos,
    maxPhotos = 6,
    onPhotosChange,
    editable = true
}: ProfilePhotosProps) {

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
                onPhotosChange?.();
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
                            onPhotosChange?.();
                        } catch (err) {
                            Alert.alert('Error', 'Failed to delete photo');
                        }
                    },
                },
            ]
        );
    };

    return (
        <View style={styles.section}>
            <Text style={styles.sectionTitle}>My Photos</Text>
            <View style={styles.photoGrid}>
                {Array.from({ length: maxPhotos }).map((_, index) => {
                    const photo = photos[index];
                    return (
                        <TouchableOpacity
                            key={index}
                            style={styles.photoSlot}
                            onPress={() => {
                                if (!editable) return;
                                photo ? handleDeletePhoto(photo.id) : handleAddPhoto();
                            }}
                            disabled={!editable}
                        >
                            {photo ? (
                                <>
                                    <Image
                                        source={{ uri: photo.url }}
                                        style={styles.photo}
                                        resizeMode="cover"
                                    />
                                    {editable && (
                                        <View style={styles.photoDeleteBadge}>
                                            <Ionicons name="close" size={12} color={Colors.white} />
                                        </View>
                                    )}
                                </>
                            ) : editable ? (
                                <View style={styles.addPhotoPlaceholder}>
                                    <Ionicons name="add" size={28} color="#7C3AED" />
                                </View>
                            ) : (
                                <View style={styles.emptySlot} />
                            )}
                        </TouchableOpacity>
                    );
                })}
            </View>
        </View>
    );
}

const styles = StyleSheet.create({
    section: {
        marginTop: 24,
    },
    sectionTitle: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 12,
    },
    photoGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
    },
    photoSlot: {
        width: PHOTO_SIZE,
        height: PHOTO_SIZE * 1.33,
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
        borderColor: '#7C3AED',
        borderStyle: 'dashed',
        borderRadius: 12,
    },
    emptySlot: {
        flex: 1,
        backgroundColor: 'rgba(255,255,255,0.05)',
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
});
