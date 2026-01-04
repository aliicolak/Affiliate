import { useEffect, useState } from 'react';
import { useAuth, useLanguage } from '../../contexts';
import { publishersApi, commissionsApi } from '../../api';
import type { PublisherDashboard, EarningsSummary } from '../../types';
import './Dashboard.css';

const Dashboard = () => {
    const { user } = useAuth();
    const { t } = useLanguage();
    const [dashboard, setDashboard] = useState<PublisherDashboard | null>(null);
    const [earnings, setEarnings] = useState<EarningsSummary | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [dashboardData, earningsData] = await Promise.all([
                    publishersApi.getDashboard().catch(() => null),
                    commissionsApi.getMySummary().catch(() => null),
                ]);
                setDashboard(dashboardData);
                setEarnings(earningsData);
            } catch {
                // Silent fail
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, []);

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: 'TRY',
        }).format(amount);
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
                    <h1>{t('dashboard.welcome')} {user?.displayName}!</h1>
                    <p>{t('dashboard.subtitle')}</p>
                </div>

                <div className="grid grid-4 animate-fade-in">
                    <div className="stat-card">
                        <div className="stat-value">{dashboard?.todayClicks || 0}</div>
                        <div className="stat-label">{t('dashboard.todayClicks')}</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-value">{dashboard?.todayConversions || 0}</div>
                        <div className="stat-label">{t('dashboard.todayConversions')}</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-value">{formatCurrency(dashboard?.thisMonthEarnings || 0)}</div>
                        <div className="stat-label">{t('dashboard.monthEarnings')}</div>
                    </div>
                    <div className="stat-card">
                        <div className="stat-value">{formatCurrency(dashboard?.availableBalance || 0)}</div>
                        <div className="stat-label">{t('dashboard.balance')}</div>
                    </div>
                </div>

                {earnings && (
                    <div className="earnings-section animate-fade-in">
                        <h2>{t('dashboard.earnings')}</h2>
                        <div className="grid grid-4">
                            <div className="stat-card">
                                <div className="stat-value text-success">{formatCurrency(earnings.totalEarnings)}</div>
                                <div className="stat-label">{t('dashboard.total')}</div>
                            </div>
                            <div className="stat-card">
                                <div className="stat-value text-warning">{formatCurrency(earnings.pendingAmount)}</div>
                                <div className="stat-label">{t('dashboard.pending')}</div>
                            </div>
                            <div className="stat-card">
                                <div className="stat-value">{formatCurrency(earnings.approvedAmount)}</div>
                                <div className="stat-label">{t('dashboard.approved')}</div>
                            </div>
                            <div className="stat-card">
                                <div className="stat-value text-success">{formatCurrency(earnings.paidAmount)}</div>
                                <div className="stat-label">{t('dashboard.paid')}</div>
                            </div>
                        </div>
                    </div>
                )}

                {!dashboard?.profile && (
                    <div className="cta-card card animate-fade-in">
                        <div className="card-body">
                            <h3>{t('dashboard.becomePublisher')}</h3>
                            <p>{t('dashboard.becomePublisherText')}</p>
                            <button className="btn btn-primary">{t('dashboard.registerPublisher')}</button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Dashboard;
