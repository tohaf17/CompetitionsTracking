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
            toast.error(error.message || "Failed to load users");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, username) => {
        if (!window.confirm(`Are you sure you want to delete user "${username}"?`)) {
            return;
        }

        try {
            await AdminUserService.deleteUser(id);
            toast.success(`User ${username} deleted successfully.`);
            setUsers(users.filter(u => u.id !== id));
        } catch (error) {
            toast.error(error.message || "Failed to delete user");
        }
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Manage System Users</h1>
                <button className="btn btn-primary" onClick={loadUsers}>Refresh</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Username</th>
                            <th>Email</th>
                            <th>Role</th>
                            <th>Registered</th>
                            <th>Actions</th>
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
                                            {item.role === 0 ? 'Admin' : item.role === 1 ? 'Trainee' : 'Guest'}
                                        </span>
                                    </td>
                                    <td>{new Date(item.createdAt).toLocaleDateString()}</td>
                                    <td>
                                        <button 
                                            className="btn btn-danger" 
                                            style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}
                                            onClick={() => handleDelete(item.id, item.username)}
                                        >
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>No users found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default AdminUsersList;
