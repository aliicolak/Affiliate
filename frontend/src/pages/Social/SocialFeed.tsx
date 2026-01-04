import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useLanguage, useAuth } from '../../contexts';
import { socialApi } from '../../api/social';
import type { ProductShareListItem } from '../../types/social';
import ShareCard from '../../components/Social/ShareCard';
import '../../components/Social/Social.css';

const SocialFeed = () => {
    const { t } = useLanguage();
    const { user } = useAuth();
    const [shares, setShares] = useState<ProductShareListItem[]>([]);
    const [page, setPage] = useState(1);
    const [hasMore, setHasMore] = useState(true);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchFeed = async () => {
            try {
                const data = await socialApi.getFeed(page);
                if (page === 1) setShares(data.items);
                else setShares(prev => [...prev, ...data.items]);

                setHasMore(page < data.totalPages);
            } catch {
                // error
            } finally {
                setIsLoading(false);
            }
        };
        fetchFeed();
    }, [page]);

    return (
        <div className="page">
            <div className="container">
                <div className="feed-layout">
                    {/* Left Sidebar */}
                    <aside className="feed-sidebar-left">
                        <div className="card">
                            <div className="card-body">
                                <h3>{t('social.trending') || 'Trending'}</h3>
                                {/* Placeholder for trending tags/topics */}
                                <p className="text-muted text-sm mt-sm">#SummerSale</p>
                                <p className="text-muted text-sm">#TechReviews</p>
                            </div>
                        </div>
                    </aside>

                    {/* Main Feed */}
                    <main className="feed-main">
                        <div className="feed-header mb-lg" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                            <h1>{t('social.feed') || 'Activity Feed'}</h1>
                            {user && (
                                <Link to="/social/new" className="btn btn-primary">
                                    {t('social.createShare') || 'Create Share'}
                                </Link>
                            )}
                        </div>

                        {shares.map(share => (
                            <ShareCard key={share.id} share={share} />
                        ))}

                        {isLoading && (
                            <div className="text-center p-lg">
                                <div className="spinner"></div>
                            </div>
                        )}

                        {!isLoading && hasMore && (
                            <button
                                className="btn btn-ghost w-100"
                                onClick={() => setPage(p => p + 1)}
                            >
                                {t('common.loadMore') || 'Load More'}
                            </button>
                        )}

                        {!isLoading && shares.length === 0 && (
                            <div className="empty-state">
                                <p>{t('social.noShares') || 'No activity yet'}</p>
                            </div>
                        )}
                    </main>

                    {/* Right Sidebar */}
                    <aside className="feed-sidebar-right">
                        <div className="card">
                            <div className="card-body">
                                <h3>{t('social.suggestions') || 'Who to follow'}</h3>
                                {/* Placeholder for suggestions */}
                                <p className="text-muted text-sm mt-sm">Coming soon...</p>
                            </div>
                        </div>
                    </aside>
                </div>
            </div>
        </div>
    );
};

export default SocialFeed;
