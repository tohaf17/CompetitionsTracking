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

  getSchedule: async (id) => {
    const response = await api.get(`/Judge/${id}/schedule`);
    return response.data;
  },

  getWorkload: async (id) => {
    const response = await api.get(`/Judge/${id}/workload`);
    return response.data;
  }
};

export default JudgeService;
