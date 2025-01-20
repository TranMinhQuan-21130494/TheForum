import React, { useState } from 'react';
import { router, useLocalSearchParams } from 'expo-router';
import { View, Text, TextInput, Button, StyleSheet, Alert, ToastAndroid, Image, TouchableOpacity, ScrollView } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import * as ImagePicker from 'expo-image-picker';

export default function AddCommentScreen() {
    const { postId } = useLocalSearchParams<{ postId?: string }>();
    const [content, setContent] = useState('');
    const [imageName, setImageName] = useState(null);
    const [imageURI, setImageURI] = useState(null);

    // Hàm xử lý đăng bài
    const handleCommentSubmit = async () => {
        console.log(imageName);
        const token = await AsyncStorage.getItem('authToken');
        if (!token) {
            Alert.alert("Lỗi", "Bạn cần phải đăng nhập trước")
            return;
        }

        if (!content.trim()) {
            Alert.alert('Lỗi', 'Vui lòng điền Nội dung!');
            return;
        }

        try {
            const response = await fetch(`http://10.0.2.2:5151/api/posts/${postId}/comments`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    content: content.trim(),
                    imageName: imageName,
                }),
            });

            if (response.ok) {
                ToastAndroid.show("Bài viết đã được đăng", 2000);
                router.back();
            } else {
                const errorData = await response.json();
                Alert.alert('Lỗi', errorData.message || 'Đăng bài không thành công.');
            }
        } catch (error) {
            console.error('Error posting data:', error);
            Alert.alert('Lỗi', 'Không thể kết nối đến máy chủ.');
        }
    };

    const handleSubmitImage = async () => {
        ToastAndroid.show("Click", 1000)
        // Kiểm tra đăng nhập
        const token = await AsyncStorage.getItem('authToken');
        if (!token) {
            ToastAndroid.show('Bạn cần đăng nhập để thực hiện điều này', 1000);
            return;
        }

        // Kiểm tra quyền truy cập ảnh
        const permissionResult = await ImagePicker.requestMediaLibraryPermissionsAsync();
        if (permissionResult.granted === false) {
            Alert.alert('Bạn cần cấp quyền truy cập ảnh để chọn ảnh đại diện');
            return;
        }

        // Mở bộ chọn ảnh
        const result = await ImagePicker.launchImageLibraryAsync({
            mediaTypes: ImagePicker.MediaTypeOptions.Images,
            allowsEditing: true,
            aspect: [1, 1],
            quality: 1,
        });


        const formData = new FormData();
        formData.append('file', {
            uri: result.assets[0].uri,
            type: 'image/jpeg', // Thay đổi theo loại ảnh bạn sử dụng
            name: 'avatar.jpg',
        });

        try {
            const response = await fetch('http://10.0.2.2:5151/api/images', {
                method: 'POST',
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${token}`,
                },
                body: formData
            });

            if (response.ok) {
                ToastAndroid.show("Hoàn tất!", 2000)
                const data = await response.json();
                console.log(data)
                setImageName(data.imageName);
                setImageURI(result.assets[0].uri);
            } else {
                const error = await response.json();
                console.log(error)
                Alert.alert('Lỗi', error.message || 'Đã có lỗi');
            }
        } catch (error) {
            console.log(error)
            Alert.alert('Lỗi ?', error.message);
        }
    }

    const handleRemoveImage = () => {
        setImageName(null);
        setImageURI(null);
    };

    return (
        <ScrollView style={styles.container}>
            <Text style={styles.label}>Ảnh</Text>
            {imageURI ? (
                <View style={styles.imageContainer}>
                    <Image source={{ uri: imageURI }} style={styles.image} />
                    <TouchableOpacity style={styles.removeButton} onPress={handleRemoveImage}>
                        <Text style={styles.removeButtonText}>Xóa ảnh</Text>
                    </TouchableOpacity>
                </View>
            ) : (
                <View style={styles.imageContainer}>
                    <Button title="Tải ảnh lên" onPress={handleSubmitImage} />
                </View>
            )}

            <Text style={styles.label}>Nội dung</Text>
            <TextInput
                style={[styles.input, styles.textArea]}
                value={content}
                onChangeText={setContent}
                placeholder="Nhập bình luận"
                multiline
                numberOfLines={5}
            />
            <Button title="Đăng bài" onPress={handleCommentSubmit} />
        </ScrollView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 20,
        backgroundColor: '#f9f9f9',
    },
    label: {
        fontSize: 16,
        fontWeight: 'bold',
        marginBottom: 5,
    },
    input: {
        borderWidth: 1,
        borderColor: '#ccc',
        borderRadius: 5,
        padding: 10,
        marginBottom: 15,
        backgroundColor: '#fff',
    },
    textArea: {
        height: 100,
        textAlignVertical: 'top', // Để văn bản bắt đầu từ trên cùng
    },
    imageContainer: {
        alignItems: 'center',
        marginBottom: 15,
    },
    image: {
        width: 200,
        height: 200,
        borderRadius: 10,
        marginBottom: 10,
    },
    removeButton: {
        backgroundColor: '#ff4d4d',
        padding: 10,
        borderRadius: 5,
    },
    removeButtonText: {
        color: '#fff',
        fontWeight: 'bold',
    },
});
