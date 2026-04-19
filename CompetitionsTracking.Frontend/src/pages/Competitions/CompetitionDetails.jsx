import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import CompetitionService from '../../services/competition.service';
import ResultService from '../../services/result.service';
import EntryService from '../../services/entry.service';
import ScoreService from '../../services/score.service';
import DisciplineService from '../../services/discipline.service';
import CategoryService from '../../services/category.service';
import { useAuth } from '../../context/AuthContext';
import toast from 'react-hot-toast';

const CompetitionDetails = () => {
    const { id } = useParams();
    const { user } = useAuth();
    const isAdmin = user?.role === 'Admin';

    const [competition, setCompetition] = useState(null);
    const [leaderboard, setLeaderboard] = useState([]);
    const [teamTally, setTeamTally] = useState([]);
    const [anomalies, setAnomalies] = useState([]);

    // Фільтри для таблиці результатів
    const [disciplines, setDisciplines] = useState([]);
    const [categories, setCategories] = useState([]);
    const [filter, setFilter] = useState({ disciplineId: '', categoryId: '' });

    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('leaderboard'); // leaderboard, tally, anomalies

    useEffect(() => {
        loadData();
    }, [id]);

    useEffect(() => {
        if (activeTab === 'leaderboard') loadLeaderboard();
        else if (activeTab === 'tally') loadTally();
        else if (activeTab === 'anomalies') loadAnomalies();
    }, [activeTab, filter]);

    const loadData = async () => {
        try {
            setLoading(true);
            const [compData, discData, catData] = await Promise.all([
                CompetitionService.getById(id),
                DisciplineService.getAll(),
                CategoryService.getAll()
            ]);
            setCompetition(compData);
            setDisciplines(discData.items || discData);
            setCategories(catData.items || catData);
        } catch (error) {
            toast.error("Не вдалося завантажити деталі змагання");
        } finally {
            setLoading(false);
        }
    };

    const loadLeaderboard = async () => {
        try {
            const data = await ResultService.getLeaderboard(id, filter.disciplineId, filter.categoryId);
            setLeaderboard(data);
        } catch (error) { }
    };

    const loadTally = async () => {
        try {
            const data = await ResultService.getTeamMedalTally(id);
            setTeamTally(data);
        } catch (error) { }
    };

    const loadAnomalies = async () => {
        try {
            const data = await ScoreService.getScoreAnomalies(id);
            setAnomalies(data);
        } catch (error) { }
    };

    const handleAwardMedals = async () => {
        try {
            await CompetitionService.awardMedals(id);
            toast.success("Медалі успішно нараховано");
            loadLeaderboard();
        } catch (error) {
            toast.error("Не вдалося нарахувати медалі");
        }
    };

    if (loading || !competition) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between mb-2">
                <div>
                    <h1 className="page-title">{competition.title}</h1>
                    <p style={{ color: 'var(--text-muted)' }}>{competition.city} | {new Date(competition.startDate).toLocaleDateString('uk-UA')} - {new Date(competition.endDate).toLocaleDateString('uk-UA')}</p>
                </div>
                {isAdmin && (
                    <button className="btn btn-primary" onClick={handleAwardMedals}>Нарахувати медалі</button>
                )}
            </div>

            <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem' }}>
                <button className={`btn ${activeTab === 'leaderboard' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('leaderboard')}>Таблиця результатів</button>
                <button className={`btn ${activeTab === 'tally' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('tally')}>Медальний залік команд</button>
                {isAdmin && <button className={`btn ${activeTab === 'anomalies' ? 'btn-danger' : 'btn-outline'}`} onClick={() => setActiveTab('anomalies')}>Аномалії оцінок</button>}
            </div>

            {activeTab === 'leaderboard' && (
                <div className="glass-panel">
                    <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                        <select className="form-group" style={{ padding: '0.4rem' }} onChange={e => setFilter({ ...filter, disciplineId: e.target.value })}>
                            <option value="">Усі дисципліни</option>
                            {disciplines.map(d => <option key={d.id} value={d.id}>{d.type}</option>)}
                        </select>
                        <select className="form-group" style={{ padding: '0.4rem' }} onChange={e => setFilter({ ...filter, categoryId: e.target.value })}>
                            <option value="">Усі категорії</option>
                            {categories.map(c => <option key={c.id} value={c.id}>{c.type}</option>)}
                        </select>
                    </div>

                    <table style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>Учасник / Команда</th>
                                <th>Дисципліна</th>
                                <th>Категорія</th>
                                <th>Фінальна оцінка</th>
                                <th>Місце (Медаль)</th>
                            </tr>
                        </thead>
                        <tbody>
                            {leaderboard.length > 0 ? leaderboard.map((lb, i) => (
                                <tr key={i} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}><strong>{lb.participantName}</strong></td>
                                    <td>{lb.disciplineName}</td>
                                    <td>{lb.categoryName}</td>
                                    <td><span className="status-badge status-active">{lb.finalScore.toFixed(2)}</span></td>
                                    <td>
                                        {lb.place} {lb.awardedMedal && <span style={{ marginLeft: '0.5rem' }}>🏅 {lb.awardedMedal}</span>}
                                    </td>
                                </tr>
                            )) : <tr><td colSpan="5" style={{ padding: '1rem', textAlign: 'center' }}>Немає результатів за вибраними фільтрами.</td></tr>}
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
                                <th style={{ padding: '0.8rem' }}>Назва команди</th>
                                <th style={{ color: '#FFD700' }}>Золото</th>
                                <th style={{ color: '#C0C0C0' }}>Срібло</th>
                                <th style={{ color: '#CD7F32' }}>Бронза</th>
                                <th>Разом</th>
                            </tr>
                        </thead>
                        <tbody>
                            {teamTally.map((t, i) => (
                                <tr key={i} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}>{t.teamName}</td>
                                    <td>{t.goldCount}</td>
                                    <td>{t.silverCount}</td>
                                    <td>{t.bronzeCount}</td>
                                    <td><strong>{t.goldCount + t.silverCount + t.bronzeCount}</strong></td>
                                </tr>
                            ))}
                            {teamTally.length === 0 && <tr><td colSpan="5" style={{ padding: '1rem', textAlign: 'center' }}>Медалей ще не нараховано.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            {activeTab === 'anomalies' && isAdmin && (
                <div className="glass-panel">
                    <h3 style={{ color: '#ff4d4f' }}>Підозрілі оцінки (відхилення &gt; 2 СКВ)</h3>
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>Ім&apos;я учасника</th>
                                <th>Суддя</th>
                                <th>Тип оцінки</th>
                                <th>Виставлена оцінка</th>
                                <th>Відхилення</th>
                            </tr>
                        </thead>
                        <tbody>
                            {anomalies.map((a, i) => (
                                <tr key={i} style={{ borderBottom: '1px solid var(--surface-border)' }}>
                                    <td style={{ padding: '0.8rem' }}>{a.participantName}</td>
                                    <td>{a.judgeName}</td>
                                    <td>{a.scoreType}</td>
                                    <td><strong>{a.givenScore.toFixed(2)}</strong></td>
                                    <td style={{ color: '#ff4d4f' }}>{a.deviation > 0 ? '+' : ''}{a.deviation.toFixed(2)}</td>
                                </tr>
                            ))}
                            {anomalies.length === 0 && <tr><td colSpan="5" style={{ padding: '1rem', textAlign: 'center' }}>Аномалій не виявлено.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default CompetitionDetails;
