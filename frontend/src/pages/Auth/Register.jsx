import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { useNavigate, NavLink } from 'react-router-dom';
import toast from 'react-hot-toast';
import './Login.css';

const Register = () => {
    const { register } = useAuth();
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: ''
    });
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (formData.password !== formData.confirmPassword) {
            toast.error('Passwords do not match');
            return;
        }

        if (formData.password.length < 6) {
            toast.error('Password must be at least 6 characters');
            return;
        }

        setLoading(true);
        const result = await register({
            username: formData.username,
            email: formData.email,
            password: formData.password,
            role: 0 // Guest role by default
        });
        setLoading(false);

        if (result.success) {
            toast.success('Account created! Please log in.');
            navigate('/login');
        } else {
            if (result.validationErrors) {
                Object.values(result.validationErrors).flat().forEach(err => toast.error(err));
            } else {
                toast.error(result.message || 'Registration failed');
            }
        }
    };

    return (
        <div className="login-container">
            <div className="login-card glass-panel">
                <center><h2 className="login-title">Create Account</h2></center>
                <p style={{ textAlign: 'center', color: 'var(--text-muted)', marginBottom: '1.5rem', fontSize: '0.9rem' }}>
                    Register as a Guest to browse competitions
                </p>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Nickname / Username</label>
                        <input
                            type="text"
                            name="username"
                            value={formData.username}
                            onChange={handleChange}
                            placeholder="Enter your username"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Email</label>
                        <input
                            type="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            placeholder="Enter your email"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Password</label>
                        <input
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            placeholder="At least 6 characters"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Confirm Password</label>
                        <input
                            type="password"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            placeholder="Repeat your password"
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary login-btn" disabled={loading}>
                        {loading ? 'Creating account...' : 'Register'}
                    </button>
                </form>
                <p style={{ textAlign: 'center', marginTop: '1.2rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>
                    Already have an account?{' '}
                    <NavLink to="/login" style={{ color: 'var(--accent-color)', fontWeight: 600 }}>
                        Log in
                    </NavLink>
                </p>
            </div>
        </div>
    );
};

export default Register;
