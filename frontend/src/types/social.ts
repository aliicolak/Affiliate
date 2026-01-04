// Social feature types

export interface ProductShare {
    id: number;
    userId: number;
    userName: string;
    userAvatarUrl?: string;
    offerId?: number;
    offerName?: string;
    offerImageUrl?: string;
    title: string;
    description?: string;
    shareType: 'Recommendation' | 'Review' | 'Showcase' | 'Comparison';
    rating?: number;
    pros?: string;
    cons?: string;
    imageUrl?: string;
    viewCount: number;
    likeCount: number;
    commentCount: number;
    isLikedByCurrentUser: boolean;
    createdUtc: string;
}

export interface ProductShareListItem {
    id: number;
    userId: number;
    userName: string;
    userAvatarUrl?: string;
    offerId?: number;
    offerName?: string;
    offerImageUrl?: string;
    title: string;
    shareType: 'Recommendation' | 'Review' | 'Showcase' | 'Comparison';
    rating?: number;
    viewCount: number;
    likeCount: number;
    commentCount: number;
    createdUtc: string;
}

export interface ShareComment {
    id: number;
    shareId: number;
    userId: number;
    userName: string;
    userAvatarUrl?: string;
    parentCommentId?: number;
    content: string;
    likeCount: number;
    createdUtc: string;
    replies?: ShareComment[];
}

export interface UserFollow {
    id: number;
    followerId: number;
    followerName: string;
    followerAvatarUrl?: string;
    followingId: number;
    followingName: string;
    followingAvatarUrl?: string;
    createdUtc: string;
}

export interface UserProfile {
    id: number;
    displayName: string;
    avatarUrl?: string;
    bio?: string;
    followerCount: number;
    followingCount: number;
    postCount: number;
    shareCount: number;
    isFollowedByCurrentUser: boolean;
    joinedUtc: string;
}

export interface PaginatedShares {
    items: ProductShareListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface PaginatedFollowers {
    items: UserFollow[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface CreateShareRequest {
    offerId: number;
    title: string;
    description?: string;
    shareType: string;
    rating?: number;
    pros?: string;
    cons?: string;
    imageUrl?: string;
}

export interface ToggleLikeRequest {
    entityType: 'BlogPost' | 'ProductShare' | 'BlogComment' | 'ShareComment';
    entityId: number;
}
