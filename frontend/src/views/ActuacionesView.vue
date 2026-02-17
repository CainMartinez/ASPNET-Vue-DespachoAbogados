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
import { actuacionService } from '@/services/actuacionService'
import { expedienteService } from '@/services/expedienteService'
import type {
  ActuacionCreateDto,
  ActuacionDto,
  ActuacionUpdateDto,
  ExpedienteDto
} from '@/types/api.types'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const confirm = useConfirm()

const searchQuery = ref('')
const viewMode = ref<'grid' | 'table'>('grid')
const actuaciones = ref<ActuacionDto[]>([])
const expedientes = ref<ExpedienteDto[]>([])
const loading = ref(false)
const showDialog = ref(false)
const showDetalleDialog = ref(false)
const isEditing = ref(false)
const currentActuacion = ref<ActuacionDto | null>(null)
const detalleActuacion = ref<ActuacionDto | null>(null)
const formData = ref<ActuacionCreateDto>({
  expedienteId: 0,
  fechaActuacion: new Date().toISOString().split('T')[0],
  tipoActuacion: '',
  descripcion: '',
  resultado: '',
  responsable: '',
  observaciones: ''
})

const tiposActuacion = [
  'Demanda',
  'Contestación',
  'Recurso',
  'Vista oral',
  'Notificación',
  'Requerimiento',
  'Providencia',
  'Auto',
  'Sentencia',
  'Diligencia',
  'Escrito',
  'Alegaciones',
  'Prueba pericial',
  'Prueba documental',
  'Otro'
]

/* ---------- Filtro por expediente desde query param ---------- */
const expedienteFiltrado = computed(() => {
  const expId = route.query.expedienteId ? Number(route.query.expedienteId) : null
  if (!expId) return null
  const exp = expedientes.value.find(e => e.id === expId)
  return exp ? `${exp.numeroExpediente} — ${exp.asunto}` : `Expediente #${expId}`
})

const limpiarFiltroExpediente = () => {
  router.replace({ query: {} })
}

/* ---------- Computed ---------- */
const actuacionesFiltradas = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()
  if (!query) return actuaciones.value

  return actuaciones.value.filter(item => {
    return (
      item.tipoActuacion.toLowerCase().includes(query) ||
      item.descripcion.toLowerCase().includes(query) ||
      (item.expedienteNumero || '').toLowerCase().includes(query) ||
      (item.expedienteAsunto || '').toLowerCase().includes(query) ||
      (item.responsable || '').toLowerCase().includes(query) ||
      (item.resultado || '').toLowerCase().includes(query)
    )
  })
})

const stats = computed(() => {
  const total = actuaciones.value.length
  const hoy = new Date().toDateString()
  const recientes = actuaciones.value.filter(a => new Date(a.fechaActuacion).toDateString() === hoy).length
  const conResultado = actuaciones.value.filter(a => a.resultado && a.resultado.trim() !== '').length
  const sinResultado = total - conResultado

  return { total, recientes, conResultado, sinResultado }
})

const formatDate = (value?: string) => {
  if (!value) return '-'
  return new Date(value).toLocaleDateString('es-ES')
}

/* ---------- Data loading ---------- */
const loadActuaciones = async () => {
  loading.value = true
  try {
    const expedienteId = route.query.expedienteId ? Number(route.query.expedienteId) : null
    actuaciones.value = expedienteId
      ? await actuacionService.getByExpedienteId(expedienteId)
      : await actuacionService.getAll()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudieron cargar las actuaciones',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

const loadExpedientes = async () => {
  try {
    expedientes.value = await expedienteService.getAll()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudieron cargar los expedientes',
      life: 3000
    })
  }
}

/* ---------- CRUD ---------- */
const abrirDialogNuevo = () => {
  isEditing.value = false
  currentActuacion.value = null
  formData.value = {
    expedienteId: Number(route.query.expedienteId) || 0,
    fechaActuacion: new Date().toISOString().split('T')[0],
    tipoActuacion: '',
    descripcion: '',
    resultado: '',
    responsable: '',
    observaciones: ''
  }
  showDialog.value = true
}

const abrirDialogEditar = (actuacion: ActuacionDto) => {
  isEditing.value = true
  currentActuacion.value = actuacion
  formData.value = {
    expedienteId: actuacion.expedienteId,
    fechaActuacion: actuacion.fechaActuacion.split('T')[0],
    tipoActuacion: actuacion.tipoActuacion,
    descripcion: actuacion.descripcion,
    resultado: actuacion.resultado || '',
    responsable: actuacion.responsable || '',
    observaciones: actuacion.observaciones || ''
  }
  showDialog.value = true
}

const abrirDetalle = (actuacion: ActuacionDto) => {
  detalleActuacion.value = actuacion
  showDetalleDialog.value = true
}

const guardarActuacion = async () => {
  try {
    if (isEditing.value && currentActuacion.value) {
      const payload: ActuacionUpdateDto = {
        fechaActuacion: formData.value.fechaActuacion,
        tipoActuacion: formData.value.tipoActuacion,
        descripcion: formData.value.descripcion,
        resultado: formData.value.resultado,
        responsable: formData.value.responsable,
        observaciones: formData.value.observaciones
      }
      await actuacionService.update(currentActuacion.value.id, payload)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Actuación actualizada correctamente',
        life: 3000
      })
    } else {
      await actuacionService.create(formData.value)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Actuación creada correctamente',
        life: 3000
      })
    }

    showDialog.value = false
    await loadActuaciones()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudo guardar la actuación',
      life: 3000
    })
  }
}

const eliminarActuacion = (actuacion: ActuacionDto) => {
  confirm.require({
    message: `¿Deseas eliminar esta actuación del ${formatDate(actuacion.fechaActuacion)}?`,
    header: 'Confirmar eliminación',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí, eliminar',
    rejectLabel: 'Cancelar',
    accept: async () => {
      try {
        await actuacionService.delete(actuacion.id)
        toast.add({
          severity: 'success',
          summary: 'Éxito',
          detail: 'Actuación eliminada correctamente',
          life: 3000
        })
        await loadActuaciones()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: error.response?.data?.mensaje || 'No se pudo eliminar la actuación',
          life: 3000
        })
      }
    }
  })
}

onMounted(async () => {
  await loadExpedientes()
  await loadActuaciones()
})

watch(
  () => route.query.expedienteId,
  () => {
    loadActuaciones()
  }
)
</script>

<template>
  <div class="actuaciones-view">
    <AppHeader />

    <div class="main-content">
      <!-- Hero -->
      <div class="page-hero">
        <div class="hero-content">
          <div class="hero-text">
            <h1 class="page-title">
              <i class="pi pi-file-edit"></i>
              Gestión de Actuaciones
            </h1>
            <p class="page-subtitle">Registro y seguimiento de actuaciones procesales</p>
          </div>
          <Button
            label="Nueva Actuación"
            icon="pi pi-file-edit"
            class="hero-btn"
            size="large"
            raised
            rounded
            @click="abrirDialogNuevo"
          />
        </div>
      </div>

      <!-- Stats -->
      <div class="stats-section">
        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon total">
                <i class="pi pi-file-edit"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.total }}</h3>
                <p class="stat-label">Total Actuaciones</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon active">
                <i class="pi pi-calendar"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.recientes }}</h3>
                <p class="stat-label">Hoy</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon review">
                <i class="pi pi-check-circle"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.conResultado }}</h3>
                <p class="stat-label">Con resultado</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon closed">
                <i class="pi pi-minus-circle"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.sinResultado }}</h3>
                <p class="stat-label">Pendientes</p>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <!-- Filters -->
      <Card class="filters-card">
        <template #content>
          <div v-if="expedienteFiltrado" class="filter-chip-bar">
            <div class="filter-chip">
              <i class="pi pi-filter"></i>
              <span>Filtrando por: <strong>{{ expedienteFiltrado }}</strong></span>
              <button class="chip-clear" @click="limpiarFiltroExpediente" v-tooltip.top="'Quitar filtro'">
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
                  placeholder="🔍 Buscar actuación por tipo, descripción, expediente o responsable..."
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

      <!-- Grid view -->
      <div v-if="viewMode === 'grid'" class="actuaciones-grid">
        <Card
          v-for="actuacion in actuacionesFiltradas"
          :key="actuacion.id"
          class="actuacion-card"
        >
          <template #content>
            <div class="actuacion-card-content">
              <div class="actuacion-header">
                <div>
                  <h3 class="actuacion-title">{{ actuacion.tipoActuacion }}</h3>
                  <p class="actuacion-expediente">{{ actuacion.expedienteNumero || 'Sin expediente' }}</p>
                </div>
                <span class="status-badge" :data-status="actuacion.resultado ? 'Con resultado' : 'Pendiente'">
                  {{ actuacion.resultado ? 'Con resultado' : 'Pendiente' }}
                </span>
              </div>

              <Divider />

              <div class="actuacion-details">
                <div class="detail-row">
                  <i class="pi pi-align-left"></i>
                  <span>{{ actuacion.descripcion.length > 80 ? actuacion.descripcion.substring(0, 80) + '...' : actuacion.descripcion }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-calendar"></i>
                  <span>Fecha: {{ formatDate(actuacion.fechaActuacion) }}</span>
                </div>
                <div v-if="actuacion.responsable" class="detail-row">
                  <i class="pi pi-user"></i>
                  <span>{{ actuacion.responsable }}</span>
                </div>
              </div>

              <Divider />

              <div class="actuacion-actions">
                <Button
                  icon="pi pi-eye"
                  label="Ver"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-view"
                  @click="abrirDetalle(actuacion)"
                />
                <Button
                  icon="pi pi-pencil"
                  label="Editar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-edit"
                  @click="abrirDialogEditar(actuacion)"
                />
                <Button
                  icon="pi pi-trash"
                  label="Eliminar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-close"
                  @click="eliminarActuacion(actuacion)"
                />
              </div>
            </div>
          </template>
        </Card>

        <div v-if="actuacionesFiltradas.length === 0" class="empty-state">
          <i class="pi pi-file-edit" style="font-size: 4rem; color: var(--accent-gold); opacity: 0.5;"></i>
          <h3>No hay actuaciones registradas</h3>
          <p>Cuando crees actuaciones aparecerán aquí</p>
          <Button
            label="Nueva Actuación"
            icon="pi pi-file-edit"
            class="mt-3"
            raised
            rounded
            size="large"
            @click="abrirDialogNuevo"
          />
        </div>
      </div>

      <!-- Table view -->
      <Card v-if="viewMode === 'table'" class="table-card">
        <template #content>
          <DataTable
            :value="actuacionesFiltradas"
            :paginator="true"
            :rows="10"
            :loading="loading"
            stripedRows
            responsiveLayout="scroll"
            class="professional-table"
            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
            :rowsPerPageOptions="[5, 10, 25, 50]"
            currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} actuaciones"
          >
            <template #empty>
              <div class="text-center p-4">
                <i class="pi pi-inbox" style="font-size: 3rem; color: var(--accent-gold); opacity: 0.5;"></i>
                <p style="margin-top: 1rem; color: var(--text-secondary);">No hay actuaciones para mostrar</p>
              </div>
            </template>

            <Column header="Fecha" style="min-width: 130px">
              <template #body="{ data }">
                {{ formatDate(data.fechaActuacion) }}
              </template>
            </Column>
            <Column field="tipoActuacion" header="Tipo" style="min-width: 160px" />
            <Column field="expedienteNumero" header="Expediente" style="min-width: 170px" />
            <Column field="descripcion" header="Descripción" style="min-width: 260px">
              <template #body="{ data }">
                {{ data.descripcion.length > 60 ? data.descripcion.substring(0, 60) + '...' : data.descripcion }}
              </template>
            </Column>
            <Column field="responsable" header="Responsable" style="min-width: 160px" />

            <Column header="Estado" style="min-width: 140px">
              <template #body="{ data }">
                <span class="status-badge" :data-status="data.resultado ? 'Con resultado' : 'Pendiente'">
                  {{ data.resultado ? 'Con resultado' : 'Pendiente' }}
                </span>
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
                    v-tooltip.top="'Ver actuación'"
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
                  <Button
                    icon="pi pi-trash"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Eliminar'"
                    @click="eliminarActuacion(data)"
                  />
                </div>
              </template>
            </Column>
          </DataTable>
        </template>
      </Card>
    </div>

    <!-- Create/Edit Dialog -->
    <Dialog
      v-model:visible="showDialog"
      :header="isEditing ? 'Editar Actuación' : 'Nueva Actuación'"
      :modal="true"
      :style="{ width: '760px', maxWidth: '95vw' }"
      :dismissableMask="true"
      class="professional-dialog"
      :contentStyle="{ padding: '2rem' }"
    >
      <div class="dialog-content">
        <div class="form-section">
          <h3 class="section-title">
            <i class="pi pi-file-edit"></i>
            Información de la Actuación
          </h3>

          <div class="form-grid">
            <div class="form-field">
              <label class="required">Expediente</label>
              <Dropdown
                v-model="formData.expedienteId"
                :options="expedientes"
                optionLabel="numeroExpediente"
                optionValue="id"
                placeholder="Seleccionar expediente"
                :filter="true"
                filterPlaceholder="Buscar expediente..."
                class="w-full"
                :disabled="isEditing"
              />
            </div>

            <div class="form-field">
              <label class="required">Tipo de actuación</label>
              <Dropdown
                v-model="formData.tipoActuacion"
                :options="tiposActuacion"
                placeholder="Seleccionar tipo"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label class="required">Fecha</label>
              <InputText
                v-model="formData.fechaActuacion"
                type="date"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label>Responsable</label>
              <InputText
                v-model="formData.responsable"
                placeholder="Nombre del responsable"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label>Resultado</label>
              <InputText
                v-model="formData.resultado"
                placeholder="Resultado de la actuación"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label class="required">Descripción</label>
              <Textarea
                v-model="formData.descripcion"
                rows="3"
                placeholder="Descripción detallada de la actuación"
                class="w-full"
                autoResize
              />
            </div>
          </div>
        </div>

        <Divider />

        <div class="form-section">
          <h3 class="section-title">
            <i class="pi pi-comment"></i>
            Observaciones
          </h3>
          <div class="form-grid">
            <div class="form-field">
              <label>Notas internas</label>
              <Textarea
                v-model="formData.observaciones"
                rows="3"
                placeholder="Notas u observaciones adicionales"
                class="w-full"
                autoResize
              />
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <Button
            :icon="isEditing ? 'pi pi-check' : 'pi pi-plus'"
            @click="guardarActuacion"
            :loading="loading"
            rounded
            raised
            size="large"
            class="dialog-action-btn"
            v-tooltip.top="isEditing ? 'Actualizar actuación' : 'Crear actuación'"
            aria-label="Guardar actuación"
          />
        </div>
      </template>
    </Dialog>

    <!-- Detail Dialog -->
    <Dialog
      v-model:visible="showDetalleDialog"
      header="Detalle de Actuación"
      :modal="true"
      :style="{ width: '720px', maxWidth: '95vw' }"
      :dismissableMask="true"
      class="professional-dialog"
      :contentStyle="{ padding: '2rem' }"
    >
      <div v-if="detalleActuacion" class="dialog-content">
        <div class="detail-block">
          <span class="detail-label">Expediente</span>
          <span class="detail-value">{{ detalleActuacion.expedienteNumero || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Asunto del expediente</span>
          <span class="detail-value">{{ detalleActuacion.expedienteAsunto || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Tipo de actuación</span>
          <span class="detail-value">{{ detalleActuacion.tipoActuacion }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Fecha</span>
          <span class="detail-value">{{ formatDate(detalleActuacion.fechaActuacion) }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Descripción</span>
          <span class="detail-value">{{ detalleActuacion.descripcion }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Resultado</span>
          <span class="detail-value">{{ detalleActuacion.resultado || 'Pendiente' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Responsable</span>
          <span class="detail-value">{{ detalleActuacion.responsable || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Observaciones</span>
          <span class="detail-value">{{ detalleActuacion.observaciones || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Fecha de registro</span>
          <span class="detail-value">{{ formatDate(detalleActuacion.fechaRegistro) }}</span>
        </div>
        <div v-if="detalleActuacion.fechaModificacion" class="detail-block">
          <span class="detail-label">Última modificación</span>
          <span class="detail-value">{{ formatDate(detalleActuacion.fechaModificacion) }}</span>
        </div>
      </div>
    </Dialog>

    <AppFooter />
  </div>
</template>

<style scoped>
.actuaciones-view {
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
  top: -50%;
  right: -10%;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(212, 175, 55, 0.15) 0%, transparent 70%);
  border-radius: 50%;
  pointer-events: none;
}

.hero-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.page-title {
  font-family: 'Playfair Display', serif;
  font-size: 2.5rem;
  color: white;
  margin: 0 0 0.5rem 0;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.page-title i {
  font-size: 2rem;
  color: var(--accent-gold);
}

.page-subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1.1rem;
  margin: 0;
}

.hero-btn {
  background: var(--accent-gold) !important;
  color: var(--primary-brown) !important;
  border-color: var(--accent-gold) !important;
  font-weight: 700 !important;
  padding: 0.875rem 2.5rem !important;
  font-size: 1.1rem !important;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.3);
  transition: all 0.3s ease;
}

.hero-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 24px rgba(212, 175, 55, 0.5);
}

.hero-btn :deep(.p-button-icon-left) {
  margin-right: 0.6rem;
}

/* Stats Section */
.stats-section {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: var(--bg-card);
  border-radius: 12px;
  border: 2px solid var(--border-color);
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
  color: white;
}

.stat-icon.active {
  background: linear-gradient(135deg, var(--accent-gold), var(--accent-gold-dark));
  color: var(--primary-brown);
}

.stat-icon.review {
  background: linear-gradient(135deg, var(--secondary-brown), var(--light-brown));
  color: white;
}

.stat-icon.closed {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
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

/* Filters */
.filters-card {
  border-radius: 12px;
  margin-bottom: 2rem;
  border: 2px solid var(--border-color);
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
  border: 2px solid var(--border-color);
  font-size: 1rem;
  transition: all 0.3s ease;
  background: var(--bg-primary);
}

.search-input-custom::placeholder {
  color: var(--text-muted);
  font-style: italic;
}

.view-toggle {
  display: flex;
  gap: 0.5rem;
}

.view-toggle Button.active {
  background: var(--accent-gold) !important;
  border-color: var(--accent-gold) !important;
  color: var(--primary-brown) !important;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.3);
  transform: translateY(-2px);
}

/* Grid layout */
.actuaciones-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  animation: fadeIn 0.5s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.actuacion-card {
  border-radius: 16px;
  border: 2px solid var(--border-color);
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.08);
  transition: all 0.3s ease;
  overflow: hidden;
}

.actuacion-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(93, 78, 55, 0.18);
  border-color: var(--accent-gold);
}

.actuacion-card :deep(.p-card-body) {
  padding: 0;
}

.actuacion-card :deep(.p-card-content) {
  padding: 0;
}

.actuacion-card-content {
  padding: 1.75rem;
}

.actuacion-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
}

.actuacion-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.2rem;
  color: var(--text-primary);
  margin: 0 0 0.35rem 0;
  font-weight: 700;
}

.actuacion-expediente {
  color: var(--text-muted);
  font-size: 0.85rem;
  margin: 0;
}

.status-badge {
  display: inline-block;
  padding: 0.3rem 0.85rem;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.status-badge[data-status="Con resultado"] {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15), rgba(34, 197, 94, 0.05));
  color: #16a34a;
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.status-badge[data-status="Pendiente"] {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.15), rgba(245, 158, 11, 0.05));
  color: #d97706;
  border: 1px solid rgba(245, 158, 11, 0.3);
}

.actuacion-details {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.detail-row {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  color: var(--text-secondary);
  font-size: 0.95rem;
  padding: 0.5rem;
  border-radius: 8px;
  transition: background 0.2s ease;
}

.detail-row:hover {
  background: rgba(212, 175, 55, 0.05);
}

.detail-row i {
  color: var(--accent-gold);
  width: 20px;
  flex-shrink: 0;
  font-size: 1.1rem;
}

.actuacion-actions {
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

/* Table styles */
.table-card {
  border-radius: 12px;
  border: 2px solid var(--border-color);
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(93, 78, 55, 0.1);
  animation: fadeIn 0.5s ease;
  background: var(--bg-card);
}

.table-card :deep(.p-card-body) {
  padding: 0;
}

.table-card :deep(.p-card-content) {
  padding: 0;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
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
  background: #ef4444 !important;
  color: white !important;
  border: none !important;
}

.table-action-btn:nth-child(3):hover {
  background: #dc2626 !important;
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.4) !important;
}

.professional-table :deep(.p-datatable-thead > tr > th) {
  background: var(--bg-secondary);
  color: var(--text-primary);
  font-weight: 600;
  border-bottom: 2px solid var(--accent-gold);
  padding: 1rem;
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.professional-table :deep(.p-datatable-tbody > tr > td) {
  padding: 0.85rem 1rem;
  border-bottom: 1px solid var(--border-color);
}

.professional-table :deep(.p-datatable-tbody > tr:hover) {
  background: rgba(212, 175, 55, 0.03) !important;
}

.professional-table :deep(.p-paginator) {
  padding: 1rem;
  border-top: 1px solid var(--border-color);
}

/* Empty state */
.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  grid-column: 1 / -1;
}

.empty-state h3 {
  font-family: 'Playfair Display', serif;
  color: var(--text-primary);
  margin: 1rem 0 0.5rem 0;
}

.empty-state p {
  color: var(--text-secondary);
  margin: 0 0 1.5rem 0;
}

/* Dialog styles */
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

/* Detail dialog */
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

.mt-3 {
  margin-top: 1rem;
}

/* Responsive */
@media (max-width: 768px) {
  .stats-section {
    grid-template-columns: repeat(2, 1fr);
  }

  .hero-content {
    flex-direction: column;
    text-align: center;
    gap: 1.5rem;
  }

  .actuaciones-grid {
    grid-template-columns: 1fr;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .form-grid .form-field {
    grid-column: span 1;
  }
}
</style>
