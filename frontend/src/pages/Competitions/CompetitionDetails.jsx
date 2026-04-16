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

    // Filters for leaderboard
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
            toast.error("Failed to load competition details");
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
            toast.success("Medals awarded successfully");
            loadLeaderboard();
        } catch (error) {
            toast.error("Failed to award medals");
        }
    };

    if (loading || !competition) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between mb-2">
                <div>
                    <h1 className="page-title">{competition.title}</h1>
                    <p style={{ color: 'var(--text-muted)' }}>{competition.city} | {new Date(competition.startDate).toLocaleDateString()} - {new Date(competition.endDate).toLocaleDateString()}</p>
                </div>
                {isAdmin && (
                    <button className="btn btn-primary" onClick={handleAwardMedals}>Award Medals</button>
                )}
            </div>

            <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem' }}>
                <button className={`btn ${activeTab === 'leaderboard' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('leaderboard')}>Leaderboard</button>
                <button className={`btn ${activeTab === 'tally' ? 'btn-primary' : 'btn-outline'}`} onClick={() => setActiveTab('tally')}>Team Medal Tally</button>
                {isAdmin && <button className={`btn ${activeTab === 'anomalies' ? 'btn-danger' : 'btn-outline'}`} onClick={() => setActiveTab('anomalies')}>Score Anomalies</button>}
            </div>

            {activeTab === 'leaderboard' && (
                <div className="glass-panel">
                    <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
                        <select className="form-group" style={{ padding: '0.4rem' }} onChange={e => setFilter({ ...filter, disciplineId: e.target.value })}>
                            <option value="">All Disciplines</option>
                            {disciplines.map(d => <option key={d.id} value={d.id}>{d.type}</option>)}
                        </select>
                        <select className="form-group" style={{ padding: '0.4rem' }} onChange={e => setFilter({ ...filter, categoryId: e.target.value })}>
                            <option value="">All Categories</option>
                            {categories.map(c => <option key={c.id} value={c.id}>{c.type}</option>)}
                        </select>
                    </div>

                    <table style={{ width: '100%', textAlign: 'left', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>Participant/Team</th>
                                <th>Discipline</th>
                                <th>Category</th>
                                <th>Final Score</th>
                                <th>Place (Medal)</th>
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
                            )) : <tr><td colSpan="5" style={{ padding: '1rem', textAlign: 'center' }}>No results matching filters.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            {activeTab === 'tally' && (
                <div className="glass-panel">
                    <h3>Medal Tally by Team</h3>
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>Team Name</th>
                                <th style={{ color: '#FFD700' }}>Gold</th>
                                <th style={{ color: '#C0C0C0' }}>Silver</th>
                                <th style={{ color: '#CD7F32' }}>Bronze</th>
                                <th>Total</th>
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
                            {teamTally.length === 0 && <tr><td colSpan="5" style={{ padding: '1rem', textAlign: 'center' }}>No medals awarded yet.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}

            {activeTab === 'anomalies' && isAdmin && (
                <div className="glass-panel">
                    <h3 style={{ color: '#ff4d4f' }}>Suspicious Score Entries (2 StdDev Difference)</h3>
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead style={{ borderBottom: '1px solid var(--surface-border)' }}>
                            <tr>
                                <th style={{ padding: '0.8rem' }}>Participant Name</th>
                                <th>Judge Name</th>
                                <th>Score Type</th>
                                <th>Given Score</th>
                                <th>Deviation</th>
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
                            {anomalies.length === 0 && <tr><td colSpan="5" style={{ padding: '1rem', textAlign: 'center' }}>No anomalies detected.</td></tr>}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default CompetitionDetails;
