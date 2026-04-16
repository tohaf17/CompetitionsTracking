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
            setCompetitions(data.items || data); // Adjust based on pagination wrapper
        } catch (error) {
            toast.error(error.message || "Failed to load competitions");
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <div className="page-container">Loading...</div>;

    return (
        <div className="page-container">
            <div className="page-header flex-between">
                <h1 className="page-title">Competitions</h1>
                <button className="btn btn-primary" onClick={loadCompetitions}>Refresh</button>
            </div>
            
            <div className="glass-panel table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Dates</th>
                            <th>Location</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {competitions.length > 0 ? (
                            competitions.map((comp) => (
                                <tr key={comp.id}>
                                    <td>{comp.id}</td>
                                    <td><strong>{comp.name}</strong></td>
                                    <td>{new Date(comp.startDate).toLocaleDateString()} - {new Date(comp.endDate).toLocaleDateString()}</td>
                                    <td>{comp.location}</td>
                                    <td>
                                        <span className={`status-badge status-${comp.status.toLowerCase()}`}>
                                            {comp.status}
                                        </span>
                                    </td>
                                    <td>
                                        <NavLink to={`/competitions/${comp.id}`} className="btn btn-outline" style={{padding: '0.3rem 0.6rem', fontSize: '0.8rem'}}>
                                            View
                                        </NavLink>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="6" style={{textAlign: 'center', padding: '2rem'}}>No competitions found.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default CompetitionsList;
