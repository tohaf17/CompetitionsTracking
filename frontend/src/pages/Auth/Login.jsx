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
            toast.success('Successfully logged in');
            navigate('/competitions');
        } else {
            toast.error(result.message);
        }
    };

    return (
        <div className="login-container">
            <div className="login-card glass-panel">
                <center><h2 className="login-title">Gymnastics Login</h2></center>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Email or Nickname</label>
                        <input
                            type="text"
                            name="identifier"
                            value={credentials.identifier}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Password</label>
                        <input
                            type="password"
                            name="password"
                            value={credentials.password}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary login-btn" disabled={loading}>
                        {loading ? 'Logging in...' : 'Login'}
                    </button>
                </form>
                <p style={{ textAlign: 'center', marginTop: '1.2rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>
                    Don&apos;t have an account?{' '}
                    <NavLink to="/register" style={{ color: 'var(--accent-color)', fontWeight: 600 }}>
                        Register
                    </NavLink>
                </p>
            </div>
        </div>
    );
};

export default Login;
