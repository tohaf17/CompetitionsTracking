import React, { useState, useEffect } from 'react';
import CompetitionService from '../../../services/competition.service';
import { unwrapCollection } from '../../../utils/unwrapCollection';
import Modal from '../../../components/UI/Modal';
import { NavLink } from 'react-router-dom';
import toast from 'react-hot-toast';
import { toastError } from '../../../utils/toastError';

const AdminCompetitions = () => {
    const [competitions, setCompetitions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ 
        title: '', city: '', 
        startDate: '', endDate: '', status: 0 
    });

    useEffect(() => {
        loadCompetitions();
    }, []);

    const loadCompetitions = async () => {
        try {
            setLoading(true);
            const data = await CompetitionService.getAll();
            setCompetitions(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити змагання');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, title) => {
        if (!window.confirm(`Видалити змагання "${title}"?`)) return;
        try {
            await CompetitionService.delete(id);
            toast.success("Змагання видалено");
            setCompetitions(competitions.filter(c => c.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити змагання');
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const dataToSubmit = {
                title: formData.title,
                city: formData.city,
                startDate: new Date(formData.startDate).toISOString(),
                endDate: new Date(formData.endDate).toISOString(),
                status: parseInt(formData.status)
            };
            const data = await CompetitionService.create(dataToSubmit);
            toast.success("Змагання створено");
            setCompetitions([...competitions, data]);
            setIsModalOpen(false);
            setFormData({ title: '', city: '', startDate: '', endDate: '', status: 0 });
        } catch (error) {
            toastError(error, 'Не вдалося створити змагання');
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Керування змаганнями</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Додати змагання</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Назва</th>
                            <th>Місто</th>
                            <th>Дати</th>
                            <th>Статус</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {competitions.length > 0 ? (
                            competitions.map((item) => {
                                const statusMap = {
                                    0: { text: "Заплановано", class: "status-planned" },
                                    1: { text: "Реєстрація відкрита", class: "status-upcoming" },
                                    2: { text: "Триває", class: "status-ongoing" },
                                    3: { text: "Завершено", class: "status-completed" }
                                };
                                const compStatus = statusMap[item.status] || { text: "Невідомо", class: "" };
                                
                                return (
                                    <tr key={item.id}>
                                        <td>{item.id}</td>
                                        <td><strong>{item.title}</strong></td>
                                        <td>{item.city}</td>
                                        <td>{new Date(item.startDate).toLocaleDateString('uk-UA')} - {new Date(item.endDate).toLocaleDateString('uk-UA')}</td>
                                        <td><span className={`status-badge ${compStatus.class}`}>{compStatus.text}</span></td>
                                        <td>
                                            <NavLink to={`/competitions/${item.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}>Переглянути</NavLink>
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(item.id, item.title)}>Видалити</button>
                                        </td>
                                    </tr>
                                )
                            })
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>Змагань не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Створити нове змагання">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Назва</label>
                        <input type="text" name="title" value={formData.title} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Місто</label>
                        <input type="text" name="city" value={formData.city} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Дата початку</label>
                        <input type="date" name="startDate" value={formData.startDate} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Дата завершення</label>
                        <input type="date" name="endDate" value={formData.endDate} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Статус</label>
                        <select name="status" value={formData.status} onChange={handleChange}>
                            <option value={0}>Заплановано</option>
                            <option value={1}>Реєстрація відкрита</option>
                            <option value={2}>Триває</option>
                            <option value={3}>Завершено</option>
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

export default AdminCompetitions;




