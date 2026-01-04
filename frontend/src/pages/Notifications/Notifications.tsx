import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useLanguage } from '../../contexts';
import { notificationsApi } from '../../api';
import type { Notification } from '../../types';
import './Notifications.css';

const Notifications = () => {
    const { t } = useLanguage();
    const navigate = useNavigate();
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchNotifications = async () => {
            try {
                const data = await notificationsApi.getMy({ pageSize: 50 });
                setNotifications(data.items || []);
            } catch {
                // Silent fail
            } finally {
                setIsLoading(false);
            }
        };
        fetchNotifications();
    }, []);

    const handleMarkAllRead = async () => {
        try {
            await notificationsApi.markAllAsRead();
            setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
        } catch {
            // Silent fail
        }
    };

    const handleMarkRead = async (id: number) => {
        try {
            await notificationsApi.markAsRead(id);
            setNotifications(prev =>
                prev.map(n => n.id === id ? { ...n, isRead: true } : n)
            );
        } catch {
            // Silent fail
        }
    };

    const handleNotificationClick = async (notification: Notification) => {
        if (!notification.isRead) {
            await handleMarkRead(notification.id);
        }
        if (notification.actionUrl) {
            navigate(notification.actionUrl);
        }
    };

    const formatDate = (dateStr: string) => {
        const date = new Date(dateStr);
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
        const diffDays = Math.floor(diffHours / 24);

        if (diffHours < 1) return 'Just now';
        if (diffHours < 24) return `${diffHours}h ago`;
        if (diffDays < 7) return `${diffDays}d ago`;
        return date.toLocaleDateString();
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
                    <div className="header-content">
                        <h1>{t('notifications.title')}</h1>
                    </div>
                    {notifications.some(n => !n.isRead) && (
                        <button className="btn btn-secondary btn-sm" onClick={handleMarkAllRead}>
                            {t('notifications.markAllRead')}
                        </button>
                    )}
                </div>

                {notifications.length === 0 ? (
                    <div className="empty-state animate-fade-in">
                        <div className="empty-state-icon">🔔</div>
                        <p>{t('notifications.noNotifications')}</p>
                    </div>
                ) : (
                    <div className="notifications-list animate-fade-in">
                        {notifications.map(notification => (
                            <div
                                key={notification.id}
                                className={`notification-item ${!notification.isRead ? 'unread' : ''}`}
                                onClick={() => handleNotificationClick(notification)}
                            >
                                <div className="notification-content">
                                    <h4>{notification.title}</h4>
                                    <p>{notification.message}</p>
                                    <span className="notification-time">{formatDate(notification.createdUtc)}</span>
                                </div>
                                {!notification.isRead && <span className="unread-dot"></span>}
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Notifications;
