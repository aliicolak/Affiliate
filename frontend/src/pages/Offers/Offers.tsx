import { useEffect, useState } from 'react';
import { useLanguage } from '../../contexts';
import { offersApi } from '../../api';
import type { Offer } from '../../types';
import './Offers.css';

const Offers = () => {
    const { t } = useLanguage();
    const [offers, setOffers] = useState<Offer[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [search, setSearch] = useState('');

    useEffect(() => {
        const fetchOffers = async () => {
            try {
                const data = await offersApi.getAll({ isActive: true });
                setOffers(data.items);
            } catch {
                // Silent fail
            } finally {
                setIsLoading(false);
            }
        };
        fetchOffers();
    }, []);

    const filteredOffers = offers.filter(offer =>
        offer.name.toLowerCase().includes(search.toLowerCase()) ||
        offer.merchantName?.toLowerCase().includes(search.toLowerCase())
    );

    if (isLoading) {
        return (
            <div className="page">
                <div className="container">
                    <div className="loading-state">
                        <div className="spinner"></div>
                        <p>{t('common.loading')}</p>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="page">
            <div className="container">
                <div className="page-header animate-fade-in">
                    <h1>{t('offers.title')}</h1>
                    <p>{t('offers.subtitle')}</p>
                </div>

                <div className="offers-toolbar animate-fade-in">
                    <input
                        type="text"
                        className="form-input"
                        placeholder={t('offers.search')}
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        style={{ maxWidth: '300px' }}
                    />
                </div>

                {filteredOffers.length === 0 ? (
                    <div className="empty-state animate-fade-in">
                        <div className="empty-state-icon">📦</div>
                        <p>{t('offers.noOffers')}</p>
                    </div>
                ) : (
                    <div className="offers-grid grid grid-3 animate-fade-in">
                        {filteredOffers.map(offer => (
                            <div key={offer.id} className="offer-card card">
                                <div className="card-body">
                                    <div className="offer-header">
                                        <h3>{offer.name}</h3>
                                        <span className={`badge ${offer.isActive ? 'badge-success' : ''}`}>
                                            {offer.isActive ? t('common.active') : t('common.inactive')}
                                        </span>
                                    </div>
                                    <p className="offer-merchant">{offer.merchantName}</p>
                                    {offer.description && (
                                        <p className="offer-description">{offer.description}</p>
                                    )}
                                    <div className="offer-commission">
                                        <span className="commission-label">{t('offers.commission')}</span>
                                        <span className="commission-value">
                                            {offer.commissionType === 'Percentage'
                                                ? `${offer.commissionValue}%`
                                                : `₺${offer.commissionValue}`}
                                        </span>
                                    </div>
                                    <button className="btn btn-primary btn-sm offer-cta">
                                        {t('offers.getLink')}
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Offers;
