import { router } from 'expo-router';
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';

const HomeScreen = () => {
  const redirectTo = (category) => {
    router.push(`/category/${category}`);
  };

  return (
    <View className="flex-1 items-center bg-white">
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={() => {redirectTo("Diembao")}}>
        <Text className="text-xl">Điểm báo</Text>
      </TouchableOpacity>
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={() => {redirectTo("Tamsu")}}>
        <Text className="text-xl">Tâm sự</Text>
      </TouchableOpacity>
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={() => {redirectTo("CNTT")}}>
        <Text className="text-xl">CNTT</Text>
      </TouchableOpacity>
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={() => {redirectTo("Ngoaingu")}}>
        <Text className="text-xl">Ngoại ngữ</Text>
      </TouchableOpacity>
      <TouchableOpacity
        className="w-11/12 px-4 py-4 my-2 bg-blue-200 items-center"
        onPress={() => {redirectTo("Game")}}>
        <Text className="text-xl">Game</Text>
      </TouchableOpacity>
    </View>
  );
};

export default HomeScreen;
