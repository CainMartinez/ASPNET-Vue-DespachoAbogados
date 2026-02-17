<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import AppHeader from '@/components/AppHeader.vue'
import AppFooter from '@/components/AppFooter.vue'
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Divider from 'primevue/divider'
import Dialog from 'primevue/dialog'
import Dropdown from 'primevue/dropdown'
import Textarea from 'primevue/textarea'
import { expedienteService } from '@/services/expedienteService'
import { clienteService } from '@/services/clienteService'
import {
  EstadoExpediente,
  type ClienteDto,
  type ExpedienteCambiarEstadoDto,
  type ExpedienteCreateDto,
  type ExpedienteDto,
  type ExpedienteUpdateDto
} from '@/types/api.types'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const confirm = useConfirm()

const searchQuery = ref('')
const viewMode = ref<'grid' | 'table'>('grid')
const expedientes = ref<ExpedienteDto[]>([])
const loading = ref(false)
const showDialog = ref(false)
const showDetalleDialog = ref(false)
const isEditing = ref(false)
const currentExpediente = ref<ExpedienteDto | null>(null)
const detalleExpediente = ref<ExpedienteDto | null>(null)
const clientes = ref<ClienteDto[]>([])
const formData = ref<ExpedienteCreateDto>({
  clienteId: 0,
  numeroExpediente: '',
  asunto: '',
  descripcion: '',
  tipoExpediente: '',
  juzgadoTribunal: '',
  numeroProcedimiento: '',
  fechaApertura: new Date().toISOString(),
  observaciones: ''
})

const estadoLabel: Record<EstadoExpediente, string> = {
  [EstadoExpediente.Abierto]: 'Abierto',
  [EstadoExpediente.EnTramite]: 'En tramite',
  [EstadoExpediente.Suspendido]: 'Suspendido',
  [EstadoExpediente.Archivado]: 'Archivado',
  [EstadoExpediente.Cerrado]: 'Cerrado'
}

const getEstadoLabel = (estado: EstadoExpediente) => {
  return estadoLabel[estado]
}

const expedientesFiltrados = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()
  if (!query) {
    return expedientes.value
  }

  return expedientes.value.filter(item => {
    return (
      item.numeroExpediente.toLowerCase().includes(query) ||
      (item.clienteNombre || '').toLowerCase().includes(query) ||
      item.asunto.toLowerCase().includes(query) ||
      item.tipoExpediente.toLowerCase().includes(query) ||
      (item.juzgadoTribunal || '').toLowerCase().includes(query) ||
      (item.numeroProcedimiento || '').toLowerCase().includes(query)
    )
  })
})

const stats = computed(() => {
  const total = expedientes.value.length
  const abiertos = expedientes.value.filter(item => item.estado === EstadoExpediente.Abierto).length
  const enTramite = expedientes.value.filter(item => item.estado === EstadoExpediente.EnTramite).length
  const cerrados = expedientes.value.filter(item => item.estado === EstadoExpediente.Cerrado).length

  return { total, abiertos, enTramite, cerrados }
})

const clienteFiltrado = computed(() => {
  const clienteId = route.query.clienteId ? Number(route.query.clienteId) : null
  if (!clienteId) return null
  const cliente = clientes.value.find(c => c.id === clienteId)
  return cliente ? `${cliente.nombre} ${cliente.apellidos}` : `Cliente #${clienteId}`
})

const limpiarFiltroCliente = () => {
  router.replace({ query: {} })
}

const formatDate = (value?: string) => {
  if (!value) {
    return '-'
  }
  return new Date(value).toLocaleDateString('es-ES')
}

const loadExpedientes = async () => {
  loading.value = true
  try {
    const clienteId = route.query.clienteId ? Number(route.query.clienteId) : null
    expedientes.value = clienteId
      ? await expedienteService.getByClienteId(clienteId)
      : await expedienteService.getAll()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudieron cargar los expedientes',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

const loadClientes = async () => {
  try {
    clientes.value = await clienteService.getAll()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudieron cargar los clientes',
      life: 3000
    })
  }
}

const abrirDialogNuevo = () => {
  isEditing.value = false
  currentExpediente.value = null
  formData.value = {
    clienteId: Number(route.query.clienteId) || 0,
    numeroExpediente: '',
    asunto: '',
    descripcion: '',
    tipoExpediente: '',
    juzgadoTribunal: '',
    numeroProcedimiento: '',
    fechaApertura: new Date().toISOString(),
    observaciones: ''
  }
  showDialog.value = true
}

const abrirDialogEditar = (expediente: ExpedienteDto) => {
  isEditing.value = true
  currentExpediente.value = expediente
  formData.value = {
    clienteId: expediente.clienteId,
    numeroExpediente: expediente.numeroExpediente,
    asunto: expediente.asunto,
    descripcion: expediente.descripcion || '',
    tipoExpediente: expediente.tipoExpediente,
    juzgadoTribunal: expediente.juzgadoTribunal || '',
    numeroProcedimiento: expediente.numeroProcedimiento || '',
    fechaApertura: expediente.fechaApertura,
    observaciones: expediente.observaciones || ''
  }
  showDialog.value = true
}

const abrirDetalle = (expediente: ExpedienteDto) => {
  detalleExpediente.value = expediente
  showDetalleDialog.value = true
}

const guardarExpediente = async () => {
  try {
    if (isEditing.value && currentExpediente.value) {
      const payload: ExpedienteUpdateDto = {
        numeroExpediente: formData.value.numeroExpediente,
        asunto: formData.value.asunto,
        descripcion: formData.value.descripcion,
        tipoExpediente: formData.value.tipoExpediente,
        clienteId: formData.value.clienteId,
        juzgadoTribunal: formData.value.juzgadoTribunal,
        numeroProcedimiento: formData.value.numeroProcedimiento,
        observaciones: formData.value.observaciones
      }
      await expedienteService.update(currentExpediente.value.id, payload)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Expediente actualizado correctamente',
        life: 3000
      })
    } else {
      await expedienteService.create(formData.value)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Expediente creado correctamente',
        life: 3000
      })
    }

    showDialog.value = false
    await loadExpedientes()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudo guardar el expediente',
      life: 3000
    })
  }
}

const cerrarExpediente = (expediente: ExpedienteDto) => {
  confirm.require({
    message: `¿Deseas cerrar el expediente ${expediente.numeroExpediente}?`,
    header: 'Confirmar cierre',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí, cerrar',
    rejectLabel: 'Cancelar',
    accept: async () => {
      try {
        const payload: ExpedienteCambiarEstadoDto = {
          estado: EstadoExpediente.Cerrado
        }
        await expedienteService.changeEstado(expediente.id, payload)
        toast.add({
          severity: 'success',
          summary: 'Éxito',
          detail: 'Expediente cerrado correctamente',
          life: 3000
        })
        await loadExpedientes()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: error.response?.data?.mensaje || 'No se pudo cerrar el expediente',
          life: 3000
        })
      }
    }
  })
}

onMounted(async () => {
  await loadClientes()
  await loadExpedientes()
})

watch(
  () => route.query.clienteId,
  () => {
    loadExpedientes()
  }
)
</script>

<template>
  <div class="expedientes-view">
    <AppHeader />

    <div class="main-content">
      <div class="page-hero">
        <div class="hero-content">
          <div class="hero-text">
            <h1 class="page-title">
              <i class="pi pi-folder-open"></i>
              Gestión de Expedientes
            </h1>
            <p class="page-subtitle">Control integral de expedientes y seguimiento jurídico</p>
          </div>
          <Button
            label="Nuevo Expediente"
            icon="pi pi-folder-open"
            class="hero-btn"
            size="large"
            raised
            rounded
            @click="abrirDialogNuevo"
          />
        </div>
      </div>

      <div class="stats-section">
        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon total">
                <i class="pi pi-folder-open"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.total }}</h3>
                <p class="stat-label">Total Expedientes</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon active">
                <i class="pi pi-clock"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.abiertos }}</h3>
                <p class="stat-label">Abiertos</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon review">
                <i class="pi pi-eye"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.enTramite }}</h3>
                <p class="stat-label">En tramite</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon closed">
                <i class="pi pi-check-circle"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.cerrados }}</h3>
                <p class="stat-label">Cerrados</p>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <Card class="filters-card">
        <template #content>
          <div v-if="clienteFiltrado" class="filter-chip-bar">
            <div class="filter-chip">
              <i class="pi pi-filter"></i>
              <span>Filtrando por: <strong>{{ clienteFiltrado }}</strong></span>
              <button class="chip-clear" @click="limpiarFiltroCliente" v-tooltip.top="'Quitar filtro'">
                <i class="pi pi-times"></i>
              </button>
            </div>
          </div>
          <div class="filters-bar">
            <div class="search-section">
              <span class="p-input-icon-left search-wrapper">
                <i class="pi pi-search"></i>
                <InputText
                  v-model="searchQuery"
                  placeholder="🔍 Buscar expediente por cliente, codigo, asunto o tipo..."
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

      <div v-if="viewMode === 'grid'" class="expedientes-grid">
        <Card
          v-for="expediente in expedientesFiltrados"
          :key="expediente.id"
          class="expediente-card"
        >
          <template #content>
            <div class="expediente-card-content">
              <div class="expediente-header">
                <div>
                  <h3 class="expediente-title">{{ expediente.numeroExpediente }}</h3>
                  <p class="expediente-cliente">{{ expediente.clienteNombre || 'Cliente sin nombre' }}</p>
                </div>
                <span class="status-badge" :data-status="getEstadoLabel(expediente.estado)">
                  {{ getEstadoLabel(expediente.estado) }}
                </span>
              </div>

              <Divider />

              <div class="expediente-details">
                <div class="detail-row">
                  <i class="pi pi-file"></i>
                  <span>{{ expediente.asunto }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-briefcase"></i>
                  <span>{{ expediente.tipoExpediente }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-calendar"></i>
                  <span>Apertura: {{ formatDate(expediente.fechaApertura) }}</span>
                </div>
              </div>

              <Divider />

              <div class="expediente-actions">
                <Button
                  icon="pi pi-eye"
                  label="Ver"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-view"
                  @click="abrirDetalle(expediente)"
                />
                <Button
                  icon="pi pi-pencil"
                  label="Editar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-edit"
                  @click="abrirDialogEditar(expediente)"
                />
                <Button
                  icon="pi pi-check"
                  label="Cerrar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-close"
                  @click="cerrarExpediente(expediente)"
                />
              </div>
            </div>
          </template>
        </Card>

        <div v-if="expedientesFiltrados.length === 0" class="empty-state">
          <i class="pi pi-folder-open" style="font-size: 4rem; color: var(--accent-gold); opacity: 0.5;"></i>
          <h3>No hay expedientes registrados</h3>
          <p>Cuando crees expedientes apareceran aqui</p>
          <Button
            label="Nuevo Expediente"
            icon="pi pi-folder-open"
            class="mt-3"
            raised
            rounded
            size="large"
            @click="abrirDialogNuevo"
          />
        </div>
      </div>

      <Card v-if="viewMode === 'table'" class="table-card">
        <template #content>
          <DataTable
            :value="expedientesFiltrados"
            :paginator="true"
            :rows="10"
            :loading="loading"
            stripedRows
            responsiveLayout="scroll"
            class="professional-table"
            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
            :rowsPerPageOptions="[5, 10, 25, 50]"
            currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} expedientes"
          >
            <template #empty>
              <div class="text-center p-4">
                <i class="pi pi-inbox" style="font-size: 3rem; color: var(--accent-gold); opacity: 0.5;"></i>
                <p style="margin-top: 1rem; color: var(--text-secondary);">No hay expedientes para mostrar</p>
              </div>
            </template>

            <Column field="numeroExpediente" header="Codigo" style="min-width: 160px" />
            <Column field="clienteNombre" header="Cliente" style="min-width: 220px" />
            <Column field="asunto" header="Asunto" style="min-width: 220px" />
            <Column field="tipoExpediente" header="Tipo" style="min-width: 180px" />

            <Column header="Estado" style="min-width: 160px">
              <template #body="{ data }">
                <span class="status-badge" :data-status="getEstadoLabel(data.estado)">
                  {{ getEstadoLabel(data.estado) }}
                </span>
              </template>
            </Column>

            <Column header="Apertura" style="min-width: 160px">
              <template #body="{ data }">
                {{ formatDate(data.fechaApertura) }}
              </template>
            </Column>

            <Column header="Acciones" style="min-width: 180px">
              <template #body="{ data }">
                <div class="action-buttons">
                  <Button
                    icon="pi pi-eye"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Ver expediente'"
                    @click="abrirDetalle(data)"
                  />
                  <Button
                    icon="pi pi-pencil"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Editar'"
                    @click="abrirDialogEditar(data)"
                  />
                </div>
              </template>
            </Column>
          </DataTable>
        </template>
      </Card>

      <Dialog
        v-model:visible="showDialog"
        :header="isEditing ? 'Editar Expediente' : 'Nuevo Expediente'"
        :modal="true"
        :style="{ width: '760px', maxWidth: '95vw' }"
        :dismissableMask="true"
        class="professional-dialog"
        :contentStyle="{ padding: '2rem' }"
      >
        <div class="dialog-content">
          <div class="form-section">
            <h3 class="section-title">
              <i class="pi pi-folder-open"></i>
              Información del Expediente
            </h3>
            <div class="form-grid">
              <div class="form-field">
                <label class="required">Cliente</label>
                <Dropdown
                  v-model="formData.clienteId"
                  :options="clientes"
                  optionLabel="nombre"
                  optionValue="id"
                  placeholder="Seleccionar cliente"
                  class="w-full"
                  :disabled="isEditing"
                >
                  <template #option="{ option }">
                    {{ option.nombre }} {{ option.apellidos }}
                  </template>
                </Dropdown>
              </div>
              <div class="form-field">
                <label class="required">Número de expediente</label>
                <InputText
                  v-model="formData.numeroExpediente"
                  placeholder="EXP-2026-0001"
                  class="w-full"
                />
              </div>
            </div>

            <div class="form-grid">
              <div class="form-field">
                <label class="required">Asunto</label>
                <InputText
                  v-model="formData.asunto"
                  placeholder="Asunto principal"
                  class="w-full"
                />
              </div>
              <div class="form-field">
                <label class="required">Tipo de expediente</label>
                <InputText
                  v-model="formData.tipoExpediente"
                  placeholder="Civil, Penal, Laboral..."
                  class="w-full"
                />
              </div>
            </div>

            <div class="form-grid">
              <div class="form-field">
                <label>Juzgado/Tribunal</label>
                <InputText
                  v-model="formData.juzgadoTribunal"
                  placeholder="Juzgado de Primera Instancia"
                  class="w-full"
                />
              </div>
              <div class="form-field">
                <label>Número de procedimiento</label>
                <InputText
                  v-model="formData.numeroProcedimiento"
                  placeholder="PROC-2026-0042"
                  class="w-full"
                />
              </div>
            </div>
          </div>

          <Divider />

          <div class="form-section">
            <h3 class="section-title">
              <i class="pi pi-file-edit"></i>
              Observaciones
            </h3>
            <div class="form-field">
              <label>Notas internas</label>
              <Textarea
                v-model="formData.observaciones"
                :rows="4"
                placeholder="Observaciones relevantes del expediente..."
                class="w-full"
              />
            </div>
          </div>
        </div>

        <template #footer>
          <div class="dialog-footer">
            <Button
              :icon="isEditing ? 'pi pi-check' : 'pi pi-plus'"
              @click="guardarExpediente"
              :loading="loading"
              rounded
              raised
              size="large"
              class="dialog-action-btn"
              v-tooltip.top="isEditing ? 'Actualizar expediente' : 'Crear expediente'"
              aria-label="Guardar expediente"
            />
          </div>
        </template>
      </Dialog>

      <Dialog
        v-model:visible="showDetalleDialog"
        header="Detalle del Expediente"
        :modal="true"
        :style="{ width: '720px', maxWidth: '95vw' }"
        :dismissableMask="true"
        class="professional-dialog"
        :contentStyle="{ padding: '2rem' }"
      >
        <div v-if="detalleExpediente" class="dialog-content">
          <div class="detail-block">
            <span class="detail-label">Número</span>
            <span class="detail-value">{{ detalleExpediente.numeroExpediente }}</span>
          </div>
          <div class="detail-block">
            <span class="detail-label">Cliente</span>
            <span class="detail-value">{{ detalleExpediente.clienteNombre || '-' }}</span>
          </div>
          <div class="detail-block">
            <span class="detail-label">Asunto</span>
            <span class="detail-value">{{ detalleExpediente.asunto }}</span>
          </div>
          <div class="detail-block">
            <span class="detail-label">Tipo</span>
            <span class="detail-value">{{ detalleExpediente.tipoExpediente }}</span>
          </div>
          <div class="detail-block">
            <span class="detail-label">Estado</span>
            <span class="detail-value">{{ getEstadoLabel(detalleExpediente.estado) }}</span>
          </div>
          <div class="detail-block">
            <span class="detail-label">Apertura</span>
            <span class="detail-value">{{ formatDate(detalleExpediente.fechaApertura) }}</span>
          </div>
          <div class="detail-block">
            <span class="detail-label">Observaciones</span>
            <span class="detail-value">{{ detalleExpediente.observaciones || '-' }}</span>
          </div>
        </div>
      </Dialog>
    </div>

    <AppFooter />
  </div>
</template>

<style scoped>
.expedientes-view {
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

.stats-section {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
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
  color: white;
}

.stat-icon.total {
  background: linear-gradient(135deg, var(--primary-brown), var(--secondary-brown));
}

.stat-icon.active {
  background: linear-gradient(135deg, var(--accent-gold), var(--accent-gold-dark));
  color: var(--primary-brown);
}

.stat-icon.review {
  background: linear-gradient(135deg, var(--secondary-brown), var(--light-brown));
}

.stat-icon.closed {
  background: linear-gradient(135deg, #10b981, #059669);
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

.filter-chip-bar {
  display: flex;
  padding: 0 0 1rem 0;
  margin-bottom: 1rem;
  border-bottom: 1px solid var(--border-color);
}

.filter-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.65rem 0.5rem 0.85rem;
  background: linear-gradient(135deg, rgba(212, 175, 55, 0.12), rgba(212, 175, 55, 0.06));
  border: 1px solid var(--accent-gold);
  border-radius: 2rem;
  font-size: 0.9rem;
  color: var(--text-primary);
}

.filter-chip i.pi-filter {
  color: var(--accent-gold);
  font-size: 0.85rem;
}

.chip-clear {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  border: none;
  background: var(--accent-gold);
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.7rem;
  margin-left: 0.25rem;
}

.chip-clear:hover {
  background: var(--primary-brown);
  transform: scale(1.1);
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

.expedientes-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  animation: fadeIn 0.5s ease;
}

.expediente-card {
  background: var(--bg-card);
  border: 2px solid var(--border-color);
  border-radius: 16px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.08);
  overflow: hidden;
}

.expediente-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(93, 78, 55, 0.18);
  border-color: var(--accent-gold);
}

.expediente-card-content {
  padding: 1.75rem;
}

.expediente-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
}

.expediente-title {
  font-size: 1.2rem;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0 0 0.35rem 0;
  font-family: 'Playfair Display', serif;
}

.expediente-cliente {
  margin: 0;
  color: var(--text-secondary);
  font-weight: 600;
}

.status-badge {
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 700;
  border: 1px solid var(--border-color);
  background: var(--bg-secondary);
  color: var(--text-primary);
  white-space: nowrap;
}

.status-badge[data-status='Abierto'] {
  border-color: var(--accent-gold);
  color: var(--accent-gold-dark);
}

.status-badge[data-status='En tramite'] {
  border-color: var(--secondary-brown);
  color: var(--secondary-brown);
}

.status-badge[data-status='Suspendido'] {
  border-color: var(--accent-gold-dark);
  color: var(--accent-gold-dark);
}

.status-badge[data-status='Archivado'] {
  border-color: var(--light-brown);
  color: var(--light-brown);
}

.status-badge[data-status='Cerrado'] {
  border-color: #10b981;
  color: #059669;
}

.expediente-details {
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

.expediente-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 0;
  padding-top: 1.25rem;
  border-top: 2px solid var(--border-color);
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

.action-btn-close {
  background: linear-gradient(135deg, #10b981, #059669) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);
}

.action-btn-close:hover {
  background: linear-gradient(135deg, #059669, #047857) !important;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(16, 185, 129, 0.4) !important;
}

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

@media (max-width: 1200px) {
  .stats-section {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 1400px) {
  .expedientes-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .main-content {
    padding: 1rem;
  }

  .hero-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
  }

  .stats-section {
    grid-template-columns: 1fr;
  }

  .expedientes-grid {
    grid-template-columns: 1fr;
  }
}

.text-center {
  text-align: center;
}

.p-4 {
  padding: 1.5rem;
}

.mt-3 {
  margin-top: 1rem;
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

.dialog-footer {
  display: flex;
  justify-content: center;
  align-items: center;
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

.detail-block {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  padding: 0.85rem 0;
  border-bottom: 1px solid var(--border-color);
}

.detail-block:last-child {
  border-bottom: none;
}

.detail-label {
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: var(--text-muted);
  font-weight: 600;
}

.detail-value {
  font-size: 1rem;
  color: var(--text-primary);
  font-weight: 600;
}

/* Dialog Styles */
.professional-dialog :deep(.p-dialog-content) {
  padding: 2rem !important;
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

.form-grid .form-field:has(label.required):not(:has(.p-textarea)) {
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

.w-full {
  width: 100%;
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }

  .form-grid .form-field {
    grid-column: span 1;
  }
}
</style>
