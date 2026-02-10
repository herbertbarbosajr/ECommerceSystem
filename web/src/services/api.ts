import axios from 'axios';
import { LoginRequest, RegisterRequest } from 'types';

// 1. Cria uma instância do axios com a URL base da API vinda do .env
// Se estiver rodando no navegador, sempre usa localhost (navegador não resolve nomes Docker)
// Se a variável não estiver definida, usa localhost:8080 como padrão
const getApiBaseUrl = () => {
  const envUrl = import.meta.env.VITE_API_BASE_URL;
  if (envUrl && !envUrl.includes('ecommerce_api')) {
    return envUrl;
  }
  // Se não tiver URL configurada ou for nome Docker, usa localhost
  return 'http://localhost:8080';
};

const apiClient = axios.create({
  baseURL: getApiBaseUrl(),
});

// 2. Interceptor para adicionar o token de autenticação em cada requisição
apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem('token'); // Corrigido de 'authToken' para 'token'
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor de resposta para tratar erros
apiClient.interceptors.response.use(
  response => response,
  error => {
    // Se for erro 401 (não autorizado), limpar token e redirecionar
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      delete apiClient.defaults.headers.common['Authorization'];
    }
    return Promise.reject(error);
  }
);

// 3. Funções de autenticação
export const auth = {
  login: async (email: string, password: string): Promise<{ token: string; refreshToken: string; expiration: string; userId: string; email: string; isAdmin: boolean }> => {
    const { data } = await apiClient.post('/api/Auth/login', { email, password } as LoginRequest);
    return data;
  },
  register: async (registerData: RegisterRequest) => {
    const { data } = await apiClient.post('/api/Auth/register', registerData);
    return data;
  },
  forgotPassword: async (email: string): Promise<void> => {
    await apiClient.post('/api/Auth/forgot-password', { email });
  },
  resetPassword: async (resetData: { email: string, token: string, newPassword: string }): Promise<void> => {
    await apiClient.post('/api/Auth/reset-password', resetData);
  },
};

// 4. Funções relacionadas a produtos
export const products = {
  getAll: async (onlyActive = false) => {
    const { data } = await apiClient.get('/api/Products', {
      params: onlyActive ? { onlyActive } : {},
    });
    // A API retorna PaginatedResponse, extrair Items se existir
    return Array.isArray(data) ? data : (data?.items || []);
  },
  getById: async (id: string) => {
    const { data } = await apiClient.get(`/api/Products/${id}`);
    return data;
  },
  create: async (productData: any) => {
    const { data } = await apiClient.post('/api/Products', productData);
    return data;
  },
  delete: async (id: number) => {
    const { data } = await apiClient.delete(`/api/Products/${id}`);
    return data;
  },
  // update: async (id: string, productData: any) => { ... }
};

// 5. Funções relacionadas a pedidos
export const orders = {
  getAll: async () => {
    const { data } = await apiClient.get('/api/Orders/all');
    return data;
  },
  getMyOrders: async () => { // Para Cliente
    const { data } = await apiClient.get('/api/Orders');
    return data;
  },
  getById: async (id: string) => {
    const { data } = await apiClient.get(`/api/Orders/${id}`);
    return data;
  },
  create: async (orderData: any) => {
    const { data } = await apiClient.post('/api/Orders', orderData);
    return data;
  },
  updateStatus: async (id: string, status: string) => {
    const { data } = await apiClient.patch(`/api/Orders/${id}/status`, { status });
    return data;
  },
};

// 6. Funções do carrinho
export const cart = {
    getItems: async () => {
        const { data } = await apiClient.get('/api/Cart');
        return data;
    },
    addItem: async (productId: number, quantity: number) => {
        const { data } = await apiClient.post('/api/Cart/items', { productId, quantity });
        return data;
    },
    updateItem: async (cartItemId: number, quantity: number) => {
        const { data } = await apiClient.put(`/api/Cart/items/${cartItemId}`, { quantity });
        return data;
    },
    removeItem: async (cartItemId: number) => {
        const { data } = await apiClient.delete(`/api/Cart/items/${cartItemId}`);
        return data;
    }
}

export default apiClient;