import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth, useTheme, useLanguage } from '../../contexts';
import './Navbar.css';

const Navbar = () => {
    const { user, isAuthenticated, logout } = useAuth();
    const { theme, toggleTheme } = useTheme();
    const { t, language, setLanguage, languages } = useLanguage();
    const location = useLocation();
    const [showLangMenu, setShowLangMenu] = useState(false);

    const isActive = (path: string) => location.pathname === path;

    const handleLogout = async () => {
        await logout();
    };

    const currentLang = languages.find(l => l.code === language);

    return (
        <nav className="navbar">
            <div className="navbar-container">
                <Link to="/" className="navbar-brand">
                    <span className="brand-text">AffiliateHub</span>
                </Link>

                <div className="navbar-menu">
                    {/* Public link - Blog */}
                    <Link to="/blog" className={`nav-link ${location.pathname.startsWith('/blog') ? 'active' : ''}`}>
                        {t('nav.blog') || 'Blog'}
                    </Link>
                    <Link to="/social" className={`nav-link ${location.pathname.startsWith('/social') ? 'active' : ''}`}>
                        {t('nav.social') || 'Social'}
                    </Link>

                    {isAuthenticated ? (
                        <>
                            <Link to="/dashboard" className={`nav-link ${isActive('/dashboard') ? 'active' : ''}`}>
                                {t('nav.dashboard')}
                            </Link>
                            <Link to="/offers" className={`nav-link ${isActive('/offers') ? 'active' : ''}`}>
                                {t('nav.offers')}
                            </Link>
                            <Link to="/collections" className={`nav-link ${isActive('/collections') ? 'active' : ''}`}>
                                {t('nav.collections') || 'Collections'}
                            </Link>
                            <Link to="/commissions" className={`nav-link ${isActive('/commissions') ? 'active' : ''}`}>
                                {t('nav.commissions')}
                            </Link>
                            <Link to="/notifications" className={`nav-link ${isActive('/notifications') ? 'active' : ''}`}>
                                {t('nav.notifications')}
                            </Link>
                        </>
                    ) : null}

                    <div className="navbar-actions">
                        {/* Theme Toggle */}
                        <button
                            className="btn btn-icon btn-ghost"
                            onClick={toggleTheme}
                            title={theme === 'light' ? t('theme.dark') : t('theme.light')}
                        >
                            {theme === 'light' ? '🌙' : '☀️'}
                        </button>

                        {/* Language Selector */}
                        <div className="dropdown">
                            <button
                                className="btn btn-ghost btn-sm"
                                onClick={() => setShowLangMenu(!showLangMenu)}
                            >
                                {currentLang?.nativeName}
                            </button>
                            {showLangMenu && (
                                <div className="dropdown-menu">
                                    {languages.map(lang => (
                                        <div
                                            key={lang.code}
                                            className={`dropdown-item ${language === lang.code ? 'active' : ''}`}
                                            onClick={() => {
                                                setLanguage(lang.code);
                                                setShowLangMenu(false);
                                            }}
                                        >
                                            {lang.nativeName}
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>

                        {isAuthenticated ? (
                            <div className="navbar-user">
                                <div className="avatar">
                                    {user?.displayName?.charAt(0).toUpperCase()}
                                </div>
                                <span className="user-name">{user?.displayName}</span>
                                <button onClick={handleLogout} className="btn btn-ghost btn-sm">
                                    {t('nav.logout')}
                                </button>
                            </div>
                        ) : (
                            <>
                                <Link to="/login" className="btn btn-ghost">
                                    {t('nav.login')}
                                </Link>
                                <Link to="/register" className="btn btn-primary">
                                    {t('nav.register')}
                                </Link>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
