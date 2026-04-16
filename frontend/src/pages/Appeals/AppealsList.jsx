import React, { useState, useEffect } from 'react';
import AppealService from '../../services/appeal.service';
import ResultService from '../../services/result.service';
import { NavLink } from 'react-router-dom';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';

const AppealsList = () => {
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';

    const [appeals, setAppeals] = useState([]);
    const [resultsData, setResultsData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [viewMode, setViewMode] = useState('pending'); // 'pending' or 'all'
    
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ resultId: '', reason: '' });

    useEffect(() => {
        loadAppeals();
    }, [viewMode]);

    const loadAppeals = async () => {
        try {
            setLoading(true);
            const data = viewMode === 'pending' 
                ? await AppealService.getPending() 
                : await AppealService.getAll();
            setAppeals(data.items || data); 
        } catch (error) {
            toast.error(error.message || `Failed to load ${viewMode} appeals`);
        } finally {
            setLoading(false);
        }
    };

    const loadResults = async () => {
        try {
            const data = await ResultService.getAll();
            setResultsData(data.items || data);
        } catch (e) {}
    }

    const handleDelete = async (id) => {
        if (!window.confirm(`Delete appeal with ID ${id}?`)) return;
        try {
            await AppealService.delete(id);
            toast.success("Appeal deleted");
            setAppeals(appeals.filter(a => a.id !== id));
        } catch (error) {
            toast.error("Failed to delete appeal");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                resultId: parseInt(formData.resultId),
                reason: formData.reason,
                status: 0, // Pending
                createdAt: new Date().toISOString(),
                resolvedAt: new Date(new Date().setHours(new Date().getHours() + 24)).toISOString() // mock offset
            };
            const data = await AppealService.create(payload);
            toast.success("Appeal submitted");
            setAppeals([...appeals, data]);
            setIsModalOpen(false);
            setFormData({ resultId: '', reason: '' });
        } catch (error) {
            toast.error("Failed to create appeal");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <div>
                    <h1 className="page-title">Appeals Directory</h1>
                    <div style={{marginTop: '0.5rem', display: 'flex', gap: '0.5rem'}}>
                        <button className={`btn ${viewMode === 'pending' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setViewMode('pending')}>Pending Only</button>
                        <button className={`btn ${viewMode === 'all' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setViewMode('all')}>All Appeals</button>
                    </div>
                </div>
                <div>
                    <button className="btn btn-outline" style={{marginRight: '1rem'}} onClick={loadAppeals}>Refresh</button>
                    {isAdmin && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadResults();
                    }}>Create Appeal</button>}
                </div>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Entry ID</th>
                            <th>Team ID</th>
                            <th>Status / Decision</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {appeals.length > 0 ? (
                            appeals.map((appeal) => (
                                <tr key={appeal.id}>
                                    <td>{appeal.id}</td>
                                    <td>{appeal.entryId || `Result -> ${appeal.resultId}`}</td>
                                    <td>{appeal.teamId || '-'}</td>
                                    <td>
                                         <span className={`status-badge ${appeal.status === 0 ? 'status-upcoming' : (appeal.status === 1 ? 'status-active' : 'status-completed')}`}>
                                            {appeal.status === 0 ? 'Pending Ref' : 'Resolved'}
                                        </span>
                                    </td>
                                    <td>
                                        <NavLink to={`/appeals/${appeal.id}`} className="btn btn-primary" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}>
                                            Review Dossier
                                        </NavLink>
                                        {isAdmin && (
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(appeal.id)}>Delete</button>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="5" style={{textAlign: 'center', padding: '2rem'}}>No pending appeals.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Appeal">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Select Target Result ID</label>
                        <select name="resultId" value={formData.resultId} onChange={handleChange} required>
                            <option value="">-- Choose Result --</option>
                            {resultsData.map(r => <option key={r.id} value={r.id}>Result ID: {r.id} | Score: {r.finalScore}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Reason for appeal</label>
                        <textarea 
                            name="reason" 
                            value={formData.reason} 
                            onChange={handleChange} 
                            required 
                            placeholder="Describe why scoring is requested to be reviewed"
                            style={{width: '100%', padding: '0.5rem', background: 'var(--surface-color)', color: '#fff', border: '1px solid var(--surface-border)'}}
                            rows={4}
                        />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">File Appeal</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AppealsList;
