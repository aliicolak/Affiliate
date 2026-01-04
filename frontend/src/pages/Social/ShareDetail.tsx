import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useLanguage, useAuth } from '../../contexts';
import { socialApi } from '../../api/social';
import type { ProductShare, ShareComment } from '../../types/social';
import LikeButton from '../../components/Social/LikeButton';
import { AddToCollectionModal } from '../../components';
import '../../components/Social/Social.css';

const ShareDetail = () => {
    const { id } = useParams<{ id: string }>();
    const { t } = useLanguage();
    const { user } = useAuth();
    const [share, setShare] = useState<ProductShare | null>(null);
    const [comments, setComments] = useState<ShareComment[]>([]);
    const [newComment, setNewComment] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [showSaveModal, setShowSaveModal] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            if (!id) return;
            try {
                const shareData = await socialApi.getShare(parseInt(id));
                setShare(shareData);
                const commentsData = await socialApi.getComments(parseInt(id));
                setComments(commentsData);
            } catch {
                // error
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [id]);

    const handlePostComment = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!share || !newComment.trim()) return;

        try {
            await socialApi.addComment(share.id, newComment);
            setNewComment('');
            // Refresh comments
            const commentsData = await socialApi.getComments(share.id);
            setComments(commentsData);
        } catch {
            // error
        }
    };

    if (isLoading || !share) {
        return (
            <div className="page">
                <div className="container center-content">
                    <div className="spinner"></div>
                </div>
            </div>
        );
    }

    return (
        <div className="page">
            <div className="container">
                <div className="feed-layout">
                    <main className="feed-main" style={{ gridColumn: '1 / -1', maxWidth: '800px', margin: '0 auto' }}>
                        <article className="share-card animate-fade-in">
                            <div className="share-card-header">
                                <Link to={`/profile/${share.userId}`} className="user-info">
                                    <div className="avatar avatar-sm">
                                        {share.userAvatarUrl ? <img src={share.userAvatarUrl} /> : share.userName.charAt(0)}
                                    </div>
                                    <div className="user-meta">
                                        <span className="user-name">{share.userName}</span>
                                        <div className="share-time">{new Date(share.createdUtc).toLocaleDateString()}</div>
                                    </div>
                                </Link>
                                <span className="share-type-badge">{share.shareType}</span>
                            </div>

                            <div className="share-content">
                                <h1 className="share-title" style={{ fontSize: '1.5rem' }}>{share.title}</h1>

                                {share.imageUrl && (
                                    <img src={share.imageUrl} alt={share.title} className="share-image" />
                                )}

                                <p className="share-description">{share.description}</p>

                                {share.offerName && (
                                    <Link to={`/offers/${share.offerId}`} className="share-offer-link">
                                        {share.offerImageUrl && <img src={share.offerImageUrl} className="share-offer-image" />}
                                        <div className="share-offer-info">
                                            <span className="share-offer-name">{share.offerName}</span>
                                        </div>
                                    </Link>
                                )}

                                {(share.pros || share.cons) && (
                                    <div className="review-section">
                                        {share.rating && <div className="mb-sm">Rating: {'⭐'.repeat(share.rating)}</div>}
                                        <div className="review-pros-cons">
                                            {share.pros && (
                                                <div>
                                                    <div className="pros-title">Pros</div>
                                                    <p>{share.pros}</p>
                                                </div>
                                            )}
                                            {share.cons && (
                                                <div>
                                                    <div className="cons-title">Cons</div>
                                                    <p>{share.cons}</p>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                )}
                            </div>

                            <div className="share-actions">
                                <LikeButton
                                    entityType="ProductShare"
                                    entityId={share.id}
                                    initialIsLiked={share.isLikedByCurrentUser}
                                    initialLikeCount={share.likeCount}
                                />
                                <span className="action-btn">
                                    <span className="icon">💬</span> {comments.length}
                                </span>
                                <button className="action-btn" onClick={() => setShowSaveModal(true)}>
                                    <span className="icon">🔖</span> Save
                                </button>
                            </div>
                        </article>

                        {showSaveModal && (
                            <AddToCollectionModal
                                entityType="ProductShare"
                                entityId={share.id}
                                onClose={() => setShowSaveModal(false)}
                            />
                        )}

                        {/* Comments */}
                        <section className="comments-section">
                            <h3>{t('social.comments')}</h3>

                            {user && (
                                <form onSubmit={handlePostComment} className="comment-form">
                                    <textarea
                                        className="form-input"
                                        value={newComment}
                                        onChange={e => setNewComment(e.target.value)}
                                        placeholder="Write a comment..."
                                    />
                                    <button type="submit" className="btn btn-primary mt-sm">Post</button>
                                </form>
                            )}

                            <div className="comments-list">
                                {comments.map(comment => (
                                    <div key={comment.id} className="comment-item">
                                        <div className="comment-header">
                                            <span className="comment-author">{comment.userName}</span>
                                            <span className="comment-date">{new Date(comment.createdUtc).toLocaleDateString()}</span>
                                        </div>
                                        <p className="comment-content">{comment.content}</p>
                                        <div className="mt-sm">
                                            <LikeButton
                                                entityType="ShareComment"
                                                entityId={comment.id}
                                                initialIsLiked={false} // API doesn't return isLiked for comments yet
                                                initialLikeCount={comment.likeCount}
                                                size="sm"
                                            />
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </section>
                    </main>
                </div>
            </div>
        </div>
    );
};

export default ShareDetail;
