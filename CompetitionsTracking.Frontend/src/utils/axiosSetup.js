import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7286/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    let errorMessage = "An error occurred";
    let validationErrors = null;

    if (error.response) {
      if (error.response.status === 401) {
        // Гість не авторизований — це нормально, мовчазний reject без toast
        return Promise.reject({ message: null, isSilent: true, type: 'unauthorized', originalError: error });
      } else if (error.response.status === 403) {
        errorMessage = "Доступ заборонено. Потрібні права адміністратора.";
      } else if (error.response.data) {
        if (error.response.data.errors) {
          validationErrors = error.response.data.errors;
          const firstErrorField = Object.keys(validationErrors)[0];
          errorMessage = validationErrors[firstErrorField][0] || "Помилка валідації";
        } else if (error.response.data.message) {
          errorMessage = error.response.data.message;
        } else if (typeof error.response.data === 'string') {
          errorMessage = error.response.data;
        }
      }
    } else if (error.request) {
      errorMessage = "Помилка мережі. Сервер недоступний.";
    }

    return Promise.reject({ message: errorMessage, validationErrors, originalError: error });
  }
);

export default api;
