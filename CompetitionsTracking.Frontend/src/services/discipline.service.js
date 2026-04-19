import api from '../utils/axiosSetup';

const DisciplineService = {
  getAll: async () => {
    const response = await api.get('/Discipline');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Discipline/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Discipline', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Discipline/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Discipline/${id}`);
    return response.data;
  }
};

export default DisciplineService;
