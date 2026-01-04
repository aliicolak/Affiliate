import { useState } from 'react';
import { socialApi } from '../../api/social';
import type { UserProfile } from '../../types/social';
import './Social.css';

interface EditProfileModalProps {
    isOpen: boolean;
    onClose: () => void;
    profile: UserProfile;
    onUpdate: () => void;
}

const EditProfileModal = ({ isOpen, onClose, profile, onUpdate }: EditProfileModalProps) => {
    const [form, setForm] = useState({
        displayName: profile.displayName,
        bio: profile.bio || '',
        avatarUrl: profile.avatarUrl || ''
    });
    const [isLoading, setIsLoading] = useState(false);

    if (!isOpen) return null;

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        try {
            await socialApi.updateProfile(form);
            onUpdate();
            onClose();
        } catch (err) {
            console.error(err);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content animate-fade-in" style={{ maxWidth: '400px' }}>
                <div className="modal-header">
                    <h2>Edit Profile</h2>
                    <button className="btn-close" onClick={onClose}>&times;</button>
                </div>
                <div className="modal-body">
                    <form onSubmit={handleSubmit}>
                        <div className="form-group">
                            <label className="form-label">Display Name</label>
                            <input
                                type="text"
                                className="form-input"
                                value={form.displayName}
                                onChange={e => setForm(f => ({ ...f, displayName: e.target.value }))}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <label className="form-label">Bio</label>
                            <textarea
                                className="form-input"
                                value={form.bio}
                                onChange={e => setForm(f => ({ ...f, bio: e.target.value }))}
                                rows={3}
                            />
                        </div>
                        <div className="form-group">
                            <label className="form-label">Avatar URL</label>
                            <input
                                type="text"
                                className="form-input"
                                value={form.avatarUrl}
                                onChange={e => setForm(f => ({ ...f, avatarUrl: e.target.value }))}
                            />
                        </div>
                        <div className="form-actions" style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px', marginTop: '1rem' }}>
                            <button type="button" className="btn btn-ghost" onClick={onClose}>Cancel</button>
                            <button type="submit" className="btn btn-primary" disabled={isLoading}>
                                {isLoading ? 'Saving...' : 'Save Changes'}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default EditProfileModal;
