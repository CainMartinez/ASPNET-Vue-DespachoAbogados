/**
 * DTOs para la API de AbogadosAPI
 * Deben coincidir exactamente con los DTOs del backend
 */

/** ==================== CLIENTE ==================== */

export interface ClienteDto {
  id: number
  nombre: string
  apellidos: string
  dniCif: string
  telefono?: string
  email?: string
  direccion?: string
  ciudad?: string
  codigoPostal?: string
  observaciones?: string
  fechaAlta: string
  fechaModificacion?: string
  totalExpedientes: number
}

export interface ClienteCreateDto {
  nombre: string
  apellidos: string
  dniCif: string
  telefono?: string
  email?: string
  direccion?: string
  ciudad?: string
  codigoPostal?: string
  observaciones?: string
}

export interface ClienteUpdateDto {
  nombre?: string
  apellidos?: string
  telefono?: string
  email?: string
  direccion?: string
  ciudad?: string
  codigoPostal?: string
  observaciones?: string
}

/** ==================== EXPEDIENTE ==================== */

export enum EstadoExpediente {
  Abierto = 0,
  EnTramite = 1,
  Suspendido = 2,
  Archivado = 3,
  Cerrado = 4
}

export interface ExpedienteDto {
  id: number
  clienteId: number
  clienteNombre?: string
  numeroExpediente: string
  asunto: string
  descripcion?: string
  tipoExpediente: string
  estado: EstadoExpediente
  juzgadoTribunal?: string
  numeroProcedimiento?: string
  fechaApertura: string
  fechaCierre?: string
  fechaModificacion?: string
  observaciones?: string
  totalActuaciones: number
  totalCitas: number
  totalDocumentos: number
}

export interface ExpedienteCreateDto {
  clienteId: number
  numeroExpediente: string
  asunto: string
  descripcion?: string
  tipoExpediente: string
  juzgadoTribunal?: string
  numeroProcedimiento?: string
  fechaApertura: string
  observaciones?: string
}

export interface ExpedienteUpdateDto {
  numeroExpediente: string
  asunto?: string
  descripcion?: string
  tipoExpediente?: string
  clienteId: number
  juzgadoTribunal?: string
  numeroProcedimiento?: string
  fechaCierre?: string
  observaciones?: string
}

export interface ExpedienteCambiarEstadoDto {
  estado: EstadoExpediente
}

/** ==================== ACTUACION ==================== */

export interface ActuacionDto {
  id: number
  expedienteId: number
  expedienteNumero?: string
  expedienteAsunto?: string
  fechaActuacion: string
  tipoActuacion: string
  descripcion: string
  resultado?: string
  responsable?: string
  observaciones?: string
  fechaRegistro: string
  fechaModificacion?: string
}

export interface ActuacionCreateDto {
  expedienteId: number
  fechaActuacion: string
  tipoActuacion: string
  descripcion: string
  resultado?: string
  responsable?: string
  observaciones?: string
}

export interface ActuacionUpdateDto {
  fechaActuacion: string
  tipoActuacion: string
  descripcion: string
  resultado?: string
  responsable?: string
  observaciones?: string
}

/** ==================== CITA ==================== */

export interface CitaDto {
  id: number
  expedienteId: number
  expedienteNumero?: string
  titulo: string
  descripcion?: string
  fechaInicio: string
  fechaFin: string
  lugar?: string
  tipoCita: string
  participantes?: string
  completada: boolean
  observaciones?: string
  fechaCreacion: string
  fechaModificacion?: string
}

export interface CitaCreateDto {
  expedienteId: number
  titulo: string
  descripcion?: string
  fechaInicio: string
  fechaFin: string
  lugar?: string
  tipoCita: string
  participantes?: string
  observaciones?: string
}

export interface CitaUpdateDto {
  titulo?: string
  descripcion?: string
  fechaInicio?: string
  fechaFin?: string
  lugar?: string
  tipoCita?: string
  participantes?: string
  observaciones?: string
}

/** ==================== DOCUMENTO (Historial de Reportes) ==================== */

export interface DocumentoDto {
  id: number
  expedienteId?: number | null
  expedienteNumero?: string
  expedienteAsunto?: string
  nombreArchivo: string
  rutaArchivo: string
  tipoDocumento: string
  tamanoBytes: number
  tamanoFormateado?: string
  extension?: string
  cargadoPor?: string
  descripcion?: string
  observaciones?: string
  fechaCarga: string
  fechaModificacion?: string
}
