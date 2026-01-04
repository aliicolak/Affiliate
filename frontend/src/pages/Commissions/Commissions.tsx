import { useEffect, useState } from 'react';
import { useLanguage } from '../../contexts';
import { commissionsApi } from '../../api';
import type { Commission, EarningsSummary } from '../../types';
import './Commissions.css';

const Commissions = () => {
    const { t } = useLanguage();
    const [commissions, setCommissions] = useState<Commission[]>([]);
    const [summary, setSummary] = useState<EarningsSummary | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [commissionsData, summaryData] = await Promise.all([
                    commissionsApi.getMy({ pageSize: 50 }).catch(() => ({ items: [] })),
                    commissionsApi.getMySummary().catch(() => null),
                ]);
                setCommissions(commissionsData.items || []);
                setSummary(summaryData);
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

    const formatDate = (dateStr: string) => {
        return new Date(dateStr).toLocaleDateString();
    };

    const getStatusBadge = (status: string) => {
        const statusMap: Record<string, string> = {
            Pending: 'badge-warning',
            Approved: 'badge-success',
            Paid: 'badge-success',
            Rejected: 'badge-error',
        };
        return statusMap[status] || '';
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
                    <h1>{t('commissions.title')}</h1>
                    <p>{t('commissions.subtitle')}</p>
                </div>

                {summary && (
                    <div className="commissions-summary grid grid-4 animate-fade-in">
                        <div className="stat-card">
                            <div className="stat-value text-success">{formatCurrency(summary.totalEarnings)}</div>
                            <div className="stat-label">{t('dashboard.total')}</div>
                        </div>
                        <div className="stat-card">
                            <div className="stat-value text-warning">{formatCurrency(summary.pendingAmount)}</div>
                            <div className="stat-label">{t('dashboard.pending')}</div>
                        </div>
                        <div className="stat-card">
                            <div className="stat-value">{formatCurrency(summary.approvedAmount)}</div>
                            <div className="stat-label">{t('dashboard.approved')}</div>
                        </div>
                        <div className="stat-card">
                            <div className="stat-value text-success">{formatCurrency(summary.paidAmount)}</div>
                            <div className="stat-label">{t('dashboard.paid')}</div>
                        </div>
                    </div>
                )}

                <div className="commissions-table-section animate-fade-in">
                    <div className="section-header">
                        <h2>{t('commissions.history')}</h2>
                        <button className="btn btn-primary btn-sm">{t('commissions.requestPayout')}</button>
                    </div>

                    {commissions.length === 0 ? (
                        <div className="empty-state">
                            <div className="empty-state-icon">💰</div>
                            <p>{t('commissions.noCommissions')}</p>
                        </div>
                    ) : (
                        <div className="table-container">
                            <table className="table">
                                <thead>
                                    <tr>
                                        <th>{t('commissions.date')}</th>
                                        <th>{t('commissions.offer')}</th>
                                        <th>{t('commissions.amount')}</th>
                                        <th>{t('commissions.status')}</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {commissions.map(commission => (
                                        <tr key={commission.id}>
                                            <td>{formatDate(commission.createdUtc)}</td>
                                            <td>{commission.offerName}</td>
                                            <td className="text-success">{formatCurrency(commission.commissionAmount)}</td>
                                            <td>
                                                <span className={`badge ${getStatusBadge(commission.status)}`}>
                                                    {commission.status}
                                                </span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Commissions;
