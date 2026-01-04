import { useState } from 'react';
import { socialApi } from '../../api/social';
import { useAuth, useLanguage } from '../../contexts';

interface FollowButtonProps {
    userId: number;
    initialIsFollowing: boolean;
    onToggle?: (newState: boolean) => void;
}

const FollowButton = ({ userId, initialIsFollowing, onToggle }: FollowButtonProps) => {
    const { user } = useAuth();
    const { t } = useLanguage();
    const [isFollowing, setIsFollowing] = useState(initialIsFollowing);
    const [isLoading, setIsLoading] = useState(false);

    if (user?.id === userId) return null;

    const handleToggleFollow = async () => {
        if (isLoading) return;

        setIsLoading(true);
        try {
            const result = await socialApi.toggleFollow(userId);
            setIsFollowing(result.isFollowing);
            if (onToggle) onToggle(result.isFollowing);
        } catch {
            // error handling
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <button
            className={`btn ${isFollowing ? 'btn-secondary' : 'btn-primary'} btn-sm`}
            onClick={handleToggleFollow}
            disabled={isLoading}
        >
            {isFollowing ? (t('social.unfollow') || 'Following') : (t('social.follow') || 'Follow')}
        </button>
    );
};

export default FollowButton;
