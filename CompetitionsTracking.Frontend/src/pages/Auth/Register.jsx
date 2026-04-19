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
            toast.error('Паролі не збігаються');
            return;
        }

        if (formData.password.length < 6) {
            toast.error('Пароль має містити щонайменше 6 символів');
            return;
        }

        setLoading(true);
        const result = await register({
            username: formData.username,
            email: formData.email,
            password: formData.password,
            role: 0 // Гість за замовчуванням
        });
        setLoading(false);

        if (result.success) {
            toast.success('Обліковий запис створено! Будь ласка, увійдіть.');
            navigate('/login');
        } else {
            if (result.validationErrors) {
                Object.values(result.validationErrors).flat().forEach(err => toast.error(err));
            } else {
                toast.error(result.message || 'Помилка реєстрації');
            }
        }
    };

    return (
        <div className="login-container">
            <div className="login-card glass-panel">
                <center><h2 className="login-title">Створити обліковий запис</h2></center>
                <p style={{ textAlign: 'center', color: 'var(--text-muted)', marginBottom: '1.5rem', fontSize: '0.9rem' }}>
                    Зареєструйтесь як гість для перегляду змагань
                </p>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Нікнейм / Ім&apos;я користувача</label>
                        <input
                            type="text"
                            name="username"
                            value={formData.username}
                            onChange={handleChange}
                            placeholder="Введіть ім'я користувача"
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
                            placeholder="Введіть ваш email"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Пароль</label>
                        <input
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            placeholder="Щонайменше 6 символів"
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label>Підтвердження пароля</label>
                        <input
                            type="password"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            placeholder="Повторіть пароль"
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary login-btn" disabled={loading}>
                        {loading ? 'Створення облікового запису...' : 'Зареєструватись'}
                    </button>
                </form>
                <p style={{ textAlign: 'center', marginTop: '1.2rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>
                    Вже маєте обліковий запис?{' '}
                    <NavLink to="/login" style={{ color: 'var(--accent-color)', fontWeight: 600 }}>
                        Увійти
                    </NavLink>
                </p>
            </div>
        </div>
    );
};

export default Register;
