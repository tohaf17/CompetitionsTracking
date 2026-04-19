import React, { useState, useEffect, useCallback } from 'react';
import AppealService from '../../services/appeal.service';
import ResultService from '../../services/result.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const AppealsList = () => {
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';

    const [appeals, setAppeals] = useState([]);
    const [resultsData, setResultsData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [viewMode, setViewMode] = useState('pending'); // 'pending' або 'all'
    
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [formData, setFormData] = useState({ resultId: '', reason: '' });

    const loadAppeals = useCallback(async () => {
        try {
            setLoading(true);
            const data = viewMode === 'pending' 
                ? await AppealService.getPending() 
                : await AppealService.getAll();
            setAppeals(unwrapCollection(data)); 
        } catch (error) {
            toastError(error, 'Не вдалося завантажити апеляції');
        } finally {
            setLoading(false);
        }
    }, [viewMode]);

    useEffect(() => {
        void loadAppeals();
    }, [loadAppeals]);

    const loadResults = async () => {
        try {
            const data = await ResultService.getAll();
            setResultsData(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити результати');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm(`Видалити апеляцію з ID ${id}?`)) return;
        try {
            await AppealService.delete(id);
            toast.success("Апеляцію видалено");
            setAppeals(appeals.filter(a => a.id !== id));
        } catch (error) {
            toastError(error, 'Не вдалося видалити апеляцію');
        }
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                resultId: parseInt(formData.resultId),
                reason: formData.reason,
                status: 0, // На розгляді
                createdAt: new Date().toISOString(),
                resolvedAt: new Date(new Date().setHours(new Date().getHours() + 24)).toISOString()
            };
            const data = await AppealService.create(payload);
            toast.success("Апеляцію подано");
            setAppeals([...appeals, data]);
            setIsModalOpen(false);
            setFormData({ resultId: '', reason: '' });
        } catch (error) {
            toastError(error, 'Не вдалося подати апеляцію');
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <div>
                    <h1 className="page-title">Реєстр апеляцій</h1>
                    <div style={{marginTop: '0.5rem', display: 'flex', gap: '0.5rem'}}>
                        <button className={`btn ${viewMode === 'pending' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setViewMode('pending')}>На розгляді</button>
                        <button className={`btn ${viewMode === 'all' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setViewMode('all')}>Усі апеляції</button>
                    </div>
                </div>
                <div>
                    <button className="btn btn-outline" style={{marginRight: '1rem'}} onClick={loadAppeals}>Оновити</button>
                    {isAdmin && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadResults();
                    }}>Подати апеляцію</button>}
                </div>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>ID результату</th>
                            <th>Учасник</th>
                            <th>Статус / Рішення</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {appeals.length > 0 ? (
                            appeals.map((appeal) => (
                                <tr key={appeal.id}>
                                    <td>{appeal.id}</td>
                                    <td>{appeal.resultId}</td>
                                    <td>{appeal.participantName || '-'}</td>
                                    <td>
                                         <span className={`status-badge ${appeal.status === 0 ? 'status-upcoming' : (appeal.status === 1 ? 'status-active' : 'status-completed')}`}>
                                            {appeal.status === 0 ? 'На розгляді' : (appeal.status === 1 ? 'Схвалено' : 'Відхилено')}
                                        </span>
                                    </td>
                                    <td>
                                        {isAdmin && (
                                            <button className="btn btn-danger" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}} onClick={() => handleDelete(appeal.id)}>Видалити</button>
                                        )}
                                        {!isAdmin && <span style={{ color: 'var(--text-muted)' }}>Немає дій</span>}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="5" style={{textAlign: 'center', padding: '2rem'}}>Апеляцій не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Подати апеляцію">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Оберіть ID результату</label>
                        <select name="resultId" value={formData.resultId} onChange={handleChange} required>
                            <option value="">-- Оберіть результат --</option>
                            {resultsData.map(r => <option key={r.id} value={r.id}>ID результату: {r.id} | Оцінка: {r.finalScore}</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Підстава для апеляції</label>
                        <textarea 
                            name="reason" 
                            value={formData.reason} 
                            onChange={handleChange} 
                            required 
                            placeholder="Опишіть причину оскарження оцінки"
                            style={{width: '100%', padding: '0.5rem', background: 'var(--surface-color)', color: '#fff', border: '1px solid var(--surface-border)'}}
                            rows={4}
                        />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Подати апеляцію</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default AppealsList;




