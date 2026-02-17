import apiClient from './apiClient'
import type {
  ClienteDto,
  ClienteCreateDto,
  ClienteUpdateDto,
  ExpedienteDto
} from '@/types/api.types'

/**
 * Servicio para operaciones CRUD de Clientes
 */
export const clienteService = {
  /**
   * Obtiene todos los clientes
   */
  async getAll(): Promise<ClienteDto[]> {
    const response = await apiClient.get<ClienteDto[]>('/clientes')
    return response.data
  },

  /**
   * Obtiene un cliente por ID
   */
  async getById(id: number): Promise<ClienteDto> {
    const response = await apiClient.get<ClienteDto>(`/clientes/${id}`)
    return response.data
  },

  /**
   * Crea un nuevo cliente
   */
  async create(cliente: ClienteCreateDto): Promise<ClienteDto> {
    const response = await apiClient.post<ClienteDto>('/clientes', cliente)
    return response.data
  },

  /**
   * Actualiza un cliente existente
   */
  async update(id: number, cliente: ClienteUpdateDto): Promise<ClienteDto> {
    const response = await apiClient.put<ClienteDto>(`/clientes/${id}`, cliente)
    return response.data
  },

  /**
   * Elimina un cliente
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/clientes/${id}`)
  },

  /**
   * Busca clientes por nombre
   */
  async searchByName(nombre: string): Promise<ClienteDto[]> {
    const response = await apiClient.get<ClienteDto[]>('/clientes/buscar', {
      params: { nombre }
    })
    return response.data
  },

  /**
   * Obtiene los expedientes de un cliente
   */
  async getExpedientes(id: number): Promise<ExpedienteDto[]> {
    const response = await apiClient.get<ExpedienteDto[]>(`/expedientes/cliente/${id}`)
    return response.data
  }
}
