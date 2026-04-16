import api from '../utils/axiosSetup';

const TeamService = {
  getAll: async () => {
    const response = await api.get('/Team');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Team/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Team', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Team/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Team/${id}`);
    return response.data;
  },

  getMetrics: async (id) => {
    const response = await api.get(`/Team/${id}/metrics`);
    return response.data;
  },

  getRoster: async (id) => {
    const response = await api.get(`/Team/${id}/roster`);
    return response.data;
  },

  addMember: async (teamId, personId) => {
    const response = await api.post(`/Team/${teamId}/members/${personId}`);
    return response.data;
  },

  removeMember: async (teamId, personId) => {
    const response = await api.delete(`/Team/${teamId}/members/${personId}`);
    return response.data;
  }
};

export default TeamService;
