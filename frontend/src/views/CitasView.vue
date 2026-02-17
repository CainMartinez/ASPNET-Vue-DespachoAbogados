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
import Calendar from 'primevue/calendar'
import { citaService } from '@/services/citaService'
import { expedienteService } from '@/services/expedienteService'
import type {
  CitaCreateDto,
  CitaDto,
  CitaUpdateDto,
  ExpedienteDto
} from '@/types/api.types'

// Tipo local para el formulario con fechas como Date
interface CitaFormData {
  expedienteId: number
  titulo: string
  descripcion: string
  fechaInicio: Date
  fechaFin: Date
  lugar: string
  tipoCita: string
  participantes: string
  observaciones: string
}

const route = useRoute()
const router = useRouter()
const toast = useToast()
const confirm = useConfirm()

const searchQuery = ref('')
const viewMode = ref<'grid' | 'table'>('grid')
const citas = ref<CitaDto[]>([])
const expedientes = ref<ExpedienteDto[]>([])
const loading = ref(false)
const showDialog = ref(false)
const showDetalleDialog = ref(false)
const isEditing = ref(false)
const currentCita = ref<CitaDto | null>(null)
const detalleCita = ref<CitaDto | null>(null)
const formData = ref<CitaFormData>({
  expedienteId: 0,
  titulo: '',
  descripcion: '',
  fechaInicio: new Date(),
  fechaFin: new Date(),
  lugar: '',
  tipoCita: '',
  participantes: '',
  observaciones: ''
})

const tiposCita = [
  'Reunión con cliente',
  'Vista oral',
  'Audiencia previa',
  'Declaración',
  'Mediación',
  'Conciliación',
  'Firma de documentos',
  'Consulta',
  'Peritaje',
  'Junta de acreedores',
  'Ratificación',
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
const citasFiltradas = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()
  if (!query) return citas.value

  return citas.value.filter(item => {
    return (
      item.titulo.toLowerCase().includes(query) ||
      item.tipoCita.toLowerCase().includes(query) ||
      (item.expedienteNumero || '').toLowerCase().includes(query) ||
      (item.descripcion || '').toLowerCase().includes(query) ||
      (item.lugar || '').toLowerCase().includes(query) ||
      (item.participantes || '').toLowerCase().includes(query)
    )
  })
})

const stats = computed(() => {
  const total = citas.value.length
  const hoy = new Date().toDateString()
  const citasHoy = citas.value.filter(c => new Date(c.fechaInicio).toDateString() === hoy).length
  const completadas = citas.value.filter(c => c.completada).length
  const pendientes = citas.value.filter(c => !c.completada).length

  return { total, citasHoy, completadas, pendientes }
})

// Helper para convertir fecha a ISO string
const toISOString = (fecha: Date): string => {
  return fecha.toISOString()
}

const formatDate = (value?: string) => {
  if (!value) return '-'
  return new Date(value).toLocaleDateString('es-ES')
}

const formatDateTime = (value?: string) => {
  if (!value) return '-'
  return new Date(value).toLocaleString('es-ES', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

/* ---------- Data loading ---------- */
const loadCitas = async () => {
  loading.value = true
  try {
    const expedienteId = route.query.expedienteId ? Number(route.query.expedienteId) : null
    citas.value = expedienteId
      ? await citaService.getByExpedienteId(expedienteId)
      : await citaService.getAll()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudieron cargar las citas',
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
  currentCita.value = null
  const now = new Date()
  const oneHourLater = new Date(now.getTime() + 60 * 60 * 1000)
  formData.value = {
    expedienteId: Number(route.query.expedienteId) || 0,
    titulo: '',
    descripcion: '',
    fechaInicio: now,
    fechaFin: oneHourLater,
    lugar: '',
    tipoCita: '',
    participantes: '',
    observaciones: ''
  }
  showDialog.value = true
}

const abrirDialogEditar = (cita: CitaDto) => {
  isEditing.value = true
  currentCita.value = cita
  formData.value = {
    expedienteId: cita.expedienteId,
    titulo: cita.titulo,
    descripcion: cita.descripcion || '',
    fechaInicio: new Date(cita.fechaInicio),
    fechaFin: new Date(cita.fechaFin),
    lugar: cita.lugar || '',
    tipoCita: cita.tipoCita,
    participantes: cita.participantes || '',
    observaciones: cita.observaciones || ''
  }
  showDialog.value = true
}

const abrirDetalle = (cita: CitaDto) => {
  detalleCita.value = cita
  showDetalleDialog.value = true
}

const guardarCita = async () => {
  try {
    if (isEditing.value && currentCita.value) {
      const payload: CitaUpdateDto = {
        titulo: formData.value.titulo,
        descripcion: formData.value.descripcion,
        fechaInicio: toISOString(formData.value.fechaInicio),
        fechaFin: toISOString(formData.value.fechaFin),
        lugar: formData.value.lugar,
        tipoCita: formData.value.tipoCita,
        participantes: formData.value.participantes,
        observaciones: formData.value.observaciones
      }
      await citaService.update(currentCita.value.id, payload)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Cita actualizada correctamente',
        life: 3000
      })
    } else {
      const payload: CitaCreateDto = {
        expedienteId: formData.value.expedienteId,
        titulo: formData.value.titulo,
        descripcion: formData.value.descripcion,
        fechaInicio: toISOString(formData.value.fechaInicio),
        fechaFin: toISOString(formData.value.fechaFin),
        lugar: formData.value.lugar,
        tipoCita: formData.value.tipoCita,
        participantes: formData.value.participantes,
        observaciones: formData.value.observaciones
      }
      await citaService.create(payload)
      toast.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Cita creada correctamente',
        life: 3000
      })
    }

    showDialog.value = false
    await loadCitas()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudo guardar la cita',
      life: 3000
    })
  }
}

const toggleCompletada = async (cita: CitaDto) => {
  try {
    await citaService.marcarCompletada(cita.id, !cita.completada)
    toast.add({
      severity: 'success',
      summary: 'Éxito',
      detail: cita.completada ? 'Cita marcada como pendiente' : 'Cita marcada como completada',
      life: 3000
    })
    await loadCitas()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudo actualizar la cita',
      life: 3000
    })
  }
}

const eliminarCita = (cita: CitaDto) => {
  confirm.require({
    message: `¿Deseas eliminar la cita "${cita.titulo}"?`,
    header: 'Confirmar eliminación',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí, eliminar',
    rejectLabel: 'Cancelar',
    accept: async () => {
      try {
        await citaService.delete(cita.id)
        toast.add({
          severity: 'success',
          summary: 'Éxito',
          detail: 'Cita eliminada correctamente',
          life: 3000
        })
        await loadCitas()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: error.response?.data?.mensaje || 'No se pudo eliminar la cita',
          life: 3000
        })
      }
    }
  })
}

onMounted(async () => {
  await loadExpedientes()
  await loadCitas()
})

watch(
  () => route.query.expedienteId,
  () => {
    loadCitas()
  }
)
</script>

<template>
  <div class="citas-view">
    <AppHeader />

    <div class="main-content">
      <!-- Hero -->
      <div class="page-hero">
        <div class="hero-content">
          <div class="hero-text">
            <h1 class="page-title">
              <i class="pi pi-calendar"></i>
              Gestión de Citas
            </h1>
            <p class="page-subtitle">Programación y seguimiento de citas y reuniones</p>
          </div>
          <Button
            label="Nueva Cita"
            icon="pi pi-calendar-plus"
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
                <i class="pi pi-calendar"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.total }}</h3>
                <p class="stat-label">Total Citas</p>
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
                <h3 class="stat-value">{{ stats.citasHoy }}</h3>
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
                <h3 class="stat-value">{{ stats.completadas }}</h3>
                <p class="stat-label">Completadas</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon closed">
                <i class="pi pi-hourglass"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.pendientes }}</h3>
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
                  placeholder="Buscar cita por título, tipo, expediente, lugar o participantes..."
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
      <div v-if="viewMode === 'grid'" class="citas-grid">
        <Card
          v-for="cita in citasFiltradas"
          :key="cita.id"
          class="cita-card"
        >
          <template #content>
            <div class="cita-card-content">
              <div class="cita-header">
                <div>
                  <h3 class="cita-title">{{ cita.titulo }}</h3>
                  <p class="cita-expediente">{{ cita.expedienteNumero || 'Sin expediente' }}</p>
                </div>
                <span class="status-badge" :data-status="cita.completada ? 'Completada' : 'Pendiente'">
                  {{ cita.completada ? 'Completada' : 'Pendiente' }}
                </span>
              </div>

              <Divider />

              <div class="cita-details">
                <div class="detail-row">
                  <i class="pi pi-tag"></i>
                  <span>{{ cita.tipoCita }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-calendar"></i>
                  <span>{{ formatDateTime(cita.fechaInicio) }} — {{ formatDateTime(cita.fechaFin) }}</span>
                </div>
                <div v-if="cita.lugar" class="detail-row">
                  <i class="pi pi-map-marker"></i>
                  <span>{{ cita.lugar }}</span>
                </div>
                <div v-if="cita.participantes" class="detail-row">
                  <i class="pi pi-users"></i>
                  <span>{{ cita.participantes }}</span>
                </div>
              </div>

              <Divider />

              <div class="cita-actions">
                <Button
                  icon="pi pi-eye"
                  label="Ver"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-view"
                  @click="abrirDetalle(cita)"
                />
                <Button
                  icon="pi pi-pencil"
                  label="Editar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-edit"
                  @click="abrirDialogEditar(cita)"
                />
                <Button
                  :icon="cita.completada ? 'pi pi-replay' : 'pi pi-check'"
                  :label="cita.completada ? 'Reabrir' : 'Completar'"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-complete"
                  @click="toggleCompletada(cita)"
                />
                <Button
                  icon="pi pi-trash"
                  label="Eliminar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-delete"
                  @click="eliminarCita(cita)"
                />
              </div>
            </div>
          </template>
        </Card>

        <div v-if="citasFiltradas.length === 0" class="empty-state">
          <i class="pi pi-calendar" style="font-size: 4rem; color: var(--accent-gold); opacity: 0.5;"></i>
          <h3>No hay citas registradas</h3>
          <p>Cuando crees citas aparecerán aquí</p>
          <Button
            label="Nueva Cita"
            icon="pi pi-calendar-plus"
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
            :value="citasFiltradas"
            :paginator="true"
            :rows="10"
            :loading="loading"
            stripedRows
            responsiveLayout="scroll"
            class="professional-table"
            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
            :rowsPerPageOptions="[5, 10, 25, 50]"
            currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} citas"
          >
            <template #empty>
              <div class="text-center p-4">
                <i class="pi pi-inbox" style="font-size: 3rem; color: var(--accent-gold); opacity: 0.5;"></i>
                <p style="margin-top: 1rem; color: var(--text-secondary);">No hay citas para mostrar</p>
              </div>
            </template>

            <Column field="titulo" header="Título" style="min-width: 180px" />
            <Column field="tipoCita" header="Tipo" style="min-width: 150px" />
            <Column field="expedienteNumero" header="Expediente" style="min-width: 160px" />
            <Column header="Inicio" style="min-width: 170px">
              <template #body="{ data }">
                {{ formatDateTime(data.fechaInicio) }}
              </template>
            </Column>
            <Column field="lugar" header="Lugar" style="min-width: 150px">
              <template #body="{ data }">
                {{ data.lugar || '-' }}
              </template>
            </Column>

            <Column header="Estado" style="min-width: 140px">
              <template #body="{ data }">
                <span class="status-badge" :data-status="data.completada ? 'Completada' : 'Pendiente'">
                  {{ data.completada ? 'Completada' : 'Pendiente' }}
                </span>
              </template>
            </Column>

            <Column header="Acciones" style="min-width: 220px">
              <template #body="{ data }">
                <div class="action-buttons">
                  <Button
                    icon="pi pi-eye"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Ver cita'"
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
                    :icon="data.completada ? 'pi pi-replay' : 'pi pi-check'"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn table-action-btn-complete"
                    :class="{ 'is-completed': data.completada }"
                    v-tooltip.top="data.completada ? 'Reabrir' : 'Completar'"
                    @click="toggleCompletada(data)"
                  />
                  <Button
                    icon="pi pi-trash"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Eliminar'"
                    @click="eliminarCita(data)"
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
      :header="isEditing ? 'Editar Cita' : 'Nueva Cita'"
      :modal="true"
      :style="{ width: '760px', maxWidth: '95vw' }"
      :dismissableMask="true"
      class="professional-dialog"
      :contentStyle="{ padding: '2rem' }"
    >
      <div class="dialog-content">
        <div class="form-section">
          <h3 class="section-title">
            <i class="pi pi-calendar"></i>
            Información de la Cita
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
              <label class="required">Título</label>
              <InputText
                v-model="formData.titulo"
                placeholder="Título de la cita"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label class="required">Tipo de cita</label>
              <Dropdown
                v-model="formData.tipoCita"
                :options="tiposCita"
                placeholder="Seleccionar tipo"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label>Lugar</label>
              <InputText
                v-model="formData.lugar"
                placeholder="Lugar de la cita"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label class="required">Fecha y hora inicio</label>
              <Calendar
                v-model="formData.fechaInicio"
                showTime
                hourFormat="24"
                dateFormat="dd/mm/yy"
                placeholder="Seleccionar fecha y hora"
                class="w-full"
                showIcon
                iconDisplay="input"
              />
            </div>

            <div class="form-field">
              <label class="required">Fecha y hora fin</label>
              <Calendar
                v-model="formData.fechaFin"
                showTime
                hourFormat="24"
                dateFormat="dd/mm/yy"
                placeholder="Seleccionar fecha y hora"
                class="w-full"
                showIcon
                iconDisplay="input"
              />
            </div>

            <div class="form-field">
              <label>Participantes</label>
              <InputText
                v-model="formData.participantes"
                placeholder="Nombres de los participantes"
                class="w-full"
              />
            </div>

            <div class="form-field">
              <label>Descripción</label>
              <Textarea
                v-model="formData.descripcion"
                rows="3"
                placeholder="Descripción detallada de la cita"
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
            @click="guardarCita"
            :loading="loading"
            rounded
            raised
            size="large"
            class="dialog-action-btn"
            v-tooltip.top="isEditing ? 'Actualizar cita' : 'Crear cita'"
            aria-label="Guardar cita"
          />
        </div>
      </template>
    </Dialog>

    <!-- Detail Dialog -->
    <Dialog
      v-model:visible="showDetalleDialog"
      header="Detalle de Cita"
      :modal="true"
      :style="{ width: '720px', maxWidth: '95vw' }"
      :dismissableMask="true"
      class="professional-dialog"
      :contentStyle="{ padding: '2rem' }"
    >
      <div v-if="detalleCita" class="dialog-content">
        <div class="detail-block">
          <span class="detail-label">Título</span>
          <span class="detail-value">{{ detalleCita.titulo }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Expediente</span>
          <span class="detail-value">{{ detalleCita.expedienteNumero || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Tipo de cita</span>
          <span class="detail-value">{{ detalleCita.tipoCita }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Fecha inicio</span>
          <span class="detail-value">{{ formatDateTime(detalleCita.fechaInicio) }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Fecha fin</span>
          <span class="detail-value">{{ formatDateTime(detalleCita.fechaFin) }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Lugar</span>
          <span class="detail-value">{{ detalleCita.lugar || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Participantes</span>
          <span class="detail-value">{{ detalleCita.participantes || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Descripción</span>
          <span class="detail-value">{{ detalleCita.descripcion || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Estado</span>
          <span class="detail-value">
            <span class="status-badge" :data-status="detalleCita.completada ? 'Completada' : 'Pendiente'">
              {{ detalleCita.completada ? 'Completada' : 'Pendiente' }}
            </span>
          </span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Observaciones</span>
          <span class="detail-value">{{ detalleCita.observaciones || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Fecha de creación</span>
          <span class="detail-value">{{ formatDate(detalleCita.fechaCreacion) }}</span>
        </div>
        <div v-if="detalleCita.fechaModificacion" class="detail-block">
          <span class="detail-label">Última modificación</span>
          <span class="detail-value">{{ formatDate(detalleCita.fechaModificacion) }}</span>
        </div>
      </div>
    </Dialog>

    <AppFooter />
  </div>
</template>

<style scoped>
.citas-view {
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
.citas-grid {
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

.cita-card {
  border-radius: 16px;
  border: 2px solid var(--border-color);
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.08);
  transition: all 0.3s ease;
  overflow: hidden;
}

.cita-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(93, 78, 55, 0.18);
  border-color: var(--accent-gold);
}

.cita-card :deep(.p-card-body) {
  padding: 0;
}

.cita-card :deep(.p-card-content) {
  padding: 0;
}

.cita-card-content {
  padding: 1.75rem;
}

.cita-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
}

.cita-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.2rem;
  color: var(--text-primary);
  margin: 0 0 0.35rem 0;
  font-weight: 700;
}

.cita-expediente {
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

.status-badge[data-status="Completada"] {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15), rgba(34, 197, 94, 0.05));
  color: #16a34a;
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.status-badge[data-status="Pendiente"] {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.15), rgba(245, 158, 11, 0.05));
  color: #d97706;
  border: 1px solid rgba(245, 158, 11, 0.3);
}

.cita-details {
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

.cita-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 0;
  padding-top: 1.25rem;
  border-top: 2px solid var(--border-color);
  flex-wrap: wrap;
}

.action-btn {
  flex: 1;
  min-width: 90px;
  height: 42px;
  padding: 0.55rem 0.75rem;
  transition: all 0.3s ease;
  font-weight: 700;
  font-size: 0.8rem;
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

.action-btn-complete {
  background: linear-gradient(135deg, #10b981, #059669) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);
}

.action-btn-complete:hover {
  background: linear-gradient(135deg, #059669, #047857) !important;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(16, 185, 129, 0.4) !important;
}

.action-btn-delete {
  background: linear-gradient(135deg, #ef4444, #dc2626) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 2px 8px rgba(239, 68, 68, 0.3);
}

.action-btn-delete:hover {
  background: linear-gradient(135deg, #dc2626, #b91c1c) !important;
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(239, 68, 68, 0.4) !important;
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
  background: #10b981 !important;
  color: white !important;
  border: none !important;
}

.table-action-btn:nth-child(3):hover {
  background: #059669 !important;
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.4) !important;
}

.table-action-btn:nth-child(3).is-completed {
  background: #f59e0b !important;
  color: white !important;
}

.table-action-btn:nth-child(3).is-completed:hover {
  background: #d97706 !important;
}

.table-action-btn:nth-child(4) {
  background: #ef4444 !important;
  color: white !important;
  border: none !important;
}

.table-action-btn:nth-child(4):hover {
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
  grid-column: span 1;
}

.form-field:has(.p-textarea) {
  grid-column: span 2;
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

  .citas-grid {
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
