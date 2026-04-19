import React, { useState, useEffect } from 'react';
import JudgeService from '../../services/judge.service';
import PersonService from '../../services/person.service';
import { NavLink } from 'react-router-dom';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const JudgesList = () => {
    const { user } = useAuth();
    const canEdit = user?.role === 'Admin' || user?.role === 'Trainee';

    const [judges, setJudges] = useState([]);
    const [persons, setPersons] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ personId: '', qualificationLevel: '' });

    useEffect(() => {
        loadJudges();
    }, []);

    const loadJudges = async () => {
        try {
            setLoading(true);
            const data = await JudgeService.getAll();
            setJudges(data.items || data); 
        } catch (error) {
            toastError(error, 'Не вдалося завантажити суддів');
        } finally {
            setLoading(false);
        }
    };

    const loadPersons = async () => {
        try {
            const data = await PersonService.getAll();
            setPersons(data.items || data);
        } catch (error) {}
    };

    const handleDelete = async (id) => {
        if (!window.confirm(`Видалити суддю з ID ${id}?`)) return;
        try {
            await JudgeService.delete(id);
            toast.success("Суддю видалено");
            setJudges(judges.filter(j => j.id !== id));
        } catch (error) {
            toast.error("Не вдалося видалити суддю");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const dataToSubmit = {
                personId: parseInt(formData.personId),
                qualificationLevel: formData.qualificationLevel
            };
            const data = await JudgeService.create(dataToSubmit);
            toast.success("Суддю створено");
            setJudges([...judges, data]);
            setIsModalOpen(false);
            setFormData({ personId: '', qualificationLevel: '' });
        } catch (error) {
            toast.error("Не вдалося створити суддю");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Суддівська колегія</h1>
                <div>
                    <button className="btn btn-outline" style={{marginRight: '1rem'}} onClick={loadJudges}>Оновити</button>
                    {canEdit && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadPersons();
                    }}>Додати суддю</button>}
                </div>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>ID особи</th>
                            <th>Кваліфікаційний рівень</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {judges.length > 0 ? (
                            judges.map((judge) => (
                                <tr key={judge.id}>
                                    <td>{judge.id}</td>
                                    <td>{judge.personId}</td>
                                    <td>
                                        <span className={`status-badge status-active`}>
                                            {judge.qualificationLevel}
                                        </span>
                                    </td>
                                    <td>
                                        <NavLink to={`/judges/${judge.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}>
                                            Розклад
                                        </NavLink>
                                        {canEdit && (
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(judge.id)}>Видалити</button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>Суддів не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Додати нового суддю">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Оберіть особу</label>
                        <select name="personId" value={formData.personId} onChange={handleChange} required>
                            <option value="">-- Оберіть особу --</option>
                            {persons.map(p => <option key={p.id} value={p.id}>{p.name} {p.surname} (ID: {p.id})</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Кваліфікаційний рівень</label>
                        <input type="text" name="qualificationLevel" value={formData.qualificationLevel} onChange={handleChange} required placeholder="напр. Міжнародний, Національний" />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Створити суддю</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default JudgesList;
