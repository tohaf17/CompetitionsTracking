import api from '../utils/axiosSetup';

const CompetitionService = {
  getAll: async (filter = {}, pagination = {}) => {
    const params = new URLSearchParams({ ...filter, ...pagination });
    const response = await api.get(`/Competition?${params.toString()}`);
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Competition/${id}`);
    return response.data;
  },

  getSummary: async (id) => {
    const response = await api.get(`/Competition/${id}/summary`);
    return response.data;
  },

  getLeaderboard: async (id) => {
    const response = await api.get(`/Competition/${id}/leaderboard`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Competition', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Competition/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Competition/${id}`);
    return response.data;
  },

  changeStatus: async (id, statusData) => {
    const response = await api.patch(`/Competition/${id}/status`, statusData);
    return response.data;
  },

  awardMedals: async (id) => {
    const response = await api.post(`/Competition/${id}/award-medals`);
    return response.data;
  }
};

export default CompetitionService;
