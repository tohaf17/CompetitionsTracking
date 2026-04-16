import api from '../utils/axiosSetup';

const EntryService = {
  getAll: async (pagination = {}) => {
    const params = new URLSearchParams(pagination);
    const response = await api.get(`/Entry?${params.toString()}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Entry/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Entry', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Entry/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Entry/${id}`);
    return response.data;
  },

  getControversialEntries: async (competitionId) => {
    const response = await api.get(`/Entry/competition/${competitionId}/controversial`);
    return response.data;
  },

  bulkUpdateStatus: async (data) => {
    const response = await api.patch('/Entry/bulk-status', data);
    return response.data;
  },

  changeStatus: async (id, statusData) => {
    const response = await api.patch(`/Entry/${id}/status`, statusData);
    return response.data;
  },

  disqualify: async (id) => {
    const response = await api.patch(`/Entry/${id}/disqualify`);
    return response.data;
  },

  transferEntry: async (id, data) => {
    const response = await api.post(`/Entry/${id}/transfer`, data);
    return response.data;
  },

  getStartList: async (competitionId) => {
    const response = await api.get(`/Entry/competition/${competitionId}/start-list`);
    return response.data;
  },

  getMissingScores: async (competitionId, expectedCount = 4) => {
    const response = await api.get(`/Entry/competition/${competitionId}/missing-scores?expectedCount=${expectedCount}`);
    return response.data;
  },

  getAnalytics: async (competitionId) => {
    const response = await api.get(`/Entry/competition/${competitionId}/analytics`);
    return response.data;
  }
};

export default EntryService;
