import { NavLink, useNavigate } from 'react-router-dom';
import './Sidebar.css';
import { useAuth } from '../../context/AuthContext';

const Sidebar = () => {
    const { isAdmin, user } = useAuth();
    const navigate = useNavigate();

    return (
        <aside className="sidebar glass-panel">
            <div className="sidebar-header">
                <h2>Гімнастика</h2>
            </div>
            <nav className="sidebar-nav">

                <NavLink to="/competitions" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Змагання
                </NavLink>
                <NavLink to="/teams" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Команди
                </NavLink>
                <NavLink to="/persons" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Учасники
                </NavLink>
                <NavLink to="/judges" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Судді
                </NavLink>
                <NavLink to="/appeals" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Апеляції
                </NavLink>


                {user && (
                    <NavLink to="/entries" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                        Заявки
                    </NavLink>
                )}

                {!user && (
                    <>
                        <div className="nav-divider"></div>
                        <button
                            className="nav-link"
                            style={{ background: 'none', border: 'none', cursor: 'pointer', textAlign: 'left', width: '100%', color: 'var(--accent-color)', fontWeight: 600 }}
                            onClick={() => navigate('/login')}
                        >
                            🔑 Увійти до системи
                        </button>
                    </>
                )}

                {isAdmin && (
                    <>
                        <div className="nav-divider"></div>
                        <div className="nav-section-title">Адміністрування</div>
                        <NavLink to="/admin/users" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Користувачі
                        </NavLink>
                        <NavLink to="/admin/competitions" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Керування змаганнями
                        </NavLink>
                        <NavLink to="/admin/categories" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Категорії
                        </NavLink>
                        <NavLink to="/admin/disciplines" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Дисципліни
                        </NavLink>
                        <NavLink to="/admin/apparatus" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Інвентар
                        </NavLink>
                    </>
                )}
            </nav>
        </aside>
    );
};

export default Sidebar;
