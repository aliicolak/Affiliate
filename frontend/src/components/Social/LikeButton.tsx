import { useState } from 'react';
import { socialApi } from '../../api/social';
import './Social.css';

interface LikeButtonProps {
    entityType: 'BlogPost' | 'ProductShare' | 'BlogComment' | 'ShareComment';
    entityId: number;
    initialIsLiked: boolean;
    initialLikeCount: number;
    size?: 'sm' | 'md';
}

const LikeButton = ({ entityType, entityId, initialIsLiked, initialLikeCount, size = 'md' }: LikeButtonProps) => {
    const [isLiked, setIsLiked] = useState(initialIsLiked);
    const [likeCount, setLikeCount] = useState(initialLikeCount);
    const [isLoading, setIsLoading] = useState(false);

    const handleToggleLike = async () => {
        if (isLoading) return;

        // Optimistic update
        const prevIsLiked = isLiked;
        const prevCount = likeCount;

        setIsLiked(!isLiked);
        setLikeCount(prev => prev + (isLiked ? -1 : 1));
        setIsLoading(true);

        try {
            const result = await socialApi.toggleLike({ entityType, entityId });
            setIsLiked(result.isLiked);
            setLikeCount(result.likeCount);
        } catch {
            // Revert on error
            setIsLiked(prevIsLiked);
            setLikeCount(prevCount);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <button
            className={`action-btn ${isLiked ? 'liked' : ''} ${size === 'sm' ? 'btn-sm' : ''}`}
            onClick={handleToggleLike}
            disabled={isLoading}
        >
            <span className="icon">{isLiked ? '❤️' : '🤍'}</span>
            <span>{likeCount}</span>
        </button>
    );
};

export default LikeButton;
