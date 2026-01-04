import apiClient from './client';
import type {
    ProductShare,
    PaginatedShares,
    ShareComment,
    UserProfile,
    PaginatedFollowers,
    CreateShareRequest,
    ToggleLikeRequest,
} from '../types/social';

export const socialApi = {
    // Feed & Shares
    getFeed: async (page: number = 1, pageSize: number = 20): Promise<PaginatedShares> => {
        const { data } = await apiClient.get('/social/feed', { params: { page, pageSize } });
        return data;
    },

    getShares: async (params?: {
        userId?: number;
        offerId?: number;
        shareType?: string;
        sortBy?: string;
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedShares> => {
        const { data } = await apiClient.get('/social/shares', { params });
        return data;
    },

    getShare: async (id: number): Promise<ProductShare> => {
        const { data } = await apiClient.get(`/social/shares/${id}`);
        return data;
    },

    createShare: async (request: CreateShareRequest): Promise<ProductShare> => {
        const { data } = await apiClient.post('/social/shares', request);
        return data;
    },

    deleteShare: async (id: number): Promise<void> => {
        await apiClient.delete(`/social/shares/${id}`);
    },

    // Comments
    getComments: async (shareId: number, page: number = 1, pageSize: number = 20): Promise<ShareComment[]> => {
        const { data } = await apiClient.get(`/social/shares/${shareId}/comments`, { params: { page, pageSize } });
        return data;
    },

    addComment: async (shareId: number, content: string, parentCommentId?: number): Promise<ShareComment> => {
        const { data } = await apiClient.post(`/social/shares/${shareId}/comments`, { content, parentCommentId });
        return data;
    },

    deleteComment: async (id: number): Promise<void> => {
        await apiClient.delete(`/social/comments/${id}`);
    },

    // Likes
    toggleLike: async (request: ToggleLikeRequest): Promise<{ isLiked: boolean; likeCount: number }> => {
        const { data } = await apiClient.post('/social/like', request);
        return data;
    },

    // Follow & Profile
    toggleFollow: async (userId: number): Promise<{ isFollowing: boolean; followerCount: number }> => {
        const { data } = await apiClient.post(`/social/users/${userId}/follow`);
        return data;
    },

    getProfile: async (userId: number): Promise<UserProfile> => {
        const { data } = await apiClient.get(`/social/users/${userId}/profile`);
        return data;
    },

    updateProfile: async (data: { displayName: string; bio?: string; avatarUrl?: string }): Promise<void> => {
        await apiClient.put('/social/profile', data);
    },

    getFollowers: async (userId: number, page: number = 1, pageSize: number = 20): Promise<PaginatedFollowers> => {
        const { data } = await apiClient.get(`/social/users/${userId}/followers`, { params: { page, pageSize } });
        return data;
    },

    getFollowing: async (userId: number, page: number = 1, pageSize: number = 20): Promise<PaginatedFollowers> => {
        const { data } = await apiClient.get(`/social/users/${userId}/following`, { params: { page, pageSize } });
        return data;
    },
};
