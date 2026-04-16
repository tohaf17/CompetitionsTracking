import React, { useState, useEffect } from 'react';
import CompetitionService from '../../../services/competition.service';
import Modal from '../../../components/UI/Modal';
import { NavLink } from 'react-router-dom';
import toast from 'react-hot-toast';

const AdminCompetitions = () => {
    const [competitions, setCompetitions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ 
        title: '', city: '', 
        startDate: '', endDate: '', status: 0 
    });

    useEffect(() => {
        loadCompetitions();
    }, []);

    const loadCompetitions = async () => {
        try {
            setLoading(true);
            const data = await CompetitionService.getAll();
            setCompetitions(data.items || data);
        } catch (error) {
            toast.error(error.message || "Failed to load competitions");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, title) => {
        if (!window.confirm(`Delete competition "${title}"?`)) return;
        try {
            await CompetitionService.delete(id);
            toast.success("Competition deleted");
            setCompetitions(competitions.filter(c => c.id !== id));
        } catch (error) {
            toast.error("Failed to delete competition");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const dataToSubmit = {
                title: formData.title,
                city: formData.city,
                startDate: new Date(formData.startDate).toISOString(),
                endDate: new Date(formData.endDate).toISOString(),
                status: parseInt(formData.status)
            };
            const data = await CompetitionService.create(dataToSubmit);
            toast.success("Competition created");
            setCompetitions([...competitions, data]);
            setIsModalOpen(false);
            setFormData({ title: '', city: '', startDate: '', endDate: '', status: 0 });
        } catch (error) {
            toast.error("Failed to create competition");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Manage Competitions</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Add Competition</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Title</th>
                            <th>City</th>
                            <th>Dates</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {competitions.length > 0 ? (
                            competitions.map((item) => {
                                const statusMap = {
                                    0: { text: "Planned", class: "status-planned" },
                                    1: { text: "Ongoing", class: "status-ongoing" },
                                    2: { text: "Completed", class: "status-completed" },
                                    3: { text: "Cancelled", class: "status-cancelled" }
                                };
                                const compStatus = statusMap[item.status] || { text: "Unknown", class: "" };
                                
                                return (
                                    <tr key={item.id}>
                                        <td>{item.id}</td>
                                        <td><strong>{item.title}</strong></td>
                                        <td>{item.city}</td>
                                        <td>{new Date(item.startDate).toLocaleDateString()} - {new Date(item.endDate).toLocaleDateString()}</td>
                                        <td><span className={`status-badge ${compStatus.class}`}>{compStatus.text}</span></td>
                                        <td>
                                            <NavLink to={`/competitions/${item.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem'}}>View</NavLink>
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(item.id, item.title)}>Delete</button>
                                        </td>
                                    </tr>
                                )
                            })
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>No competitions found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Competition">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Title</label>
                        <input type="text" name="title" value={formData.title} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>City</label>
                        <input type="text" name="city" value={formData.city} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Start Date</label>
                        <input type="date" name="startDate" value={formData.startDate} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>End Date</label>
                        <input type="date" name="endDate" value={formData.endDate} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Status</label>
                        <select name="status" value={formData.status} onChange={handleChange}>
                            <option value={0}>Planned</option>
                            <option value={1}>Ongoing</option>
                            <option value={2}>Completed</option>
                            <option value={3}>Cancelled</option>
                        </select>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Create</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AdminCompetitions;
