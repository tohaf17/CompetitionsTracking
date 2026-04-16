import React, { useState, useEffect } from 'react';
import ApparatusService from '../../../services/apparatus.service';
import Modal from '../../../components/UI/Modal';
import toast from 'react-hot-toast';

const AdminApparatus = () => {
    const [apparatus, setApparatus] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ type: '' });

    useEffect(() => {
        loadApparatus();
    }, []);

    const loadApparatus = async () => {
        try {
            setLoading(true);
            const data = await ApparatusService.getAll();
            setApparatus(data.items || data);
        } catch (error) {
            toast.error("Failed to load apparatus");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Delete apparatus "${name}"?`)) return;
        try {
            await ApparatusService.delete(id);
            toast.success("Apparatus deleted");
            setApparatus(apparatus.filter(a => a.id !== id));
        } catch (error) {
            toast.error("Failed to delete apparatus");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const data = await ApparatusService.create({ type: formData.type });
            toast.success("Apparatus created");
            setApparatus([...apparatus, data]);
            setIsModalOpen(false);
            setFormData({ type: '' });
        } catch (error) {
            toast.error("Failed to create apparatus");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Manage Apparatus</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Add Apparatus</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {apparatus.length > 0 ? (
                            apparatus.map((item) => (
                                <tr key={item.id}>
                                    <td>{item.id}</td>
                                    <td><strong>{item.type || item.name}</strong></td>
                                    <td>
                                        <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(item.id, item.type || item.name)}>Delete</button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="3" style={{textAlign: 'center', padding: '2rem'}}>No apparatus found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Apparatus">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Apparatus Type Name (e.g. Hoop, Ball)</label>
                        <input type="text" name="type" value={formData.type} onChange={handleChange} required />
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

export default AdminApparatus;
