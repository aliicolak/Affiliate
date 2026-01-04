import api from './client';
import type { Collection, CollectionDetail } from '../types/collection';

export const collectionsApi = {
    getMy: () => api.get<Collection[]>('/collections').then(r => r.data),
    getUser: (userId: number) => api.get<Collection[]>(`/collections/user/${userId}`).then(r => r.data),
    search: (term: string) => api.get<Collection[]>('/collections/search', { params: { q: term } }).then(r => r.data),
    get: (id: number) => api.get<CollectionDetail>(`/collections/${id}`).then(r => r.data),
    create: (data: { name: string; description?: string; isPublic: boolean }) => api.post<number>('/collections', data).then(r => r.data),
    delete: (id: number) => api.delete(`/collections/${id}`).then(r => r.data),
    addItem: (collectionId: number, data: { entityType: string; entityId: number }) => api.post<number>(`/collections/${collectionId}/items`, data).then(r => r.data),
    removeItem: (itemId: number) => api.delete(`/collections/items/${itemId}`).then(r => r.data)
};
