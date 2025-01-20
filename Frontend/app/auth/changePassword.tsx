import { useState } from "react";
import { Text, View, Image, TouchableOpacity, Alert } from "react-native";
import Spinner from 'react-native-loading-spinner-overlay';
import { MyTextInput, MyPasswordInput } from "@/components/MyTextInput";
import AsyncStorage from '@react-native-async-storage/async-storage';
import { router } from 'expo-router';
import { ToastAndroid } from "react-native";

export default function ChangePassword() {
    const [oldPassword, setOldPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [recheckPassword, setRecheckPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const doChangePassword = async () => {
        // Loading
        setLoading(true);
        const token = await AsyncStorage.getItem('authToken');

        // Tạo hàm timeout
        const timeout = new Promise((_, reject) =>
            setTimeout(() => reject(new Error('Request timeout')), 5000)
        );

        try {
            const requestBody = JSON.stringify({ oldPassword, newPassword, recheckPassword });

            // Gọi API với timeout
            const response = await Promise.race([
                fetch('http://10.0.2.2:5151/api/users/me/changePassword', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`,
                    },
                    body: requestBody,
                }),
                timeout,
            ]);

            if (response.ok) {
                ToastAndroid.show("Đổi mật khẩu thành công", 2000)
                router.replace("/")
            } else {
                const data = await response.json();
                console.log(data)
                const message = data.message;
                Alert.alert("Đổi mật khẩu thất bại", message);
            }
        } catch (error) {
            // Xử lý lỗi (timeout hoặc lỗi khác)
            Alert.alert("Lỗi", error.message);
        } finally {
            // Tắt màn hình loading
            setLoading(false);
        }
    };

    return (
        <View className="flex-1 items-center bg-white">
            <Image
                className="w-32 h-32 my-24"
                source={require('@/assets/images/react-logo.png')} />
            <Text
                className="text-3xl my-4">
                Đổi mật khẩu
            </Text>
            <MyPasswordInput
                value={oldPassword}
                onChange={setOldPassword}
                placeholder="Mật khẩu cũ..." />
            <MyPasswordInput
                value={newPassword}
                onChange={setNewPassword}
                placeholder="Mật khẩu mới..." />
            <MyPasswordInput
                value={recheckPassword}
                onChange={setRecheckPassword}
                placeholder="Nhập lại mật khẩu mới..." />
            <TouchableOpacity
                className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
                onPress={doChangePassword}>
                <Text className="text-xl">Đổi mật khẩu</Text>
            </TouchableOpacity>
            <Spinner
                textStyle={{ color: '#fff' }}
                cancelable={true}
                visible={loading}
                textContent="Đang tải" />
        </View>
    )
}