import api from './client';
import type {
    Merchant,
    Offer,
    PublisherDashboard,
    Commission,
    EarningsSummary,
    Notification,
    PaginatedResponse,
    PlatformStats,
    ClickStats
} from '../types';

// Merchants API
export const merchantsApi = {
    getAll: async (): Promise<Merchant[]> => {
        const response = await api.get<Merchant[]>('/merchants');
        return response.data;
    },

    getById: async (id: number): Promise<Merchant> => {
        const response = await api.get<Merchant>(`/merchants/${id}`);
        return response.data;
    },
};

// Offers API
export const offersApi = {
    getAll: async (params?: {
        merchantId?: number;
        categoryId?: number;
        isActive?: boolean;
        page?: number;
        pageSize?: number;
        search?: string;
    }): Promise<PaginatedResponse<Offer>> => {
        const response = await api.get<PaginatedResponse<Offer>>('/offers', { params });
        return response.data;
    },

    getById: async (id: number): Promise<Offer> => {
        const response = await api.get<Offer>(`/offers/${id}`);
        return response.data;
    },
};

// Publishers API
export const publishersApi = {
    register: async (data: {
        companyName?: string;
        websiteUrl?: string;
        promotionMethods?: string;
        countryCode?: string;
    }): Promise<{ publisherId: number }> => {
        const response = await api.post<{ publisherId: number }>('/publishers/register', data);
        return response.data;
    },

    getDashboard: async (): Promise<PublisherDashboard> => {
        const response = await api.get<PublisherDashboard>('/publishers/dashboard');
        return response.data;
    },

    updateProfile: async (data: {
        companyName?: string;
        websiteUrl?: string;
        promotionMethods?: string;
        taxId?: string;
        preferredPaymentMethod?: string;
        paymentDetails?: string;
    }): Promise<void> => {
        await api.put('/publishers/profile', data);
    },
};

// Commissions API
export const commissionsApi = {
    getMy: async (params?: {
        startDate?: string;
        endDate?: string;
        status?: string;
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedResponse<Commission>> => {
        const response = await api.get<PaginatedResponse<Commission>>('/commissions/my', { params });
        return response.data;
    },

    getMySummary: async (params?: {
        startDate?: string;
        endDate?: string;
    }): Promise<EarningsSummary> => {
        const response = await api.get<EarningsSummary>('/commissions/my/summary', { params });
        return response.data;
    },

    requestPayout: async (data: {
        amount: number;
        currency?: string;
        paymentMethod: string;
        paymentDetails?: string;
    }): Promise<{ payoutId: number }> => {
        const response = await api.post<{ payoutId: number }>('/commissions/payout-request', data);
        return response.data;
    },
};

// Reports API
export const reportsApi = {
    getPlatformStats: async (): Promise<PlatformStats> => {
        const response = await api.get<PlatformStats>('/reports/platform-stats');
        return response.data;
    },

    getMyPerformance: async (params?: {
        startDate?: string;
        endDate?: string;
    }): Promise<{
        period: { start: string; end: string };
        summary: { totalClicks: number; totalConversions: number; conversionRate: number; totalEarnings: number };
        dailyStats: ClickStats[];
    }> => {
        const response = await api.get('/reports/my-performance', { params });
        return response.data;
    },
};

// Notifications API
export const notificationsApi = {
    getMy: async (params?: {
        unreadOnly?: boolean;
        page?: number;
        pageSize?: number;
    }): Promise<PaginatedResponse<Notification> & { unreadCount: number }> => {
        const response = await api.get('/notifications', { params });
        return response.data;
    },

    markAsRead: async (id: number): Promise<void> => {
        await api.put(`/notifications/${id}/read`);
    },

    markAllAsRead: async (): Promise<{ markedCount: number }> => {
        const response = await api.put<{ markedCount: number }>('/notifications/mark-all-read');
        return response.data;
    },
};

// Tracking API
export const trackingApi = {
    generateCode: async (): Promise<{ code: string }> => {
        const response = await api.post<{ code: string }>('/tracking/generate-code');
        return response.data;
    },

    getStats: async (params?: {
        publisherId?: number;
        merchantId?: number;
        offerId?: number;
        startDate?: string;
        endDate?: string;
        groupBy?: string;
    }): Promise<ClickStats[]> => {
        const response = await api.get<ClickStats[]>('/tracking/stats', { params });
        return response.data;
    },
};
