import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useNavigate, NavLink } from 'react-router-dom';
import toast from 'react-hot-toast';
import './Login.css';

const Login = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [credentials, setCredentials] = useState({ identifier: '', password: '' });
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setCredentials(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        const result = await login(credentials);
        setLoading(false);
        if (result.success) {
            toast.success('Успішний вхід');
            navigate('/competitions');
        } else {
            toast.error(result.message);
        }
    };

    const handleGuestAccess = () => {
        navigate('/competitions');
    };

    return (
        <div className="login-container">
            <div className="login-card glass-panel">
                <center><h2 className="login-title">Вхід до системи</h2></center>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Email або нікнейм</label>
                        <input
                            type="text"
                            name="identifier"
                            value={credentials.identifier}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Пароль</label>
                        <input
                            type="password"
                            name="password"
                            value={credentials.password}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary login-btn" disabled={loading}>
                        {loading ? 'Вхід...' : 'Увійти'}
                    </button>
                </form>

                <div style={{ margin: '1rem 0', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                    <hr style={{ flex: 1, border: 'none', borderTop: '1px solid var(--surface-border)' }} />
                    <span style={{ color: 'var(--text-muted)', fontSize: '0.8rem', whiteSpace: 'nowrap' }}>або</span>
                    <hr style={{ flex: 1, border: 'none', borderTop: '1px solid var(--surface-border)' }} />
                </div>

                <button
                    type="button"
                    className="btn btn-outline login-btn"
                    onClick={handleGuestAccess}
                    style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem' }}
                >
                    👁 Переглянути як гість
                </button>

                <p style={{ textAlign: 'center', marginTop: '1.2rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>
                    Немає облікового запису?{' '}
                    <NavLink to="/register" style={{ color: 'var(--accent-color)', fontWeight: 600 }}>
                        Зареєструватись
                    </NavLink>
                </p>
            </div>
        </div>
    );
};

export default Login;
