import api from '../utils/axiosSetup';

const AdminUserService = {
  getAll: async () => {
    const response = await api.get('/Auth/users');
    return response.data;
  },

  deleteUser: async (id) => {
    const response = await api.delete(`/Auth/users/${id}`);
    return response.data;
  },

  approveUser: async (id) => {
    const response = await api.post(`/Auth/users/${id}/approve`);
    return response.data;
  }
};

export default AdminUserService;
