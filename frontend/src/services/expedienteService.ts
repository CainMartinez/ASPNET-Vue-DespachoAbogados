import apiClient from './apiClient'
import type { ExpedienteCambiarEstadoDto, ExpedienteCreateDto, ExpedienteDto, ExpedienteUpdateDto } from '@/types/api.types'

/**
 * Servicio para operaciones de Expedientes
 */
export const expedienteService = {
  /**
   * Obtiene todos los expedientes
   */
  async getAll(): Promise<ExpedienteDto[]> {
    const response = await apiClient.get<ExpedienteDto[]>('/expedientes')
    return response.data
  },

  /**
   * Obtiene expedientes por cliente
   */
  async getByClienteId(clienteId: number): Promise<ExpedienteDto[]> {
    const response = await apiClient.get<ExpedienteDto[]>(`/expedientes/cliente/${clienteId}`)
    return response.data
  },

  /**
   * Crea un expediente
   */
  async create(payload: ExpedienteCreateDto): Promise<ExpedienteDto> {
    const response = await apiClient.post<ExpedienteDto>('/expedientes', payload)
    return response.data
  },

  /**
   * Actualiza un expediente
   */
  async update(id: number, payload: ExpedienteUpdateDto): Promise<ExpedienteDto> {
    const response = await apiClient.put<ExpedienteDto>(`/expedientes/${id}`, payload)
    return response.data
  },

  /**
   * Cambia el estado de un expediente
   */
  async changeEstado(id: number, payload: ExpedienteCambiarEstadoDto): Promise<ExpedienteDto> {
    const response = await apiClient.patch<ExpedienteDto>(`/expedientes/${id}/estado`, payload)
    return response.data
  }
}
