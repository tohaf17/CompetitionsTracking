import api from '../utils/axiosSetup';

const ScoreService = {
  getAll: async () => {
    const response = await api.get('/Score');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Score/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Score', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Score/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Score/${id}`);
    return response.data;
  },

  getScoreAnomalies: async (competitionId) => {
    const response = await api.get(`/Score/competition/${competitionId}/anomalies`);
    return response.data;
  },

  getScoresByEntry: async (entryId) => {
    const response = await api.get(`/Score/entry/${entryId}`);
    return response.data;
  },

  getEntryScoreBreakdown: async (entryId) => {
    const response = await api.get(`/Score/entry/${entryId}/breakdown`);
    return response.data;
  }
};

export default ScoreService;
