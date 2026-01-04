import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useLanguage } from '../../contexts';
import { blogApi } from '../../api/blog';
import type { BlogPost, BlogCategory } from '../../types/blog';
import './Blog.css';

const BlogEditor = () => {
    const { id } = useParams<{ id: string }>();
    const { t } = useLanguage();
    const navigate = useNavigate();
    const isEditing = !!id;

    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [excerpt, setExcerpt] = useState('');
    const [coverImageUrl, setCoverImageUrl] = useState('');
    const [categoryId, setCategoryId] = useState<number | undefined>();
    const [tags, setTags] = useState('');
    const [categories, setCategories] = useState<BlogCategory[]>([]);
    const [isLoading, setIsLoading] = useState(isEditing);
    const [isSaving, setIsSaving] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchData = async () => {
            try {
                const cats = await blogApi.getCategories(true);
                setCategories(cats);

                if (isEditing && id) {
                    const post = await blogApi.getPost(id);
                    setTitle(post.title);
                    setContent(post.content);
                    setExcerpt(post.excerpt || '');
                    setCoverImageUrl(post.coverImageUrl || '');
                    setCategoryId(post.categoryId);
                    setTags(post.tags || '');
                }
            } catch {
                setError('Failed to load data');
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [id, isEditing]);

    const handleSubmit = async (e: React.FormEvent, publish: boolean = false) => {
        e.preventDefault();
        if (!title.trim() || !content.trim()) {
            setError('Title and content are required');
            return;
        }

        setIsSaving(true);
        setError('');

        try {
            let post: BlogPost;
            if (isEditing && id) {
                post = await blogApi.updatePost({
                    id: parseInt(id),
                    title,
                    content,
                    excerpt: excerpt || undefined,
                    coverImageUrl: coverImageUrl || undefined,
                    categoryId,
                    tags: tags || undefined,
                });
            } else {
                post = await blogApi.createPost({
                    title,
                    content,
                    excerpt: excerpt || undefined,
                    coverImageUrl: coverImageUrl || undefined,
                    categoryId,
                    tags: tags || undefined,
                    publish,
                });
            }
            navigate(`/blog/${post.slug}`);
        } catch (err: unknown) {
            const error = err as { response?: { data?: { detail?: string } } };
            setError(error.response?.data?.detail || 'Failed to save post');
        } finally {
            setIsSaving(false);
        }
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

    return (
        <div className="page">
            <div className="container">
                <div className="page-header animate-fade-in">
                    <h1>{isEditing ? (t('blog.editPost') || 'Edit Post') : (t('blog.newPost') || 'New Post')}</h1>
                </div>

                <form className="blog-editor-form card animate-fade-in">
                    <div className="card-body">
                        {error && <div className="alert alert-error">{error}</div>}

                        <div className="form-group">
                            <label className="form-label">{t('blog.title') || 'Title'}</label>
                            <input
                                type="text"
                                className="form-input"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Enter post title..."
                                disabled={isSaving}
                            />
                        </div>

                        <div className="form-row" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                            <div className="form-group">
                                <label className="form-label">{t('blog.category') || 'Category'}</label>
                                <select
                                    className="form-input select"
                                    value={categoryId || ''}
                                    onChange={(e) => setCategoryId(e.target.value ? parseInt(e.target.value) : undefined)}
                                    disabled={isSaving}
                                >
                                    <option value="">Select category...</option>
                                    {categories.map(cat => (
                                        <option key={cat.id} value={cat.id}>{cat.name}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="form-group">
                                <label className="form-label">{t('blog.tags') || 'Tags'}</label>
                                <input
                                    type="text"
                                    className="form-input"
                                    value={tags}
                                    onChange={(e) => setTags(e.target.value)}
                                    placeholder="tag1, tag2, tag3"
                                    disabled={isSaving}
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label className="form-label">{t('blog.coverImage') || 'Cover Image URL'}</label>
                            <input
                                type="url"
                                className="form-input"
                                value={coverImageUrl}
                                onChange={(e) => setCoverImageUrl(e.target.value)}
                                placeholder="https://..."
                                disabled={isSaving}
                            />
                        </div>

                        <div className="form-group">
                            <label className="form-label">{t('blog.excerpt') || 'Excerpt'}</label>
                            <textarea
                                className="form-input"
                                value={excerpt}
                                onChange={(e) => setExcerpt(e.target.value)}
                                placeholder="Short description..."
                                rows={3}
                                disabled={isSaving}
                            />
                        </div>

                        <div className="form-group">
                            <label className="form-label">{t('blog.content') || 'Content'}</label>
                            <textarea
                                className="form-input"
                                value={content}
                                onChange={(e) => setContent(e.target.value)}
                                placeholder="Write your post content here... (HTML supported)"
                                rows={15}
                                style={{ fontFamily: 'monospace' }}
                                disabled={isSaving}
                            />
                        </div>

                        <div className="form-actions" style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end' }}>
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={() => navigate(-1)}
                                disabled={isSaving}
                            >
                                {t('common.cancel') || 'Cancel'}
                            </button>
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={(e) => handleSubmit(e, false)}
                                disabled={isSaving}
                            >
                                {isSaving ? <span className="spinner"></span> : (t('blog.saveDraft') || 'Save Draft')}
                            </button>
                            <button
                                type="submit"
                                className="btn btn-primary"
                                onClick={(e) => handleSubmit(e, true)}
                                disabled={isSaving}
                            >
                                {isSaving ? <span className="spinner"></span> : (t('blog.publish') || 'Publish')}
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default BlogEditor;
