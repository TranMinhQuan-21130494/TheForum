import React, { useState } from 'react';
import { View, Text, ActivityIndicator, TouchableOpacity, Image, Alert } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { router } from 'expo-router';
import { useFocusEffect } from '@react-navigation/native';
import * as ImagePicker from 'expo-image-picker';

export default function ProfileScreen() {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  const fetchProfile = async () => {
    try {
      const token = await AsyncStorage.getItem('authToken');
      if (!token) {
        setLoading(false);
        return;
      }

      const response = await fetch('http://10.0.2.2:5151/api/users/me', {
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

  const doLogout = () => {
    AsyncStorage.setItem('authToken', '');
    setProfile(null);
  }

  const formatDate = (isoString: string) => {
    const date = new Date(isoString); // Chuyển chuỗi ISO thành đối tượng Date
    const day = date.getDate().toString().padStart(2, '0'); // Lấy ngày (định dạng 2 chữ số)
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Lấy tháng (định dạng 2 chữ số)
    const year = date.getFullYear(); // Lấy năm

    return `${day}-${month}-${year}`; // Kết hợp thành chuỗi ngày-tháng-năm
  }

  const changeAvatarImage = async () => {
    // Kiểm tra đăng nhập
    const token = await AsyncStorage.getItem('authToken');
    if (!token) {
      Alert.alert('Bạn cần đăng nhập để thay đổi ảnh đại diện');
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
    formData.append('AvatarImage', {
      uri: result.assets[0].uri,
      type: 'image/jpeg', // Thay đổi theo loại ảnh bạn sử dụng
      name: 'avatar.jpg',
    });

    try {
      const response = await fetch('http://10.0.2.2:5151/api/users/me', {
        method: 'POST',
        headers: {
          'Content-Type': 'multipart/form-data',
          Authorization: `Bearer ${token}`,
        },
        body: formData
      });

      if (response.ok) {
        Alert.alert('Ảnh đại diện đã được thay đổi!');
        fetchProfile();
      } else {
        const error = await response.json();
        console.log(error)
        Alert.alert('Lỗi', error.message || 'Không thể thay đổi ảnh đại diện');
      }
    } catch (error) {
      console.log(error)
      Alert.alert('Lỗi ?', error.message);
    }
  };

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

  if (!profile) {
    return (
      <View className="flex-1 items-center bg-white">
        <Text className="text-2xl font-bold my-4">Bạn chưa đăng nhập</Text>
        <TouchableOpacity
          className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
          onPress={() => router.push("/auth/login")}>
          <Text className="text-xl">Đăng nhập</Text>
        </TouchableOpacity>
        <TouchableOpacity
          className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
          onPress={() => router.push("/auth/login")}>
          <Text className="text-xl">Đăng ký</Text>
        </TouchableOpacity>
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
        onPress={changeAvatarImage}>
        <Text className="text-xl">Đổi ảnh đại diện</Text>
      </TouchableOpacity>
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={() => router.replace("/auth/changePassword")}>
        <Text className="text-xl">Đổi mật khẩu</Text>
      </TouchableOpacity>
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={doLogout}>
        <Text className="text-xl">Đăng xuất</Text>
      </TouchableOpacity>
    </View>
  );
}
