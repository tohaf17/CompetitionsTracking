import React from 'react';
import Modal from '../UI/Modal';

const EntryFormModal = ({ 
    isOpen, 
    onClose, 
    onSubmit, 
    formData, 
    handleChange, 
    competitions, 
    participantOptions, 
    disciplines, 
    apparatuses,
    categories, 
    performanceType, 
    setPerformanceType,
    isCoach,
    isAdmin,
    levelMap,
    teams
}) => {
    const hasParticipants = participantOptions.length > 0;

    return (
        <Modal isOpen={isOpen} onClose={onClose} title="Створити нову заявку">
            <form onSubmit={onSubmit}>
                <div className="form-group">
                    <label>Змагання</label>
                    <select name="competitionId" value={formData.competitionId} onChange={handleChange} required>
                        <option value="">-- Оберіть змагання --</option>
                        {competitions.map(c => (
                            <option key={c.id} value={c.id}>
                                {c.title} ({levelMap[c.level] || 'тип невідомий'})
                            </option>
                        ))}
                    </select>
                </div>

                <div className="form-group">
                    <label>Учасник або команда</label>
                    <select name="participantId" value={formData.participantId} onChange={handleChange} required={isCoach}>
                        <option value="">
                            {hasParticipants
                                ? `-- Оберіть зі списку ${isAdmin ? 'або створіть вручну' : ''} --`
                                : '-- Немає доступних учасників або команд --'}
                        </option>
                        {participantOptions.map(p => (
                            <option key={`${p.type}-${p.id}`} value={p.id}>
                                {p.name} ({p.type === 'Team' ? 'команда' : `учасник${p.age != null ? `, ${p.age} р.` : ''}`})
                            </option>
                        ))}
                    </select>
                </div>

                {!formData.participantId && isAdmin && (
                    <div className="glass-panel" style={{ padding: '1rem', marginBottom: '1rem', background: 'rgba(255,255,255,0.03)' }}>
                        <div className="form-group">
                            <label>Ім'я учасника</label>
                            <input type="text" name="participantName" value={formData.participantName} onChange={handleChange} placeholder="Введіть ім'я" required />
                        </div>
                        <div className="form-group">
                            <label>Прізвище учасника</label>
                            <input type="text" name="participantSurname" value={formData.participantSurname} onChange={handleChange} placeholder="Введіть прізвище" required />
                        </div>
                        <div className="form-group">
                            <label>Команда / Клуб</label>
                            <input
                                type="text"
                                name="teamName"
                                list="teams-list"
                                value={formData.teamName}
                                onChange={handleChange}
                                placeholder="Оберіть або введіть назву"
                                required
                            />
                            <datalist id="teams-list">
                                {teams.map(t => <option key={t.id} value={t.name} />)}
                            </datalist>
                        </div>
                    </div>
                )}

                <div className="form-row" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                    <div className="form-group">
                        <label>Тип виступу</label>
                        <select 
                            value={performanceType} 
                            onChange={(e) => setPerformanceType(e.target.value)} 
                            required
                        >
                            <option value="">-- Тип --</option>
                            <option value="Індивідуальна">Індивідуальна</option>
                            <option value="Групова">Групова</option>
                        </select>
                    </div>
                    <div className="form-group">
                        <label>Предмет</label>
                        <select name="apparatusId" value={formData.apparatusId || ''} onChange={handleChange} required disabled={!performanceType}>
                            <option value="">-- Предмет --</option>
                            {apparatuses.map(a => (
                                <option key={a.id} value={a.id}>{a.type}</option>
                            ))}
                        </select>
                    </div>
                </div>

                <div className="form-group">
                    <label>Категорія</label>
                    <select name="categoryId" value={formData.categoryId} onChange={handleChange} required>
                        <option value="">-- Оберіть категорію --</option>
                        {categories.map(c => (
                            <option key={c.id} value={c.id}>
                                {c.type} ({c.minAge}-{c.maxAge} р.)
                            </option>
                        ))}
                    </select>
                </div>

                <div className="modal-footer">
                    <button type="button" className="btn btn-outline" onClick={onClose}>Скасувати</button>
                    <button type="submit" className="btn btn-primary" disabled={isCoach && !hasParticipants}>Подати заявку</button>
                </div>
            </form>
        </Modal>
    );
};

export default EntryFormModal;
