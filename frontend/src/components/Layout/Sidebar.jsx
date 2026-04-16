import { NavLink } from 'react-router-dom';
import './Sidebar.css';
import { useAuth } from '../../context/AuthContext';

const Sidebar = () => {
    const { isAdmin } = useAuth();
    return (
        <aside className="sidebar glass-panel">
            <div className="sidebar-header">
                <h2>Gymnastics</h2>
            </div>
            <nav className="sidebar-nav">
                <NavLink to="/competitions" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Competitions
                </NavLink>
                <NavLink to="/teams" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Teams
                </NavLink>
                <NavLink to="/persons" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Persons
                </NavLink>
                <NavLink to="/judges" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Judges
                </NavLink>
                <NavLink to="/appeals" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                    Appeals
                </NavLink>
                
                {isAdmin && (
                    <>
                        <div className="nav-divider"></div>
                        <div className="nav-section-title">Admin</div>
                        <NavLink to="/admin/users" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Manage Users
                        </NavLink>
                        <NavLink to="/admin/competitions" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Manage Competitions
                        </NavLink>
                        <NavLink to="/admin/categories" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Categories
                        </NavLink>
                        <NavLink to="/admin/disciplines" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Disciplines
                        </NavLink>
                        <NavLink to="/admin/apparatus" className={({ isActive }) => isActive ? "nav-link active" : "nav-link"}>
                            Apparatus
                        </NavLink>
                    </>
                )}
            </nav>
        </aside>
    );
};

export default Sidebar;
