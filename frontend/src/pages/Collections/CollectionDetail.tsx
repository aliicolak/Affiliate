import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import type { CollectionDetail as CollectionDetailType } from '../../types';
import { collectionsApi } from '../../api';

const CollectionDetail = () => {
    const { id } = useParams<{ id: string }>();
    const [collection, setCollection] = useState<CollectionDetailType | null>(null);

    useEffect(() => {
        if (id) {
            collectionsApi.get(Number(id)).then(setCollection);
        }
    }, [id]);

    const handleRemoveItem = async (itemId: number) => {
        if (window.confirm('Remove item?')) {
            await collectionsApi.removeItem(itemId);
            setCollection(prev => prev ? { ...prev, items: prev.items.filter(i => i.id !== itemId) } : null);
        }
    };

    if (!collection) return <div>Loading...</div>;

    return (
        <div className="page-container">
            <div className="header mb-4">
                <Link to="/collections">← Back to Collections</Link>
                <h1 className="text-2xl font-bold mt-2">{collection.name}</h1>
                <p className="text-gray-500">{collection.description}</p>
            </div>

            <div className="items-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {collection.items.map(item => (
                    <div key={item.id} className="card p-3 relative border rounded shadow-sm hover:shadow-md transition-shadow">
                        <button
                            className="absolute top-2 right-2 text-red-500 hover:text-red-700 bg-white rounded-full w-6 h-6 flex items-center justify-center shadow-sm"
                            onClick={(e) => { e.preventDefault(); handleRemoveItem(item.id); }}
                        >
                            ✕
                        </button>

                        {item.entityType === 'ProductShare' && item.entityData && (
                            <Link to={`/social/share/${item.entityId}`} className="flex gap-4">
                                {item.entityData.imageUrl && (
                                    <img src={item.entityData.imageUrl} className="w-24 h-24 object-cover rounded" alt={item.entityData.title} />
                                )}
                                <div>
                                    <h4 className="font-bold">{item.entityData.title}</h4>
                                    <span className="badge bg-blue-100 text-blue-800 text-xs mr-2">Share</span>
                                    {item.entityData.price && <p className="text-sm font-semibold text-green-600">{item.entityData.price} TL</p>}
                                </div>
                            </Link>
                        )}

                        {item.entityType === 'Offer' && item.entityData && (
                            <div className="flex flex-col gap-2">
                                <h4 className="font-bold">{item.entityData.title}</h4>
                                <span className="badge bg-green-100 text-green-800 text-xs w-fit">Offer</span>
                                {item.entityData.price && <p className="text-sm font-semibold text-green-600">{item.entityData.price} TL</p>}
                                <div className="mt-2 flex gap-2">
                                    <a href={item.entityData.url} target="_blank" rel="noopener noreferrer" className="btn btn-sm btn-primary flex-1 text-center">Go to Deal</a>
                                    <Link to={`/social/new?offerId=${item.entityId}`} className="btn btn-sm btn-outline-secondary">Share</Link>
                                </div>
                            </div>
                        )}
                    </div>
                ))}

                {collection.items.length === 0 && (
                    <div className="col-span-full text-center py-10 text-gray-400">
                        This collection is empty.
                    </div>
                )}
            </div>
        </div>
    );
};

export default CollectionDetail;
