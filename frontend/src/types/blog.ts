// Blog types for frontend

export interface BlogPost {
    id: number;
    authorId: number;
    authorName: string;
    authorAvatarUrl?: string;
    categoryId?: number;
    categoryName?: string;
    title: string;
    slug: string;
    excerpt?: string;
    content: string;
    coverImageUrl?: string;
    tags?: string;
    status: 'Draft' | 'Published' | 'Archived';
    isFeatured: boolean;
    viewCount: number;
    likeCount: number;
    commentCount: number;
    readingTimeMinutes: number;
    metaTitle?: string;
    metaDescription?: string;
    createdUtc: string;
    publishedUtc?: string;
}

export interface BlogPostListItem {
    id: number;
    authorId: number;
    authorName: string;
    authorAvatarUrl?: string;
    categoryName?: string;
    title: string;
    slug: string;
    excerpt?: string;
    coverImageUrl?: string;
    tags?: string;
    isFeatured: boolean;
    viewCount: number;
    likeCount: number;
    commentCount: number;
    readingTimeMinutes: number;
    publishedUtc?: string;
}

export interface BlogComment {
    id: number;
    postId: number;
    userId: number;
    userName: string;
    userAvatarUrl?: string;
    parentCommentId?: number;
    content: string;
    likeCount: number;
    createdUtc: string;
    replies?: BlogComment[];
}

export interface BlogCategory {
    id: number;
    name: string;
    slug: string;
    description?: string;
    iconUrl?: string;
    postCount: number;
    displayOrder: number;
    isActive: boolean;
}

export interface PaginatedBlogPosts {
    items: BlogPostListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface PaginatedComments {
    items: BlogComment[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface CreateBlogPostRequest {
    title: string;
    content: string;
    excerpt?: string;
    coverImageUrl?: string;
    categoryId?: number;
    tags?: string;
    publish?: boolean;
}

export interface UpdateBlogPostRequest {
    id: number;
    title: string;
    content: string;
    excerpt?: string;
    coverImageUrl?: string;
    categoryId?: number;
    tags?: string;
    metaTitle?: string;
    metaDescription?: string;
}
