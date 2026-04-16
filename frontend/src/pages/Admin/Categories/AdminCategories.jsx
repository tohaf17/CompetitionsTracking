import React, { useState, useEffect } from 'react';
import CategoryService from '../../../services/category.service';
import Modal from '../../../components/UI/Modal';
import toast from 'react-hot-toast';

const AdminCategories = () => {
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ type: '', minAge: '', maxAge: '' });

    useEffect(() => {
        loadCategories();
    }, []);

    const loadCategories = async () => {
        try {
            setLoading(true);
            const data = await CategoryService.getAll();
            setCategories(data.items || data);
        } catch (error) {
            toast.error("Failed to load categories");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id, name) => {
        if (!window.confirm(`Delete category "${name}"?`)) return;
        try {
            await CategoryService.delete(id);
            toast.success("Category deleted");
            setCategories(categories.filter(c => c.id !== id));
        } catch (error) {
            toast.error("Failed to delete category");
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const data = await CategoryService.create({ 
                type: formData.type, 
                minAge: parseInt(formData.minAge) || 0,
                maxAge: parseInt(formData.maxAge) || 99
            });
            toast.success("Category created");
            setCategories([...categories, data]);
            setIsModalOpen(false);
            setFormData({ type: '', minAge: '', maxAge: '' });
        } catch (error) {
            toast.error("Failed to create category");
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Manage Categories</h1>
                <button className="btn btn-primary" onClick={() => setIsModalOpen(true)}>Add Category</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Age Requirement</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {categories.length > 0 ? (
                            categories.map((item) => (
                                <tr key={item.id}>
                                    <td>{item.id}</td>
                                    <td><strong>{item.type}</strong></td>
                                    <td>{item.minAge} - {item.maxAge}</td>
                                    <td>
                                        <button className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}>Edit</button>
                                        <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginLeft: '0.5rem'}} onClick={() => handleDelete(item.id, item.type)}>Delete</button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="4" style={{textAlign: 'center', padding: '2rem'}}>No categories found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Create New Category">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Category Type Name (e.g. Juniors)</label>
                        <input type="text" name="type" value={formData.type} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Min Age</label>
                        <input type="number" name="minAge" value={formData.minAge} onChange={handleChange} required />
                    </div>
                    <div className="form-group">
                        <label>Max Age</label>
                        <input type="number" name="maxAge" value={formData.maxAge} onChange={handleChange} required />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Create Category</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AdminCategories;
