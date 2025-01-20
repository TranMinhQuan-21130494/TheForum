import React, { useState } from 'react';
import { View, Text, ActivityIndicator, TouchableOpacity, Image, ToastAndroid } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { router, useLocalSearchParams } from 'expo-router';
import { useFocusEffect } from '@react-navigation/native';
import { jwtDecode } from "jwt-decode";

export default function ProfileScreen() {
    const { userId } = useLocalSearchParams();
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [role, setRole] = useState(null);

    const fetchProfile = async () => {
        try {
            const token = await AsyncStorage.getItem('authToken');
            if (token) {
                const decoded = jwtDecode(token);
                setRole(decoded.role);
            }

            const response = await fetch(`http://10.0.2.2:5151/api/users/${userId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
            });

            if (response.ok) {
                const data = await response.json();
                setProfile(data);
            } else {
                console.error('Failed to fetch profile:', response.status);
            }
        } catch (error) {
            console.error('Error fetching profile:', error);
        } finally {
            setLoading(false);
        }
    };

    const formatDate = (isoString: string) => {
        const date = new Date(isoString); // Chuyển chuỗi ISO thành đối tượng Date
        const day = date.getDate().toString().padStart(2, '0'); // Lấy ngày (định dạng 2 chữ số)
        const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Lấy tháng (định dạng 2 chữ số)
        const year = date.getFullYear(); // Lấy năm

        return `${day}-${month}-${year}`; // Kết hợp thành chuỗi ngày-tháng-năm
    }

    const viewUserActivity = () => {
        router.push(`/user/${userId}/activity`)
    }

    const blockUser = async (id, status) => {
        const token = await AsyncStorage.getItem('authToken');

        const response = await fetch(`http://10.0.2.2:5151/api/admin/users/${id}?status=${status}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`,
            },
        });
        if (response.ok) {
            ToastAndroid.show("Hoàn tất", 2000);
        } else {
            console.error('Failed to fetch profile:', response.status);
        }
     }

    useFocusEffect(
        React.useCallback(() => {
            fetchProfile(); // Gọi API mỗi khi màn hình được focus
        }, [])
    );

    if (loading) {
        return (
            <View className='flex-1 items-center'>
                <ActivityIndicator size="large" color="#4CAF50" />
            </View>
        );
    }

    return (
        <View className="flex-1 items-center bg-white">
            <Text className="text-2xl font-bold my-4">Thông tin cá nhân</Text>
            <Image
                className='w-52 h-52 rounded-full my-4'
                source={{ uri: `http://10.0.2.2:5151/${profile.avatarImageURI}` }} // Thay bằng URL hình ảnh của bạn
            />
            <View className='w-11/12 mx-2 my-2'>
                <Text className="text-xl">Tên: {profile.name}</Text>
                <Text className="text-xl">Loại tài khoản: {profile.role}</Text>
                <Text className="text-xl">Trạng thái: {profile.status}</Text>
                <Text className="text-xl">Ngày tạo: {formatDate(profile.createdTime)}</Text>
            </View>
            <TouchableOpacity
                className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
                onPress={viewUserActivity}>
                <Text className="text-xl">Xem hoạt động</Text>
            </TouchableOpacity>
            {role === 'ADMIN' && (
                <TouchableOpacity
                    className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
                    onPress={() => blockUser(profile.id, "BANNED")}>
                    <Text className="text-xl">Khóa tài khoản</Text>
                </TouchableOpacity>
            )}
        </View>
    );
}
