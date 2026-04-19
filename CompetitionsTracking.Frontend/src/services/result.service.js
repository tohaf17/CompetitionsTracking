import api from '../utils/axiosSetup';

const ResultService = {
  getAll: async () => {
    const response = await api.get('/Result');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/Result/${id}`);
    return response.data;
  },

  create: async (data) => {
    const response = await api.post('/Result', data);
    return response.data;
  },

  update: async (id, data) => {
    const response = await api.put(`/Result/${id}`, data);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/Result/${id}`);
    return response.data;
  },

  getTeamMedalTally: async (competitionId) => {
    const response = await api.get(`/Result/competition/${competitionId}/team-tally`);
    return response.data;
  },

  getLeaderboard: async (competitionId, disciplineId, categoryId) => {
    let params = [];
    if (disciplineId) params.push(`disciplineId=${disciplineId}`);
    if (categoryId) params.push(`categoryId=${categoryId}`);
    const query = params.length > 0 ? `?${params.join('&')}` : '';
    
    const response = await api.get(`/Result/competition/${competitionId}/leaderboard${query}`);
    return response.data;
  },

  getCountryMedalTally: async (competitionId) => {
    const response = await api.get(`/Result/competition/${competitionId}/country-tally`);
    return response.data;
  },

  getDisciplineRecords: async (disciplineId, topN = 10) => {
    const response = await api.get(`/Result/discipline/${disciplineId}/records?topN=${topN}`);
    return response.data;
  }
};

export default ResultService;
