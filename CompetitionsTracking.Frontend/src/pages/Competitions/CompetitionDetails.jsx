import React, { useState, useEffect, useCallback } from 'react';
import { useParams, Link } from 'react-router-dom';
import CompetitionService from '../../services/competition.service';
import ResultService from '../../services/result.service';
import ScoreService from '../../services/score.service';
import DisciplineService from '../../services/discipline.service';
import CategoryService from '../../services/category.service';
import EntryService from '../../services/entry.service';
import JudgeService from '../../services/judge.service';
import ApparatusService from '../../services/apparatus.service';
import { useAuth } from '../../context/AuthContext';
import { unwrapCollection } from '../../utils/unwrapCollection';
import { toastError } from '../../utils/toastError';
import Modal from '../../components/UI/Modal';
import toast from 'react-hot-toast';

const CompetitionDetails = () => {
    const { id } = useParams();
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';
    const isCoach = user?.role === 'Trainee';
    const canViewEntries = isAdmin || isCoach;
    const levelMap = {
        0: 'Локальне',
        1: 'Національне',
        2: 'Міжнародне'
    };

    const [competition, setCompetition] = useState(null);
    const [leaderboard, setLeaderboard] = useState([]);
    const [teamTally, setTeamTally] = useState([]);
    const [anomalies, setAnomalies] = useState([]);
    const [entries, setEntries] = useState([]);

    const [disciplines, setDisciplines] = useState([]);
    const [categories, setCategories] = useState([]);
    const [apparatuses, setApparatuses] = useState([]);
    const [filter, setFilter] = useState({ apparatusId: '', categoryId: '' });
    const [performanceType, setPerformanceType] = useState('');

    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('leaderboard'); 

    const [isScoreModalOpen, setIsScoreModalOpen] = useState(false);
    const [isBreakdownModalOpen, setIsBreakdownModalOpen] = useState(false);
    const [scoreBreakdown, setScoreBreakdown] = useState(null);
    const [selectedEntry, setSelectedEntry] = useState(null);
    const [judges, setJudges] = useState([]);
    const [scoreData, setScoreData] = useState({ judgeId: '', scoreType: 'DA', value: '' });

    const loadData = useCallback(async () => {
        try {
            setLoading(true);
            const [compData, discData, catData, appData] = await Promise.all([
                CompetitionService.getById(id),
                DisciplineService.getAll(),
                CategoryService.getAll(),
                ApparatusService.getAll()
            ]);
            setCompetition(compData);
            setDisciplines(unwrapCollection(discData));
            setCategories(unwrapCollection(catData));
            setApparatuses(unwrapCollection(appData));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити деталі змагання');
        } finally {
            setLoading(false);
        }
    }, [id]);

    const loadLeaderboard = useCallback(async () => {
        try {
            let computedDiscId = '';
            if (performanceType && filter.apparatusId) {
                const matchedDisc = disciplines.find(d => d.type.startsWith(performanceType) && d.apparatusId === parseInt(filter.apparatusId));
                computedDiscId = matchedDisc ? matchedDisc.id : -1;
            }
            const data = await ResultService.getLeaderboard(id, computedDiscId, filter.categoryId);
            let results = unwrapCollection(data);

            if (performanceType && !filter.apparatusId) {
                results = results.filter(r => r.disciplineName && r.disciplineName.startsWith(performanceType));
            }

            setLeaderboard(results);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити таблицю результатів');
        }
    }, [filter.categoryId, filter.apparatusId, performanceType, disciplines, id]);

    const loadTally = useCallback(async () => {
        try {
            const data = await ResultService.getTeamMedalTally(id);
            setTeamTally(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити медальний залік');
        }
    }, [id]);

    const loadAnomalies = useCallback(async () => {
        try {
            const data = await ScoreService.getScoreAnomalies(id);
            setAnomalies(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити аномалії оцінок');
        }
    }, [id]);

    const loadEntries = useCallback(async () => {
        try {
            const data = await EntryService.getByCompetition(id);
            setEntries(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити список заявок');
        }
    }, [id]);

    useEffect(() => {
        void loadData();
    }, [loadData]);

    useEffect(() => {
        if (activeTab === 'leaderboard') void loadLeaderboard();
        else if (activeTab === 'tally') void loadTally();
        else if (activeTab === 'anomalies') void loadAnomalies();
        else if (activeTab === 'entries') void loadEntries();
    }, [activeTab, loadAnomalies, loadLeaderboard, loadTally, loadEntries]);

    const handleAwardMedals = async () => {
        try {
            await CompetitionService.awardMedals(id);
            toast.success("Медалі успішно нараховано");
            if (activeTab === 'leaderboard') void loadLeaderboard();
            if (activeTab === 'tally') void loadTally();
        } catch (error) {
            toastError(error, 'Не вдалося нарахувати медалі');
        }
    };

    const handleScoreSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                entryId: selectedEntry.id,
                judgeId: parseInt(scoreData.judgeId),
                type: scoreData.scoreType,
                scoreValue: parseFloat(scoreData.value)
            };
            await ScoreService.create(payload);
            toast.success("Оцінку успішно виставлено");
            setIsScoreModalOpen(false);
            setScoreData({ judgeId: '', scoreType: 'DA', value: '' });
        } catch (error) {
            toastError(error, 'Не вдалося виставити оцінку');
        }
    };

    const handleDeleteEntry = async (entryId) => {
        if (!window.confirm("Видалити цю заявку?")) return;
        try {
            await EntryService.delete(entryId);
            toast.success("Заявку видалено");
            loadEntries();
        } catch (error) {
            toastError(error, 'Не вдалося видалити заявку');
        }
    };

    const openScoreModal = async (entry) => {
        setSelectedEntry(entry);
        try {
            const res = await JudgeService.getAll();
            setJudges(unwrapCollection(res));
            setIsScoreModalOpen(true);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити список суддів');
        }
    };

    const handleViewBreakdown = async (entry) => {
        try {
            setSelectedEntry(entry);
            const data = await ScoreService.getEntryScoreBreakdown(entry.entryId);
            setScoreBreakdown(data);
            setIsBreakdownModalOpen(true);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити деталізацію оцінок');
        }
    };

    const getApplicationStatusText = (status) => {
        switch (status) {
            case 0: return 'Очікує';
            case 1: return 'Прийнято';
            case 2: return 'Відхилено';
            case 3: return 'Повторно подано';
            default: return status;
        }
    };

    if (loading || !competition) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between mb-2">
                <div>
                    <h1 className="page-title">{competition.title}</h1>
                    <p style={{ color: 'var(--text-muted)' }}>
                        {levelMap[competition.level] || 'Невідомий тип'} | {competition.country ? `${competition.country}, ` : ''}{competition.city} | {new Date(competition.startDate).toLocaleDateString('uk-UA')} - {new Date(competition.endDate).toLocaleDateString('uk-UA')}
                    </p>
                </div>
                {isAdmin && (
                    <button className="btn btn-primary" onClick={handleAwardMedals}>Нарахувати медалі</button>
                )}
            </div>

            <div className="flex gap-2 mb-2" style={{overflowX: 'auto', paddingBottom: '0.5rem'}}>
                <button className={`btn ${activeTab === 'leaderboard' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('leaderboard')}>Таблиця результатів</button>
                <button className={`btn ${activeTab === 'tally' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('tally')}>Медальний залік команд</button>
                {canViewEntries && <button className={`btn ${activeTab === 'entries' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('entries')}>Список заявок</button>}
                {isAdmin && <button className={`btn ${activeTab === 'anomalies' ? 'btn-danger' : 'btn-outline'}`} onClick={() => setActiveTab('anomalies')}>Аномалії оцінок</button>}
            </div>

            {activeTab === 'leaderboard' && (
                <div className="glass-panel">
                    <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                        <select 
                            className="form-group" 
                            style={{ padding: '0.4rem' }} 
                            value={performanceType}
                            onChange={e => {
                                setPerformanceType(e.target.value);
                                setFilter({ ...filter, apparatusId: '' });
                            }}
                        >
                            <option value="">Усі типи виступів</option>
                            <option value="Індивідуальна">Індивідуальна</option>
                            <option value="Групова">Групова</option>
                        </select>

                        <select 
                            className="form-group" 
                            style={{ padding: '0.4rem' }} 
                            value={filter.apparatusId}
                            onChange={e => setFilter({ ...filter, apparatusId: e.target.value })}
                            disabled={!performanceType}
                        >
                            <option value="">Усі предмети</option>
                            {apparatuses.map(a => (
                                <option key={a.id} value={a.id}>{a.type}</option>
                            ))}
                        </select>

                        <select className="form-group" style={{ padding: '0.4rem' }} onChange={e => setFilter({ ...filter, categoryId: e.target.value })}>
                            <option value="">Усі категорії</option>
                            {categories.map(c => <option key={c.id} value={c.id}>{c.type}</option>)}
                        </select>
                    </div>

                    <table style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>№</th>
                                <th>Учасник / Команда</th>
                                <th>Дисципліна</th>
                                <th>Категорія</th>
                                <th>Фінальна оцінка</th>
                                <th>Місце (Медаль)</th>
                            </tr>
                        </thead>
                        <tbody>
                            {leaderboard.length > 0 ? leaderboard.map((lb, i) => (
                                <tr key={i} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}>{i + 1}</td>
                                    <td>
                                        {lb.participantId ? (
                                            <Link to={lb.participantType === 'Team' ? `/teams/${lb.participantId}` : `/persons/${lb.participantId}`} style={{ color: 'var(--text-color)', textDecoration: 'none', fontWeight: 600 }}
                                                onMouseEnter={e => e.currentTarget.style.color = 'var(--primary-color)'}
                                                onMouseLeave={e => e.currentTarget.style.color = 'var(--text-color)'}
                                            >
                                                {lb.participantName}
                                            </Link>
                                        ) : (
                                            <strong>{lb.participantName}</strong>
                                        )}
                                    </td>
                                    <td>{lb.disciplineName}</td>
                                    <td>{lb.categoryName}</td>
                                    <td><span className="status-badge status-active">{lb.finalScore.toFixed(2)}</span></td>
                                    <td>
                                        {lb.place} {lb.awardedMedal && <span style={{ marginLeft: '0.5rem' }}>🏅 {lb.awardedMedal}</span>}
                                        <button 
                                            className="btn btn-outline" 
                                            style={{ marginLeft: '1rem', padding: '0.2rem 0.5rem', fontSize: '0.7rem' }}
                                            onClick={() => handleViewBreakdown(lb)}
                                        >
                                            Деталі
                                        </button>
                                    </td>
                                </tr>
                            )) : <tr><td colSpan="6" style={{ padding: '1rem', textAlign: 'center' }}>Немає результатів за вибраними фільтрами.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            {activeTab === 'tally' && (
                <div className="glass-panel">
                    <h3>Медальний залік по командах</h3>
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>№</th>
                                <th>Назва команди</th>
                                <th style={{ color: '#FFD700' }}>Золото</th>
                                <th style={{ color: '#C0C0C0' }}>Срібло</th>
                                <th style={{ color: '#CD7F32' }}>Бронза</th>
                                <th style={{ textAlign: 'right', paddingRight: '1rem' }}>Усього медалей</th>
                            </tr>
                        </thead>
                        <tbody>
                            {teamTally.map((t, i) => (
                                <tr key={i} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}>{i + 1}</td>
                                    <td style={{ padding: '0.8rem' }}>
                                        <Link to={`/teams/${t.teamId}`} style={{ color: 'var(--text-color)', textDecoration: 'none', fontWeight: 600 }}
                                            onMouseEnter={e => e.currentTarget.style.color = 'var(--primary-color)'}
                                            onMouseLeave={e => e.currentTarget.style.color = 'var(--text-color)'}
                                        >
                                            {t.teamName}
                                        </Link>
                                    </td>
                                    <td>{t.goldMedals}</td>
                                    <td>{t.silverMedals}</td>
                                    <td>{t.bronzeMedals}</td>
                                    <td style={{ textAlign: 'right', paddingRight: '1rem' }}><strong>{t.totalMedals}</strong></td>
                                </tr>
                            ))}
                            {teamTally.length === 0 && <tr><td colSpan="6" style={{ padding: '1rem', textAlign: 'center' }}>Медалей ще не нараховано.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            {canViewEntries && activeTab === 'entries' && (
                <div className="glass-panel">
                    <h3>Список заявок на змагання</h3>
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>№</th>
                                <th>Учасник</th>
                                <th>Команда</th>
                                <th>Дисципліна | Категорія</th>
                                <th>Статус</th>
                                {isAdmin && <th>Дії</th>}
                            </tr>
                        </thead>
                        <tbody>
                            {entries.length > 0 ? entries.map((e, i) => (
                                <tr key={e.id} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}>{i + 1}</td>
                                    <td>
                                        {e.participantId ? (
                                            <Link to={e.participantType === 'Team' ? `/teams/${e.participantId}` : `/persons/${e.participantId}`} style={{ color: 'var(--text-color)', textDecoration: 'none', fontWeight: 600 }}
                                                onMouseEnter={ev => ev.currentTarget.style.color = 'var(--primary-color)'}
                                                onMouseLeave={ev => ev.currentTarget.style.color = 'var(--text-color)'}
                                            >
                                                {e.participantName}
                                            </Link>
                                        ) : (
                                            <strong>{e.participantName}</strong>
                                        )}
                                    </td>
                                    <td>{e.teamName || '-'}</td>
                                    <td>{e.disciplineName} | {e.categoryName}</td>
                                    <td>
                                        <span className={`status-badge ${e.applicationStatus === 1 ? 'status-active' : 'status-upcoming'}`}>
                                            {getApplicationStatusText(e.applicationStatus)}
                                        </span>
                                    </td>
                                    {isAdmin && (
                                    <td>
                                        {e.applicationStatus === 1 && e.entryStatus === 0 && (
                                        <button 
                                            className="btn btn-primary" 
                                            style={{ padding: '0.2rem 0.5rem', fontSize: '0.75rem', marginRight: '0.5rem' }} 
                                            onClick={() => openScoreModal(e)}
                                        >
                                            Оцінити
                                        </button>
                                        )}
                                        <button 
                                            className="btn btn-danger" 
                                            style={{ padding: '0.2rem 0.5rem', fontSize: '0.75rem' }} 
                                            onClick={() => handleDeleteEntry(e.id)}
                                        >
                                            Видалити
                                        </button>
                                    </td>
                                    )}
                                </tr>
                            )) : <tr><td colSpan={isAdmin ? 6 : 5} style={{ padding: '1rem', textAlign: 'center' }}>Заявок не знайдено.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            {activeTab === 'anomalies' && isAdmin && (
                <div className="glass-panel">
                    <h3 style={{ color: '#ff4d4f' }}>Підозрілі оцінки (відхилення &gt;= 1.5)</h3>
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>№</th>
                                <th>Ім&apos;я учасника</th>
                                <th>Суддя</th>
                                <th>Тип оцінки</th>
                                <th>Виставлена оцінка</th>
                                <th>Відхилення</th>
                            </tr>
                        </thead>
                        <tbody>
                            {anomalies.map((a, i) => (
                                <tr key={i} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}>{i + 1}</td>
                                    <td style={{ padding: '0.8rem' }}>{a.participantName}</td>
                                    <td>{a.judgeName}</td>
                                    <td>{a.scoreType}</td>
                                    <td><strong>{a.scoreValue.toFixed(2)}</strong></td>
                                    <td style={{ color: '#ff4d4f' }}>{a.deviation > 0 ? '+' : ''}{a.deviation.toFixed(2)}</td>
                                </tr>
                            ))}
                            {anomalies.length === 0 && <tr><td colSpan="6" style={{ padding: '1rem', textAlign: 'center' }}>Аномалій не виявлено.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            <Modal isOpen={isBreakdownModalOpen} onClose={() => setIsBreakdownModalOpen(false)} title={`Деталізація оцінок: ${selectedEntry?.participantName}`}>
                {scoreBreakdown ? (
                    <div>
                        <div style={{ marginBottom: '1rem', padding: '1rem', background: 'rgba(255,255,255,0.05)', borderRadius: '8px' }}>
                            <p><strong>Фінальна оцінка:</strong> {scoreBreakdown.finalScore.toFixed(2)}</p>
                        </div>
                        <table style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                            <thead>
                                <tr style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <th style={{ padding: '0.5rem' }}>Тип</th>
                                    <th>Суддя</th>
                                    <th>Бал</th>
                                </tr>
                            </thead>
                            <tbody>
                                {scoreBreakdown.scores.map((s, idx) => (
                                    <tr key={idx} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                        <td style={{ padding: '0.5rem' }}>{s.scoreType}</td>
                                        <td>{s.judgeName}</td>
                                        <td><strong>{s.value.toFixed(2)}</strong></td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : <p>Завантаження...</p>}
                <div className="modal-footer">
                    <button className="btn btn-primary" onClick={() => setIsBreakdownModalOpen(false)}>Закрити</button>
                </div>
            </Modal>

            <Modal isOpen={isScoreModalOpen} onClose={() => setIsScoreModalOpen(false)} title={`Оцінити виступ: ${selectedEntry?.participantName}`}>
                <form onSubmit={handleScoreSubmit}>
                    <div className="form-group">
                        <label>Суддя</label>
                        <select 
                            value={scoreData.judgeId} 
                            onChange={(e) => setScoreData({...scoreData, judgeId: e.target.value})} 
                            required
                        >
                            <option value="">-- Оберіть суддю --</option>
                            {judges.map(j => <option key={j.id} value={j.id}>{j.fullName} (Квал: {j.qualificationLevel})</option>)}
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Тип оцінки</label>
                        <select 
                            value={scoreData.scoreType} 
                            onChange={(e) => setScoreData({...scoreData, scoreType: e.target.value})} 
                            required
                        >
                            <option value="DA">Складність тіла (DA)</option>
                            <option value="DB">Складність інвентарю (DB)</option>
                            <option value="A">Артистизм (A)</option>
                            <option value="E">Виконання (E)</option>
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Значення оцінки</label>
                        <input 
                            type="number" 
                            step="0.01" 
                            min="0" 
                            max="20" 
                            value={scoreData.value} 
                            onChange={(e) => setScoreData({...scoreData, value: e.target.value})} 
                            required 
                        />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => setIsScoreModalOpen(false)}>Скасувати</button>
                        <button type="submit" className="btn btn-primary">Виставити оцінку</button>
                    </div>
                </form>
            </Modal>
        </div>
    );
};

export default CompetitionDetails;
