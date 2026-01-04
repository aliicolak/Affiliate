import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useLanguage } from '../../contexts';
import { socialApi } from '../../api/social';
import { offersApi } from '../../api';
import type { Offer } from '../../types';
import '../../components/Social/Social.css';

const ShareEditor = () => {
    const { t } = useLanguage();
    const navigate = useNavigate();
    const [searchTerm, setSearchTerm] = useState('');
    const [foundOffers, setFoundOffers] = useState<Offer[]>([]);
    const [selectedOffer, setSelectedOffer] = useState<Offer | null>(null);
    const [isLoadingOffers, setIsLoadingOffers] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const [form, setForm] = useState({
        title: '',
        description: '',
        shareType: 'Recommendation',
        rating: 5,
        pros: '',
        cons: ''
    });

    // Search offers debounce
    useEffect(() => {
        if (!searchTerm || selectedOffer) {
            setFoundOffers([]);
            return;
        }

        const timeoutId = setTimeout(async () => {
            setIsLoadingOffers(true);
            try {
                const data = await offersApi.getAll({ search: searchTerm, pageSize: 5 });
                setFoundOffers(data.items);
            } catch {
                setFoundOffers([]);
            } finally {
                setIsLoadingOffers(false);
            }
        }, 500);

        return () => clearTimeout(timeoutId);
    }, [searchTerm, selectedOffer]);

    const handleSelectOffer = (offer: Offer) => {
        setSelectedOffer(offer);
        setSearchTerm(''); // Clear search to hide list
        setForm(prev => ({
            ...prev,
            title: prev.title || `Check out ${offer.name}!`
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedOffer) return;

        setIsSubmitting(true);
        try {
            const newShare = await socialApi.createShare({
                offerId: selectedOffer.id,
                title: form.title,
                description: form.description,
                shareType: form.shareType,
                rating: form.rating,
                pros: form.pros,
                cons: form.cons,
                imageUrl: undefined // Uses offer image by default
            });
            navigate(`/social/share/${newShare.id}`);
        } catch (error) {
            console.error('Failed to create share', error);
            // Show error toast?
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="page">
            <div className="container">
                <div className="feed-layout" style={{ display: 'block', maxWidth: '800px', margin: '0 auto' }}>
                    <div className="card animate-fade-in">
                        <div className="card-body">
                            <h1 className="mb-lg">{t('social.createShare') || 'Create New Share'}</h1>

                            {/* Offer Selection */}
                            <div className="form-group mb-lg">
                                <label className="form-label">{t('social.selectProduct') || 'Select Product'}</label>
                                {!selectedOffer ? (
                                    <div style={{ position: 'relative' }}>
                                        <input
                                            type="text"
                                            className="form-input"
                                            placeholder="Search for a product..."
                                            value={searchTerm}
                                            onChange={e => setSearchTerm(e.target.value)}
                                        />
                                        {isLoadingOffers && <div className="spinner spinner-sm" style={{ position: 'absolute', right: 10, top: 10 }}></div>}

                                        {foundOffers.length > 0 && (
                                            <div className="dropdown-menu show" style={{ width: '100%', maxHeight: '300px', overflowY: 'auto' }}>
                                                {foundOffers.map(offer => (
                                                    <div
                                                        key={offer.id}
                                                        className="dropdown-item"
                                                        onClick={() => handleSelectOffer(offer)}
                                                        style={{ display: 'flex', alignItems: 'center', gap: '10px' }}
                                                    >
                                                        <div style={{ fontWeight: 'bold' }}>{offer.name}</div>
                                                        <div className="text-muted text-sm">{offer.merchantName}</div>
                                                    </div>
                                                ))}
                                            </div>
                                        )}
                                    </div>
                                ) : (
                                    <div className="selected-offer-preview" style={{
                                        padding: '1rem',
                                        background: 'var(--color-bg-secondary)',
                                        borderRadius: 'var(--radius-md)',
                                        display: 'flex',
                                        justifyContent: 'space-between',
                                        alignItems: 'center'
                                    }}>
                                        <div>
                                            <strong>{selectedOffer.name}</strong>
                                            <div className="text-muted">{selectedOffer.merchantName}</div>
                                        </div>
                                        <button
                                            className="btn btn-ghost btn-sm"
                                            onClick={() => { setSelectedOffer(null); setSearchTerm(''); }}
                                        >
                                            Change
                                        </button>
                                    </div>
                                )}
                            </div>

                            <form onSubmit={handleSubmit}>
                                <div className="form-group">
                                    <label className="form-label">Type</label>
                                    <div className="btn-group" style={{ display: 'flex', gap: '10px', marginBottom: '1rem' }}>
                                        {['Recommendation', 'Review', 'Showcase'].map(type => (
                                            <button
                                                key={type}
                                                type="button"
                                                className={`btn ${form.shareType === type ? 'btn-primary' : 'btn-ghost'}`}
                                                onClick={() => setForm(f => ({ ...f, shareType: type }))}
                                            >
                                                {type}
                                            </button>
                                        ))}
                                    </div>
                                </div>

                                <div className="form-group">
                                    <label className="form-label">Title</label>
                                    <input
                                        type="text"
                                        className="form-input"
                                        required
                                        value={form.title}
                                        onChange={e => setForm(f => ({ ...f, title: e.target.value }))}
                                        placeholder="Give your share a catchy title"
                                    />
                                </div>

                                <div className="form-group">
                                    <label className="form-label">Description</label>
                                    <textarea
                                        className="form-input"
                                        rows={5}
                                        value={form.description}
                                        onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
                                        placeholder="Why do you recommend this?"
                                    />
                                </div>

                                {form.shareType === 'Review' && (
                                    <>
                                        <div className="form-group">
                                            <label className="form-label">Rating</label>
                                            <div style={{ fontSize: '1.5rem', cursor: 'pointer' }}>
                                                {[1, 2, 3, 4, 5].map(star => (
                                                    <span
                                                        key={star}
                                                        onClick={() => setForm(f => ({ ...f, rating: star }))}
                                                        style={{ color: star <= form.rating ? '#FFD700' : '#ddd' }}
                                                    >
                                                        ★
                                                    </span>
                                                ))}
                                            </div>
                                        </div>

                                        <div className="grid grid-2" style={{ gap: '1rem' }}>
                                            <div className="form-group">
                                                <label className="form-label" style={{ color: '#10b981' }}>Pros</label>
                                                <textarea
                                                    className="form-input"
                                                    rows={3}
                                                    value={form.pros}
                                                    onChange={e => setForm(f => ({ ...f, pros: e.target.value }))}
                                                    placeholder="What's good?"
                                                />
                                            </div>
                                            <div className="form-group">
                                                <label className="form-label" style={{ color: '#ef4444' }}>Cons</label>
                                                <textarea
                                                    className="form-input"
                                                    rows={3}
                                                    value={form.cons}
                                                    onChange={e => setForm(f => ({ ...f, cons: e.target.value }))}
                                                    placeholder="What's not so good?"
                                                />
                                            </div>
                                        </div>
                                    </>
                                )}

                                <div className="form-actions mt-xl" style={{ display: 'flex', justifyContent: 'flex-end', gap: '1rem' }}>
                                    <button
                                        type="button"
                                        className="btn btn-ghost"
                                        onClick={() => navigate('/social')}
                                    >
                                        Cancel
                                    </button>
                                    <button
                                        type="submit"
                                        className="btn btn-primary"
                                        disabled={!selectedOffer || isSubmitting}
                                    >
                                        {isSubmitting ? 'Publishing...' : 'Publish Share'}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ShareEditor;
