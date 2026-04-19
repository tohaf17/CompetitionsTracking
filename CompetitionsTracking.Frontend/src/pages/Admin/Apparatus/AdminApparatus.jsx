import React, { useState, useEffect } from 'react';
import ApparatusService from '../../../services/apparatus.service';
import Modal from '../../../components/UI/Modal';
import toast from 'react-hot-toast';

const AdminApparatus = () => {
    const [apparatus, setApparatus] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ type: '' });

    const [editingId, setEditingId] = useState(null);
    useEffect(() => {
        loadApparatus();
    }, []);

    const loadApparatus = async () => {
        try {
            setLoading(true);
            const data = await ApparatusService.getAll();
            setApparatus(data.items || data);
        } catch (error) {
            toast.error("Не вдалося завантажити інвентар");
        } finally {
            setLoading(false);
        }
    };
 
    
    const handleDelete = async (id, name) => {
        if (!window.confirm(`Видалити інвентар "${name}"?`)) return;
        try {
            await ApparatusService.delete(id);
            toast.success("Інвентар видалено");
            setApparatus(apparatus.filter(a => a.id !== id));
        } catch (error) {
            toast.error("Не вдалося видалити інвентар");
        }
    };

    const handleOpenCreate = () => {
        setEditingId(null);
        setFormData({ type: '' });
        setIsModalOpen(true);
    };

    const handleOpenEdit = (item) => {
        setEditingId(item.id);
        setFormData({ type: item.type || item.name });
        setIsModalOpen(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editingId) {
                await ApparatusService.update(editingId, { type: formData.type });
                toast.success("Інвентар оновлено");
                setApparatus(apparatus.map(a => 
                    a.id === editingId ? { ...a, type: formData.type, name: formData.type } : a
                ));
            } else {
                const data = await ApparatusService.create({ type: formData.type });
                toast.success("Інвентар створено");
                setApparatus([...apparatus, data]);
            }
            setIsModalOpen(false);
            setFormData({ type: '' });
            setEditingId(null);
        } catch (error) {
            toast.error(editingId ? "Не вдалося оновити інвентар" : "Не вдалося створити інвентар");
        }
    };
    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Керування інвентарем</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Додати інвентар</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Назва</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {apparatus.length > 0 ? (
                            apparatus.map((item, index) => (
                                <tr key={item.id}>
                                    <td>{index + 1}</td>
                                    <td><strong>{item.type || item.name}</strong></td>
                                    <td>
                                        <button 
                                            className="btn btn-warning" 
                                            style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}} 
                                            onClick={() => handleOpenEdit(item)}
                                        >
                                            Оновити
                                        </button>
                                        <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(item.id, item.type || item.name)}>Видалити</button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="3" style={{textAlign: 'center', padding: '2rem'}}>Інвентаря не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} 
            onClose={() => setIsModalOpen(false)} title={editingId ? "Оновити інвентар" : "Додати новий інвентар"}>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Назва інвентарю (напр. Обруч, М&apos;яч)</label>
                        <input 
                            type="text" 
                            name="type" 
                            value={formData.type} 
                            onChange={handleChange} 
                            required 
                        />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">
                            {editingId ? "Зберегти зміни" : "Створити"}
                        </button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AdminApparatus;
