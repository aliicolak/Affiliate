export interface Collection {
    id: number;
    name: string;
    description?: string;
    isPublic: boolean;
    isDefault: boolean;
    itemCount: number;
    createdUtc: string;
}

export interface CollectionItem {
    id: number;
    entityType: 'Offer' | 'ProductShare';
    entityId: number;
    entityData: any;
    addedUtc: string;
}

export interface CollectionDetail extends Omit<Collection, 'itemCount'> {
    items: CollectionItem[];
}
