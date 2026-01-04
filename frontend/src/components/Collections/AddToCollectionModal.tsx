import { useEffect, useState } from 'react';
import { collectionsApi } from '../../api';
import type { Collection } from '../../types';

interface Props {
    entityType: 'Offer' | 'ProductShare';
    entityId: number;
    onClose: () => void;
}

const AddToCollectionModal = ({ entityType, entityId, onClose }: Props) => {
    const [collections, setCollections] = useState<Collection[]>([]);
    const [newName, setNewName] = useState('');
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        collectionsApi.getMy().then(setCollections);
    }, []);

    const handleCreate = async () => {
        if (!newName) return;
        setLoading(true);
        try {
            const id = await collectionsApi.create({ name: newName, isPublic: true });
            await collectionsApi.addItem(id, { entityType, entityId });
            onClose();
        } catch (e) {
            console.error(e);
        } finally {
            setLoading(false);
        }
    };

    const handleAddTo = async (collectionId: number) => {
        setLoading(true);
        try {
            await collectionsApi.addItem(collectionId, { entityType, entityId });
            onClose();
        } catch (e) {
            console.error(e);
            alert('Failed to add (maybe already exists?)');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded p-6 max-w-sm w-full shadow-xl">
                <h3 className="text-lg font-bold mb-4">Save to Collection</h3>

                <div className="max-h-60 overflow-y-auto mb-4 border rounded">
                    {collections.length === 0 && <p className="text-center p-2 text-gray-500">No collections yet</p>}
                    {collections.map(c => (
                        <button
                            key={c.id}
                            onClick={() => handleAddTo(c.id)}
                            className="w-full text-left p-3 hover:bg-gray-100 border-b last:border-0 flex justify-between"
                        >
                            <span>{c.name}</span>
                            <span className="text-gray-400 text-xs">({c.itemCount})</span>
                        </button>
                    ))}
                </div>

                <div className="border-t pt-4">
                    <p className="text-sm mb-2 font-semibold">Create New Collection</p>
                    <div className="flex gap-2">
                        <input
                            type="text"
                            className="border rounded p-2 flex-1 text-sm"
                            value={newName}
                            onChange={e => setNewName(e.target.value)}
                            placeholder="Collection Name"
                        />
                        <button
                            className="bg-blue-600 text-white rounded px-3 py-1 text-sm disabled:opacity-50"
                            onClick={handleCreate}
                            disabled={!newName || loading}
                        >
                            Save
                        </button>
                    </div>
                </div>

                <button className="w-full mt-4 text-gray-500 text-sm hover:underline" onClick={onClose}>Cancel</button>
            </div>
        </div>
    );
};

export default AddToCollectionModal;
