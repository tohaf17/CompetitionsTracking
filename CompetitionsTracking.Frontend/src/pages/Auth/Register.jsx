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
        confirmPassword: '',
        role: 2 // Гість за замовчуванням
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
            role: parseInt(formData.role)
        });
        setLoading(false);

        if (result.success) {
            if (parseInt(formData.role) === 2) {
                toast.success('Обліковий запис створено! Будь ласка, увійдіть.');
                navigate('/login');
            } else {
                toast.success('Дякуємо! Ваш акаунт очікує підтвердження адміністратором. Ви можете переглядати дані як гість.', {
                    duration: 6000
                });
                // Редірект на гостьовий UI
                navigate('/competitions');
            }
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
                        <label>Роль у системі</label>
                        <select 
                            name="role" 
                            value={formData.role} 
                            onChange={handleChange}
                            className="btn-outline"
                            style={{ width: '100%', padding: '0.75rem', borderRadius: '8px', background: 'rgba(255,255,255,0.05)', color: 'white', border: '1px solid var(--surface-border)' }}
                        >
                            <option value="2" style={{ color: 'black' }}>Гість (лише перегляд)</option>
                            <option value="1" style={{ color: 'black' }}>Тренер (Coach) - потребує підтвердження</option>
                            <option value="0" style={{ color: 'black' }}>Адміністратор - потребує підтвердження</option>
                        </select>
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
