import React, { useState, useEffect, useCallback } from 'react';
import AppealService from '../../services/appeal.service';
import ResultService from '../../services/result.service';
import CompetitionService from '../../services/competition.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import Modal from '../../components/UI/Modal';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';
import { toastError } from '../../utils/toastError';

const AppealsList = () => {
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';
    const isCoach = user?.role === 'Trainee';
    const canSubmitAppeal = isAdmin || user?.role === 'Trainee';

    const [appeals, setAppeals] = useState([]);
    const [resultsData, setResultsData] = useState([]);
    const [competitions, setCompetitions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [viewMode, setViewMode] = useState('pending'); // 'pending' або 'all'
    const [selectedCompetitionId, setSelectedCompetitionId] = useState('');

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isDossierModalOpen, setIsDossierModalOpen] = useState(false);
    const [dossier, setDossier] = useState(null);
    const [approvalData, setApprovalData] = useState({ scoreIdToEdit: '', newScoreValue: '' });
    const [formData, setFormData] = useState({ resultId: '', reason: '' });

    const [profileError, setProfileError] = useState(false);

    const loadAppeals = useCallback(async () => {
        try {
            setLoading(true);
            setProfileError(false);
            const data = viewMode === 'pending'
                ? await AppealService.getPending()
                : await AppealService.getAll();
            setAppeals(unwrapCollection(data));
        } catch (error) {
            if (error?.response?.data?.message?.includes('прив\'язано профіль тренера')) {
                setProfileError(true);
            } else {
                toastError(error, 'Не вдалося завантажити апеляції');
            }
        } finally {
            setLoading(false);
        }
    }, [viewMode]);

    useEffect(() => {
        void loadAppeals();
        void loadCompetitions();
    }, [loadAppeals]);

    const loadCompetitions = async () => {
        try {
            const data = await CompetitionService.getAll();
            setCompetitions(unwrapCollection(data));
        } catch (error) {
            console.error('Failed to load competitions', error);
        }
    };

    const loadResults = async () => {
        try {
            const data = await ResultService.getAppealable();
            setResultsData(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити результати');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm(`Видалити апеляцію?`)) return;
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
            const selectedResult = resultsData.find(r => r.id === parseInt(formData.resultId));

            if (!isAdmin && selectedResult?.competitionLevel === 2) {
                toast.error("Тренер не може подавати апеляції на міжнародні змагання");
                return;
            }

            if (selectedResult && selectedResult.competitionStatus !== 2) {
                toast.error("Апеляції можна подавати лише для змагань, що тривають");
                return;
            }

            const payload = {
                resultId: parseInt(formData.resultId),
                reason: formData.reason,
                status: 0, // На розгляді
                createdAt: new Date().toISOString(),
                resolvedAt: new Date(new Date().setHours(new Date().getHours() + 24)).toISOString()
            };
            const data = await AppealService.create(payload);
            toast.success("Апеляцію подано");
            void loadAppeals();
            setIsModalOpen(false);
            setFormData({ resultId: '', reason: '' });
        } catch (error) {
            toastError(error, 'Не вдалося подати апеляцію');
        }
    };

    const handleViewDossier = async (id) => {
        try {
            const data = await AppealService.getDossier(id);
            setDossier(data);
            setIsDossierModalOpen(true);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити досьє апеляції');
        }
    };

    const handleApprove = async () => {
        if (!approvalData.scoreIdToEdit || !approvalData.newScoreValue) {
            toast.error("Будь ласка, оберіть оцінку та введіть нове значення");
            return;
        }
        try {
            await AppealService.approve(dossier.appealId, {
                scoreIdToEdit: parseInt(approvalData.scoreIdToEdit),
                newScoreValue: parseFloat(approvalData.newScoreValue)
            });
            toast.success("Апеляцію схвалено, результати перераховано");
            setIsDossierModalOpen(false);
            void loadAppeals();
        } catch (error) {
            toastError(error, 'Не вдалося схвалити апеляцію');
        }
    };

    const handleReject = async () => {
        try {
            await AppealService.update(dossier.appealId, { status: 2, reason: dossier.reason }); // 2 = Rejected
            toast.success("Апеляцію відхилено");
            setIsDossierModalOpen(false);
            void loadAppeals();
        } catch (error) {
            toastError(error, 'Не вдалося відхилити апеляцію');
        }
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    if (profileError) {
        return (
            <div className="page-container">
                <div className="glass-panel" style={{ padding: '3rem', textAlign: 'center', marginTop: '2rem' }}>
                    <h2 style={{ color: '#f59e0b', marginBottom: '1rem' }}>Профіль тренера не знайдено</h2>
                    <p style={{ marginBottom: '2rem' }}>
                        Для перегляду та подачі апеляцій ваш акаунт має бути прив&apos;язаний до профілю тренера.<br/>
                        Ми вже запустили процес автоматичного відновлення профілів. Будь ласка, спробуйте оновити сторінку або зверніться до адміністратора.
                    </p>
                    <button className="btn btn-primary" onClick={loadAppeals}>Оновити</button>
                </div>
            </div>
        );
    }

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <div>
                    <h1 className="page-title">Реєстр апеляцій</h1>
                    <div style={{ marginTop: '0.5rem', display: 'flex', gap: '0.5rem' }}>
                        <button className={`btn ${viewMode === 'pending' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setViewMode('pending')}>На розгляді</button>
                        <button className={`btn ${viewMode === 'all' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setViewMode('all')}>Усі апеляції</button>
                    </div>
                    <div style={{ marginTop: '0.5rem' }}>
                        <select
                            className="form-control"
                            style={{ padding: '0.4rem', borderRadius: '4px', background: 'var(--surface-color)', color: '#fff', border: '1px solid var(--surface-border)' }}
                            value={selectedCompetitionId}
                            onChange={(e) => setSelectedCompetitionId(e.target.value)}
                        >
                            <option value="">Усі змагання</option>
                            {competitions.map(c => <option key={c.id} value={c.id}>{c.title}</option>)}
                        </select>
                    </div>
                </div>
                <div>
                    <button className="btn btn-outline" style={{ marginRight: '1rem' }} onClick={loadAppeals}>Оновити</button>
                    {canSubmitAppeal && <button className="btn btn-primary" onClick={() => {
                        setIsModalOpen(true);
                        loadResults();
                    }}>Подати апеляцію</button>}
                </div>
            </div>

            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Виступ</th>
                            <th>Учасник</th>
                            <th>Змагання</th>
                            <th>Статус / Рішення</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {appeals
                            .filter(a => !selectedCompetitionId || a.competitionId === parseInt(selectedCompetitionId))
                            .length > 0 ? (
                            appeals
                                .filter(a => !selectedCompetitionId || a.competitionId === parseInt(selectedCompetitionId))
                                .map((appeal, index) => (
                                    <tr key={appeal.id}>
                                        <td>{index + 1}</td>
                                        <td>Виступ {index + 1}</td>
                                        <td><strong>{appeal.participantName || '-'}</strong></td>
                                        <td>{appeal.competitionName || '-'}</td>
                                        <td>
                                            <span className={`status-badge ${appeal.status === 0 ? 'status-upcoming' : (appeal.status === 1 ? 'status-active' : 'status-completed')}`}>
                                                {appeal.status === 0 ? 'На розгляді' : (appeal.status === 1 ? 'Схвалено' : 'Відхилено')}
                                            </span>
                                        </td>
                                        <td>
                                            {isAdmin && appeal.status === 0 && (
                                                <button className="btn btn-primary" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem', marginRight: '0.5rem' }} onClick={() => handleViewDossier(appeal.id)}>Розглянути</button>
                                            )}
                                            {isAdmin && (
                                                <button className="btn btn-danger" style={{ padding: '0.3rem 0.6rem', fontSize: '0.8rem' }} onClick={() => handleDelete(appeal.id)}>Видалити</button>
                                            )}
                                            {!isAdmin && <span style={{ color: 'var(--text-muted)' }}>Немає дій</span>}
                                        </td>
                                    </tr>
                                ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>Апеляцій не знайдено для вибраних критеріїв.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} title="Подати апеляцію">
                <form onSubmit={handleCreate}>
                    <div className="form-group">
                        <label>Оберіть виступ</label>
                        <select name="resultId" value={formData.resultId} onChange={handleChange} required>
                            <option value="">-- Оберіть результат --</option>
                            {resultsData.map(r => (
                                <option key={r.id} value={r.id}>
                                    {r.participantName} - {r.competitionName}, виступ #{r.entryId}, оцінка: {Number(r.finalScore).toFixed(2)}
                                </option>
                            ))}
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
                            style={{ width: '100%', padding: '0.5rem', background: 'var(--surface-color)', color: '#fff', border: '1px solid var(--surface-border)' }}
                            rows={4}
                        />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Подати апеляцію</button>
                    </div>
                </form>
            </Modal>
            <Modal isOpen={isDossierModalOpen} onClose={() => setIsDossierModalOpen(false)} title="Розгляд апеляції">
                {dossier ? (
                    <div>
                        <p><strong>Причина:</strong> {dossier.reason}</p>
                        <p><strong>Поточний бал:</strong> {dossier.finalScore.toFixed(2)}</p>

                        <div style={{ marginTop: '1rem' }}>
                            <h4>Оцінки суддів:</h4>
                            <table style={{ width: '100%', textAlign: 'left', marginTop: '0.5rem' }}>
                                <thead>
                                    <tr>
                                        <th>Тип</th>
                                        <th>Суддя</th>
                                        <th>Бал</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {dossier.scores.map(s => (
                                        <tr key={s.scoreId}>
                                            <td>{s.scoreType}</td>
                                            <td>{s.judgeName}</td>
                                            <td>{s.value.toFixed(2)}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>

                        {dossier.status === 0 && (
                            <div className="glass-panel" style={{ marginTop: '1.5rem', padding: '1rem' }}>
                                <h4>Прийняти рішення:</h4>
                                <div className="form-group">
                                    <label>Оцінка для коригування</label>
                                    <select
                                        value={approvalData.scoreIdToEdit}
                                        onChange={e => setApprovalData({ ...approvalData, scoreIdToEdit: e.target.value })}
                                    >
                                        <option value="">-- Оберіть оцінку --</option>
                                        {dossier.scores.map(s => <option key={s.scoreId} value={s.scoreId}>{s.scoreType} - {s.judgeName}</option>)}
                                    </select>
                                </div>
                                <div className="form-group">
                                    <label>Нове значення балу</label>
                                    <input
                                        type="number"
                                        step="0.01"
                                        value={approvalData.newScoreValue}
                                        onChange={e => setApprovalData({ ...approvalData, newScoreValue: e.target.value })}
                                    />
                                </div>
                                <div className="flex-between" style={{ marginTop: '1rem' }}>
                                    <button className="btn btn-danger" onClick={handleReject}>Відхилити апеляцію</button>
                                    <button className="btn btn-primary" onClick={handleApprove}>Схвалити та оновити бал</button>
                                </div>
                            </div>
                        )}
                    </div>
                ) : <p>Завантаження...</p>}
                <div className="modal-footer">
                    <button className="btn btn-outline" onClick={() => setIsDossierModalOpen(false)}>Закрити</button>
                </div>
            </Modal>
        </div>
    );

};
export default AppealsList;
