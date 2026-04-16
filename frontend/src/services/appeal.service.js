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
  }
};

export default AppealService;
