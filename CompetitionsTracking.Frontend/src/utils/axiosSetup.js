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
        errorMessage = "Unauthorized. Please log in.";
      } else if (error.response.status === 403) {
         errorMessage = "Access forbidden. Admin privileges required.";
      } else if (error.response.data) {
        if (error.response.data.errors) {
            validationErrors = error.response.data.errors;
            const firstErrorField = Object.keys(validationErrors)[0];
            errorMessage = validationErrors[firstErrorField][0] || "Validation Error";
        } else if (error.response.data.message) {
            errorMessage = error.response.data.message;
        } else if (typeof error.response.data === 'string') {
            errorMessage = error.response.data;
        }
      }
    } else if (error.request) {
      errorMessage = "Network error. Server might be down.";
    }

    return Promise.reject({ message: errorMessage, validationErrors, originalError: error });
  }
);

export default api;
