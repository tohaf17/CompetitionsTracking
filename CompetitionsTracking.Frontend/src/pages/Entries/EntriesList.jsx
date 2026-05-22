import React, { useState, useEffect } from 'react';
import EntryService from '../../services/entry.service';
import CompetitionService from '../../services/competition.service';
import PersonService from '../../services/person.service';
import DisciplineService from '../../services/discipline.service';
import CategoryService from '../../services/category.service';
import ApparatusService from '../../services/apparatus.service';
import TeamService from '../../services/team.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';
import EntryFormModal from '../../components/Entries/EntryFormModal';

const EntriesList = () => {
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';
    const isCoach = user?.role === 'Trainee';
    const canCreate = isAdmin || isCoach;

    const [entries, setEntries] = useState([]);
    const [competitions, setCompetitions] = useState([]);
    const [disciplines, setDisciplines] = useState([]);
    const [apparatuses, setApparatuses] = useState([]);
    const [categories, setCategories] = useState([]);
    const [teams, setTeams] = useState([]);
    const [participantOptions, setParticipantOptions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [performanceType, setPerformanceType] = useState('');
    const [profileError, setProfileError] = useState(false);

    const [formData, setFormData] = useState({
        competitionId: '', 
        participantId: '',
        participantName: '', 
        participantSurname: '', 
        teamName: '',
        apparatusId: '', 
        categoryId: ''
    });

    useEffect(() => {
        loadInitialData();
    }, []);

    const loadInitialData = async () => {
        try {
            setLoading(true);
            const data = await EntryService.getAll();
            setEntries(unwrapCollection(data));
            await loadFormHelpers();
        } catch (error) {
            if (error?.response?.data?.message?.includes('прив\'язано профіль тренера')) {
                setProfileError(true);
            } else {
                toastError(error, 'Не вдалося завантажити дані');
            }
        } finally {
            setLoading(false);
        }
    };

    const loadFormHelpers = async () => {
        try {
            const [comp, disc, cat, app, teamRes, peopleRes, ownParticipantsRes] = await Promise.all([
                CompetitionService.getAll(),
                DisciplineService.getAll(),
                CategoryService.getAll(),
                ApparatusService.getAll(),
                TeamService.getAll(),
                isAdmin ? PersonService.getAll() : Promise.resolve([]),
                isCoach ? EntryService.getMyParticipants() : Promise.resolve([])
            ]);
            
            const allCompetitions = unwrapCollection(comp);
            setCompetitions(isCoach
                ? allCompetitions.filter(c => c.status === 1 && c.level !== 2)
                : allCompetitions);
            setDisciplines(unwrapCollection(disc));
            setCategories(unwrapCollection(cat));
            setApparatuses(unwrapCollection(app));
            const loadedTeams = unwrapCollection(teamRes);
            setTeams(loadedTeams);

            if (isAdmin) {
                const options = [
                    ...unwrapCollection(peopleRes).map(p => ({ id: p.id, name: `${p.name} ${p.surname}`, type: 'Person' })),
                    ...loadedTeams.map(t => ({ id: t.id, name: t.name, type: 'Team' }))
                ];
                setParticipantOptions(options);
            } else {
                setParticipantOptions(unwrapCollection(ownParticipantsRes));
            }
        } catch (error) {
            console.error('Error loading helpers:', error);
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const selectedCompetition = competitions.find(c => c.id === parseInt(formData.competitionId));
            if (isCoach && (!selectedCompetition || selectedCompetition.status !== 1 || selectedCompetition.level === 2)) {
                toast.error("Тренер може подавати заявки лише на відкриті не міжнародні змагання.");
                return;
            }

            const selectedCategory = categories.find(c => c.id === parseInt(formData.categoryId));
            const selectedParticipant = participantOptions.find(p => p.id === parseInt(formData.participantId));
            if (selectedCategory && selectedParticipant?.age != null) {
                if ((selectedCategory.minAge != null && selectedParticipant.age < selectedCategory.minAge) ||
                    (selectedCategory.maxAge != null && selectedParticipant.age > selectedCategory.maxAge)) {
                    toast.error(`Вік учасника (${selectedParticipant.age}) не відповідає категорії ${selectedCategory.type}.`);
                    return;
                }
            }

            const selectedDisc = disciplines.find(d => 
                d.type.startsWith(performanceType) && 
                d.apparatusId === parseInt(formData.apparatusId)
            );

            if (!selectedDisc) {
                toast.error("Така комбінація дисципліни та предмета не знайдена в системі.");
                return;
            }

            const payload = {
                competitionId: parseInt(formData.competitionId),
                participantId: formData.participantId ? parseInt(formData.participantId) : undefined,
                participantName: formData.participantId ? undefined : formData.participantName,
                participantSurname: formData.participantId ? undefined : formData.participantSurname,
                teamName: formData.participantId ? undefined : formData.teamName,
                disciplineId: selectedDisc.id,
                categoryId: parseInt(formData.categoryId)
            };

            await EntryService.create(payload);
            toast.success("Заявку подано");
            setIsModalOpen(false);
            loadInitialData();
            setFormData({ competitionId: '', participantId: '', participantName: '', participantSurname: '', teamName: '', apparatusId: '', categoryId: '' });
            setPerformanceType('');
        } catch (error) {
            toastError(error, 'Не вдалося створити заявку');
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

    const handleUpdateApplicationStatus = async (id, newStatus) => {
        try {
            await EntryService.changeApplicationStatus(id, { newStatus });
            toast.success("Статус оновлено");
            loadInitialData();
        } catch (error) {
            toastError(error, 'Помилка оновлення статусу');
        }
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    if (profileError) {
        return (
            <div className="page-container">
                <div className="glass-panel" style={{ padding: '3rem', textAlign: 'center' }}>
                    <h2 style={{ color: '#f59e0b' }}>Профіль тренера не знайдено</h2>
                    <p>Будь ласка, зверніться до адміністратора.</p>
                    <button className="btn btn-primary" onClick={loadInitialData}>Оновити</button>
                </div>
            </div>
        );
    }

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Управління заявками</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Подати нову заявку</button>
            </div>

            <div className="glass-panel table-container">
                <table style={{ width: '100%', textAlign: 'left' }}>
                    <thead>
                        <tr style={{ borderBottom: '2px solid var(--surface-border)' }}>
                            <th style={{ padding: '1rem' }}>№</th>
                            <th>Змагання</th>
                            <th>Учасник</th>
                            <th>Дисципліна | Категорія</th>
                            <th>Дата</th>
                            <th>Статус</th>
                            <th style={{ textAlign: 'right', paddingRight: '1rem' }}>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {entries.map((entry, index) => (
                            <tr key={entry.id} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                <td style={{ padding: '1rem' }}>{index + 1}</td>
                                <td>{entry.competitionName}</td>
                                <td><strong>{entry.participantName}</strong></td>
                                <td>{entry.disciplineName} | {entry.categoryName}</td>
                                <td>{new Date(entry.submittedAt).toLocaleDateString()}</td>
                                <td>
                                    <span className={`status-badge status-${entry.applicationStatus === 1 ? 'active' : entry.applicationStatus === 2 ? 'cancelled' : 'pending'}`}>
                                        {entry.applicationStatus === 1 ? 'Прийнято' : entry.applicationStatus === 2 ? 'Відхилено' : 'Очікує'}
                                    </span>
                                </td>
                                <td style={{ textAlign: 'right', paddingRight: '1rem' }}>
                                    {isAdmin && entry.applicationStatus === 0 && (
                                        <>
                                            <button className="btn btn-primary" style={{ padding: '0.3rem 0.6rem', marginRight: '0.5rem', backgroundColor: '#10b981' }} onClick={() => handleUpdateApplicationStatus(entry.id, 1)}>Прийняти</button>
                                            <button className="btn btn-outline" style={{ padding: '0.3rem 0.6rem', color: '#ef4444' }} onClick={() => handleUpdateApplicationStatus(entry.id, 2)}>Відхилити</button>
                                        </>
                                    )}
                                    {isAdmin && <button className="btn btn-danger" style={{ padding: '0.3rem 0.6rem', marginLeft: '0.5rem' }} onClick={() => handleDelete(entry.id)}>Видалити</button>}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <EntryFormModal 
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSubmit={handleCreate}
                formData={formData}
                handleChange={(e) => setFormData({ ...formData, [e.target.name]: e.target.value })}
                competitions={competitions}
                participantOptions={participantOptions}
                disciplines={disciplines}
                apparatuses={apparatuses}
                categories={categories}
                performanceType={performanceType}
                setPerformanceType={setPerformanceType}
                isCoach={isCoach}
                isAdmin={isAdmin}
                levelMap={{ 0: 'Локальне', 1: 'Національне', 2: 'Міжнародне' }}
                teams={teams}
            />
        </div>
    );
};

export default EntriesList;
