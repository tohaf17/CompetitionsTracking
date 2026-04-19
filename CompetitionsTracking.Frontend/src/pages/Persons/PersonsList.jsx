import React, { useState, useEffect } from 'react';
import PersonService from '../../services/person.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const PersonsList = () => {
    const { user } = useAuth();
    const canEdit = user?.role === 'Admin' || user?.role === 'Trainee';

    const [persons, setPersons] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({
        name: '', surname: '', country: '',
        dateOfBirth: '', gender: 0, type: 'Person', mentorId: ''
    });

    useEffect(() => {
        loadPersons();
    }, []);

    const loadPersons = async () => {
        try {
            setLoading(true);
            const data = await PersonService.getAll();
            setPersons(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити учасників');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Видалити особу "${name}"?`)) return;
        try {
            await PersonService.delete(id);
            toast.success("Особу видалено");
            setPersons(persons.filter(p => p.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити особу');
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const dataToSubmit = {
                ...formData,
                mentorId: formData.mentorId ? parseInt(formData.mentorId) : null,
                gender: parseInt(formData.gender),
                dateOfBirth: new Date(formData.dateOfBirth).toISOString()
            };
            const data = await PersonService.create(dataToSubmit);
            toast.success("Особу створено");
            setPersons([...persons, data]);
            setIsModalOpen(false);
            setFormData({ name: '', surname: '', country: '', dateOfBirth: '', gender: 0, type: 'Person', mentorId: '' });
        } catch (error) {
            toastError(error, 'Не вдалося створити особу');
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (<div className="page-container">
        <div className="page-header flex-between">
            <h1 className="page-title">Реєстр учасників</h1>
            <div>
                <button className="btn btn-outline" style={{ marginRight: '1rem' }} onClick={loadPersons}>Оновити</button>
                {canEdit && <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Додати особу</button>}
            </div>
        </div>

        <div className="glass-panel table-container">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>ПІБ</th>
                        <th>Країна</th>
                        <th>Вік</th>
                        <th>Тип / Роль</th>
                        <th>Дії</th>
                    </tr>
                </thead>
                <tbody>
                    {persons.length > 0 ? (
                        persons.map((person) => {
                            const age = new Date().getFullYear() - new Date(person.dateOfBirth).getFullYear();
                            return (
                                <tr key={person.id}>
                                    <td>{person.id}</td>
                                    <td><strong>{person.name} {person.surname}</strong></td>
                                    <td>{person.country}</td>
                                    <td>{age} р.</td>
                                    <td>
                                        <span className={`status-badge status-ongoing`}>
                                            {person.type}
                                        </span>
                                    </td>
                                    <td>
                                        {canEdit && (
                                            <button className="btn btn-danger" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem' }} onClick={() => handleDelete(person.id, person.name)}>Видалити</button>
                                        )}
                                        {!canEdit && <span style={{ color: 'var(--text-muted)' }}>Немає дій</span>}
                                    </td>
                                </tr>
                            );
                        })
                    ) : (
                        <tr>
                            <td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>Учасників не знайдено.</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>

        <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Додати нову особу">
            <form onSubmit={handleCreate}>
                <div className="form-group">
                    <label>Ім&apos;я</label>
                    <input type="text" name="name" value={formData.name} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Прізвище</label>
                    <input type="text" name="surname" value={formData.surname} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Країна</label>
                    <input type="text" name="country" value={formData.country} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Дата народження</label>
                    <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Стать</label>
                    <select name="gender" value={formData.gender} onChange={handleChange}>
                        <option value={0}>Чоловіча</option>
                        <option value={1}>Жіноча</option>
                    </select>
                </div>
                <div className="form-group">
                    <label>Тип / Роль</label>
                    <select name="type" value={formData.type} onChange={handleChange}>
                        <option value="Person">Особа / Гімнаст</option>
                    </select>
                </div>
                <div className="modal-footer">
                    <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                    <button type="submit" className="btn btn-primary">Створити особу</button>
                </div>
            </form>
        </Modal>
    </div>
    );
};

export default PersonsList;




