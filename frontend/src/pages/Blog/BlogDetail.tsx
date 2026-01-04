import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useLanguage, useAuth } from '../../contexts';
import { blogApi } from '../../api/blog';
import type { BlogPost, BlogComment } from '../../types/blog';
import './Blog.css';

const BlogDetail = () => {
    const { slug } = useParams<{ slug: string }>();
    const { t } = useLanguage();
    const { user, isAuthenticated } = useAuth();
    const [post, setPost] = useState<BlogPost | null>(null);
    const [comments, setComments] = useState<BlogComment[]>([]);
    const [newComment, setNewComment] = useState('');
    const [isLoading, setIsLoading] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            if (!slug) return;
            setIsLoading(true);
            try {
                const postData = await blogApi.getPost(slug);
                setPost(postData);

                const commentsData = await blogApi.getComments(postData.id);
                setComments(commentsData.items);
            } catch {
                // Silent fail
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [slug]);

    const handleSubmitComment = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!post || !newComment.trim()) return;

        setIsSubmitting(true);
        try {
            await blogApi.addComment(post.id, newComment.trim());
            setNewComment('');
            // Refresh comments
            const commentsData = await blogApi.getComments(post.id);
            setComments(commentsData.items);
        } catch {
            // Silent fail
        } finally {
            setIsSubmitting(false);
        }
    };

    const formatDate = (dateStr?: string) => {
        if (!dateStr) return '';
        return new Date(dateStr).toLocaleDateString('tr-TR', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    };

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

    if (!post) {
        return (
            <div className="page">
                <div className="container">
                    <div className="empty-state">
                        <div className="empty-state-icon">📄</div>
                        <p>Post not found</p>
                        <Link to="/blog" className="btn btn-primary">Back to Blog</Link>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="page">
            <div className="container">
                <article className="blog-detail animate-fade-in">
                    <header className="blog-detail-header">
                        {post.categoryName && (
                            <span className="blog-detail-category">{post.categoryName}</span>
                        )}
                        <h1 className="blog-detail-title">{post.title}</h1>
                        <div className="blog-detail-meta">
                            <div className="blog-detail-author">
                                <div className="avatar avatar-sm">
                                    {post.authorName.charAt(0)}
                                </div>
                                <span>{post.authorName}</span>
                            </div>
                            <span>{formatDate(post.publishedUtc)}</span>
                            <span>{post.readingTimeMinutes} min read</span>
                            <span>{post.viewCount} views</span>
                        </div>
                    </header>

                    {post.coverImageUrl && (
                        <img
                            src={post.coverImageUrl}
                            alt={post.title}
                            className="blog-detail-cover"
                        />
                    )}

                    <div
                        className="blog-detail-content"
                        dangerouslySetInnerHTML={{ __html: post.content }}
                    />

                    {/* Tags */}
                    {post.tags && (
                        <div className="blog-tags mt-lg">
                            {post.tags.split(',').map((tag, i) => (
                                <Link key={i} to={`/blog?tag=${tag.trim()}`} className="badge">
                                    #{tag.trim()}
                                </Link>
                            ))}
                        </div>
                    )}

                    {/* Edit button for author */}
                    {user && post.authorId === user.id && (
                        <div className="mt-lg">
                            <Link to={`/blog/edit/${post.id}`} className="btn btn-secondary">
                                {t('common.edit') || 'Edit Post'}
                            </Link>
                        </div>
                    )}

                    {/* Comments Section */}
                    <section className="comments-section">
                        <h3>{t('blog.comments') || 'Comments'} ({comments.length})</h3>

                        {isAuthenticated && (
                            <form className="comment-form" onSubmit={handleSubmitComment}>
                                <textarea
                                    className="form-input"
                                    placeholder={t('blog.writeComment') || 'Write a comment...'}
                                    value={newComment}
                                    onChange={(e) => setNewComment(e.target.value)}
                                    disabled={isSubmitting}
                                />
                                <button
                                    type="submit"
                                    className="btn btn-primary mt-sm"
                                    disabled={isSubmitting || !newComment.trim()}
                                >
                                    {isSubmitting ? <span className="spinner"></span> : (t('blog.postComment') || 'Post Comment')}
                                </button>
                            </form>
                        )}

                        <div className="comments-list">
                            {comments.length === 0 ? (
                                <p className="text-muted">{t('blog.noComments') || 'No comments yet.'}</p>
                            ) : (
                                comments.map(comment => (
                                    <div key={comment.id} className="comment-item">
                                        <div className="comment-header">
                                            <div className="avatar avatar-sm">
                                                {comment.userName.charAt(0)}
                                            </div>
                                            <span className="comment-author">{comment.userName}</span>
                                            <span className="comment-date">{formatDate(comment.createdUtc)}</span>
                                        </div>
                                        <p className="comment-content">{comment.content}</p>
                                    </div>
                                ))
                            )}
                        </div>
                    </section>
                </article>
            </div>
        </div>
    );
};

export default BlogDetail;
