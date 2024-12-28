import { Link } from "expo-router";
import { Text, View } from "react-native";

export default function Index() {
  return (
    <View className="flex-1 justify-center items-center bg-white">
      <Text>Hello world!</Text>
      <Link href='/auth/login' className="bg-blue-500">Đăng nhập</Link>
    </View>
  )
}
