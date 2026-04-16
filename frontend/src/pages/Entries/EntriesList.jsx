import React, { useState, useEffect } from 'react';
import EntryService from '../../services/entry.service';
import CompetitionService from '../../services/competition.service';
import PersonService from '../../services/person.service';
import DisciplineService from '../../services/discipline.service';
import CategoryService from '../../services/category.service';
import ScoreService from '../../services/score.service';
import JudgeService from '../../services/judge.service';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';

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
    
    // Entry Modal
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ 
        competitionId: '', participantId: '', disciplineId: '', categoryId: '' 
    });

    // Score Modal
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
            setEntries(data.items || data); 
        } catch (error) {
            toast.error(error.message || "Failed to load entries");
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
            setCompetitions(comp.items || comp);
            setPersons(pers.items || pers);
            setDisciplines(disc.items || disc);
            setCategories(cat.items || cat);
        } catch (error) {}
    };

    const loadJudgesData = async () => {
        try {
            const res = await JudgeService.getAll();
            setJudges(res.items || res);
        } catch(e) {}
    };

    const handleDelete = async (id) => {
        if (!window.confirm(`Delete entry with ID ${id}?`)) return;
        try {
            await EntryService.delete(id);
            toast.success("Entry deleted");
            setEntries(entries.filter(e => e.id !== id));
        } catch (error) {
            toast.error("Failed to delete entry");
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
            toast.success("Entry submitted");
            setEntries([...entries, data]);
            setIsModalOpen(false);
            setFormData({ competitionId: '', participantId: '', disciplineId: '', categoryId: '' });
        } catch (error) {
            toast.error("Failed to create entry");
        }
    };

    const handleScoreSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                entryId: selectedEntry,
                judgeId: parseInt(scoreData.judgeId),
                scoreType: scoreData.scoreType,
                value: parseFloat(scoreData.value),
                isValid: true
            };
            await ScoreService.create(payload);
            toast.success("Score submitted successfully");
            setIsScoreModalOpen(false);
            setScoreData({ judgeId: '', scoreType: 'DA', value: '' });
        } catch (error) {
            toast.error("Failed to submit score");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleScoreChange = (e) => {
        setScoreData({ ...scoreData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Competition Entries</h1>
                <div>
                    <button className="btn btn-outline" style={{marginRight: '1rem'}} onClick={loadEntries}>Refresh</button>
                    {canEdit && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadFormData();
                    }}>Add Entry</button>}
                </div>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Competition ID</th>
                            <th>Participant ID</th>
                            <th>Discipline</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {entries.length > 0 ? (
                            entries.map((entry) => (
                                <tr key={entry.id}>
                                    <td>{entry.id}</td>
                                    <td>{entry.competitionId}</td>
                                    <td>{entry.participantId}</td>
                                    <td>Cat: {entry.categoryId} | Disc: {entry.disciplineId}</td>
                                    <td>
                                         <span className={`status-badge ${entry.entryStatus === 0 ? 'status-active' : 'status-cancelled'}`}>
                                            {entry.entryStatus === 0 ? 'Active' : 'Disqualified'}
                                        </span>
                                    </td>
                                    <td>
                                        <button className="btn btn-primary" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}} onClick={() => {
                                            setSelectedEntry(entry.id);
                                            setIsScoreModalOpen(true);
                                            loadJudgesData();
                                        }}>Evaluate (Score)</button>
                                        
                                        {canEdit && (
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(entry.id)}>Delete</button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>No entries found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Entry">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Competition</label>
                        <select name="competitionId" value={formData.competitionId} onChange={handleChange} required>
                            <option value="">-- Choose Competition --</option>
                            {competitions.map(c => <option key={c.id} value={c.id}>{c.title}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Participant (Person ID)</label>
                        <select name="participantId" value={formData.participantId} onChange={handleChange} required>
                            <option value="">-- Choose Participant --</option>
                            {persons.map(p => <option key={p.id} value={p.id}>{p.name} {p.surname}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Discipline</label>
                        <select name="disciplineId" value={formData.disciplineId} onChange={handleChange} required>
                            <option value="">-- Choose Discipline --</option>
                            {disciplines.map(d => <option key={d.id} value={d.id}>{d.type}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Category</label>
                        <select name="categoryId" value={formData.categoryId} onChange={handleChange} required>
                            <option value="">-- Choose Category --</option>
                            {categories.map(c => <option key={c.id} value={c.id}>{c.type} ({c.minAge}-{c.maxAge}yo)</option>)}
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Create Entry</button>
                    </div>
                </form>
            </Modal>

            <Modal isOpen={isScoreModalOpen} onClose={() => setIsScoreModalOpen(false)} title={`Evaluate Entry #${selectedEntry}`}>
                <form onSubmit={handleScoreSubmit}>
                    <div className="form-group">
                        <label>Judge</label>
                        <select name="judgeId" value={scoreData.judgeId} onChange={handleScoreChange} required>
                            <option value="">-- Choose Judge Profile --</option>
                            {judges.map(j => <option key={j.id} value={j.id}>Judge ID: {j.id} (Person: {j.personId})</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Score Type</label>
                        <select name="scoreType" value={scoreData.scoreType} onChange={handleScoreChange} required>
                            <option value="DA">Difficulty Body (DA)</option>
                            <option value="DB">Difficulty Apparatus (DB)</option>
                            <option value="A">Artistry (A)</option>
                            <option value="E">Execution (E)</option>
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Score Value</label>
                        <input type="number" step="0.01" min="0" max="20" name="value" value={scoreData.value} onChange={handleScoreChange} required />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsScoreModalOpen(false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Submit Score</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default EntriesList;
