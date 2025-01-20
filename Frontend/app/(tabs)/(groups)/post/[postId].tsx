import AsyncStorage from '@react-native-async-storage/async-storage';
import { router, useLocalSearchParams } from 'expo-router';
import { jwtDecode } from 'jwt-decode';
import React, { useState, useEffect } from 'react';
import { View, Text, TouchableOpacity, FlatList, StyleSheet, Image, Alert, TouchableWithoutFeedback, Modal, ToastAndroid } from 'react-native';

export default function CommentsScreen() {
    const { postId } = useLocalSearchParams(); // Nhận ID bài viết và tiêu đề từ params
    const [title, setTitle] = useState('');
    const [comments, setComments] = useState([]);
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(3); // Giả sử có 3 trang
    const [role, setRole] = useState(null);


    useEffect(() => {
        fetchTitle();
        fetchComments();
    }, [pageNumber]);

    useEffect(() => {
        setPageNumber(1);
        fetchTitle();
        fetchComments();
    }, [postId]);

    const fetchTitle = async () => {
        try {
            const token = await AsyncStorage.getItem('authToken');
            if (token) {
                const decoded = jwtDecode(token);
                setRole(decoded.role);
            }

            const response = await fetch(`http://10.0.2.2:5151/api/posts/${postId}`);
            const data = await response.json();
            setTitle(data.title);
        } catch (error) {
            console.error('Error fetching title:', error);
        }
    }

    const fetchComments = async () => {
        try {
            const response = await fetch(`http://10.0.2.2:5151/api/posts/${postId}/comments?pageNumber=${pageNumber}&pageSize=10`);
            const data = await response.json();
            setComments(data);
            // Nếu API trả về tổng số trang, cập nhật totalPages
            // setTotalPages(data.totalPages);
        } catch (error) {
            console.error('Error fetching comments:', error);
        }
    };

    const handleAddComment = async () => {
        router.replace(`/post/addComment?postId=${postId}`)
    };

    const [modalVisible, setModalVisible] = useState(false);

    const handleLongPress = () => {
        setModalVisible(true);
    };

    const handleViewUser = (id) => {
        router.push(`/user/${id}`)
    };

    const handleChangeCommentStatus = async (id, status) => {
        const token = await AsyncStorage.getItem('authToken');

        const response = await fetch(`http://10.0.2.2:5151/api/admin/comments/${id}?status=${status}`, {
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

    const renderComment = ({ item }) => (
        <TouchableWithoutFeedback onLongPress={handleLongPress}>
            <View style={styles.comment}>
                <Image source={{ uri: `http://10.0.2.2:5151/${item.user.avatarImageURI}` }} style={styles.avatar} />
                <View style={styles.commentContent}>
                    <Text style={styles.commentUser}>{item.user.name}</Text>
                    {item.imageName && (
                        <Image source={{ uri: `http://10.0.2.2:5151/images/${item.imageName}` }} style={styles.commentImage} />
                    )}
                    {item.status !== "HIDED" && <Text style={styles.commentText}>{item.content}</Text>}
                    {item.status === "HIDED" && <Text style={styles.commentText}>[Bình luận đã bị ẩn]</Text>}
                    <Text style={styles.commentTime}>{new Date(item.createdTime).toLocaleString()}</Text>
                </View>
                <Modal
                    visible={modalVisible}
                    transparent={true}
                    animationType="fade"
                    onRequestClose={() => setModalVisible(false)}
                >
                    <TouchableWithoutFeedback onPress={() => setModalVisible(false)}>
                        <View style={styles.modalOverlay}>
                            <View style={styles.modalContent}>
                                <TouchableOpacity onPress={() => handleViewUser(item.user.id)} style={styles.modalButton}>
                                    <Text style={styles.modalButtonText}>Xem người dùng</Text>
                                </TouchableOpacity>
                                {role === 'ADMIN' && (
                                    <TouchableOpacity onPress={() => handleChangeCommentStatus(item.id, "HIDED")} style={styles.modalButton}>
                                        <Text style={styles.modalButtonText}>Ẩn bình luận</Text>
                                    </TouchableOpacity>
                                )}
                                <TouchableOpacity onPress={() => setModalVisible(false)} style={styles.modalButton}>
                                    <Text style={styles.modalButtonText}>Hủy</Text>
                                </TouchableOpacity>
                            </View>
                        </View>
                    </TouchableWithoutFeedback>
                </Modal>
            </View>
        </TouchableWithoutFeedback>
    );

    return (
        <View style={styles.container}>
            {/* Tiêu đề bài viết */}
            <Text style={styles.title}>{title}</Text>
            {/* Nút thêm bình luận */}
            <View style={styles.commentInputContainer}>
                <TouchableOpacity style={styles.addButton} onPress={handleAddComment}>
                    <Text style={styles.addButtonText}>Bình luận</Text>
                </TouchableOpacity>
            </View>
            {/* Danh sách bình luận */}
            <FlatList
                data={comments}
                renderItem={renderComment}
                keyExtractor={(item) => item.id}
                style={styles.commentsList}
            />
            {/* Phân trang */}
            <View style={styles.pagination}>
                <TouchableOpacity
                    disabled={pageNumber === 1}
                    onPress={() => setPageNumber((prev) => Math.max(prev - 1, 1))}
                >
                    <Text style={[styles.pageButton, pageNumber === 1 && styles.disabled]}>Trang trước</Text>
                </TouchableOpacity>
                <Text style={styles.pageNumber}>{pageNumber}</Text>
                <TouchableOpacity
                    disabled={pageNumber === totalPages}
                    onPress={() => setPageNumber((prev) => Math.min(prev + 1, totalPages))}
                >
                    <Text style={[styles.pageButton, pageNumber === totalPages && styles.disabled]}>Trang sau</Text>
                </TouchableOpacity>
            </View>
        </View>
    );
}

const styles = StyleSheet.create({
    container: { flex: 1, padding: 16 },
    title: { fontSize: 20, fontWeight: 'bold', marginBottom: 16 },
    commentInputContainer: { flexDirection: 'row', marginBottom: 16 },
    input: { flex: 1, borderWidth: 1, borderColor: '#ccc', padding: 8, borderRadius: 4 },
    addButton: { marginLeft: 8, backgroundColor: '#007BFF', padding: 10, borderRadius: 4 },
    addButtonText: { color: '#fff', fontWeight: 'bold' },
    commentsList: { flex: 1 },
    comment: { flexDirection: 'row', marginBottom: 16 },
    avatar: { width: 40, height: 40, borderRadius: 20, marginRight: 8 },
    commentContent: { flex: 1 },
    commentUser: { fontWeight: 'bold' },
    commentImage: { width: '100%', aspectRatio: 16 / 9, borderRadius: 8, marginBottom: 8 },
    commentText: { marginVertical: 4 },
    commentTime: { fontSize: 12, color: '#666' },
    pagination: { flexDirection: 'row', justifyContent: 'center', alignItems: 'center', marginTop: 16 },
    pageButton: { marginHorizontal: 8, fontSize: 16, color: '#007BFF' },
    disabled: { color: '#ccc' },
    pageNumber: { fontSize: 16, fontWeight: 'bold' },
    modalOverlay: {
        flex: 1,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    modalContent: {
        width: '80%',
        backgroundColor: '#fff',
        borderRadius: 8,
        padding: 16,
        alignItems: 'center',
    },
    modalButton: {
        paddingVertical: 12,
        paddingHorizontal: 24,
        marginVertical: 8,
        backgroundColor: '#007BFF',
        borderRadius: 8,
        width: '100%',
        alignItems: 'center',
    },
    modalButtonText: {
        color: '#fff',
        fontSize: 16,
        fontWeight: 'bold',
    },
});
