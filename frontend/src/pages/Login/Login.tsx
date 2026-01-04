import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth, useLanguage } from '../../contexts';
import './Auth.css';

const Login = () => {
    const [userNameOrEmail, setUserNameOrEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const { login } = useAuth();
    const { t } = useLanguage();
    const navigate = useNavigate();

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            await login(userNameOrEmail, password);
            navigate('/dashboard');
        } catch (err: unknown) {
            const error = err as { response?: { data?: { detail?: string } | string } };
            setError(
                typeof error.response?.data === 'string'
                    ? error.response.data
                    : error.response?.data?.detail || 'Login failed'
            );
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-container animate-fade-in">
                <div className="auth-header">
                    <h1>{t('auth.welcome')}</h1>
                    <p>{t('auth.signInSubtitle')}</p>
                </div>

                <form onSubmit={handleSubmit} className="auth-form">
                    {error && <div className="alert alert-error">{error}</div>}

                    <div className="form-group">
                        <label className="form-label">{t('auth.email')}</label>
                        <input
                            type="text"
                            className="form-input"
                            placeholder="email@example.com"
                            value={userNameOrEmail}
                            onChange={(e) => setUserNameOrEmail(e.target.value)}
                            required
                            disabled={isLoading}
                        />
                    </div>

                    <div className="form-group">
                        <label className="form-label">{t('auth.password')}</label>
                        <input
                            type="password"
                            className="form-input"
                            placeholder="••••••••"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            disabled={isLoading}
                        />
                    </div>

                    <button type="submit" className="btn btn-primary btn-lg auth-submit" disabled={isLoading}>
                        {isLoading ? <span className="spinner"></span> : t('auth.signIn')}
                    </button>
                </form>

                <div className="auth-footer">
                    <p>{t('auth.noAccount')} <Link to="/register">{t('auth.signUp')}</Link></p>
                </div>
            </div>
        </div>
    );
};

export default Login;
