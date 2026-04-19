import React, { useState, useEffect } from 'react';
import EntryService from '../../services/entry.service';
import CompetitionService from '../../services/competition.service';
import PersonService from '../../services/person.service';
import DisciplineService from '../../services/discipline.service';
import CategoryService from '../../services/category.service';
import ScoreService from '../../services/score.service';
import JudgeService from '../../services/judge.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const EntriesList = () => {
    const { user } = useAuth();
    const canEdit = user?.role === 'Admin' || user?.role === 'Trainee';

    const [entries, setEntries] = useState([]);
    const [competitions, setCompetitions] = useState([]);
    const [persons, setPersons] = useState([]);
    const [disciplines, setDisciplines] = useState([]);
    const [categories, setCategories] = useState([]);
    const [judges, setJudges] = useState([]);
    const [loading, setLoading] = useState(true);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({
        competitionId: '', participantId: '', disciplineId: '', categoryId: ''
    });


    const [isScoreModalOpen, setIsScoreModalOpen] = useState(false);
    const [selectedEntry, setSelectedEntry] = useState(null);
    const [scoreData, setScoreData] = useState({
        judgeId: '', scoreType: 'DA', value: ''
    });

    useEffect(() => {
        loadEntries();
    }, []);

    const loadEntries = async () => {
        try {
            setLoading(true);
            const data = await EntryService.getAll();
            setEntries(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити заявки');
        } finally {
            setLoading(false);
        }
    };

    const loadFormData = async () => {
        try {
            const [comp, pers, disc, cat] = await Promise.all([
                CompetitionService.getAll(),
                PersonService.getAll(),
                DisciplineService.getAll(),
                CategoryService.getAll()
            ]);
            setCompetitions(unwrapCollection(comp));
            setPersons(unwrapCollection(pers));
            setDisciplines(unwrapCollection(disc));
            setCategories(unwrapCollection(cat));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити довідники для заявки');
        }
    };

    const loadJudgesData = async () => {
        try {
            const res = await JudgeService.getAll();
            setJudges(unwrapCollection(res));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити список суддів');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm(`Видалити заявку з ID ${id}?`)) return;
        try {
            await EntryService.delete(id);
            toast.success("Заявку видалено");
            setEntries(entries.filter(e => e.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити заявку');
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                competitionId: parseInt(formData.competitionId),
                participantId: parseInt(formData.participantId),
                disciplineId: parseInt(formData.disciplineId),
                categoryId: parseInt(formData.categoryId)
            };
            const data = await EntryService.create(payload);
            toast.success("Заявку подано");
            setEntries([...entries, data]);
            setIsModalOpen(false);
            setFormData({ competitionId: '', participantId: '', disciplineId: '', categoryId: '' });
        } catch (error) {
            toastError(error, 'Не вдалося створити заявку');
        }
    };

    const handleScoreSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                entryId: selectedEntry,
                judgeId: parseInt(scoreData.judgeId),
                type: scoreData.scoreType,
                scoreValue: parseFloat(scoreData.value)
            };
            await ScoreService.create(payload);
            toast.success("Оцінку успішно виставлено");
            setIsScoreModalOpen(false);
            setScoreData({ judgeId: '', scoreType: 'DA', value: '' });
        } catch (error) {
            toastError(error, 'Не вдалося виставити оцінку');
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleScoreChange = (e) => {
        setScoreData({ ...scoreData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Заявки на змагання</h1>
                <div>
                    <button className="btn btn-outline" style={{ marginRight: '1rem' }} onClick={loadEntries}>Оновити</button>
                    {canEdit && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadFormData();
                    }}>Додати заявку</button>}
                </div>
            </div>

            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>ID змагання</th>
                            <th>ID учасника</th>
                            <th>Дисципліна</th>
                            <th>Статус</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {entries.length > 0 ? (
                            entries.map((entry) => (
                                <tr key={entry.id}>
                                    <td>{entry.id}</td>
                                    <td>{entry.competitionId}</td>
                                    <td>{entry.participantId}</td>
                                    <td>Кат: {entry.categoryId} | Диcц: {entry.disciplineId}</td>
                                    <td>
                                        <span className={`status-badge ${entry.entryStatus === 0 ? 'status-active' : 'status-cancelled'}`}>
                                            {entry.entryStatus === 0 ? 'Активна' : 'Дискваліфікована'}
                                        </span>
                                    </td>
                                    <td>
                                        <button className="btn btn-primary" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem' }} onClick={() => {
                                            setSelectedEntry(entry.id);
                                            setIsScoreModalOpen(true);
                                            loadJudgesData();
                                        }}>Оцінити</button>

                                        {canEdit && (
                                            <button className="btn btn-danger" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem' }} onClick={() => handleDelete(entry.id)}>Видалити</button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>Заявок не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Створити нову заявку">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Змагання</label>
                        <select name="competitionId" value={formData.competitionId} onChange={handleChange} required>
                            <option value="">-- Оберіть змагання --</option>
                            {competitions.map(c => <option key={c.id} value={c.id}>{c.title}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Учасник</label>
                        <select name="participantId" value={formData.participantId} onChange={handleChange} required>
                            <option value="">-- Оберіть учасника --</option>
                            {persons.map(p => <option key={p.id} value={p.id}>{p.name} {p.surname}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Дисципліна</label>
                        <select name="disciplineId" value={formData.disciplineId} onChange={handleChange} required>
                            <option value="">-- Оберіть дисципліну --</option>
                            {disciplines.map(d => <option key={d.id} value={d.id}>{d.type}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Категорія</label>
                        <select name="categoryId" value={formData.categoryId} onChange={handleChange} required>
                            <option value="">-- Оберіть категорію --</option>
                            {categories.map(c => <option key={c.id} value={c.id}>{c.type} ({c.minAge}-{c.maxAge} р.)</option>)}
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Створити заявку</button>
                    </div>
                </form>
            </Modal>

            <Modal isOpen={isScoreModalOpen} onClose={() => setIsScoreModalOpen(false)} title={`Оцінити заявку #${selectedEntry}`}>
                <form onSubmit={handleScoreSubmit}>
                    <div className="form-group">
                        <label>Суддя</label>
                        <select name="judgeId" value={scoreData.judgeId} onChange={handleScoreChange} required>
                            <option value="">-- Оберіть суддю --</option>
                            {judges.map(j => <option key={j.id} value={j.id}>Суддя ID: {j.id} (Особа: {j.personId})</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Тип оцінки</label>
                        <select name="scoreType" value={scoreData.scoreType} onChange={handleScoreChange} required>
                            <option value="DA">Складність тіла (DA)</option>
                            <option value="DB">Складність інвентарю (DB)</option>
                            <option value="A">Артистизм (A)</option>
                            <option value="E">Виконання (E)</option>
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Значення оцінки</label>
                        <input type="number" step="0.01" min="0" max="20" name="value" value={scoreData.value} onChange={handleScoreChange} required />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsScoreModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Виставити оцінку</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default EntriesList;




