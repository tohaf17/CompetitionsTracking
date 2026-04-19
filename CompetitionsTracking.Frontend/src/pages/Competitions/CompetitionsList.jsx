import React, { useState, useEffect } from 'react';
import CompetitionService from '../../services/competition.service';
import { NavLink } from 'react-router-dom';
import { unwrapCollection } from '../../utils/unwrapCollection';
import './CompetitionsList.css';
import { toastError } from '../../utils/toastError';

const CompetitionsList = () => {
    const [competitions, setCompetitions] = useState([]);
    const [loading, setLoading] = useState(true);

    const statusMap = {
        0: { text: 'Заплановано', class: 'status-planned' },
        1: { text: 'Реєстрація відкрита', class: 'status-upcoming' },
        2: { text: 'Триває', class: 'status-ongoing' },
        3: { text: 'Завершено', class: 'status-completed' }
    };

    useEffect(() => {
        loadCompetitions();
    }, []);

    const loadCompetitions = async () => {
        try {
            setLoading(true);
            const data = await CompetitionService.getAll();
            setCompetitions(unwrapCollection(data));
        } catch (error) {
            toastError(error, 'Не вдалося завантажити змагання');
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
                            competitions.map((comp) => {
                                const compStatus = statusMap[comp.status] || { text: 'Невідомо', class: '' };

                                return (
                                <tr key={comp.id}>
                                    <td>{comp.id}</td>
                                    <td><strong>{comp.title}</strong></td>
                                    <td>{new Date(comp.startDate).toLocaleDateString('uk-UA')} - {new Date(comp.endDate).toLocaleDateString('uk-UA')}</td>
                                    <td>{comp.city}</td>
                                    <td>
                                        <span className={`status-badge ${compStatus.class}`}>
                                            {compStatus.text}
                                        </span>
                                    </td>
                                    <td>
                                        <NavLink to={`/competitions/${comp.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}>
                                            Переглянути
                                        </NavLink>
                                    </td>
                                </tr>
                                );
                            })
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




