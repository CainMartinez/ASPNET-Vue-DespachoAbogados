import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ClienteDto } from '@/types/api.types'
import { clienteService } from '@/services/clienteService'

/**
 * Store de Pinia para gestión de estado de Clientes
 */
export const useClienteStore = defineStore('cliente', () => {
  // State
  const clientes = ref<ClienteDto[]>([])
  const clienteActual = ref<ClienteDto | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Actions
  async function fetchClientes() {
    loading.value = true
    error.value = null
    try {
      clientes.value = await clienteService.getAll()
    } catch (err: any) {
      error.value = err.message || 'Error al cargar clientes'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchClienteById(id: number) {
    loading.value = true
    error.value = null
    try {
      clienteActual.value = await clienteService.getById(id)
    } catch (err: any) {
      error.value = err.message || 'Error al cargar cliente'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function searchClientes(nombre: string) {
    loading.value = true
    error.value = null
    try {
      clientes.value = await clienteService.searchByName(nombre)
    } catch (err: any) {
      error.value = err.message || 'Error al buscar clientes'
      throw err
    } finally {
      loading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    clientes,
    clienteActual,
    loading,
    error,
    // Actions
    fetchClientes,
    fetchClienteById,
    searchClientes,
    clearError
  }
})
