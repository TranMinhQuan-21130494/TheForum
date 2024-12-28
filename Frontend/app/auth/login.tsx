import { useState } from "react";
import { Text, View, Image, Alert, Platform, ToastAndroid, Pressable, TouchableHighlight, TouchableOpacity, ActivityIndicator } from "react-native";
import { MyTextInput, MyPasswordInput } from "@/components/MyTextInput";

function alert(message: string) {
    if (Platform.OS === 'android') {
        ToastAndroid.show(message, 2000)
    }
    else {
        Alert.alert(message)
    }
}

const sleep = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

export default function Index() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loginLoading, setLoginLoading] = useState(false);
    const doLogin = async () => {
        // Loading
        setLoginLoading(true)

        // Gọi API
        await sleep(3000)

        // Hoàn tất công việc và thoát khỏi loading
        alert("Đăng nhập thành công")
    }

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
            {loginLoading ?? (<ActivityIndicator size="large" />)}
        </View>
    )
}