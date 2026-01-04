import apiClient from './client';
import type {
    BlogPost,
    PaginatedBlogPosts,
    BlogCategory,
    PaginatedComments,
    CreateBlogPostRequest,
    UpdateBlogPostRequest,
} from '../types/blog';

export const blogApi = {
    // Posts
    getPosts: async (params?: {
        categoryId?: number;
        authorId?: number;
        tag?: string;
        search?: string;
        isFeatured?: boolean;
        sortBy?: string;
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedBlogPosts> => {
        const { data } = await apiClient.get('/blog/posts', { params });
        return data;
    },

    getPost: async (slugOrId: string): Promise<BlogPost> => {
        const { data } = await apiClient.get(`/blog/posts/${slugOrId}`);
        return data;
    },

    createPost: async (request: CreateBlogPostRequest): Promise<BlogPost> => {
        const { data } = await apiClient.post('/blog/posts', request);
        return data;
    },

    updatePost: async (request: UpdateBlogPostRequest): Promise<BlogPost> => {
        const { data } = await apiClient.put(`/blog/posts/${request.id}`, request);
        return data;
    },

    deletePost: async (id: number): Promise<void> => {
        await apiClient.delete(`/blog/posts/${id}`);
    },

    publishPost: async (id: number, publish: boolean = true): Promise<void> => {
        await apiClient.post(`/blog/posts/${id}/publish`, null, { params: { publish } });
    },

    getMyPosts: async (params?: {
        includeDrafts?: boolean;
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedBlogPosts> => {
        const { data } = await apiClient.get('/blog/my-posts', { params });
        return data;
    },

    // Comments
    getComments: async (postId: number, page: number = 1, pageSize: number = 20): Promise<PaginatedComments> => {
        const { data } = await apiClient.get(`/blog/posts/${postId}/comments`, { params: { page, pageSize } });
        return data;
    },

    addComment: async (postId: number, content: string, parentCommentId?: number): Promise<void> => {
        await apiClient.post(`/blog/posts/${postId}/comments`, { content, parentCommentId });
    },

    deleteComment: async (id: number): Promise<void> => {
        await apiClient.delete(`/blog/comments/${id}`);
    },

    // Categories
    getCategories: async (includeEmpty: boolean = false): Promise<BlogCategory[]> => {
        const { data } = await apiClient.get('/blog/categories', { params: { includeEmpty } });
        return data;
    },
};
