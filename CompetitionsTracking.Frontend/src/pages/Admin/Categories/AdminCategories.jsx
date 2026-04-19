import React, { useState, useEffect } from 'react';
import CategoryService from '../../../services/category.service';
import Modal from '../../../components/UI/Modal';
import toast from 'react-hot-toast';

const AdminCategories = () => {
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ type: '', minAge: '', maxAge: '' });

    useEffect(() => {
        loadCategories();
    }, []);

    const loadCategories = async () => {
        try {
            setLoading(true);
            const data = await CategoryService.getAll();
            setCategories(data.items || data);
        } catch (error) {
            toast.error("Не вдалося завантажити категорії");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Видалити категорію "${name}"?`)) return;
        try {
            await CategoryService.delete(id);
            toast.success("Категорію видалено");
            setCategories(categories.filter(c => c.id !== id));
        } catch (error) {
            toast.error("Не вдалося видалити категорію");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const data = await CategoryService.create({ 
                type: formData.type, 
                minAge: parseInt(formData.minAge) || 0,
                maxAge: parseInt(formData.maxAge) || 99
            });
            toast.success("Категорію створено");
            setCategories([...categories, data]);
            setIsModalOpen(false);
            setFormData({ type: '', minAge: '', maxAge: '' });
        } catch (error) {
            toast.error("Не вдалося створити категорію");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Керування категоріями</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Додати категорію</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Назва</th>
                            <th>Вікові межі</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {categories.length > 0 ? (
                            categories.map((item) => (
                                <tr key={item.id}>
                                    <td>{item.id}</td>
                                    <td><strong>{item.type}</strong></td>
                                    <td>{item.minAge} - {item.maxAge} р.</td>
                                    <td>
                                        <button className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}>Редагувати</button>
                                        <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginLeft: '0.5rem'}} onClick={() => handleDelete(item.id, item.type)}>Видалити</button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="4" style={{textAlign: 'center', padding: '2rem'}}>Категорій не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Створити нову категорію">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Назва категорії (напр. Юніори)</label>
                        <input type="text" name="type" value={formData.type} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Мінімальний вік</label>
                        <input type="number" name="minAge" value={formData.minAge} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Максимальний вік</label>
                        <input type="number" name="maxAge" value={formData.maxAge} onChange={handleChange} required />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Створити категорію</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AdminCategories;
