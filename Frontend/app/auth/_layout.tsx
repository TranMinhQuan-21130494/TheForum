import { Stack } from "expo-router";
import "@/global.css";

export default function AuthLayout() {
  return (
    <Stack>
      <Stack.Screen name="login" options={{title: "Đăng nhập"}}/>
      <Stack.Screen name="changePassword" options={{title: "Đổi mật khẩu"}}/>
    </Stack>
  );
}