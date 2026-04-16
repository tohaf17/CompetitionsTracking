import React, { useState, useEffect } from 'react';
import TeamService from '../../services/team.service';
import PersonService from '../../services/person.service';
import { NavLink } from 'react-router-dom';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';

const TeamsList = () => {
    const { user } = useAuth();
    const canEdit = user?.role === 'Admin' || user?.role === 'Trainee'; // Based on Authorization rules

    const [teams, setTeams] = useState([]);
    const [coaches, setCoaches] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ name: '', coachId: '', type: 'Team' });

    useEffect(() => {
        loadTeams();
    }, []);

    const loadTeams = async () => {
        try {
            setLoading(true);
            const data = await TeamService.getAll();
            setTeams(data.items || data); 
        } catch (error) {
            toast.error(error.message || "Failed to load teams");
        } finally {
            setLoading(false);
        }
    };

    const loadCoaches = async () => {
        try {
            const data = await PersonService.getAll();
            setCoaches(data.items || data);
        } catch (error) {}
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Delete team "${name}"?`)) return;
        try {
            await TeamService.delete(id);
            toast.success("Team deleted");
            setTeams(teams.filter(p => p.id !== id));
        } catch (error) {
            toast.error("Failed to delete team");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const dataToSubmit = {
                ...formData,
                coachId: parseInt(formData.coachId)
            };
            const data = await TeamService.create(dataToSubmit);
            toast.success("Team created");
            setTeams([...teams, data]);
            setIsModalOpen(false);
            setFormData({ name: '', coachId: '', type: 'Team' });
        } catch (error) {
            toast.error("Failed to create team");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Teams</h1>
                <div>
                    <button className="btn btn-outline" style={{marginRight: '1rem'}} onClick={loadTeams}>Refresh</button>
                    {canEdit && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadCoaches();
                    }}>Add Team</button>}
                </div>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Team Name</th>
                            <th>Coach ID</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {teams.length > 0 ? (
                            teams.map((team) => (
                                <tr key={team.id}>
                                    <td>{team.id}</td>
                                    <td><strong>{team.name}</strong></td>
                                    <td>{team.coachId ? `Person ID: ${team.coachId}` : 'No Coach'}</td>
                                    <td>
                                        <NavLink to={`/teams/${team.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}>
                                            View Details
                                        </NavLink>
                                        {canEdit && (
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(team.id, team.name)}>Delete</button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="5" style={{textAlign: 'center', padding: '2rem'}}>No teams found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Team">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Team Name</label>
                        <input type="text" name="name" value={formData.name} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Coach</label>
                        <select name="coachId" value={formData.coachId} onChange={handleChange} required>
                            <option value="">-- Select Coach --</option>
                            {coaches.map(c => <option key={c.id} value={c.id}>{c.name} {c.surname}</option>)}
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Create Team</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default TeamsList;
