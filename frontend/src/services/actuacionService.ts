import apiClient from './apiClient'
import type { ActuacionCreateDto, ActuacionDto, ActuacionUpdateDto } from '@/types/api.types'

/**
 * Servicio para operaciones de Actuaciones
 */
export const actuacionService = {
  /**
   * Obtiene todas las actuaciones
   */
  async getAll(): Promise<ActuacionDto[]> {
    const response = await apiClient.get<ActuacionDto[]>('/actuaciones')
    return response.data
  },

  /**
   * Obtiene una actuación por ID
   */
  async getById(id: number): Promise<ActuacionDto> {
    const response = await apiClient.get<ActuacionDto>(`/actuaciones/${id}`)
    return response.data
  },

  /**
   * Obtiene actuaciones por expediente
   */
  async getByExpedienteId(expedienteId: number): Promise<ActuacionDto[]> {
    const response = await apiClient.get<ActuacionDto[]>(`/actuaciones/expediente/${expedienteId}`)
    return response.data
  },

  /**
   * Crea una actuación
   */
  async create(payload: ActuacionCreateDto): Promise<ActuacionDto> {
    const response = await apiClient.post<ActuacionDto>('/actuaciones', payload)
    return response.data
  },

  /**
   * Actualiza una actuación
   */
  async update(id: number, payload: ActuacionUpdateDto): Promise<ActuacionDto> {
    const response = await apiClient.put<ActuacionDto>(`/actuaciones/${id}`, payload)
    return response.data
  },

  /**
   * Elimina una actuación
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/actuaciones/${id}`)
  },

  /**
   * Obtiene actuaciones por rango de fechas
   */
  async getByFechaRango(fechaInicio: string, fechaFin: string): Promise<ActuacionDto[]> {
    const response = await apiClient.get<ActuacionDto[]>('/actuaciones/rango-fechas', {
      params: { fechaInicio, fechaFin }
    })
    return response.data
  }
}
