import React, { useState, useEffect } from 'react';
import CompetitionService from '../../services/competition.service';
import { NavLink } from 'react-router-dom';
import './CompetitionsList.css';
import toast from 'react-hot-toast';

const CompetitionsList = () => {
    const [competitions, setCompetitions] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadCompetitions();
    }, []);

    const loadCompetitions = async () => {
        try {
            setLoading(true);
            const data = await CompetitionService.getAll();
            setCompetitions(data.items || data);
        } catch (error) {
            toast.error(error.message || "Не вдалося завантажити змагання");
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <div className="page-container">Завантаження...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Змагання</h1>
                <button className="btn btn-primary" onClick={loadCompetitions}>Оновити</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Назва</th>
                            <th>Дати</th>
                            <th>Місто</th>
                            <th>Статус</th>
                            <th>Дії</th>
                        </tr>
                    </thead>
                    <tbody>
                        {competitions.length > 0 ? (
                            competitions.map((comp) => (
                                <tr key={comp.id}>
                                    <td>{comp.id}</td>
                                    <td><strong>{comp.name}</strong></td>
                                    <td>{new Date(comp.startDate).toLocaleDateString('uk-UA')} - {new Date(comp.endDate).toLocaleDateString('uk-UA')}</td>
                                    <td>{comp.location}</td>
                                    <td>
                                        <span className={`status-badge status-${comp.status.toLowerCase()}`}>
                                            {comp.status}
                                        </span>
                                    </td>
                                    <td>
                                        <NavLink to={`/competitions/${comp.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}>
                                            Переглянути
                                        </NavLink>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>Змагань не знайдено.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default CompetitionsList;
