import React, { useState, useEffect } from 'react';
import JudgeService from '../../services/judge.service';
import PersonService from '../../services/person.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
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
    const [isCreatePersonMode, setIsCreatePersonMode] = useState(false);
    
    const [formData, setFormData] = useState({ 
        personId: '', 
        qualificationLevel: '',
        name: '',
        surname: '',
        country: '',
        dateOfBirth: '',
        gender: 0
    });

    useEffect(() => {
        loadJudges();
    }, []);

    const loadJudges = async () => {
        try {
            setLoading(true);
            const data = await JudgeService.getAll();
            setJudges(unwrapCollection(data)); 
        } catch (error) {
            toastError(error, 'Не вдалося завантажити суддів');
        } finally {
            setLoading(false);
        }
    };

    const loadPersons = async () => {
        try {
            const data = await PersonService.getAll();
            setPersons(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити список осіб');
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Видалити суддю ${name}?`)) return;
        try {
            await JudgeService.delete(id);
            toast.success("Суддю видалено");
            setJudges(judges.filter(j => j.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити суддю');
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            let finalPersonId = formData.personId;
            
            if (isCreatePersonMode) {
                const newPerson = await PersonService.create({
                    name: formData.name,
                    surname: formData.surname,
                    country: formData.country,
                    gender: parseInt(formData.gender),
                    dateOfBirth: new Date(formData.dateOfBirth).toISOString()
                });
                finalPersonId = newPerson.id;
            }

            const dataToSubmit = {
                personId: parseInt(finalPersonId),
                qualificationLevel: formData.qualificationLevel
            };
            
            await JudgeService.create(dataToSubmit);
            toast.success("Суддю створено");
            loadJudges(); 
            setIsModalOpen(false);
            resetForm();
        } catch (error) {
            toastError(error, 'Не вдалося створити суддю');
        }
    };

    const resetForm = () => {
        setFormData({ 
            personId: '', 
            qualificationLevel: '', 
            name: '', 
            surname: '', 
            country: '', 
            dateOfBirth: '', 
            gender: 0 
        });
        setIsCreatePersonMode(false);
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
                            <th>№</th>
                            <th>ПІБ Судді</th>
                            <th>Кваліфікаційний рівень</th>
                            <th>Змагання</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {judges.length > 0 ? (
                            judges.map((judge, index) => (
                                <tr key={judge.id}>
                                    <td>{index + 1}</td>
                                    <td><strong>{judge.fullName}</strong></td>
                                    <td>
                                        <span className={`status-badge status-active`}>
                                            {judge.qualificationLevel}
                                        </span>
                                    </td>
                                    <td>
                                        {judge.competitions?.length > 0 ? (
                                            <ul style={{ margin: 0, paddingLeft: '1rem' }}>
                                                {judge.competitions.map((comp, idx) => <li key={idx}>{comp}</li>)}
                                            </ul>
                                        ) : (
                                            <span style={{ color: 'var(--text-muted)' }}>Не брав участі</span>
                                        )}
                                    </td>
                                    <td>
                                        {canEdit && (
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(judge.id, judge.fullName)}>Видалити</button>
                                        )}
                                        {!canEdit && <span style={{ color: 'var(--text-muted)' }}>Немає дій</span>}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="4" style={{textAlign: 'center', padding: '2rem'}}>Суддів не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Додати нового суддю">
                <form onSubmit={handleCreate}>
                    <div className="form-group" style={{marginBottom: '1rem', borderBottom: '1px solid var(--border-color)', paddingBottom: '1rem'}}>
                        <label style={{display: 'flex', alignItems: 'center', cursor: 'pointer'}}>
                            <input 
                                type="checkbox" 
                                checked={isCreatePersonMode} 
                                onChange={(e) => setIsCreatePersonMode(e.target.checked)}
                                style={{marginRight: '0.5rem', width: 'auto', height: 'auto'}}
                            />
                            Створити нову особу замість вибору з існуючих
                        </label>
                    </div>

                    {!isCreatePersonMode ? (
                        <div className="form-group">
                            <label>Оберіть особу</label>
                            <select name="personId" value={formData.personId} onChange={handleChange} required={!isCreatePersonMode}>
                                <option value="">-- Оберіть особу --</option>
                                {persons.map(p => <option key={p.id} value={p.id}>{p.name} {p.surname} (Країна: {p.country})</option>)}
                            </select>
                        </div>
                    ) : (
                        <>
                            <div className="grid grid-2">
                                <div className="form-group">
                                    <label>Ім'я</label>
                                    <input type="text" name="name" value={formData.name} onChange={handleChange} required={isCreatePersonMode} />
                                </div>
                                <div className="form-group">
                                    <label>Прізвище</label>
                                    <input type="text" name="surname" value={formData.surname} onChange={handleChange} required={isCreatePersonMode} />
                                </div>
                            </div>
                            <div className="form-group">
                                <label>Країна</label>
                                <input type="text" name="country" value={formData.country} onChange={handleChange} required={isCreatePersonMode} />
                            </div>
                            <div className="grid grid-2">
                                <div className="form-group">
                                    <label>Дата народження</label>
                                    <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} required={isCreatePersonMode} />
                                </div>
                                <div className="form-group">
                                    <label>Стать</label>
                                    <select name="gender" value={formData.gender} onChange={handleChange} required={isCreatePersonMode}>
                                        <option value={0}>Чоловіча</option>
                                        <option value={1}>Жіноча</option>
                                    </select>
                                </div>
                            </div>
                        </>
                    )}

                    <div className="form-group" style={{marginTop: '1rem'}}>
                        <label>Кваліфікаційний рівень</label>
                        <input type="text" name="qualificationLevel" value={formData.qualificationLevel} onChange={handleChange} required placeholder="напр. Міжнародний, Національний" />
                    </div>

                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Зберегти суддю</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default JudgesList;
