import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'https://localhost:7068/api', 
  headers: {
    'Content-Type': 'application/json',
  },
});

export function setAuthToken(token?: string) {
  if (token) apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  else delete apiClient.defaults.headers.common['Authorization'];
}

apiClient.interceptors.response.use(
  (r) => r,
  (error) => {
    const failedUrl: string | undefined = error?.config?.url;
    // Updated check for new auth paths
    const isAuthEndpoint = failedUrl && (failedUrl.includes('/auth/login') || failedUrl.includes('/auth/register'));
    
    if (error.response?.status === 401 && !isAuthEndpoint) {
      // Optional: Clear storage if token is invalid
      localStorage.removeItem('auth');
      window.location.href = '/account/login';
      return Promise.reject(error);
    }
    return Promise.reject(error);
  }
);

export default apiClient;