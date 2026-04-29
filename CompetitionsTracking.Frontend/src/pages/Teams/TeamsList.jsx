import React, { useState, useEffect } from 'react';
import TeamService from '../../services/team.service';
import PersonService from '../../services/person.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const TeamsList = () => {
    const { user } = useAuth();
    const canEdit = user?.role === 'Admin' || user?.role === 'Trainee';

    const [teams, setTeams] = useState([]);
    const [coaches, setCoaches] = useState([]);
    const [loading, setLoading] = useState(true);
    
    const [isTeamModalOpen, setIsTeamModalOpen] = useState(false);
    const [teamFormData, setTeamFormData] = useState({ name: '', coachId: '', type: 'Team' });

    const [selectedTeam, setSelectedTeam] = useState(null);
    const [roster, setRoster] = useState([]);
    const [isRosterModalOpen, setIsRosterModalOpen] = useState(false);
    
    const [isAddMemberModalOpen, setIsAddMemberModalOpen] = useState(false);
    const [memberFormData, setMemberFormData] = useState({ 
        name: '', 
        surname: '', 
        country: '', 
        dateOfBirth: '', 
        gender: 1
    });

    useEffect(() => {
        loadTeams();
    }, []);

    const loadTeams = async () => {
        try {
            setLoading(true);
            const data = await TeamService.getAll();
            setTeams(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити команди');
        } finally {
            setLoading(false);
        }
    };

    const loadCoaches = async () => {
        try {
            const data = await PersonService.getAll();
            setCoaches(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити список тренерів');
        }
    };

    const handleDeleteTeam = async (id, name) => {
        if (!window.confirm(`Видалити команду "${name}"?`)) return;
        try {
            await TeamService.delete(id);
            toast.success("Команду видалено");
            setTeams(teams.filter(p => p.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити команду');
        }
    };

    const handleCreateTeam = async (e) => {
        e.preventDefault();
        try {
            const dataToSubmit = {
                ...teamFormData,
                coachId: parseInt(teamFormData.coachId)
            };
            const data = await TeamService.create(dataToSubmit);
            toast.success("Команду створено");
            loadTeams(); // Reload to get names properly
            setIsTeamModalOpen(false);
            setTeamFormData({ name: '', coachId: '', type: 'Team' });
        } catch (error) {
            toastError(error, 'Не вдалося створити команду');
        }
    };

    const openRoster = async (team) => {
        setSelectedTeam(team);
        try {
            const data = await TeamService.getRoster(team.id);
            setRoster(data.members || []);
            setIsRosterModalOpen(true);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити склад команди');
        }
    };

    const handleRemoveMember = async (personId) => {
        if (!window.confirm("Видалити учасника зі складу команди?")) return;
        try {
            await TeamService.removeMember(selectedTeam.id, personId);
            toast.success("Учасника видалено");
            setRoster(roster.filter(m => m.personId !== personId));
        } catch (error) {
            toastError(error, 'Не вдалося видалити учасника');
        }
    };

    const handleAddMember = async (e) => {
        e.preventDefault();
        try {
            // 1. Create the person
            const newPerson = await PersonService.create({
                ...memberFormData,
                gender: parseInt(memberFormData.gender),
                dateOfBirth: new Date(memberFormData.dateOfBirth).toISOString()
            });
            
            // 2. Add as member to team
            await TeamService.addMember(selectedTeam.id, newPerson.id);
            
            toast.success("Учасника додано до складу");
            
            // 3. Refresh roster
            const updatedRoster = await TeamService.getRoster(selectedTeam.id);
            setRoster(updatedRoster.members || []);
            
            setIsAddMemberModalOpen(false);
            setMemberFormData({ name: '', surname: '', country: '', dateOfBirth: '', gender: 0 });
        } catch (error) {
            toastError(error, 'Не вдалося додати учасника');
        }
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Команди</h1>
                <div>
                    <button className="btn btn-outline" style={{marginRight: '1rem'}} onClick={loadTeams}>Оновити</button>
                    {canEdit && <button className="btn btn-primary" onClick={() => {
                        setIsTeamModalOpen(true);
                        loadCoaches();
                    }}>Додати команду</button>}
                </div>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Назва команди</th>
                            <th>Тренер</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {teams.length > 0 ? (
                            teams.map((team, index) => (
                                <tr key={team.id}>
                                    <td>{index + 1}</td>
                                    <td><strong>{team.name}</strong></td>
                                    <td>{team.coachFullName || 'Без тренера'}</td>
                                    <td>
                                        <button 
                                            className="btn btn-outline" 
                                            style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}
                                            onClick={() => openRoster(team)}
                                        >
                                            Склад команди
                                        </button>
                                        {canEdit && (
                                            <button 
                                                className="btn btn-danger" 
                                                style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} 
                                                onClick={() => handleDeleteTeam(team.id, team.name)}
                                            >
                                                Видалити
                                            </button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="4" style={{textAlign: 'center', padding: '2rem'}}>Команд не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            
            <Modal isOpen={isTeamModalOpen} onClose={() => setIsTeamModalOpen(false)} title="Створити нову команду">
                <form onSubmit={handleCreateTeam}>
                    <div className="form-group">
                        <label>Назва команди</label>
                        <input 
                            type="text" 
                            name="name" 
                            value={teamFormData.name} 
                            onChange={(e) => setTeamFormData({...teamFormData, name: e.target.value})} 
                            required 
                        />
                    </div>
                    <div className="form-group">
                        <label>Тренер</label>
                        <select 
                            name="coachId" 
                            value={teamFormData.coachId} 
                            onChange={(e) => setTeamFormData({...teamFormData, coachId: e.target.value})} 
                            required
                        >
                            <option value="">-- Оберіть тренера --</option>
                            {coaches.map(c => <option key={c.id} value={c.id}>{c.name} {c.surname}</option>)}
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsTeamModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Створити команду</button>
                    </div>
                </form>
            </Modal>

            <Modal isOpen={isRosterModalOpen} onClose={() => setIsRosterModalOpen(false)} title={`Склад команди: ${selectedTeam?.name}`}>
                <div style={{marginBottom: '1.5rem'}}>
                    {canEdit && (
                        <button className="btn btn-primary" onClick={() => setIsAddMemberModalOpen(true)}>
                            Додати учасника
                        </button>
                    )}
                </div>
                <div className="table-container">
                    <table style={{width: '100%'}}>
                        <thead>
                            <tr>
                                <th>№</th>
                                <th>ПІБ</th>
                                <th>Країна</th>
                                {canEdit && <th>Дії</th>}
                            </tr>
                        </thead>
                        <tbody>
                            {roster.length > 0 ? (
                                roster.map((member, idx) => (
                                    <tr key={member.personId}>
                                        <td>{idx + 1}</td>
                                        <td>{member.fullName}</td>
                                        <td>{member.country}</td>
                                        {canEdit && (
                                            <td>
                                                <button 
                                                    className="btn btn-danger" 
                                                    style={{padding: '0.2rem 0.5rem', fontSize: '0.75rem'}}
                                                    onClick={() => handleRemoveMember(member.personId)}
                                                >
                                                    Видалити
                                                </button>
                                            </td>
                                        )}
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td colSpan={canEdit ? 4 : 3} style={{textAlign: 'center', padding: '1rem'}}>Склад команди порожній</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
                <div className="modal-footer">
                    <button type="button" className="btn btn-outline" onClick={() => setIsRosterModalOpen(false)}>Закрити</button>
                </div>
            </Modal>

            <Modal isOpen={isAddMemberModalOpen} onClose={() => setIsAddMemberModalOpen(false)} title="Додати нового учасника до складу">
                <form onSubmit={handleAddMember}>
                    <div className="grid grid-2">
                        <div className="form-group">
                            <label>Ім'я</label>
                            <input 
                                type="text" 
                                value={memberFormData.name} 
                                onChange={(e) => setMemberFormData({...memberFormData, name: e.target.value})} 
                                required 
                            />
                        </div>
                        <div className="form-group">
                            <label>Прізвище</label>
                            <input 
                                type="text" 
                                value={memberFormData.surname} 
                                onChange={(e) => setMemberFormData({...memberFormData, surname: e.target.value})} 
                                required 
                            />
                        </div>
                    </div>
                    <div className="form-group">
                        <label>Країна</label>
                        <input 
                            type="text" 
                            value={memberFormData.country} 
                            onChange={(e) => setMemberFormData({...memberFormData, country: e.target.value})} 
                            required 
                        />
                    </div>
                    <div className="grid grid-2">
                        <div className="form-group">
                            <label>Дата народження</label>
                            <input 
                                type="date" 
                                value={memberFormData.dateOfBirth} 
                                onChange={(e) => setMemberFormData({...memberFormData, dateOfBirth: e.target.value})} 
                                required 
                            />
                        </div>
                        <div className="form-group">
                            <label>Стать</label>
                            <select 
                                value={memberFormData.gender} 
                                onChange={(e) => setMemberFormData({...memberFormData, gender: e.target.value})}
                                required
                            >
                                <option value={0}>Чоловіча</option>
                                <option value={1}>Жіноча</option>
                            </select>
                        </div>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsAddMemberModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Зберегти та додати</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default TeamsList;
