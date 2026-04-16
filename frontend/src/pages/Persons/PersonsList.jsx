import React, { useState, useEffect } from 'react';
import PersonService from '../../services/person.service';
import { NavLink } from 'react-router-dom';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';

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
            setPersons(data.items || data);
        } catch (error) {
            toast.error(error.message || "Failed to load persons");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Delete person "${name}"?`)) return;
        try {
            await PersonService.delete(id);
            toast.success("Person deleted");
            setPersons(persons.filter(p => p.id !== id));
        } catch (error) {
            toast.error("Failed to delete person");
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
            toast.success("Person created");
            setPersons([...persons, data]);
            setIsModalOpen(false);
            setFormData({ name: '', surname: '', country: '', dateOfBirth: '', gender: 0, type: 'Person', mentorId: '' });
        } catch (error) {
            toast.error("Failed to create person");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (<div className="page-container">
        <div className="page-header flex-between">
            <h1 className="page-title">Persons Directory</h1>
            <div>
                <button className="btn btn-outline" style={{ marginRight: '1rem' }} onClick={loadPersons}>Refresh</button>
                {canEdit && <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Add Person</button>}
            </div>
        </div>

        <div className="glass-panel table-container">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Country</th>
                        <th>Age</th>
                        <th>Type/Role</th>
                        <th>Actions</th>
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
                                    <td>{age} years</td>
                                    <td>
                                        <span className={`status-badge ${person.type === 'Judge' ? 'status-planned' : 'status-ongoing'}`}>
                                            {person.type}
                                        </span>
                                    </td>
                                    <td>
                                        <NavLink to={`/persons/${person.id}`} className="btn btn-outline" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem' }}>
                                            Profile
                                        </NavLink>
                                        {canEdit && (
                                            <button className="btn btn-danger" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem' }} onClick={() => handleDelete(person.id, person.name)}>Delete</button>
                                        )}
                                    </td>
                                </tr>
                            );
                        })
                    ) : (
                        <tr>
                            <td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>No persons found.</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>

        <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Person">
            <form onSubmit={handleCreate}>
                <div className="form-group">
                    <label>First Name</label>
                    <input type="text" name="name" value={formData.name} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Last Name</label>
                    <input type="text" name="surname" value={formData.surname} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Country</label>
                    <input type="text" name="country" value={formData.country} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Date of Birth</label>
                    <input type="date" name="dateOfBirth" value={formData.dateOfBirth} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Gender</label>
                    <select name="gender" value={formData.gender} onChange={handleChange}>
                        <option value={0}>Male</option>
                        <option value={1}>Female</option>
                    </select>
                </div>
                <div className="form-group">
                    <label>Type / Role</label>
                    <select name="type" value={formData.type} onChange={handleChange}>
                        <option value="Person">Person / Gymnast</option>
                        <option value="Judge">Judge</option>
                    </select>
                </div>
                <div className="modal-footer">
                    <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Cancel</button>
                    <button type="submit" className="btn btn-primary">Create Person</button>
                </div>
            </form>
        </Modal>
    </div>
    );
};

export default PersonsList;
