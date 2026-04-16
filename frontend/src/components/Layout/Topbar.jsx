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

    return (
        <header className="topbar glass-panel">
            <div className="topbar-content">
                <div className="topbar-left">
                    {/* Placeholder for breadcrumbs or page title if needed */}
                </div>
                <div className="topbar-right">
                    {user ? (
                        <div className="user-profile">
                            <span className="user-role">{user.role}</span>
                            <span className="user-name">{user.username}</span>
                            <button onClick={handleLogout} className="btn btn-outline ml-2">
                                Logout
                            </button>
                        </div>
                    ) : (
                        <button onClick={() => navigate('/login')} className="btn btn-primary">
                            Login
                        </button>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Topbar;
