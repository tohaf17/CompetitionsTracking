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
        title: '', city: '', country: 'Україна', level: 1,
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
                country: formData.country,
                level: parseInt(formData.level),
                startDate: new Date(formData.startDate).toISOString(),
                endDate: new Date(formData.endDate).toISOString(),
                status: parseInt(formData.status)
            };
            await CompetitionService.create(dataToSubmit);
            toast.success("Змагання створено успішно");
            loadCompetitions(); 
            setIsModalOpen(false);
            setFormData({ title: '', city: '', country: 'Україна', level: 1, startDate: '', endDate: '', status: 0 });
        } catch (error) {
            toastError(error, 'Не вдалося створити змагання');
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    const statusMap = {
        0: { text: "Заплановано", class: "status-planned" },
        1: { text: "Реєстрація", class: "status-upcoming" },
        2: { text: "Триває", class: "status-ongoing" },
        3: { text: "Завершено", class: "status-completed" }
    };

    const levelMap = {
        0: "Локальне",
        1: "Національне",
        2: "Міжнародне"
    };

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
                            <th>№</th>
                            <th>Назва</th>
                            <th>Тип</th>
                            <th>Місто</th>
                            <th>Країна</th>
                            <th>Дати проведення</th>
                            <th>Статус</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {competitions.length > 0 ? (
                            competitions.map((item, index) => {
                                const compStatus = statusMap[item.status] || { text: "Невідомо", class: "" };
                                
                                return (
                                    <tr key={item.id}>
                                        <td>{index + 1}</td>
                                        <td><strong>{item.title}</strong></td>
                                        <td>{levelMap[item.level] || 'Невідомо'}</td>
                                        <td>{item.city}</td>
                                        <td>{item.country || '-'}</td>
                                        <td>{new Date(item.startDate).toLocaleDateString('uk-UA')} - {new Date(item.endDate).toLocaleDateString('uk-UA')}</td>
                                        <td><span className={`status-badge ${compStatus.class}`}>{compStatus.text}</span></td>
                                        <td>
                                            <NavLink 
                                                to={`/competitions/${item.id}`} 
                                                className="btn btn-outline" 
                                                style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}
                                            >
                                                Переглянути
                                            </NavLink>
                                            <button 
                                                className="btn btn-danger" 
                                                style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} 
                                                onClick={() => handleDelete(item.id, item.title)}
                                            >
                                                Видалити
                                            </button>
                                        </td>
                                    </tr>
                                )
                            })
                        ) : (
                            <tr>
                                <td colSpan="8" style={{textAlign: 'center', padding: '2rem'}}>Змагань не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Створити нове змагання">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Назва змагання</label>
                        <input type="text" name="title" value={formData.title} onChange={handleChange} required placeholder="напр. Кубок України 2024" />
                    </div>
                    <div className="form-group">
                        <label>Місто</label>
                        <input type="text" name="city" value={formData.city} onChange={handleChange} required placeholder="напр. Київ" />
                    </div>
                    <div className="form-group">
                        <label>Тип змагання</label>
                        <select name="level" value={formData.level} onChange={handleChange}>
                            <option value={0}>Локальне</option>
                            <option value={1}>Національне</option>
                            <option value={2}>Міжнародне</option>
                        </select>
                    </div>
                    {parseInt(formData.level) === 2 && (
                        <div className="form-group">
                            <label>Країна проведення</label>
                            <input type="text" name="country" value={formData.country} onChange={handleChange} required placeholder="напр. Польща" />
                        </div>
                    )}
                    <div className="grid grid-2">
                        <div className="form-group">
                            <label>Дата початку</label>
                            <input type="date" name="startDate" value={formData.startDate} onChange={handleChange} required />
                        </div>
                        <div className="form-group">
                            <label>Дата завершення</label>
                            <input type="date" name="endDate" value={formData.endDate} onChange={handleChange} required />
                        </div>
                    </div>
                    <div className="form-group">
                        <label>Початковий статус</label>
                        <select name="status" value={formData.status} onChange={handleChange}>
                            <option value={0}>Заплановано</option>
                            <option value={1}>Реєстрація відкрита</option>
                            <option value={2}>Триває</option>
                            <option value={3}>Завершено</option>
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Зберегти змагання</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AdminCompetitions;
