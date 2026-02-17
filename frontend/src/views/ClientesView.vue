<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { useClienteStore } from '@/stores/clienteStore'
import { clienteService } from '@/services/clienteService'
import type { ClienteDto, ClienteCreateDto, ClienteUpdateDto } from '@/types/api.types'

import AppHeader from '@/components/AppHeader.vue'
import AppFooter from '@/components/AppFooter.vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import Card from 'primevue/card'
import Textarea from 'primevue/textarea'
import Avatar from 'primevue/avatar'
import Divider from 'primevue/divider'

const router = useRouter()
const toast = useToast()
const confirm = useConfirm()
const clienteStore = useClienteStore()

// State
const searchQuery = ref('')
const showDialog = ref(false)
const isEditing = ref(false)
const currentCliente = ref<ClienteDto | null>(null)
const viewMode = ref<'grid' | 'table'>('grid')

// Formulario
const formData = ref<ClienteCreateDto>({
  nombre: '',
  apellidos: '',
  dniCif: '',
  email: '',
  telefono: '',
  direccion: '',
  ciudad: '',
  codigoPostal: '',
  observaciones: ''
})

// Estadísticas calculadas
const stats = computed(() => {
  const total = clienteStore.clientes.length
  const conExpedientes = clienteStore.clientes.filter(c => (c.totalExpedientes || 0) > 0).length
  const totalExpedientes = clienteStore.clientes.reduce((sum, c) => sum + (c.totalExpedientes || 0), 0)
  const nuevosEsteMes = clienteStore.clientes.filter(c => {
    const fecha = new Date(c.fechaAlta)
    const ahora = new Date()
    return fecha.getMonth() === ahora.getMonth() && fecha.getFullYear() === ahora.getFullYear()
  }).length

  return { total, conExpedientes, totalExpedientes, nuevosEsteMes }
})

// Clientes filtrados
const clientesFiltrados = computed(() => {
  if (!searchQuery.value.trim()) {
    return clienteStore.clientes
  }

  const query = searchQuery.value.toLowerCase()
  return clienteStore.clientes.filter(cliente => {
    return (
      cliente.nombre.toLowerCase().includes(query) ||
      cliente.apellidos.toLowerCase().includes(query) ||
      cliente.dniCif.toLowerCase().includes(query) ||
      (cliente.email && cliente.email.toLowerCase().includes(query)) ||
      (cliente.telefono && cliente.telefono.includes(query)) ||
      (cliente.ciudad && cliente.ciudad.toLowerCase().includes(query))
    )
  })
})

onMounted(async () => {
  await cargarClientes()
})

async function cargarClientes() {
  try {
    await clienteStore.fetchClientes()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'No se pudieron cargar los clientes',
      life: 3000
    })
  }
}

function abrirDialogNuevo() {
  isEditing.value = false
  formData.value = {
    nombre: '',
    apellidos: '',
    dniCif: '',
    email: '',
    telefono: '',
    direccion: '',
    ciudad: '',
    codigoPostal: '',
    observaciones: ''
  }
  showDialog.value = true
}

function abrirDialogEditar(cliente: ClienteDto) {
  isEditing.value = true
  currentCliente.value = cliente
  formData.value = {
    nombre: cliente.nombre,
    apellidos: cliente.apellidos,
    dniCif: cliente.dniCif,
    email: cliente.email || '',
    telefono: cliente.telefono || '',
    direccion: cliente.direccion || '',
    ciudad: cliente.ciudad || '',
    codigoPostal: cliente.codigoPostal || '',
    observaciones: cliente.observaciones || ''
  }
  showDialog.value = true
}

async function guardarCliente() {
  try {
    if (isEditing.value && currentCliente.value?.id) {
      // Actualizar
      const updateData: ClienteUpdateDto = {
        nombre: formData.value.nombre,
        apellidos: formData.value.apellidos,
        email: formData.value.email,
        telefono: formData.value.telefono,
        direccion: formData.value.direccion,
        ciudad: formData.value.ciudad,
        codigoPostal: formData.value.codigoPostal,
        observaciones: formData.value.observaciones
      }
      await clienteService.update(currentCliente.value.id, updateData)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Cliente actualizado correctamente',
        life: 3000
      })
    } else {
      // Crear
      await clienteService.create(formData.value)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Cliente creado correctamente',
        life: 3000
      })
    }

    showDialog.value = false
    await cargarClientes()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'Error al guardar cliente',
      life: 3000
    })
  }
}

function eliminarCliente(cliente: ClienteDto) {
  confirm.require({
    message: `¿Está seguro de eliminar el cliente ${cliente.nombre} ${cliente.apellidos}?`,
    header: 'Confirmar eliminación',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí, eliminar',
    rejectLabel: 'Cancelar',
    accept: async () => {
      try {
        await clienteService.delete(cliente.id)
        toast.add({
          severity: 'success',
          summary: 'Éxito',
          detail: 'Cliente eliminado correctamente',
          life: 3000
        })
        await cargarClientes()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: error.response?.data?.mensaje || 'Error al eliminar cliente',
          life: 3000
        })
      }
    }
  })
}

function verExpedientes(cliente: ClienteDto) {
  router.push({
    name: 'expedientes',
    query: { clienteId: cliente.id.toString() }
  })
}
</script>

<template>
  <div class="clientes-view">
    <AppHeader />

    <div class="main-content">
      <!-- Hero Section -->
      <div class="page-hero">
        <div class="hero-content">
          <div class="hero-text">
            <h1 class="page-title">
              <i class="pi pi-users"></i>
              Cartera de Clientes
            </h1>
            <p class="page-subtitle">Gestión integral de clientes y relaciones profesionales</p>
          </div>
          <Button
            label="Nuevo Cliente"
            icon="pi pi-user-plus"
            @click="abrirDialogNuevo"
            class="hero-btn"
            size="large"
            raised
            rounded
          />
        </div>
      </div>

      <!-- Estadísticas -->
      <div class="stats-section">
        <Card class="stat-card fade-in">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon total">
                <i class="pi pi-users"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.total }}</h3>
                <p class="stat-label">Total Clientes</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card fade-in" style="animation-delay: 0.1s">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon active">
                <i class="pi pi-briefcase"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.conExpedientes }}</h3>
                <p class="stat-label">Con Expedientes</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card fade-in" style="animation-delay: 0.2s">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon expedientes">
                <i class="pi pi-folder-open"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.totalExpedientes }}</h3>
                <p class="stat-label">Expedientes Totales</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card fade-in" style="animation-delay: 0.3s">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon new">
                <i class="pi pi-user-plus"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.nuevosEsteMes }}</h3>
                <p class="stat-label">Nuevos este mes</p>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <!-- Barra de búsqueda y filtros -->
      <Card class="filters-card">
        <template #content>
          <div class="filters-bar">
            <div class="search-section">
              <span class="p-input-icon-left search-wrapper">
                <i class="pi pi-search"></i>
                <InputText
                  v-model="searchQuery"
                  placeholder="🔍 Buscar cliente por nombre, DNI/CIF, email..."
                  class="search-input-custom"
                />
              </span>
            </div>

            <div class="view-toggle">
              <Button
                icon="pi pi-th-large"
                :class="{ 'active': viewMode === 'grid' }"
                @click="viewMode = 'grid'"
                :text="viewMode !== 'grid'"
                :raised="viewMode === 'grid'"
                rounded
                v-tooltip.top="'Vista en tarjetas'"
              />
              <Button
                icon="pi pi-list"
                :class="{ 'active': viewMode === 'table' }"
                @click="viewMode = 'table'"
                :text="viewMode !== 'table'"
                :raised="viewMode === 'table'"
                rounded
                v-tooltip.top="'Vista en tabla'"
              />
            </div>
          </div>
        </template>
      </Card>

      <!-- Grid View -->
      <div v-if="viewMode === 'grid'" class="clients-grid">
        <Card v-for="cliente in clientesFiltrados" :key="cliente.id" class="client-card">
          <template #content>
            <div class="client-card-content">
              <div class="client-header">
                <Avatar
                  :label="cliente.nombre.charAt(0)"
                  class="client-avatar"
                  size="large"
                  shape="circle"
                />
                <div class="client-info">
                  <h3 class="client-name">{{ cliente.nombre }}</h3>
                  <p class="client-dni">{{ cliente.dniCif }}</p>
                </div>
              </div>

              <Divider />

              <div class="client-details">
                <div class="detail-row">
                  <i class="pi pi-envelope"></i>
                  <span>{{ cliente.email || 'Sin email' }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-phone"></i>
                  <span>{{ cliente.telefono || 'Sin teléfono' }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-map-marker"></i>
                  <span>{{ cliente.direccion || 'Sin dirección' }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-folder-open detail-icon"></i>
                  <span>{{ cliente.totalExpedientes || 0 }} expediente(s)</span>
                </div>
              </div>

              <Divider />

              <div class="client-actions">
                <Button
                  icon="pi pi-eye"
                  label="Ver"
                  @click="verExpedientes(cliente)"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-view"
                />
                <Button
                  icon="pi pi-pencil"
                  label="Editar"
                  @click="abrirDialogEditar(cliente)"
                  text
                  raised
                  severity="info"
                  size="small"
                  class="action-btn action-btn-edit"
                />
                <Button
                  icon="pi pi-trash"
                  label="Eliminar"
                  @click="eliminarCliente(cliente)"
                  severity="danger"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-delete"
                />
              </div>
            </div>
          </template>
        </Card>

        <div v-if="clientesFiltrados.length === 0" class="empty-state">
          <i class="pi pi-users" style="font-size: 4rem; color: var(--accent-gold); opacity: 0.5;"></i>
          <h3>No hay clientes registrados</h3>
          <p>Comienza agregando tu primer cliente</p>
          <Button
            label="Agregar Cliente"
            icon="pi pi-user-plus"
            @click="abrirDialogNuevo"
            class="mt-3"
            raised
            rounded
            size="large"
          />
        </div>
      </div>

      <!-- Table View -->
      <Card v-if="viewMode === 'table'" class="table-card">
        <template #content>
          <DataTable
            :value="clientesFiltrados"
            :paginator="true"
            :rows="10"
            :loading="clienteStore.loading"
            stripedRows
            responsiveLayout="scroll"
            class="professional-table"
            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
            :rowsPerPageOptions="[5, 10, 25, 50]"
            currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} clientes"
          >
            <template #empty>
              <div class="text-center p-4">
                <i class="pi pi-inbox" style="font-size: 3rem; color: var(--accent-gold); opacity: 0.5;"></i>
                <p style="margin-top: 1rem; color: var(--text-secondary);">No hay clientes para mostrar</p>
              </div>
            </template>

            <Column header="Cliente" style="min-width: 250px">
              <template #body="{ data }">
                <div class="client-cell">
                  <Avatar
                    :label="data.nombre.charAt(0)"
                    class="mr-2"
                    size="normal"
                    shape="circle"
                    style="background-color: var(--accent-gold); color: var(--primary-navy);"
                  />
                  <div>
                    <div class="client-name-table">{{ data.nombre }} {{ data.apellidos }}</div>
                    <div class="client-dni-table">{{ data.dniCif }}</div>
                  </div>
                </div>
              </template>
            </Column>

            <Column field="email" header="Email" style="min-width: 200px">
              <template #body="{ data }">
                <span v-if="data.email" class="detail-text">
                  <i class="pi pi-envelope mr-1"></i>{{ data.email }}
                </span>
                <span v-else class="text-muted">Sin email</span>
              </template>
            </Column>

            <Column field="telefono" header="Teléfono" style="min-width: 150px">
              <template #body="{ data }">
                <span v-if="data.telefono" class="detail-text">
                  <i class="pi pi-phone mr-1"></i>{{ data.telefono }}
                </span>
                <span v-else class="text-muted">Sin teléfono</span>
              </template>
            </Column>

            <Column field="ciudad" header="Ciudad" style="min-width: 150px">
              <template #body="{ data }">
                <span v-if="data.ciudad" class="detail-text">{{ data.ciudad }}</span>
                <span v-else class="text-muted">-</span>
              </template>
            </Column>

            <Column header="Expedientes" style="min-width: 120px; text-align: center">
              <template #body="{ data }">
                <span class="expedientes-count">
                  <i class="pi pi-folder-open"></i>
                  {{ data.totalExpedientes || 0 }}
                </span>
              </template>
            </Column>

            <Column header="Acciones" style="min-width: 200px">
              <template #body="{ data }">
                <div class="action-buttons">
                  <Button
                    icon="pi pi-eye"
                    @click="verExpedientes(data)"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Ver expedientes'"
                  />
                  <Button
                    icon="pi pi-pencil"
                    @click="abrirDialogEditar(data)"
                    text
                    raised
                    rounded
                    severity="info"
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Editar'"
                  />
                  <Button
                    icon="pi pi-trash"
                    @click="eliminarCliente(data)"
                    severity="danger"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Eliminar'"
                  />
                </div>
              </template>
            </Column>
          </DataTable>
        </template>
      </Card>
    </div>

    <!-- Dialog para crear/editar -->
    <Dialog
      v-model:visible="showDialog"
      :header="isEditing ? 'Editar Cliente' : 'Nuevo Cliente'"
      :modal="true"
      :style="{ width: '750px', maxWidth: '95vw' }"
      :dismissableMask="true"
      class="professional-dialog"
      :contentStyle="{ padding: '2rem' }"
    >
      <div class="dialog-content">
        <div class="form-section">
          <h3 class="section-title">
            <i class="pi pi-user"></i>
            Información Personal
          </h3>
          <div class="form-grid">
            <div class="form-field">
              <label for="nombre" class="required">Nombre</label>
              <InputText 
                id="nombre" 
                v-model="formData.nombre" 
                placeholder="Nombre del cliente"
                class="w-full"
              />
            </div>
            <div class="form-field">
              <label for="apellidos" class="required">Apellidos</label>
              <InputText 
                id="apellidos" 
                v-model="formData.apellidos" 
                placeholder="Apellidos del cliente"
                class="w-full"
              />
            </div>
          </div>

          <div class="form-grid">
            <div class="form-field">
              <label for="dniCif" class="required">DNI/CIF</label>
              <InputText 
                id="dniCif" 
                v-model="formData.dniCif" 
                :disabled="isEditing"
                placeholder="00000000X o B00000000"
                class="w-full"
              />
              <small v-if="isEditing" class="text-muted">El DNI/CIF no puede modificarse</small>
            </div>
            <div class="form-field">
              <label for="email">Email</label>
              <InputText 
                id="email" 
                v-model="formData.email" 
                type="email"
                placeholder="correo@ejemplo.com"
                class="w-full"
              />
            </div>
          </div>

          <div class="form-field">
            <label for="telefono">Teléfono</label>
            <InputText 
              id="telefono" 
              v-model="formData.telefono" 
              placeholder="+34 600 000 000"
              class="w-full"
            />
          </div>
        </div>

        <Divider />

        <div class="form-section">
          <h3 class="section-title">
            <i class="pi pi-map-marker"></i>
            Dirección
          </h3>
          <div class="form-field">
            <label for="direccion">Dirección completa</label>
            <InputText 
              id="direccion" 
              v-model="formData.direccion" 
              placeholder="Calle, número, piso, puerta..."
              class="w-full"
            />
          </div>

          <div class="form-grid">
            <div class="form-field">
              <label for="ciudad">Ciudad</label>
              <InputText 
                id="ciudad" 
                v-model="formData.ciudad" 
                placeholder="Madrid"
                class="w-full"
              />
            </div>
            <div class="form-field">
              <label for="codigoPostal">Código Postal</label>
              <InputText 
                id="codigoPostal" 
                v-model="formData.codigoPostal" 
                placeholder="28001"
                class="w-full"
              />
            </div>
          </div>
        </div>

        <Divider />

        <div class="form-section">
          <h3 class="section-title">
            <i class="pi pi-file-edit"></i>
            Información Adicional
          </h3>
          <div class="form-field">
            <label for="observaciones">Observaciones</label>
            <Textarea 
              id="observaciones" 
              v-model="formData.observaciones" 
              :rows="4"
              placeholder="Notas adicionales sobre el cliente..."
              class="w-full"
            />
          </div>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <Button
            :icon="isEditing ? 'pi pi-check' : 'pi pi-plus'"
            @click="guardarCliente"
            :loading="clienteStore.loading"
            rounded
            raised
            size="large"
            class="dialog-action-btn"
            v-tooltip.top="isEditing ? 'Actualizar' : 'Crear Cliente'"
            aria-label="Guardar cliente"
          />
        </div>
      </template>
    </Dialog>
  </div>

  <AppFooter />
</template>

<style scoped>
.clientes-view {
  min-height: 100vh;
  background-color: var(--bg-secondary);
  display: flex;
  flex-direction: column;
}

.main-content {
  max-width: 1600px;
  width: 100%;
  margin: 0 auto;
  padding: 2rem;
  flex: 1;
}

/* Hero Section */
.page-hero {
  background: linear-gradient(135deg, var(--primary-brown) 0%, var(--secondary-brown) 100%);
  border-radius: 12px;
  padding: 2.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 4px 16px rgba(93, 78, 55, 0.2);
  position: relative;
  overflow: hidden;
}

.page-hero::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, var(--accent-gold-light) 0%, transparent 70%);
  opacity: 0.1;
  border-radius: 50%;
}

.hero-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
  z-index: 1;
}

.hero-text {
  flex: 1;
}

.page-title {
  font-family: 'Playfair Display', serif;
  font-size: 2.5rem;
  font-weight: 700;
  color: white;
  margin: 0 0 0.5rem 0;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.page-title i {
  color: var(--accent-gold);
  font-size: 2rem;
}

.page-subtitle {
  font-size: 1.1rem;
  color: rgba(255, 255, 255, 0.9);
  margin: 0;
}

.hero-btn {
  background: var(--accent-gold) !important;
  border-color: var(--accent-gold) !important;
  color: var(--primary-brown) !important;
  font-weight: 700;
  padding: 0.875rem 2.5rem !important;
  font-size: 1.1rem !important;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.3);
}

.hero-btn :deep(.p-button-icon-left) {
  margin-right: 0.6rem;
}

.hero-btn:hover {
  background: var(--accent-gold-light) !important;
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(212, 175, 55, 0.5);
}

/* Stats Section */
.stats-section {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

@media (max-width: 1200px) {
  .stats-section {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .stats-section {
    grid-template-columns: 1fr;
  }
}

.stat-card {
  background: var(--bg-card);
  border: 2px solid var(--border-color);
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(93, 78, 55, 0.1);
  transition: all 0.2s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(93, 78, 55, 0.15);
  border-color: var(--accent-gold);
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 0.5rem;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.8rem;
  flex-shrink: 0;
}

.stat-icon.total {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
}

.stat-icon.active {
  background: linear-gradient(135deg, var(--accent-gold), var(--accent-gold-dark));
  color: var(--primary-brown);
}

.stat-icon.expedientes {
  background: linear-gradient(135deg, var(--secondary-brown), var(--primary-brown));
  color: var(--accent-gold);
}

.stat-icon.new {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
  color: white;
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 0.25rem 0;
  font-family: 'Playfair Display', serif;
}

.stat-label {
  font-size: 0.9rem;
  color: var(--text-secondary);
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-weight: 600;
}

/* Filters Card */
.filters-card {
  background: var(--bg-card);
  border: 2px solid var(--border-color);
  border-radius: 12px;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(93, 78, 55, 0.1);
}

.filters-card :deep(.p-card-body) {
  padding: 1.5rem;
}

.filters-card :deep(.p-card-content) {
  padding: 0;
}

.filters-bar {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2rem;
  flex-wrap: wrap;
}

.search-section {
  display: flex;
  gap: 0.75rem;
  flex: 1;
  min-width: 300px;
  justify-content: center;
}

.search-wrapper {
  flex: 1;
  max-width: 900px;
}

.search-input-custom {
  width: 100%;
  background: var(--bg-primary);
  border: 2px solid var(--border-color);
  color: var(--text-primary);
  font-size: 1rem;
}

.search-input-custom::placeholder {
  color: var(--text-muted);
  font-style: italic;
}

.search-input-custom:focus {
  border-color: var(--accent-gold);
  box-shadow: 0 0 0 3px rgba(212, 175, 55, 0.15);
}

.view-toggle {
  display: flex;
  gap: 0.5rem;
}

.view-toggle Button {
  transition: all 0.2s ease;
  width: 3rem;
  height: 3rem;
}

.view-toggle Button.active {
  background: var(--accent-gold) !important;
  border-color: var(--accent-gold) !important;
  color: var(--primary-brown) !important;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.3);
  transform: translateY(-2px);
}

.view-toggle Button:not(.active) {
  color: var(--text-secondary);
}

.view-toggle Button:not(.active):hover {
  color: var(--primary-brown);
  transform: translateY(-1px);
}

/* Grid View */
.clients-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  animation: fadeIn 0.5s ease;
}

@media (max-width: 1400px) {
  .clients-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .clients-grid {
    grid-template-columns: 1fr;
  }
}

.client-card {
  background: var(--bg-card);
  border: 2px solid var(--border-color);
  border-radius: 16px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.08);
  overflow: hidden;
}

.client-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(93, 78, 55, 0.18);
  border-color: var(--accent-gold);
}

.client-card-content {
  padding: 1.75rem;
}

.client-header {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  margin-bottom: 0;
  padding-bottom: 1.25rem;
}

.client-avatar {
  background: linear-gradient(135deg, var(--accent-gold), var(--accent-gold-light)) !important;
  color: var(--primary-brown) !important;
  font-weight: 700;
  font-size: 1.75rem;
  width: 70px !important;
  height: 70px !important;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.3);
}

.client-info {
  flex: 1;
}

.client-name {
  font-size: 1.35rem;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 0.35rem 0;
  font-family: 'Playfair Display', serif;
  line-height: 1.2;
}

.client-dni {
  font-size: 0.95rem;
  color: var(--accent-gold);
  margin: 0;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.client-details {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.25rem 0;
}

.detail-row {
  display: flex;
  align-items: center;
  gap: 1rem;
  color: var(--text-secondary);
  font-size: 0.95rem;
  padding: 0.5rem;
  border-radius: 8px;
  transition: background 0.2s ease;
}

.detail-row:hover {
  background: var(--bg-secondary);
}

.detail-row i {
  color: var(--accent-gold);
  width: 20px;
  flex-shrink: 0;
  font-size: 1.1rem;
}

.detail-row .detail-icon {
  /* Quitar cualquier background del icono de carpeta */
  background: transparent !important;
}

.client-actions {
  display: flex;
  gap: 0.85rem;
  margin-top: 0;
  padding-top: 1.5rem;
  border-top: 2px solid var(--border-color);
  align-items: stretch;
}

.action-btn {
  flex: 1;
  min-width: 110px;
  height: 42px;
  padding: 0.55rem 1rem;
  transition: all 0.3s ease;
  font-weight: 700;
  font-size: 0.85rem;
  letter-spacing: 0.2px;
  border-radius: 12px !important;
}

.action-btn-view {
  background: linear-gradient(135deg, var(--accent-gold), var(--accent-gold-light)) !important;
  color: var(--primary-brown) !important;
  border: none !important;
  box-shadow: 0 2px 8px rgba(212, 175, 55, 0.3);
}

.action-btn-view:hover {
  background: linear-gradient(135deg, var(--accent-gold-light), #F0D875) !important;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(212, 175, 55, 0.4) !important;
}

.action-btn-edit {
  background: linear-gradient(135deg, var(--primary-brown), var(--secondary-brown)) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 2px 8px rgba(93, 78, 55, 0.3);
}

.action-btn-edit:hover {
  background: linear-gradient(135deg, var(--secondary-brown), var(--light-brown)) !important;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(93, 78, 55, 0.4) !important;
}

.action-btn-delete {
  background: linear-gradient(135deg, #dc2626, #ef4444) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 2px 8px rgba(220, 38, 38, 0.3);
}

.action-btn-delete:hover {
  background: linear-gradient(135deg, #b91c1c, #dc2626) !important;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(220, 38, 38, 0.4) !important;
}

/* Table View */
.table-card {
  background: var(--bg-card);
  border: 2px solid var(--border-color);
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(93, 78, 55, 0.1);
  animation: fadeIn 0.5s ease;
}

.table-card :deep(.p-card-body) {
  padding: 1.5rem;
}

.table-card :deep(.p-card-content) {
  padding: 0;
}

.professional-table {
  background: transparent;
}

.professional-table :deep(.p-datatable-wrapper) {
  border-radius: 8px;
  overflow: hidden;
}

.professional-table :deep(th) {
  padding: 1rem 1.25rem !important;
}

.professional-table :deep(td) {
  padding: 1rem 1.25rem !important;
}

.client-cell {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.client-name-table {
  font-weight: 600;
  color: var(--text-primary);
  font-size: 1.05rem;
}

.client-dni-table {
  font-size: 0.9rem;
  color: var(--accent-gold);
  margin-top: 0.15rem;
}

.detail-text {
  color: var(--text-secondary);
}

.detail-text i {
  color: var(--accent-gold);
}

.expedientes-count {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--text-secondary);
  font-weight: 600;
}

.expedientes-count i {
  color: var(--accent-gold);
  font-size: 1.1rem;
}

.text-muted {
  color: #64748b;
  font-style: italic;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
}

.table-action-btn {
  width: 2.5rem;
  height: 2.5rem;
  transition: all 0.2s ease;
}

.table-action-btn:first-child {
  background: var(--accent-gold) !important;
  color: var(--primary-brown) !important;
  border: none !important;
}

.table-action-btn:first-child:hover {
  background: var(--accent-gold-light) !important;
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.4) !important;
}

.table-action-btn:nth-child(2) {
  background: var(--primary-brown) !important;
  color: white !important;
  border: none !important;
}

.table-action-btn:nth-child(2):hover {
  background: var(--secondary-brown) !important;
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.4) !important;
}

.table-action-btn:nth-child(3) {
  background: #dc2626 !important;
  color: white !important;
  border: none !important;
}

.table-action-btn:nth-child(3):hover {
  background: #b91c1c !important;
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(220, 38, 38, 0.4) !important;
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  grid-column: 1 / -1;
}

.empty-state h3 {
  color: var(--text-primary);
  font-family: 'Playfair Display', serif;
  font-size: 1.5rem;
  margin: 1rem 0 0.5rem 0;
}

.empty-state p {
  color: var(--text-secondary);
  margin: 0 0 1.5rem 0;
}

/* Dialog Styles */
.professional-dialog :deep(.p-dialog-content) {
  padding: 2rem !important;
}

.professional-dialog :deep(.p-dialog-footer) {
  display: flex !important;
  justify-content: center !important;
  align-items: center !important;
  padding: 1.5rem 2rem !important;
  background: var(--bg-secondary);
  border-top: 1px solid var(--border-color);
  gap: 1.5rem;
}

.dialog-action-btn {
  width: 3.25rem;
  height: 3.25rem;
  border-radius: 14px !important;
  background: linear-gradient(135deg, var(--accent-gold), var(--accent-gold-light)) !important;
  color: var(--primary-brown) !important;
  border: none !important;
  box-shadow: 0 6px 16px rgba(212, 175, 55, 0.35);
  transition: all 0.2s ease;
}

.dialog-action-btn:hover {
  background: linear-gradient(135deg, var(--accent-gold-light), #F0D875) !important;
  transform: translateY(-2px);
  box-shadow: 0 10px 22px rgba(212, 175, 55, 0.45);
}

.dialog-content {
  padding: 0;
}

.form-section {
  margin-bottom: 2rem;
}

.form-section:last-child {
  margin-bottom: 0;
}

.section-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.3rem;
  color: var(--text-primary);
  margin: 0 0 1.5rem 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid var(--accent-gold);
}

.section-title i {
  color: var(--accent-gold);
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
  grid-column: span 2;
}

.form-grid .form-field:has(#nombre),
.form-grid .form-field:has(#apellidos),
.form-grid .form-field:has(#dniCif),
.form-grid .form-field:has(#email),
.form-grid .form-field:has(#ciudad),
.form-grid .form-field:has(#codigoPostal) {
  grid-column: span 1;
}

.form-field label {
  font-weight: 600;
  color: var(--text-primary);
  font-size: 0.95rem;
}

.form-field label.required::after {
  content: ' *';
  color: #ef4444;
}

.form-field small {
  font-size: 0.85rem;
  color: var(--text-secondary);
  margin-top: -0.25rem;
}

/* Animations */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in {
  animation: fadeIn 0.5s ease forwards;
}

/* Responsive */
@media (max-width: 768px) {
  .main-content {
    padding: 1rem;
  }

  .page-title {
    font-size: 1.75rem;
  }

  .hero-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
  }

  .filters-bar {
    flex-direction: column;
    align-items: stretch;
  }

  .search-section {
    width: 100%;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .form-grid .form-field {
    grid-column: span 1;
  }
}

.w-full {
  width: 100%;
}

.mr-1 {
  margin-right: 0.25rem;
}

.mr-2 {
  margin-right: 0.5rem;
}

.mt-3 {
  margin-top: 1rem;
}

.text-center {
  text-align: center;
}

.p-4 {
  padding: 1.5rem;
}
</style>
