import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import TeamService from '../../services/team.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import { toastError } from '../../utils/toastError';
import '../Competitions/CompetitionsList.css';

const StatCard = ({ label, value, sub, color }) => (
    <div style={{
        background: 'rgba(255,255,255,0.04)',
        border: '1px solid var(--surface-border)',
        borderRadius: '10px',
        padding: '1.2rem 1.5rem',
        display: 'flex',
        flexDirection: 'column',
        gap: '0.3rem',
        minWidth: '150px',
    }}>
        <span style={{ fontSize: '0.78rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{label}</span>
        <span style={{ fontSize: '1.8rem', fontWeight: 700, color: color || 'var(--text-color)' }}>{value}</span>
        {sub && <span style={{ fontSize: '0.78rem', color: 'var(--text-muted)' }}>{sub}</span>}
    </div>
);

const TeamProfile = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [team, setTeam] = useState(null);
    const [metrics, setMetrics] = useState(null);
    const [loading, setLoading] = useState(true);

    const loadAll = useCallback(async () => {
        try {
            setLoading(true);
            const [teamData, metricsData] = await Promise.all([
                TeamService.getById(id),
                TeamService.getMetrics(id)
            ]);
            setTeam(teamData);
            setMetrics(unwrapCollection(metricsData)[0] || null);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити профіль команди');
        } finally {
            setLoading(false);
        }
    }, [id]);

    useEffect(() => { void loadAll(); }, [loadAll]);

    if (loading) return (
        <div className="page-container" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '40vh' }}>
            <div style={{ color: 'var(--text-muted)', fontSize: '1.1rem' }}>Завантаження профілю...</div>
        </div>
    );

    if (!team) return (
        <div className="page-container">
            <p style={{ color: 'var(--text-muted)' }}>Команду не знайдено.</p>
            <button className="btn btn-outline" style={{ marginTop: '1rem' }} onClick={() => navigate(-1)}>← Назад</button>
        </div>
    );

    return (
        <div className="page-container">
            <div style={{ marginBottom: '2rem' }}>
                <button
                    className="btn btn-outline"
                    style={{ marginBottom: '1rem', padding: '0.4rem 0.9rem', fontSize: '0.85rem' }}
                    onClick={() => navigate(-1)}
                >
                    ← Назад до списку
                </button>

                <div className="glass-panel" style={{ padding: '2rem', display: 'flex', gap: '2rem', alignItems: 'flex-start', flexWrap: 'wrap' }}>
                    <div style={{
                        width: 80, height: 80, borderRadius: '12px',
                        background: 'linear-gradient(135deg, var(--primary-color), #6366f1)',
                        border: '1px solid var(--surface-border)',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        fontSize: '2.5rem', flexShrink: 0, color: '#fff',
                        boxShadow: '0 8px 16px rgba(99, 102, 241, 0.2)'
                    }}>
                        🏆
                    </div>

                    <div style={{ flex: 1, minWidth: 250 }}>
                        <h1 className="page-title" style={{ marginBottom: '0.5rem' }}>{team.name}</h1>
                        <p style={{ color: 'var(--text-muted)', fontSize: '1rem', marginBottom: '1rem' }}>
                            Тренер: <Link to={`/persons/${team.coachId}`} style={{ color: 'var(--primary-color)', textDecoration: 'none', fontWeight: 600 }}>{team.coachFullName}</Link>
                        </p>
                        
                        {metrics && (
                            <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                                <StatCard label="Учасників" value={metrics.totalParticipants} />
                                <StatCard label="Рейтингові бали" value={metrics.cumulativePoints.toFixed(1)} color="var(--primary-color)" />
                                <StatCard label="Сер. бал / спортсмена" value={metrics.averagePointsPerParticipant.toFixed(2)} />
                            </div>
                        )}
                    </div>
                </div>
            </div>

            <div className="glass-panel" style={{ padding: '1.5rem' }}>
                <h3 style={{ marginBottom: '1.5rem', fontSize: '1.1rem', color: 'var(--text-color)', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                    👥 Склад команди
                </h3>
                {team.members && team.members.length > 0 ? (
                    <div className="table-container">
                        <table>
                            <thead>
                                <tr>
                                    <th style={{ width: '50px' }}>№</th>
                                    <th>ПІБ Спортсмена</th>
                                    <th>Країна</th>
                                    <th style={{ textAlign: 'right' }}>Дії</th>
                                </tr>
                            </thead>
                            <tbody>
                                {team.members.map((m, i) => (
                                    <tr key={m.personId}>
                                        <td>{i + 1}</td>
                                        <td>
                                            <Link
                                                to={`/persons/${m.personId}`}
                                                style={{ color: 'var(--text-color)', textDecoration: 'none', fontWeight: 600 }}
                                                onMouseEnter={e => e.currentTarget.style.color = 'var(--primary-color)'}
                                                onMouseLeave={e => e.currentTarget.style.color = 'var(--text-color)'}
                                            >
                                                {m.fullName}
                                            </Link>
                                        </td>
                                        <td style={{ color: 'var(--text-muted)' }}>{m.country}</td>
                                        <td style={{ textAlign: 'right' }}>
                                            <Link to={`/persons/${m.personId}`} className="btn btn-outline" style={{ padding: '0.2rem 0.6rem', fontSize: '0.75rem' }}>
                                                Профіль
                                            </Link>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
                        У цій команді поки що немає зареєстрованих учасників.
                    </p>
                )}
            </div>
        </div>
    );
};

export default TeamProfile;
