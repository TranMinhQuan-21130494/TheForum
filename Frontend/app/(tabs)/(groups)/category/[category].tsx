import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, FlatList, Image, TouchableOpacity, Button, ActivityIndicator } from 'react-native';
import { useLocalSearchParams, router, useFocusEffect } from 'expo-router';

export default function PostListScreen() {
  const { category } = useLocalSearchParams();
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  // Hàm fetch dữ liệu từ API
  const fetchPosts = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `http://10.0.2.2:5151/api/posts?category=${category}&pageNumber=${currentPage}&pageSize=${pageSize}`
      );
      const data = await response.json();
      setPosts(data);
    } catch (error) {
      console.error('Error fetching posts:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPosts();
  }, [currentPage]);

  useEffect(() => {
    setCurrentPage(1);
    fetchPosts();
  }, [category]);

  // Xử lý chuyển trang
  const handleNextPage = () => {
    setCurrentPage((prev) => prev + 1);
  };

  const handlePreviousPage = () => {
    if (currentPage > 1) {
      setCurrentPage((prev) => prev - 1);
    }
  };

  // Xử lý nhấn vào bài viết
  const handlePostPress = (id) => {
    router.push(`/post/${id}`); // Chuyển hướng đến trang chi tiết bài viết
  };

  const renderItem = ({ item }) => (
    <TouchableOpacity onPress={() => handlePostPress(item.id)} style={styles.post}>
      {/* Avatar và tên người dùng */}
      <View style={styles.user}>
        <Image
          source={{ uri: `http://10.0.2.2:5151/${item.user.avatarImageURI}` }}
          style={styles.avatar}
        />
        <Text style={styles.userName}>{item.user.name}</Text>
      </View>

      {/* Thông tin bài viết */}
      <Text style={styles.title}>{item.title}</Text>
      <Text style={styles.meta}>
        Danh mục: {item.category} | Bình luận: {item.commentCount}
      </Text>
      <Text style={styles.time}>
        Ngày tạo: {new Date(item.createdTime).toLocaleDateString()}
      </Text>
      <Text style={styles.time}>
        Cập nhật lần cuối: {new Date(item.lastActivityTime).toLocaleDateString()}
      </Text>
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      {/* Header với tên Category và nút "Tạo bài viết" */}
      <View style={styles.header}>
        <Text style={styles.categoryName}>{category}</Text>
        <Button title="Tạo bài viết" onPress={() => router.push(`/post/create?category=${category}`)} />
      </View>

      {/* Danh sách bài viết */}
      {loading ? (
        <ActivityIndicator size="large" color="#007BFF" />
      ) : (
        <FlatList
          data={posts}
          keyExtractor={(item) => item.id}
          renderItem={renderItem}
          contentContainerStyle={styles.postList}
        />
      )}

      {/* Phần chuyển trang */}
      <View style={styles.pagination}>
        <TouchableOpacity
          onPress={handlePreviousPage}
          style={[styles.pageButton, currentPage === 1 && styles.disabledButton]}
          disabled={currentPage === 1}
        >
          <Text style={styles.pageText}>Trang trước</Text>
        </TouchableOpacity>
        <Text style={styles.currentPage}>{currentPage}</Text>
        <TouchableOpacity onPress={handleNextPage} style={styles.pageButton}>
          <Text style={styles.pageText}>Trang sau</Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f9f9f9',
    padding: 10,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 10,
  },
  categoryName: {
    fontSize: 20,
    fontWeight: 'bold',
  },
  postList: {
    paddingBottom: 20,
  },
  post: {
    marginBottom: 15,
    padding: 15,
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 5,
    backgroundColor: '#fff',
  },
  user: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 10,
  },
  avatar: {
    width: 40,
    height: 40,
    borderRadius: 20,
    marginRight: 10,
  },
  userName: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 5,
  },
  meta: {
    fontSize: 14,
    color: '#555',
    marginBottom: 5,
  },
  time: {
    fontSize: 12,
    color: '#888',
  },
  pagination: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: 10,
  },
  pageButton: {
    padding: 10,
    marginHorizontal: 5,
    borderRadius: 5,
    borderWidth: 1,
    borderColor: '#ccc',
  },
  disabledButton: {
    backgroundColor: '#ddd',
    borderColor: '#ddd',
  },
  currentPage: {
    fontSize: 16,
    fontWeight: 'bold',
    marginHorizontal: 10,
  },
  pageText: {
    color: '#007BFF',
  },
});
