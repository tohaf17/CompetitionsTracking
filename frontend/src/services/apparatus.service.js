import api from '../utils/axiosSetup';

const ApparatusService = {
  getAll: async () => {
    const response = await api.get('/Apparatus');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Apparatus/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Apparatus', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Apparatus/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Apparatus/${id}`);
    return response.data;
  }
};

export default ApparatusService;
