import axios, { type AxiosInstance, type AxiosError } from 'axios'

// En producción (Docker), usar ruta relativa que nginx maneja
// En desarrollo local, usar variable de entorno o puerto directo
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

/**
 * Instancia configurada de Axios para consumir la API de AbogadosAPI
 */
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
})

/**
 * Interceptor de request - Agregar token JWT cuando se implemente autenticación
 */
apiClient.interceptors.request.use(
  (config) => {
    // TODO: Agregar token JWT cuando se implemente autenticación
    // const token = localStorage.getItem('auth_token')
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

/**
 * Interceptor de response - Manejo centralizado de errores
 */
apiClient.interceptors.response.use(
  (response) => {
    return response
  },
  (error: AxiosError) => {
    // Manejo de errores común
    if (error.response) {
      // El servidor respondió con un código de error
      switch (error.response.status) {
        case 400:
          console.error('Bad Request:', error.response.data)
          break
        case 401:
          console.error('No autorizado - Redirigir a login')
          // TODO: Redirigir a login cuando se implemente autenticación
          break
        case 403:
          console.error('Prohibido - No tienes permisos')
          break
        case 404:
          console.error('Recurso no encontrado')
          break
        case 500:
          console.error('Error del servidor')
          break
        default:
          console.error('Error:', error.response.status)
      }
    } else if (error.request) {
      // La petición se hizo pero no hubo respuesta
      console.error('Sin respuesta del servidor:', error.request)
    } else {
      // Error al configurar la petición
      console.error('Error:', error.message)
    }
    return Promise.reject(error)
  }
)

export default apiClient
