import { useEffect, useState } from 'react';
import type { Collection } from '../../types';
import { collectionsApi } from '../../api';
import { Link } from 'react-router-dom';

const Collections = () => {
    const [collections, setCollections] = useState<Collection[]>([]);
    const [searchResults, setSearchResults] = useState<Collection[]>([]);
    const [activeTab, setActiveTab] = useState<'my' | 'explore'>('my');
    const [searchTerm, setSearchTerm] = useState('');
    const [showCreate, setShowCreate] = useState(false);
    const [newName, setNewName] = useState('');

    useEffect(() => {
        collectionsApi.getMy().then(setCollections);
        collectionsApi.search('').then(setSearchResults);
    }, []);

    const handleCreate = async () => {
        if (!newName) return;
        await collectionsApi.create({ name: newName, isPublic: true });
        collectionsApi.getMy().then(setCollections);
        setShowCreate(false);
        setNewName('');
    };

    const handleDelete = async (id: number) => {
        if (window.confirm('Delete this collection?')) {
            await collectionsApi.delete(id);
            setCollections(prev => prev.filter(c => c.id !== id));
        }
    };

    const handleSearch = async (e: React.FormEvent) => {
        e.preventDefault();
        const res = await collectionsApi.search(searchTerm);
        setSearchResults(res);
    };

    const displayList = activeTab === 'my' ? collections : searchResults;

    return (
        <div className="page-container">
            <div className="flex flex-col md:flex-row justify-between items-center mb-6 gap-4">
                <div className="flex gap-6 border-b w-full md:w-auto">
                    <button
                        className={`pb-2 px-4 font-semibold ${activeTab === 'my' ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'}`}
                        onClick={() => setActiveTab('my')}
                    >
                        My Collections
                    </button>
                    <button
                        className={`pb-2 px-4 font-semibold ${activeTab === 'explore' ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'}`}
                        onClick={() => setActiveTab('explore')}
                    >
                        Explore
                    </button>
                </div>
                {activeTab === 'my' && (
                    <button className="btn btn-primary" onClick={() => setShowCreate(true)}>Create New</button>
                )}
            </div>

            {activeTab === 'explore' && (
                <form onSubmit={handleSearch} className="mb-6 flex gap-2">
                    <input
                        type="text"
                        value={searchTerm}
                        onChange={e => setSearchTerm(e.target.value)}
                        placeholder="Search collections..."
                        className="form-input flex-1 border rounded p-2"
                    />
                    <button type="submit" className="btn btn-secondary">Search</button>
                </form>
            )}

            {showCreate && (
                <div className="create-form card p-4 mb-4 border rounded shadow-sm">
                    <h3 className="font-bold mb-2">Create Collection</h3>
                    <div className="flex gap-2">
                        <input
                            type="text"
                            value={newName}
                            onChange={e => setNewName(e.target.value)}
                            placeholder="Collection Name"
                            className="form-control flex-1 border rounded p-2"
                        />
                        <button className="btn btn-success" onClick={handleCreate}>Save</button>
                        <button className="btn btn-secondary" onClick={() => setShowCreate(false)}>Cancel</button>
                    </div>
                </div>
            )}

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                {displayList.map(c => (
                    <div key={c.id} className="card p-4 hover:shadow-md transition-shadow border rounded bg-white">
                        <Link to={`/collections/${c.id}`} className="block">
                            <h3 className="font-bold text-lg mb-1">{c.name}</h3>
                            {c.description && <p className="text-gray-500 text-sm mb-2">{c.description}</p>}
                            <div className="flex justify-between items-center text-sm text-gray-400">
                                <span>{c.itemCount} items</span>
                                <span>{new Date(c.createdUtc).toLocaleDateString()}</span>
                            </div>
                        </Link>

                        {activeTab === 'my' && (
                            <div className="mt-3 pt-3 border-t flex justify-end">
                                <button className="text-red-500 text-sm hover:underline" onClick={() => handleDelete(c.id)}>Delete</button>
                            </div>
                        )}
                    </div>
                ))}

                {displayList.length === 0 && (
                    <div className="col-span-full text-center py-10 text-gray-400">
                        {activeTab === 'my' ? "You haven't created any collections yet." : "No collections found."}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Collections;
