import api from '../utils/axiosSetup';

const JudgeService = {
  getAll: async () => {
    const response = await api.get('/Judge');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Judge/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Judge', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Judge/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Judge/${id}`);
    return response.data;
  },

  getAnalytics: async (id) => {
    const response = await api.get(`/Judge/${id}/analytics`);
    return response.data;
  },

  getPendingEvaluations: async (id, competitionId) => {
    const response = await api.get(`/Judge/${id}/competitions/${competitionId}/pending`);
    return response.data;
  },

  getConflicts: async (id) => {
    const response = await api.get(`/Judge/${id}/conflicts`);
    return response.data;
  },

  getWorkload: async (id, competitionId) => {
    const response = await api.get(`/Judge/${id}/competitions/${competitionId}/workload`);
    return response.data;
  },

  getScores: async (id, competitionId) => {
    const response = await api.get(`/Judge/${id}/competitions/${competitionId}/scores`);
    return response.data;
  }
};

export default JudgeService;
