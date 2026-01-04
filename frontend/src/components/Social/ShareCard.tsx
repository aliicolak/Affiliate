import { Link } from 'react-router-dom';
import type { ProductShareListItem } from '../../types/social';
import LikeButton from './LikeButton';
import './Social.css'; // Expecting CSS in pages/Social/Social.css or define here for component

interface ShareCardProps {
    share: ProductShareListItem;
}

const ShareCard = ({ share }: ShareCardProps) => {
    const formatDate = (dateStr: string) => {
        return new Date(dateStr).toLocaleDateString();
    };

    return (
        <article className="share-card animate-fade-in">
            <div className="share-card-header">
                <Link to={`/profile/${share.userId}`} className="user-info">
                    <div className="avatar avatar-sm">
                        {share.userAvatarUrl ? (
                            <img src={share.userAvatarUrl} alt={share.userName} />
                        ) : (
                            share.userName.charAt(0)
                        )}
                    </div>
                    <div className="user-meta">
                        <span className="user-name">{share.userName}</span>
                        <div className="share-time">{formatDate(share.createdUtc)}</div>
                    </div>
                </Link>
                <span className="share-type-badge">{share.shareType}</span>
            </div>

            <div className="share-content">
                <Link to={`/social/share/${share.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
                    <h3 className="share-title">{share.title}</h3>
                </Link>

                {share.offerName && (
                    <Link to={`/offers/${share.offerId || ''}`} className="share-offer-link">
                        {share.offerImageUrl && (
                            <img src={share.offerImageUrl} alt={share.offerName} className="share-offer-image" />
                        )}
                        <div className="share-offer-info">
                            <span className="share-offer-name">{share.offerName}</span>
                            <span className="text-muted text-sm">View Offer →</span>
                        </div>
                    </Link>
                )}

                {share.rating && (
                    <div className="share-rating mb-md">
                        {'⭐'.repeat(share.rating)}
                    </div>
                )}
            </div>

            <div className="share-actions">
                <LikeButton
                    entityType="ProductShare"
                    entityId={share.id}
                    initialIsLiked={false} // List item doesn't carry liked state usually, might need optimization
                    initialLikeCount={share.likeCount}
                />
                <Link to={`/social/share/${share.id}`} className="action-btn">
                    <span className="icon">💬</span>
                    <span>{share.commentCount}</span>
                </Link>
                <button className="action-btn">
                    <span className="icon">↗️</span>
                </button>
            </div>
        </article>
    );
};

export default ShareCard;
