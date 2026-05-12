import React, { useState, useEffect } from 'react';
import EntryService from '../../services/entry.service';
import CompetitionService from '../../services/competition.service';
import PersonService from '../../services/person.service';
import DisciplineService from '../../services/discipline.service';
import CategoryService from '../../services/category.service';
import ScoreService from '../../services/score.service';
import JudgeService from '../../services/judge.service';
import TeamService from '../../services/team.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const EntriesList = () => {
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';
    const isCoach = user?.role === 'Trainee';
    const canCreate = isAdmin || isCoach;
    const levelMap = {
        0: 'Локальне',
        1: 'Національне',
        2: 'Міжнародне'
    };

    const [entries, setEntries] = useState([]);
    const [competitions, setCompetitions] = useState([]);
    const [persons, setPersons] = useState([]);
    const [disciplines, setDisciplines] = useState([]);
    const [categories, setCategories] = useState([]);
    const [judges, setJudges] = useState([]);
    const [participantOptions, setParticipantOptions] = useState([]);
    const [loading, setLoading] = useState(true);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({
        competitionId: '', 
        participantId: '',
        participantName: '', 
        participantSurname: '', 
        teamName: '',
        disciplineId: '', 
        categoryId: ''
    });

    const [isScoreModalOpen, setIsScoreModalOpen] = useState(false);
    const [selectedEntry, setSelectedEntry] = useState(null);
    const [scoreData, setScoreData] = useState({
        judgeId: '', scoreType: 'DA', value: ''
    });

    useEffect(() => {
        loadEntries();
        loadFormData();
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

    const [teams, setTeams] = useState([]);

    const loadFormData = async () => {
        try {
            const [comp, disc, cat, teamRes, peopleRes, ownParticipantsRes] = await Promise.all([
                CompetitionService.getAll(),
                DisciplineService.getAll(),
                CategoryService.getAll(),
                TeamService.getAll(),
                isAdmin ? PersonService.getAll() : Promise.resolve([]),
                isCoach ? EntryService.getMyParticipants() : Promise.resolve([])
            ]);
            const allCompetitions = unwrapCollection(comp);
            setCompetitions(isCoach
                ? allCompetitions.filter(c => c.level !== 2 && (c.status === 0 || c.status === 1))
                : allCompetitions);
            setDisciplines(unwrapCollection(disc));
            setCategories(unwrapCollection(cat));
            const loadedTeams = unwrapCollection(teamRes);
            setTeams(loadedTeams);
            const adminOptions = [
                ...unwrapCollection(peopleRes).map(p => ({ id: p.id, name: `${p.name} ${p.surname}`, type: 'Person' })),
                ...loadedTeams.map(t => ({ id: t.id, name: t.name, type: 'Team' }))
            ];
            setParticipantOptions(isCoach ? unwrapCollection(ownParticipantsRes) : adminOptions);
        } catch (error) {
            console.error('Error loading form data:', error);
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
        if (!window.confirm(`Видалити заявку?`)) return;
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
                participantId: formData.participantId ? parseInt(formData.participantId) : undefined,
                participantName: formData.participantId ? undefined : formData.participantName,
                participantSurname: formData.participantId ? undefined : formData.participantSurname,
                teamName: formData.participantId ? undefined : formData.teamName,
                disciplineId: parseInt(formData.disciplineId),
                categoryId: parseInt(formData.categoryId)
            };
            await EntryService.create(payload);
            toast.success("Заявку подано");
            loadEntries();
            setIsModalOpen(false);
            setFormData({ 
                competitionId: '', 
                participantId: '',
                participantName: '', 
                participantSurname: '', 
                teamName: '', 
                disciplineId: '', 
                categoryId: '' 
            });
        } catch (error) {
            toastError(error, 'Не вдалося створити заявку');
        }
    };

    const handleScoreSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                entryId: selectedEntry.id,
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

    const handleUpdateApplicationStatus = async (id, newStatus) => {
        try {
            await EntryService.changeApplicationStatus(id, { newStatus });
            toast.success("Статус заявки оновлено");
            loadEntries();
        } catch (error) {
            toastError(error, 'Не вдалося оновити статус заявки');
        }
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    const getAppStatusBadge = (status) => {
        switch (status) {
            case 0: return <span className="status-badge status-pending">Очікує</span>;
            case 1: return <span className="status-badge status-active">Прийнято</span>;
            case 2: return <span className="status-badge status-cancelled">Відхилено</span>;
            default: return <span className="status-badge">{status}</span>;
        }
    };

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Управління заявками</h1>
                <div>
                    <button className="btn btn-outline" style={{ marginRight: '1rem' }} onClick={loadEntries}>Оновити</button>
                    {canCreate && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadFormData();
                    }}>Подати нову заявку</button>}
                </div>
            </div>

            <div className="glass-panel table-container">
                <table style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                    <thead>
                        <tr style={{ borderBottom: '2px solid var(--surface-border)' }}>
                            <th style={{ padding: '1rem' }}>№</th>
                            <th>Змагання</th>
                            <th>Учасник</th>
                            <th>Дисципліна | Категорія</th>
                            <th>Дата подачі</th>
                            <th>Статус</th>
                            <th style={{ textAlign: 'right', paddingRight: '1rem' }}>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {entries.length > 0 ? (
                            entries.map((entry, index) => (
                                <tr key={entry.id} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '1rem' }}>{index + 1}</td>
                                    <td>{entry.competitionName}</td>
                                    <td><strong>{entry.participantName}</strong></td>
                                    <td>{entry.disciplineName} | {entry.categoryName}</td>
                                    <td>{new Date(entry.submittedAt).toLocaleDateString('uk-UA')}</td>
                                    <td>{getAppStatusBadge(entry.applicationStatus)}</td>
                                    <td style={{ textAlign: 'right', paddingRight: '1rem' }}>
                                        {isAdmin && entry.applicationStatus === 0 && (
                                            <>
                                                <button 
                                                    className="btn btn-primary" 
                                                    style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem', backgroundColor: '#10b981', borderColor: '#10b981' }} 
                                                    onClick={() => handleUpdateApplicationStatus(entry.id, 1)}
                                                >
                                                    Прийняти
                                                </button>
                                                <button 
                                                    className="btn btn-outline" 
                                                    style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem', color: '#ef4444', borderColor: '#ef4444' }} 
                                                    onClick={() => handleUpdateApplicationStatus(entry.id, 2)}
                                                >
                                                    Відхилити
                                                </button>
                                            </>
                                        )}
                                        
                                        {isAdmin && (
                                            <button className="btn btn-danger" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem' }} onClick={() => handleDelete(entry.id)}>Видалити</button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="7" style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-muted)' }}>Заявок не знайдено.</td>
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
                            {competitions.map(c => <option key={c.id} value={c.id}>{c.title} ({levelMap[c.level] || 'тип невідомий'})</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>{isCoach ? 'Підопічний або команда' : 'Існуючий учасник / команда'}</label>
                        <select name="participantId" value={formData.participantId} onChange={handleChange} required={isCoach}>
                            <option value="">-- {isCoach ? 'Оберіть зі своїх підопічних або команд' : 'Можна не обирати і створити вручну'} --</option>
                            {participantOptions.map(p => (
                                <option key={`${p.type}-${p.id}`} value={p.id}>
                                    {p.name} ({p.type === 'Team' ? 'команда' : 'учасник'})
                                </option>
                            ))}
                        </select>
                    </div>
                    {!formData.participantId && isAdmin && (
                        <>
                            <div className="form-group">
                                <label>Ім'я учасника</label>
                                <input type="text" name="participantName" value={formData.participantName} onChange={handleChange} placeholder="Введіть ім'я" required={!formData.participantId} />
                            </div>
                            <div className="form-group">
                                <label>Прізвище учасника</label>
                                <input type="text" name="participantSurname" value={formData.participantSurname} onChange={handleChange} placeholder="Введіть прізвище" required={!formData.participantId} />
                            </div>
                            <div className="form-group">
                                <label>Команда / Клуб</label>
                                <input
                                    type="text"
                                    name="teamName"
                                    list="teams-list"
                                    value={formData.teamName}
                                    onChange={handleChange}
                                    placeholder="Оберіть або введіть назву"
                                    required={!formData.participantId}
                                />
                                <datalist id="teams-list">
                                    {teams.map(t => <option key={t.id} value={t.name} />)}
                                </datalist>
                            </div>
                        </>
                    )}
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
                        <button type="submit" className="btn btn-primary">Подати заявку</button>
                    </div>
                </form>
            </Modal>

            <Modal isOpen={isScoreModalOpen} onClose={() => setIsScoreModalOpen(false)} title={`Оцінити виступ: ${selectedEntry?.participantName}`}>
                <form onSubmit={handleScoreSubmit}>
                    <div className="form-group">
                        <label>Суддя</label>
                        <select name="judgeId" value={scoreData.judgeId} onChange={handleScoreChange} required>
                            <option value="">-- Оберіть суддю --</option>
                            {judges.map(j => <option key={j.id} value={j.id}>{j.fullName} (Квал: {j.qualificationLevel})</option>)}
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
