import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import './Topbar.css';

const Topbar = () => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const roleLabel = {
        Admin: 'Admin',
        Trainee: 'Coach',
        Guest: 'Guest'
    }[user?.role] || user?.role;

    return (
        <header className="topbar glass-panel">
            <div className="topbar-content">
                <div className="topbar-left">
                </div>
                <div className="topbar-right">
                    {user ? (
                        <div className="user-profile">
                            <span className="user-role">{roleLabel}</span>
                            <span className="user-name">{user.username}</span>
                            <button onClick={handleLogout} className="btn btn-outline ml-2">
                                Вийти
                            </button>
                        </div>
                    ) : (
                        <div className="user-profile">
                            <span className="user-role" style={{ color: 'var(--text-muted)' }}>Гість</span>
                            <button onClick={() => navigate('/login')} className="btn btn-primary ml-2">
                                Увійти
                            </button>
                        </div>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Topbar;
