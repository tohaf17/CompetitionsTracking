import api from '../utils/axiosSetup';

const PersonService = {
  getAll: async () => {
    const response = await api.get('/Person');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Person/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Person', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Person/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Person/${id}`);
    return response.data;
  },

  getPerformanceHistory: async (id) => {
    const response = await api.get(`/Person/${id}/performance`);
    return response.data;
  },

  getMentees: async (id) => {
    const response = await api.get(`/Person/${id}/mentees`);
    return response.data;
  },

  getTeamAffiliations: async (id) => {
    const response = await api.get(`/Person/${id}/teams`);
    return response.data;
  }
};

export default PersonService;
