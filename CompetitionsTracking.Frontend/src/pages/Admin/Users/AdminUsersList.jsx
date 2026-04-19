import React, { useState, useEffect } from 'react';
import AdminUserService from '../../../services/admin_users.service';
import toast from 'react-hot-toast';
import { toastError } from '../../../utils/toastError';

const AdminUsersList = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('all'); // 'all' or 'pending'

    useEffect(() => {
        loadUsers();
    }, []);

    const loadUsers = async () => {
        try {
            setLoading(true);
            const data = await AdminUserService.getAll();
            setUsers(data);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити користувачів');
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
            toastError(error, 'Не вдалося видалити користувача');
        }
    };

    const handleApprove = async (id, username) => {
        try {
            await AdminUserService.approveUser(id);
            toast.success(`Користувача "${username}" підтверджено`);
            setUsers(users.map(u => u.id === id ? { ...u, isApproved: true } : u));
        } catch (error) {
            toastError(error, 'Не вдалося підтвердити користувача');
        }
    };

    const filteredUsers = activeTab === 'pending' 
        ? users.filter(u => !u.isApproved) 
        : users;

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Керування користувачами</h1>
                <div className="flex" style={{ gap: '1rem' }}>
                    <button className="btn btn-outline" onClick={loadUsers}>Оновити</button>
                </div>
            </div>

            <div className="flex" style={{ marginBottom: '1rem', gap: '0.5rem' }}>
                <button 
                    className={`btn ${activeTab === 'all' ? 'btn-primary' : 'btn-outline'}`}
                    onClick={() => setActiveTab('all')}
                    style={{ padding: '0.5rem 1rem' }}
                >
                    Усі користувачі ({users.length})
                </button>
                <button 
                    className={`btn ${activeTab === 'pending' ? 'btn-primary' : 'btn-outline'}`}
                    onClick={() => setActiveTab('pending')}
                    style={{ padding: '0.5rem 1rem' }}
                >
                    Очікують підтвердження ({users.filter(u => !u.isApproved).length})
                </button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Ім&apos;я користувача</th>
                            <th>Email</th>
                            <th>Роль</th>
                            <th>Статус</th>
                            <th>Дата реєстрації</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredUsers.length > 0 ? (
                            filteredUsers.map((item,index) => (
                                <tr key={item.id}>
                                    <td>{index + 1}</td>
                                    <td><strong>{item.username}</strong></td>
                                    <td>{item.email || '-'}</td>
                                    <td>
                                        <span className={`status-badge ${item.role === 0 ? 'status-active' : item.role === 1 ? 'status-completed' : 'status-cancelled'}`}>
                                            {item.role === 0 ? 'Адміністратор' : item.role === 1 ? 'Тренер' : 'Гість'}
                                        </span>
                                    </td>
                                    <td>
                                        <span className={`status-badge ${item.isApproved ? 'status-active' : 'status-cancelled'}`}>
                                            {item.isApproved ? 'Підтверджено' : 'Очікує'}
                                        </span>
                                    </td>
                                    <td>{new Date(item.createdAt).toLocaleDateString('uk-UA')}</td>
                                    <td>
                                        <div className="flex" style={{ gap: '0.5rem' }}>
                                            {!item.isApproved && (
                                                <button 
                                                    className="btn btn-primary" 
                                                    style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}
                                                    onClick={() => handleApprove(item.id, item.username)}
                                                >
                                                    Підтвердити
                                                </button>
                                            )}
                                            <button 
                                                className="btn btn-danger" 
                                                style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}
                                                onClick={() => handleDelete(item.id, item.username)}
                                            >
                                                Видалити
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="7" style={{textAlign: 'center', padding: '2rem'}}>
                                    {activeTab === 'pending' ? 'Немає нових запитів на підтвердження.' : 'Користувачів не знайдено.'}
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default AdminUsersList;
