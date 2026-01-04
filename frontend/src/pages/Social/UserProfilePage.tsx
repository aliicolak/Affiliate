import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useLanguage, useAuth } from '../../contexts';
import { socialApi } from '../../api/social';
import type { UserProfile, ProductShareListItem } from '../../types/social';
import ShareCard from '../../components/Social/ShareCard';
import FollowButton from '../../components/Social/FollowButton';
import EditProfileModal from '../../components/Social/EditProfileModal';
import '../../components/Social/Social.css';

const UserProfilePage = () => {
    const { userId } = useParams<{ userId: string }>();
    const { t } = useLanguage();
    const { user: currentUser } = useAuth();
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [shares, setShares] = useState<ProductShareListItem[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);

    const targetUserId = userId ? parseInt(userId) : currentUser?.id;

    useEffect(() => {
        const fetchData = async () => {
            if (!targetUserId) return;
            try {
                const [profileData, sharesData] = await Promise.all([
                    socialApi.getProfile(targetUserId),
                    socialApi.getShares({ userId: targetUserId, pageSize: 10 })
                ]);
                setProfile(profileData);
                setShares(sharesData.items);
            } catch {
                // error
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [targetUserId]);

    if (isLoading || !profile) {
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
                <header className="profile-header animate-fade-in">
                    <img
                        src={profile.avatarUrl || `https://ui-avatars.com/api/?name=${profile.displayName}`}
                        className="profile-avatar-lg"
                        alt={profile.displayName}
                    />
                    <div className="profile-info">
                        <div className="profile-name-row">
                            <h1 className="profile-name">{profile.displayName}</h1>
                            {currentUser?.id !== profile.id ? (
                                <FollowButton
                                    userId={profile.id}
                                    initialIsFollowing={profile.isFollowedByCurrentUser}
                                />
                            ) : (
                                <button className="btn btn-secondary btn-sm" onClick={() => setIsEditModalOpen(true)}>
                                    Edit Profile
                                </button>
                            )}
                        </div>
                        {profile.bio && <p className="profile-bio">{profile.bio}</p>}

                        <div className="profile-stats">
                            <div className="stat-item">
                                <span className="stat-value">{profile.postCount}</span>
                                <span className="stat-label">Posts</span>
                            </div>
                            <div className="stat-item">
                                <span className="stat-value">{profile.shareCount}</span>
                                <span className="stat-label">Shares</span>
                            </div>
                            <div className="stat-item">
                                <span className="stat-value">{profile.followerCount}</span>
                                <span className="stat-label">Followers</span>
                            </div>
                            <div className="stat-item">
                                <span className="stat-value">{profile.followingCount}</span>
                                <span className="stat-label">Following</span>
                            </div>
                        </div>
                    </div>
                </header>

                <section className="profile-content animate-fade-in">
                    <h2 className="mb-lg">{t('social.recentShares') || 'Recent Activity'}</h2>
                    <div className="feed-main">
                        {shares.length > 0 ? (
                            shares.map(share => <ShareCard key={share.id} share={share} />)
                        ) : (
                            <p className="text-muted">No shares yet.</p>
                        )}
                    </div>
                </section>

                <EditProfileModal
                    isOpen={isEditModalOpen}
                    onClose={() => setIsEditModalOpen(false)}
                    profile={profile}
                    onUpdate={() => {
                        socialApi.getProfile(profile.id).then(setProfile);
                    }}
                />
            </div>
        </div>
    );
};

export default UserProfilePage;
