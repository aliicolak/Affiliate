// API Types - TypeScript interfaces matching backend DTOs

// Auth
export interface LoginRequest {
    userNameOrEmail: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    displayName: string;
    userName?: string;
}

export interface AuthResponse {
    accessToken: string;
    expiresUtc: string;
    refreshToken: string;
}

export interface MeResponse {
    id: number;
    displayName: string;
    email: string | null;
    roles: string[];
}

// Merchant
export interface Merchant {
    id: number;
    name: string;
    slug: string;
    description?: string;
    websiteUrl?: string;
    logoUrl?: string;
    isActive: boolean;
    createdUtc: string;
}

// Offer
export interface Offer {
    id: number;
    merchantId: number;
    merchantName: string;
    name: string;
    description?: string;
    commissionType: string;
    commissionValue: number;
    trackingUrl: string;
    isActive: boolean;
    startDate?: string;
    endDate?: string;
    createdUtc: string;
}

// Publisher
export interface Publisher {
    id: number;
    userId: number;
    companyName?: string;
    websiteUrl?: string;
    status: string;
    countryCode: string;
    createdUtc: string;
}

export interface PublisherDashboard {
    profile: Publisher;
    todayClicks: number;
    todayConversions: number;
    thisMonthEarnings: number;
    pendingBalance: number;
    availableBalance: number;
}

// Commission
export interface Commission {
    id: number;
    publisherId: number;
    offerId: number;
    offerName: string;
    commissionAmount: number;
    currency: string;
    status: string;
    createdUtc: string;
}

export interface EarningsSummary {
    totalEarnings: number;
    pendingAmount: number;
    approvedAmount: number;
    paidAmount: number;
}

// Notification
export interface Notification {
    id: number;
    type: string;
    title: string;
    message: string;
    isRead: boolean;
    actionUrl?: string;
    createdUtc: string;
}

// Click Stats
export interface ClickStats {
    date: string;
    clicks: number;
    conversions: number;
}

// Platform Stats (Admin)
export interface PlatformStats {
    merchants: { total: number; active: number };
    publishers: { total: number; active: number };
    offers: { total: number; active: number };
    clicks: { today: number; thisMonth: number; total: number };
    conversions: { today: number; thisMonth: number; total: number };
    commissions: {
        totalAmount: number;
        pendingAmount: number;
        approvedAmount: number;
        paidAmount: number;
    };
}

// Pagination
export interface PaginatedResponse<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
}

// API Error
export interface ApiError {
    status: number;
    title: string;
    detail: string;
    errors?: Record<string, string[]>;
}

export * from './collection';
export * from './social'; // Assuming social exists too
