import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useLanguage } from '../../contexts';
import { blogApi } from '../../api/blog';
import type { BlogPostListItem, BlogCategory } from '../../types/blog';
import './Blog.css';

const BlogList = () => {
    const { t } = useLanguage();
    const [searchParams, setSearchParams] = useSearchParams();
    const [posts, setPosts] = useState<BlogPostListItem[]>([]);
    const [categories, setCategories] = useState<BlogCategory[]>([]);
    const [totalPages, setTotalPages] = useState(1);
    const [isLoading, setIsLoading] = useState(true);

    const page = parseInt(searchParams.get('page') || '1');
    const categoryId = searchParams.get('category') ? parseInt(searchParams.get('category')!) : undefined;
    const search = searchParams.get('search') || '';

    useEffect(() => {
        const fetchData = async () => {
            setIsLoading(true);
            try {
                const [postsData, categoriesData] = await Promise.all([
                    blogApi.getPosts({ page, categoryId, search: search || undefined, pageSize: 9 }),
                    blogApi.getCategories(true),
                ]);
                setPosts(postsData.items);
                setTotalPages(postsData.totalPages);
                setCategories(categoriesData);
            } catch {
                // Silent fail
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [page, categoryId, search]);

    const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        const formData = new FormData(e.currentTarget);
        const searchValue = formData.get('search') as string;
        setSearchParams(prev => {
            if (searchValue) prev.set('search', searchValue);
            else prev.delete('search');
            prev.set('page', '1');
            return prev;
        });
    };

    const handleCategoryFilter = (catId?: number) => {
        setSearchParams(prev => {
            if (catId) prev.set('category', catId.toString());
            else prev.delete('category');
            prev.set('page', '1');
            return prev;
        });
    };

    const formatDate = (dateStr?: string) => {
        if (!dateStr) return '';
        return new Date(dateStr).toLocaleDateString();
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
                    <h1>{t('blog.title') || 'Blog'}</h1>
                    <p>{t('blog.subtitle') || 'Discover insights and guides'}</p>
                </div>

                <div className="blog-layout animate-fade-in">
                    {/* Sidebar */}
                    <aside className="blog-sidebar">
                        <form onSubmit={handleSearch} className="blog-search">
                            <input
                                type="text"
                                name="search"
                                className="form-input"
                                placeholder={t('common.search') || 'Search...'}
                                defaultValue={search}
                            />
                        </form>

                        <div className="blog-categories">
                            <h3>{t('blog.categories') || 'Categories'}</h3>
                            <ul>
                                <li>
                                    <button
                                        className={`category-link ${!categoryId ? 'active' : ''}`}
                                        onClick={() => handleCategoryFilter()}
                                    >
                                        {t('common.all') || 'All'} ({posts.length})
                                    </button>
                                </li>
                                {categories.map(cat => (
                                    <li key={cat.id}>
                                        <button
                                            className={`category-link ${categoryId === cat.id ? 'active' : ''}`}
                                            onClick={() => handleCategoryFilter(cat.id)}
                                        >
                                            {cat.name} ({cat.postCount})
                                        </button>
                                    </li>
                                ))}
                            </ul>
                        </div>
                    </aside>

                    {/* Posts Grid */}
                    <main className="blog-main">
                        {posts.length === 0 ? (
                            <div className="empty-state">
                                <div className="empty-state-icon">📝</div>
                                <p>{t('blog.noPosts') || 'No posts found'}</p>
                            </div>
                        ) : (
                            <div className="blog-grid">
                                {posts.map(post => (
                                    <article key={post.id} className="blog-card card">
                                        {post.coverImageUrl && (
                                            <Link to={`/blog/${post.slug}`} className="blog-card-image">
                                                <img src={post.coverImageUrl} alt={post.title} />
                                            </Link>
                                        )}
                                        <div className="blog-card-body">
                                            {post.categoryName && (
                                                <span className="blog-card-category">{post.categoryName}</span>
                                            )}
                                            <Link to={`/blog/${post.slug}`}>
                                                <h2 className="blog-card-title">{post.title}</h2>
                                            </Link>
                                            {post.excerpt && (
                                                <p className="blog-card-excerpt">{post.excerpt}</p>
                                            )}
                                            <div className="blog-card-meta">
                                                <div className="blog-card-author">
                                                    <div className="avatar avatar-sm">
                                                        {post.authorName.charAt(0)}
                                                    </div>
                                                    <span>{post.authorName}</span>
                                                </div>
                                                <div className="blog-card-stats">
                                                    <span>{post.readingTimeMinutes} min</span>
                                                    <span>•</span>
                                                    <span>{formatDate(post.publishedUtc)}</span>
                                                </div>
                                            </div>
                                        </div>
                                    </article>
                                ))}
                            </div>
                        )}

                        {/* Pagination */}
                        {totalPages > 1 && (
                            <div className="pagination">
                                {Array.from({ length: totalPages }, (_, i) => i + 1).map(p => (
                                    <button
                                        key={p}
                                        className={`btn btn-sm ${p === page ? 'btn-primary' : 'btn-ghost'}`}
                                        onClick={() => setSearchParams(prev => { prev.set('page', p.toString()); return prev; })}
                                    >
                                        {p}
                                    </button>
                                ))}
                            </div>
                        )}
                    </main>
                </div>
            </div>
        </div>
    );
};

export default BlogList;
