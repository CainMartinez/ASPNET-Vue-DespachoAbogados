import apiClient from './apiClient'
import type { DocumentoDto } from '@/types/api.types'

/**
 * Tipos de reportes disponibles
 */
export interface TipoReporte {
  id: string
  nombre: string
  descripcion: string
  icono: string
  endpoint: string
}

export const TIPOS_REPORTE: TipoReporte[] = [
  {
    id: 'clientes',
    nombre: 'Informe de Clientes',
    descripcion: 'Listado completo de clientes con información de contacto y expedientes asociados',
    icono: 'pi pi-users',
    endpoint: '/reportes/clientes'
  },
  {
    id: 'expedientes-por-estado',
    nombre: 'Expedientes por Estado',
    descripcion: 'Expedientes agrupados por estado con totalizaciones de actuaciones y citas',
    icono: 'pi pi-briefcase',
    endpoint: '/reportes/expedientes-por-estado'
  },
  {
    id: 'actuaciones-por-expediente',
    nombre: 'Actuaciones por Expediente',
    descripcion: 'Actuaciones agrupadas por expediente con subtotales por tipo y detalle cronológico',
    icono: 'pi pi-list',
    endpoint: '/reportes/actuaciones-por-expediente'
  }
]

/**
 * Servicio para generación y gestión de reportes
 */
export const documentoService = {
  /**
   * Obtiene el historial de documentos/reportes generados
   */
  async getAll(): Promise<DocumentoDto[]> {
    const response = await apiClient.get<DocumentoDto[]>('/documentos')
    return response.data
  },

  /**
   * Genera un reporte — el backend lo guarda en disco y crea el registro
   * Devuelve el DocumentoDto con el ID para posterior descarga
   */
  async generarReporte(endpoint: string): Promise<DocumentoDto> {
    const response = await apiClient.get<DocumentoDto>(endpoint)
    return response.data
  },

  /**
   * Descarga un reporte previamente generado
   */
  async descargarReporte(documentoId: number, nombreArchivo: string): Promise<void> {
    const response = await apiClient.get(`/reportes/descargar/${documentoId}`, {
      responseType: 'blob'
    })
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', nombreArchivo)
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
  },

  /**
   * Elimina un registro de documento
   */
  async delete(id: number): Promise<void> {
    await apiClient.delete(`/documentos/${id}`)
  }
}
