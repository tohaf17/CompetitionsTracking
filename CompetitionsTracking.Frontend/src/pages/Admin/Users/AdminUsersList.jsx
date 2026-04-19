import React, { useState, useEffect } from 'react';
import AdminUserService from '../../../services/admin_users.service';
import toast from 'react-hot-toast';

const AdminUsersList = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadUsers();
    }, []);

    const loadUsers = async () => {
        try {
            setLoading(true);
            const data = await AdminUserService.getAll();
            setUsers(data);
        } catch (error) {
            toast.error(error.message || "Не вдалося завантажити користувачів");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, username) => {
        if (!window.confirm(`Ви впевнені, що хочете видалити користувача "${username}"?`)) {
            return;
        }

        try {
            await AdminUserService.deleteUser(id);
            toast.success(`Користувача ${username} успішно видалено.`);
            setUsers(users.filter(u => u.id !== id));
        } catch (error) {
            toast.error(error.message || "Не вдалося видалити користувача");
        }
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Керування користувачами</h1>
                <button className="btn btn-primary" onClick={loadUsers}>Оновити</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Ім&apos;я користувача</th>
                            <th>Email</th>
                            <th>Роль</th>
                            <th>Дата реєстрації</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.length > 0 ? (
                            users.map((item) => (
                                <tr key={item.id}>
                                    <td>{item.id}</td>
                                    <td><strong>{item.username}</strong></td>
                                    <td>{item.email || '-'}</td>
                                    <td>
                                        <span className={`status-badge ${item.role === 0 ? 'status-active' : 'status-completed'}`}>
                                            {item.role === 0 ? 'Адміністратор' : item.role === 1 ? 'Стажер' : 'Гість'}
                                        </span>
                                    </td>
                                    <td>{new Date(item.createdAt).toLocaleDateString('uk-UA')}</td>
                                    <td>
                                        <button 
                                            className="btn btn-danger" 
                                            style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}
                                            onClick={() => handleDelete(item.id, item.username)}
                                        >
                                            Видалити
                                        </button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>Користувачів не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default AdminUsersList;
