import { useState } from "react";
import { Text, View, Image, TouchableOpacity, Alert } from "react-native";
import Spinner from 'react-native-loading-spinner-overlay';
import { MyTextInput, MyPasswordInput } from "@/components/MyTextInput";
import AsyncStorage from '@react-native-async-storage/async-storage';
import { router } from 'expo-router';
import { ToastAndroid } from "react-native";

export default function LoginScreen() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loginLoading, setLoginLoading] = useState(false);
    const doLogin = async () => {
        // Loading
        setLoginLoading(true);
    
        // Tạo hàm timeout
        const timeout = new Promise((_, reject) =>
            setTimeout(() => reject(new Error('Request timeout')), 5000)
        );
    
        try {
            const requestBody = JSON.stringify({ email, password });

            // Gọi API với timeout
            const response = await Promise.race([
                fetch('http://10.0.2.2:5151/api/account/token', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: requestBody,
                }),
                timeout,
            ]);
    
            if (response.ok) {
                const data = await response.json();
                const token = data.token;
                await AsyncStorage.setItem('authToken', token);
                ToastAndroid.show("Đăng nhập thành công", 2000)
                router.replace("/")
            } else {
                Alert.alert("Đăng nhập thất bại", "Tài khoản hoặc mật khẩu của bạn không đúng");
            }
        } catch (error) {    
            // Xử lý lỗi (timeout hoặc lỗi khác)
            Alert.alert("Lỗi", error.message);
        } finally {
            // Tắt màn hình loading
            setLoginLoading(false);
        }
    };
    
    return (
        <View className="flex-1 items-center bg-white">
            <Image
                className="w-32 h-32 my-24"
                source={require('@/assets/images/react-logo.png')} />
            <Text
                className="text-3xl my-4">
                Đăng nhập TheForum
            </Text>
            <MyTextInput
                value={email}
                onChange={setEmail}
                placeholder="Email..." />
            <MyPasswordInput
                value={password}
                onChange={setPassword}
                placeholder="Mật khẩu..." />
            <TouchableOpacity
                className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
                onPress={doLogin}>
                <Text className="text-xl">Đăng nhập</Text>
            </TouchableOpacity>
            <Spinner
                textStyle={{ color: '#fff' }}
                cancelable={true}
                visible={loginLoading}
                textContent="Đang tải" />
        </View>
    )
}