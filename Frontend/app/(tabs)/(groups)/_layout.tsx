import { Stack } from 'expo-router/stack';

export default function Layout() {
  return (
    <Stack>
      <Stack.Screen name="category/[category]" options={{ headerShown: false }} />
      <Stack.Screen name="post/[postId]" options={{ headerShown: false }} />
      <Stack.Screen name="post/addComment" options={{ headerShown: false }} />
      <Stack.Screen name="post/create" options={{ headerShown: false }} />
      <Stack.Screen name="user/[userId]" options={{ headerShown: false }} />
      <Stack.Screen name="user/[userId]/activity" options={{ headerShown: false }} />
    </Stack>
  );
}
