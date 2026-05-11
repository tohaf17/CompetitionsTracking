import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import PersonService from '../../services/person.service';
import { unwrapCollection } from '../../utils/unwrapCollection';
import { toastError } from '../../utils/toastError';
import '../Competitions/CompetitionsList.css';

const MEDAL_MAP = {
    1: { label: 'Золото', emoji: '🥇', color: '#FFD700' },
    2: { label: 'Срібло', emoji: '🥈', color: '#C0C0C0' },
    3: { label: 'Бронза', emoji: '🥉', color: '#CD7F32' },
};

const GENDER_MAP = { 0: 'Чоловіча', 1: 'Жіноча' };

const StatCard = ({ label, value, sub, color }) => (
    <div style={{
        background: 'rgba(255,255,255,0.04)',
        border: '1px solid var(--surface-border)',
        borderRadius: '10px',
        padding: '1.2rem 1.5rem',
        display: 'flex',
        flexDirection: 'column',
        gap: '0.3rem',
        minWidth: '120px',
    }}>
        <span style={{ fontSize: '0.78rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>{label}</span>
        <span style={{ fontSize: '2rem', fontWeight: 700, color: color || 'var(--text-color)' }}>{value}</span>
        {sub && <span style={{ fontSize: '0.78rem', color: 'var(--text-muted)' }}>{sub}</span>}
    </div>
);

const PersonProfile = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [person, setPerson] = useState(null);
    const [performance, setPerformance] = useState([]);
    const [teams, setTeams] = useState([]);
    const [mentees, setMentees] = useState([]);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('history');

    const loadAll = useCallback(async () => {
        try {
            setLoading(true);
            const [personData, perfData, teamsData, menteesData] = await Promise.all([
                PersonService.getById(id),
                PersonService.getPerformanceHistory(id),
                PersonService.getTeamAffiliations(id),
                PersonService.getMentees(id),
            ]);
            setPerson(personData);
            setPerformance(Array.isArray(perfData) ? perfData : []);
            setTeams(Array.isArray(teamsData) ? teamsData : []);
            setMentees(Array.isArray(menteesData) ? menteesData : unwrapCollection(menteesData) || []);
        } catch (error) {
            toastError(error, 'Не вдалося завантажити профіль учасника');
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

    if (!person) return (
        <div className="page-container">
            <p style={{ color: 'var(--text-muted)' }}>Учасника не знайдено.</p>
            <button className="btn btn-outline" style={{ marginTop: '1rem' }} onClick={() => navigate(-1)}>← Назад</button>
        </div>
    );

    const medals = performance.filter(p => p.placement >= 1 && p.placement <= 3);
    const gold = medals.filter(p => p.placement === 1).length;
    const silver = medals.filter(p => p.placement === 2).length;
    const bronze = medals.filter(p => p.placement === 3).length;
    const totalCompetitions = performance.length;
    const bestScore = performance.length > 0 ? Math.max(...performance.map(p => p.finalScore)) : null;

    const fullName = `${person.name} ${person.surname}`;
    const age = person.dateOfBirth
        ? Math.floor((Date.now() - new Date(person.dateOfBirth)) / (1000 * 60 * 60 * 24 * 365.25))
        : null;

    return (
        <div className="page-container">
            <div style={{ marginBottom: '2rem' }}>
                <button
                    className="btn btn-outline"
                    style={{ marginBottom: '1rem', padding: '0.4rem 0.9rem', fontSize: '0.85rem' }}
                    onClick={() => navigate(-1)}
                >
                    ← Назад
                </button>

                <div className="glass-panel" style={{ padding: '2rem', display: 'flex', gap: '2rem', alignItems: 'flex-start', flexWrap: 'wrap' }}>
                 
                    <div style={{
                        width: 80, height: 80, borderRadius: '50%',
                        background: 'linear-gradient(135deg, #3a3a3a, #1a1a1a)',
                        border: '2px solid var(--surface-border)',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        fontSize: '2rem', flexShrink: 0,
                    }}>
                        {person.gender === 1 ? '👩' : '👨'}
                    </div>

                    <div style={{ flex: 1, minWidth: 200 }}>
                        <h1 className="page-title" style={{ marginBottom: '0.3rem' }}>{fullName}</h1>
                        <div style={{ display: 'flex', gap: '1.5rem', flexWrap: 'wrap', color: 'var(--text-muted)', fontSize: '0.9rem', marginBottom: '0.8rem' }}>
                            {person.country && <span>🌍 {person.country}</span>}
                            {age !== null && <span>🎂 {age} р.</span>}
                            {person.gender !== undefined && <span>⚥ {GENDER_MAP[person.gender] ?? '—'}</span>}
                            {person.dateOfBirth && <span>📅 {new Date(person.dateOfBirth).toLocaleDateString('uk-UA')}</span>}
                        </div>

                        {teams.length > 0 && (
                            <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
                                {teams.map((t, i) => (
                                    <span key={i} style={{
                                        background: 'rgba(255,255,255,0.08)',
                                        border: '1px solid var(--surface-border)',
                                        borderRadius: '20px',
                                        padding: '0.2rem 0.8rem',
                                        fontSize: '0.8rem',
                                        color: 'var(--text-color)',
                                    }}>
                                        {t.role === 'Coach' ? '🏋️ Тренер' : '🤸 Учасник'} — {t.teamName}
                                    </span>
                                ))}
                            </div>
                        )}
                    </div>

                    <div style={{ display: 'flex', gap: '0.8rem', flexWrap: 'wrap' }}>
                        <StatCard label="Всього змагань" value={totalCompetitions} />
                        <StatCard label="🥇 Золото" value={gold} color="#FFD700" />
                        <StatCard label="🥈 Срібло" value={silver} color="#C0C0C0" />
                        <StatCard label="🥉 Бронза" value={bronze} color="#CD7F32" />
                        {bestScore !== null && <StatCard label="Найкраща оцінка" value={bestScore.toFixed(2)} />}
                    </div>
                </div>
            </div>

            <div className="flex gap-2 mb-2" style={{ overflowX: 'auto', paddingBottom: '0.5rem' }}>
                <button
                    className={`btn ${activeTab === 'history' ? 'btn-primary' : 'btn-outline'}`}
                    onClick={() => setActiveTab('history')}
                >
                    📊 Історія виступів
                </button>
                <button
                    className={`btn ${activeTab === 'medals' ? 'btn-primary' : 'btn-outline'}`}
                    onClick={() => setActiveTab('medals')}
                >
                    🏅 Медалі ({gold + silver + bronze})
                </button>
                {mentees.length > 0 && (
                    <button
                        className={`btn ${activeTab === 'mentees' ? 'btn-primary' : 'btn-outline'}`}
                        onClick={() => setActiveTab('mentees')}
                    >
                        👥 Підопічні ({mentees.length})
                    </button>
                )}
            </div>

            {activeTab === 'history' && (
                <div className="glass-panel" style={{ padding: '1.5rem' }}>
                    <h3 style={{ marginBottom: '1rem', fontSize: '1rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        Повна історія виступів
                    </h3>
                    {performance.length === 0 ? (
                        <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
                            Виступів ще не зафіксовано.
                        </p>
                    ) : (
                        <div className="table-container">
                            <table>
                                <thead>
                                    <tr>
                                        <th>№</th>
                                        <th>Змагання</th>
                                        <th>Дата</th>
                                        <th>Дисципліна</th>
                                        <th>Фінальна оцінка</th>
                                        <th>Місце / Медаль</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {performance.map((p, i) => {
                                        const medal = MEDAL_MAP[p.placement];
                                        return (
                                            <tr key={i}>
                                                <td>{i + 1}</td>
                                                <td>
                                                    <Link
                                                        to={`/competitions/${p.competitionId}`}
                                                        style={{ color: 'var(--primary-color)', textDecoration: 'none' }}
                                                    >
                                                        {p.competitionName}
                                                    </Link>
                                                </td>
                                                <td style={{ color: 'var(--text-muted)' }}>
                                                    {new Date(p.competitionDate).toLocaleDateString('uk-UA')}
                                                </td>
                                                <td>{p.apparatusName}</td>
                                                <td>
                                                    <span className="status-badge status-active">
                                                        {p.finalScore.toFixed(2)}
                                                    </span>
                                                </td>
                                                <td>
                                                    {medal ? (
                                                        <span style={{ color: medal.color, fontWeight: 600 }}>
                                                            {medal.emoji} {p.placement} місце
                                                        </span>
                                                    ) : (
                                                        <span style={{ color: 'var(--text-muted)' }}>
                                                            {p.placement} місце
                                                        </span>
                                                    )}
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            )}

            {activeTab === 'medals' && (
                <div className="glass-panel" style={{ padding: '1.5rem' }}>
                    <h3 style={{ marginBottom: '1rem', fontSize: '1rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        Нагороди
                    </h3>
                    {medals.length === 0 ? (
                        <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
                            Медалей ще немає.
                        </p>
                    ) : (
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.8rem' }}>
                            {medals.map((p, i) => {
                                const medal = MEDAL_MAP[p.placement];
                                return (
                                    <div key={i} style={{
                                        display: 'flex',
                                        alignItems: 'center',
                                        gap: '1rem',
                                        padding: '1rem 1.2rem',
                                        background: 'rgba(255,255,255,0.03)',
                                        border: `1px solid ${medal.color}33`,
                                        borderRadius: '10px',
                                        flexWrap: 'wrap',
                                    }}>
                                        <span style={{ fontSize: '2rem' }}>{medal.emoji}</span>
                                        <div style={{ flex: 1, minWidth: 180 }}>
                                            <div style={{ fontWeight: 600, marginBottom: '0.2rem' }}>
                                                {medal.label} — {p.apparatusName}
                                            </div>
                                            <div style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>
                                                <Link to={`/competitions/${p.competitionId}`} style={{ color: 'inherit' }}>
                                                    {p.competitionName}
                                                </Link>
                                                {' · '}
                                                {new Date(p.competitionDate).toLocaleDateString('uk-UA')}
                                            </div>
                                        </div>
                                        <span style={{
                                            background: `${medal.color}22`,
                                            color: medal.color,
                                            border: `1px solid ${medal.color}55`,
                                            borderRadius: '20px',
                                            padding: '0.3rem 0.9rem',
                                            fontWeight: 700,
                                            fontSize: '1rem',
                                        }}>
                                            {p.finalScore.toFixed(2)} балів
                                        </span>
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
            )}

            {activeTab === 'mentees' && (
                <div className="glass-panel" style={{ padding: '1.5rem' }}>
                    <h3 style={{ marginBottom: '1rem', fontSize: '1rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                        Підопічні учасники
                    </h3>
                    {mentees.length === 0 ? (
                        <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
                            Підопічних немає.
                        </p>
                    ) : (
                        <div className="table-container">
                            <table>
                                <thead>
                                    <tr>
                                        <th>№</th>
                                        <th>ПІБ</th>
                                        <th>Країна</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {mentees.map((m, i) => (
                                        <tr key={m.personId ?? i}>
                                            <td>{i + 1}</td>
                                            <td>
                                                <Link
                                                    to={`/persons/${m.personId}`}
                                                    style={{ color: 'var(--primary-color)', textDecoration: 'none' }}
                                                >
                                                    {m.fullName}
                                                </Link>
                                            </td>
                                            <td style={{ color: 'var(--text-muted)' }}>{m.country}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default PersonProfile;
