import api from '../utils/axiosSetup';

const AppealService = {
  getPending: async (filter = {}) => {
    const params = new URLSearchParams(filter);
    const response = await api.get(`/Appeal/pending?${params.toString()}`);
    return response.data;
  },

  getDossier: async (id) => {
    const response = await api.get(`/Appeal/${id}/dossier`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Appeal', data);
    return response.data;
  },
  
  getAll: async () => {
    const response = await api.get('/Appeal');
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Appeal/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Appeal/${id}`);
    return response.data;
  },

  approve: async (id, data) => {
    const response = await api.post(`/Appeal/${id}/approve`, data);
    return response.data;
  }
};

export default AppealService;
