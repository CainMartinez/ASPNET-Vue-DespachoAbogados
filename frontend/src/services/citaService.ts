import apiClient from './apiClient'
import type { CitaCreateDto, CitaDto, CitaUpdateDto } from '@/types/api.types'

/**
 * Servicio para operaciones de Citas
 */
export const citaService = {
  /**
   * Obtiene todas las citas
   */
  async getAll(): Promise<CitaDto[]> {
    const response = await apiClient.get<CitaDto[]>('/citas')
    return response.data
  },

  /**
   * Obtiene una cita por ID
   */
  async getById(id: number): Promise<CitaDto> {
    const response = await apiClient.get<CitaDto>(`/citas/${id}`)
    return response.data
  },

  /**
   * Obtiene citas por expediente
   */
  async getByExpedienteId(expedienteId: number): Promise<CitaDto[]> {
    const response = await apiClient.get<CitaDto[]>(`/citas/expediente/${expedienteId}`)
    return response.data
  },

  /**
   * Crea una cita
   */
  async create(payload: CitaCreateDto): Promise<CitaDto> {
    const response = await apiClient.post<CitaDto>('/citas', payload)
    return response.data
  },

  /**
   * Actualiza una cita
   */
  async update(id: number, payload: CitaUpdateDto): Promise<CitaDto> {
    const response = await apiClient.put<CitaDto>(`/citas/${id}`, payload)
    return response.data
  },

  /**
   * Elimina una cita
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/citas/${id}`)
  },

  /**
   * Marca una cita como completada o pendiente
   */
  async marcarCompletada(id: number, completada: boolean): Promise<CitaDto> {
    const response = await apiClient.patch<CitaDto>(`/citas/${id}/completada`, completada, {
      headers: { 'Content-Type': 'application/json' }
    })
    return response.data
  },

  /**
   * Obtiene citas pendientes
   */
  async getPendientes(): Promise<CitaDto[]> {
    const response = await apiClient.get<CitaDto[]>('/citas/pendientes')
    return response.data
  },

  /**
   * Obtiene citas por rango de fechas
   */
  async getByFechaRango(fechaInicio: string, fechaFin: string): Promise<CitaDto[]> {
    const response = await apiClient.get<CitaDto[]>('/citas/rango-fechas', {
      params: { fechaInicio, fechaFin }
    })
    return response.data
  }
}
