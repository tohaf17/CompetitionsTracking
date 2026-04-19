import React, { useState, useEffect } from 'react';
import DisciplineService from '../../../services/discipline.service';
import ApparatusService from '../../../services/apparatus.service';
import { unwrapCollection } from '../../../utils/unwrapCollection';
import Modal from '../../../components/UI/Modal';
import toast from 'react-hot-toast';
import { toastError } from '../../../utils/toastError';

const AdminDisciplines = () => {
    const [disciplines, setDisciplines] = useState([]);
    const [apparatuses, setApparatuses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ type: '', isGroup: false, apparatusId: '' });

    useEffect(() => {
        loadDisciplines();
    }, []);

    const loadDisciplines = async () => {
        try {
            setLoading(true);
            const data = await DisciplineService.getAll();
            setDisciplines(unwrapCollection(data));
        } catch {
            toast.error("Не вдалося завантажити дисципліни");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Видалити дисципліну "${name}"?`)) return;
        try {
            await DisciplineService.delete(id);
            toast.success("Дисципліну видалено");
            setDisciplines(disciplines.filter(d => d.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити дисципліну');
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const data = await DisciplineService.create({ 
                type: formData.type, 
                isGroup: formData.isGroup,
                apparatusId: parseInt(formData.apparatusId) || null
            });
            toast.success("Дисципліну створено");
            setDisciplines([...disciplines, data]);
            setIsModalOpen(false);
            setFormData({ type: '', isGroup: false, apparatusId: '' });
        } catch (error) {
            toastError(error, 'Не вдалося створити дисципліну');
        }
    };

    const openCreateModal = async () => {
        setIsModalOpen(true);
        try {
            const data = await ApparatusService.getAll();
            setApparatuses(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити інвентар');
        }
    };

    const handleChange = (e) => {
        const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
        setFormData({ ...formData, [e.target.name]: value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Керування дисциплінами</h1>
                <button className="btn btn-primary" onClick={openCreateModal}>Додати дисципліну</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Назва</th>
                            <th>Групова</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {disciplines.length > 0 ? (
                            disciplines.map((item) => (
                                <tr key={item.id}>
                                    <td>{item.id}</td>
                                    <td><strong>{item.type || item.name}</strong></td>
                                    <td>{item.isGroup ? 'Так (Групова)' : 'Ні (Індивідуальна)'}</td>
                                    <td>
                                        <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginLeft: '0.5rem'}} onClick={() => handleDelete(item.id, item.type || item.name)}>Видалити</button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="4" style={{textAlign: 'center', padding: '2rem'}}>Дисциплін не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Створити нову дисципліну">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Назва дисципліни</label>
                        <input type="text" name="type" value={formData.type} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>
                            <input type="checkbox" name="isGroup" checked={formData.isGroup} onChange={handleChange} style={{width: 'auto', marginRight: '0.5rem'}}/>
                            Групова дисципліна?
                        </label>
                    </div>
                    <div className="form-group">
                        <label>Інвентар (необов&apos;язково)</label>
                        <select name="apparatusId" value={formData.apparatusId} onChange={handleChange}>
                            <option value="">-- Без інвентарю --</option>
                            {apparatuses.map(a => <option key={a.id} value={a.id}>{a.type}</option>)}
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Створити</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AdminDisciplines;




