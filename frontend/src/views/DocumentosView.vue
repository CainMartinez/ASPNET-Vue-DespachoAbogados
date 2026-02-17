<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
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
import { documentoService, TIPOS_REPORTE } from '@/services/documentoService'
import type { DocumentoDto } from '@/types/api.types'
import type { TipoReporte } from '@/services/documentoService'

const toast = useToast()
const confirm = useConfirm()

const searchQuery = ref('')
const viewMode = ref<'grid' | 'table'>('grid')
const documentos = ref<DocumentoDto[]>([])
const loading = ref(false)
const generatingReport = ref<string | null>(null)
const showDetalleDialog = ref(false)
const detalleDocumento = ref<DocumentoDto | null>(null)

/* ---------- Computed ---------- */
const documentosFiltrados = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()
  if (!query) return documentos.value

  return documentos.value.filter(item => {
    return (
      item.nombreArchivo.toLowerCase().includes(query) ||
      item.tipoDocumento.toLowerCase().includes(query) ||
      (item.descripcion || '').toLowerCase().includes(query) ||
      (item.cargadoPor || '').toLowerCase().includes(query)
    )
  })
})

const stats = computed(() => {
  const total = documentos.value.length
  const totalBytes = documentos.value.reduce((sum, d) => sum + (d.tamanoBytes || 0), 0)
  const tamano =
    totalBytes > 1048576
      ? `${(totalBytes / 1048576).toFixed(1)} MB`
      : `${(totalBytes / 1024).toFixed(0)} KB`
  const hoy = new Date().toDateString()
  const recientes = documentos.value.filter(
    d => new Date(d.fechaCarga).toDateString() === hoy
  ).length
  const tipos = new Set(documentos.value.map(d => d.tipoDocumento)).size

  return { total, tamano, recientes, tipos }
})

const formatDate = (value?: string) => {
  if (!value) return '-'
  const d = new Date(value)
  return d.toLocaleDateString('es-ES') + ' ' + d.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })
}

const formatFileSize = (bytes?: number) => {
  if (!bytes || bytes === 0) return '-'
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1048576) return `${(bytes / 1024).toFixed(1)} KB`
  return `${(bytes / 1048576).toFixed(1)} MB`
}

/* ---------- Data loading ---------- */
const loadDocumentos = async () => {
  loading.value = true
  try {
    documentos.value = await documentoService.getAll()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || 'No se pudieron cargar los documentos',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

/* ---------- Report generation ---------- */
const generarReporte = async (tipo: TipoReporte) => {
  generatingReport.value = tipo.id
  try {
    const documento = await documentoService.generarReporte(tipo.endpoint)
    toast.add({
      severity: 'success',
      summary: 'Reporte generado',
      detail: `${tipo.nombre} generado correctamente`,
      life: 3000
    })
    // Auto-download the report
    await documentoService.descargarReporte(documento.id, documento.nombreArchivo)
    await loadDocumentos()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: error.response?.data?.mensaje || `No se pudo generar el reporte: ${tipo.nombre}`,
      life: 4000
    })
  } finally {
    generatingReport.value = null
  }
}

/* ---------- Re-download ---------- */
const descargarReporte = async (documento: DocumentoDto) => {
  try {
    await documentoService.descargarReporte(documento.id, documento.nombreArchivo)
    toast.add({
      severity: 'info',
      summary: 'Descarga iniciada',
      detail: documento.nombreArchivo,
      life: 2000
    })
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'No se pudo descargar el reporte',
      life: 3000
    })
  }
}

/* ---------- Detail ---------- */
const abrirDetalle = (documento: DocumentoDto) => {
  detalleDocumento.value = documento
  showDetalleDialog.value = true
}

/* ---------- Delete ---------- */
const eliminarDocumento = (documento: DocumentoDto) => {
  confirm.require({
    message: `¿Deseas eliminar el registro "${documento.nombreArchivo}" del historial?`,
    header: 'Confirmar eliminación',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí, eliminar',
    rejectLabel: 'Cancelar',
    accept: async () => {
      try {
        await documentoService.delete(documento.id)
        toast.add({
          severity: 'success',
          summary: 'Éxito',
          detail: 'Registro eliminado del historial',
          life: 3000
        })
        await loadDocumentos()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: 'Error',
          detail: error.response?.data?.mensaje || 'No se pudo eliminar el registro',
          life: 3000
        })
      }
    }
  })
}

const getReportIcon = (tipoDocumento: string) => {
  const tipo = TIPOS_REPORTE.find(t => t.nombre === tipoDocumento)
  return tipo ? tipo.icono : 'pi pi-file-pdf'
}

onMounted(async () => {
  await loadDocumentos()
})
</script>

<template>
  <div class="documentos-view">
    <AppHeader />

    <div class="main-content">
      <!-- Hero -->
      <div class="page-hero">
        <div class="hero-content">
          <div class="hero-text">
            <h1 class="page-title">
              <i class="pi pi-file-pdf"></i>
              Reportes y Documentos
            </h1>
            <p class="page-subtitle">
              Genera informes PDF y consulta el historial de reportes descargados
            </p>
          </div>
        </div>
      </div>

      <!-- Report Generation Cards -->
      <div class="section-header">
        <h2 class="section-heading">
          <i class="pi pi-bolt"></i>
          Generar Nuevo Reporte
        </h2>
      </div>

      <div class="report-generators">
        <Card
          v-for="tipo in TIPOS_REPORTE"
          :key="tipo.id"
          class="report-gen-card"
        >
          <template #content>
            <div class="report-gen-content">
              <div class="report-gen-icon">
                <i :class="tipo.icono"></i>
              </div>
              <div class="report-gen-info">
                <h3 class="report-gen-title">{{ tipo.nombre }}</h3>
                <p class="report-gen-desc">{{ tipo.descripcion }}</p>
              </div>
              <Button
                icon="pi pi-download"
                label="Generar"
                class="report-gen-btn"
                raised
                rounded
                :loading="generatingReport === tipo.id"
                :disabled="generatingReport !== null"
                @click="generarReporte(tipo)"
              />
            </div>
          </template>
        </Card>
      </div>

      <!-- Stats -->
      <div class="stats-section">
        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon total">
                <i class="pi pi-file-pdf"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.total }}</h3>
                <p class="stat-label">Total Reportes</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon active">
                <i class="pi pi-tags"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.tipos }}</h3>
                <p class="stat-label">Tipos</p>
              </div>
            </div>
          </template>
        </Card>

        <Card class="stat-card">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon review">
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
              <div class="stat-icon closed">
                <i class="pi pi-database"></i>
              </div>
              <div class="stat-info">
                <h3 class="stat-value">{{ stats.tamano }}</h3>
                <p class="stat-label">Almacenamiento</p>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <!-- History section header -->
      <div class="section-header">
        <h2 class="section-heading">
          <i class="pi pi-history"></i>
          Historial de Reportes
        </h2>
      </div>

      <!-- Filters -->
      <Card class="filters-card">
        <template #content>
          <div class="filters-bar">
            <div class="search-section">
              <span class="p-input-icon-left search-wrapper">
                <i class="pi pi-search"></i>
                <InputText
                  v-model="searchQuery"
                  placeholder="🔍 Buscar por nombre, tipo o descripción..."
                  class="search-input-custom"
                />
              </span>
            </div>

            <div class="view-toggle">
              <Button
                icon="pi pi-th-large"
                :class="{ active: viewMode === 'grid' }"
                @click="viewMode = 'grid'"
                :text="viewMode !== 'grid'"
                :raised="viewMode === 'grid'"
                rounded
                v-tooltip.top="'Vista en tarjetas'"
              />
              <Button
                icon="pi pi-list"
                :class="{ active: viewMode === 'table' }"
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
      <div v-if="viewMode === 'grid'" class="documentos-grid">
        <Card
          v-for="documento in documentosFiltrados"
          :key="documento.id"
          class="documento-card"
        >
          <template #content>
            <div class="documento-card-content">
              <div class="documento-header">
                <div class="documento-header-info">
                  <div class="documento-file-icon">
                    <i :class="getReportIcon(documento.tipoDocumento)"></i>
                  </div>
                  <div>
                    <h3 class="documento-title">{{ documento.tipoDocumento }}</h3>
                    <p class="documento-filename">{{ documento.nombreArchivo }}</p>
                  </div>
                </div>
                <span class="tipo-badge">PDF</span>
              </div>

              <Divider />

              <div class="documento-details">
                <div class="detail-row">
                  <i class="pi pi-calendar"></i>
                  <span>{{ formatDate(documento.fechaCarga) }}</span>
                </div>
                <div class="detail-row">
                  <i class="pi pi-database"></i>
                  <span>{{ formatFileSize(documento.tamanoBytes) }}</span>
                </div>
                <div v-if="documento.descripcion" class="detail-row">
                  <i class="pi pi-info-circle"></i>
                  <span class="detail-text-clamp">{{ documento.descripcion }}</span>
                </div>
              </div>

              <Divider />

              <div class="documento-actions">
                <Button
                  icon="pi pi-download"
                  label="Descargar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-view"
                  @click="descargarReporte(documento)"
                />
                <Button
                  icon="pi pi-eye"
                  label="Detalle"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-edit"
                  @click="abrirDetalle(documento)"
                />
                <Button
                  icon="pi pi-trash"
                  label="Eliminar"
                  text
                  raised
                  size="small"
                  class="action-btn action-btn-delete"
                  @click="eliminarDocumento(documento)"
                />
              </div>
            </div>
          </template>
        </Card>

        <div v-if="documentosFiltrados.length === 0 && !loading" class="empty-state">
          <i
            class="pi pi-file-pdf"
            style="font-size: 4rem; color: var(--accent-gold); opacity: 0.5"
          ></i>
          <h3>No hay reportes generados</h3>
          <p>Genera un reporte desde los botones de arriba para comenzar</p>
        </div>
      </div>

      <!-- Table view -->
      <Card v-if="viewMode === 'table'" class="table-card">
        <template #content>
          <DataTable
            :value="documentosFiltrados"
            :paginator="true"
            :rows="10"
            :loading="loading"
            stripedRows
            responsiveLayout="scroll"
            class="professional-table"
            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
            :rowsPerPageOptions="[5, 10, 25, 50]"
            currentPageReportTemplate="Mostrando {first} a {last} de {totalRecords} reportes"
          >
            <template #empty>
              <div class="text-center p-4">
                <i
                  class="pi pi-inbox"
                  style="font-size: 3rem; color: var(--accent-gold); opacity: 0.5"
                ></i>
                <p style="margin-top: 1rem; color: var(--text-secondary)">
                  No hay reportes para mostrar
                </p>
              </div>
            </template>

            <Column header="Tipo" style="min-width: 220px">
              <template #body="{ data }">
                <div class="table-file-cell">
                  <i :class="getReportIcon(data.tipoDocumento)" class="table-file-icon"></i>
                  <span>{{ data.tipoDocumento }}</span>
                </div>
              </template>
            </Column>
            <Column field="nombreArchivo" header="Archivo" style="min-width: 280px" />
            <Column header="Tamaño" style="min-width: 110px">
              <template #body="{ data }">
                {{ formatFileSize(data.tamanoBytes) }}
              </template>
            </Column>
            <Column header="Generado" style="min-width: 170px">
              <template #body="{ data }">
                {{ formatDate(data.fechaCarga) }}
              </template>
            </Column>

            <Column header="Acciones" style="min-width: 180px">
              <template #body="{ data }">
                <div class="action-buttons">
                  <Button
                    icon="pi pi-download"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Descargar'"
                    @click="descargarReporte(data)"
                  />
                  <Button
                    icon="pi pi-eye"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Ver detalle'"
                    @click="abrirDetalle(data)"
                  />
                  <Button
                    icon="pi pi-trash"
                    text
                    raised
                    rounded
                    size="small"
                    class="table-action-btn"
                    v-tooltip.top="'Eliminar'"
                    @click="eliminarDocumento(data)"
                  />
                </div>
              </template>
            </Column>
          </DataTable>
        </template>
      </Card>
    </div>

    <!-- Detail Dialog -->
    <Dialog
      v-model:visible="showDetalleDialog"
      header="Detalle del Reporte"
      :modal="true"
      :style="{ width: '720px', maxWidth: '95vw' }"
      :dismissableMask="true"
      class="professional-dialog"
      :contentStyle="{ padding: '2rem' }"
    >
      <div v-if="detalleDocumento" class="dialog-content">
        <div class="detail-block">
          <span class="detail-label">Tipo de reporte</span>
          <span class="detail-value">{{ detalleDocumento.tipoDocumento }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Nombre del archivo</span>
          <span class="detail-value">{{ detalleDocumento.nombreArchivo }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Descripción</span>
          <span class="detail-value">{{ detalleDocumento.descripcion || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Tamaño</span>
          <span class="detail-value">{{ formatFileSize(detalleDocumento.tamanoBytes) }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Generado por</span>
          <span class="detail-value">{{ detalleDocumento.cargadoPor || '-' }}</span>
        </div>
        <div class="detail-block">
          <span class="detail-label">Fecha de generación</span>
          <span class="detail-value">{{ formatDate(detalleDocumento.fechaCarga) }}</span>
        </div>
        <div v-if="detalleDocumento.observaciones" class="detail-block">
          <span class="detail-label">Observaciones</span>
          <span class="detail-value">{{ detalleDocumento.observaciones }}</span>
        </div>
      </div>
      <template #footer>
        <div class="dialog-footer">
          <Button
            icon="pi pi-download"
            @click="detalleDocumento && descargarReporte(detalleDocumento)"
            rounded
            raised
            size="large"
            class="dialog-action-btn"
            v-tooltip.top="'Descargar reporte'"
            aria-label="Descargar"
          />
        </div>
      </template>
    </Dialog>

    <AppFooter />
  </div>
</template>

<style scoped>
.documentos-view {
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

/* Section headers */
.section-header {
  margin-bottom: 1.25rem;
}

.section-heading {
  font-family: 'Playfair Display', serif;
  font-size: 1.5rem;
  color: var(--text-primary);
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.section-heading i {
  color: var(--accent-gold);
  font-size: 1.3rem;
}

/* Report generators */
.report-generators {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.report-gen-card {
  border-radius: 16px;
  border: 2px solid var(--border-color);
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.08);
  transition: all 0.3s ease;
  overflow: hidden;
}

.report-gen-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(93, 78, 55, 0.18);
  border-color: var(--accent-gold);
}

.report-gen-card :deep(.p-card-body) {
  padding: 0;
}

.report-gen-card :deep(.p-card-content) {
  padding: 0;
}

.report-gen-content {
  padding: 1.75rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 1.25rem;
}

.report-gen-icon {
  width: 72px;
  height: 72px;
  border-radius: 16px;
  background: linear-gradient(135deg, var(--primary-brown), var(--secondary-brown));
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.report-gen-icon i {
  font-size: 2rem;
  color: var(--accent-gold);
}

.report-gen-info {
  flex: 1;
}

.report-gen-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.15rem;
  color: var(--text-primary);
  margin: 0 0 0.5rem 0;
  font-weight: 700;
}

.report-gen-desc {
  color: var(--text-secondary);
  font-size: 0.88rem;
  margin: 0;
  line-height: 1.5;
}

.report-gen-btn {
  background: var(--accent-gold) !important;
  color: var(--primary-brown) !important;
  border-color: var(--accent-gold) !important;
  font-weight: 700 !important;
  padding: 0.65rem 2rem !important;
  font-size: 1rem !important;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.3);
  transition: all 0.3s ease;
  width: 100%;
}

.report-gen-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(212, 175, 55, 0.5);
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
.documentos-grid {
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

.documento-card {
  border-radius: 16px;
  border: 2px solid var(--border-color);
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.08);
  transition: all 0.3s ease;
  overflow: hidden;
}

.documento-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 28px rgba(93, 78, 55, 0.18);
  border-color: var(--accent-gold);
}

.documento-card :deep(.p-card-body) {
  padding: 0;
}

.documento-card :deep(.p-card-content) {
  padding: 0;
}

.documento-card-content {
  padding: 1.75rem;
}

.documento-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
}

.documento-header-info {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex: 1;
  min-width: 0;
}

.documento-file-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  background: linear-gradient(135deg, rgba(212, 175, 55, 0.15), rgba(212, 175, 55, 0.05));
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.documento-file-icon i {
  font-size: 1.5rem;
  color: var(--accent-gold);
}

.documento-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.05rem;
  color: var(--text-primary);
  margin: 0 0 0.25rem 0;
  font-weight: 700;
  word-break: break-word;
}

.documento-filename {
  color: var(--text-muted);
  font-size: 0.8rem;
  margin: 0;
  word-break: break-all;
}

.tipo-badge {
  display: inline-block;
  padding: 0.3rem 0.85rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: white;
  border: none;
  white-space: nowrap;
}

.documento-details {
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

.detail-text-clamp {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.documento-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 0;
  padding-top: 1.25rem;
  border-top: 2px solid var(--border-color);
}

.action-btn {
  flex: 1;
  min-width: 90px;
  height: 42px;
  padding: 0.55rem 0.75rem;
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
  background: linear-gradient(135deg, var(--accent-gold-light), #f0d875) !important;
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

.table-file-cell {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.table-file-icon {
  font-size: 1.3rem;
  color: var(--accent-gold);
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
  background: linear-gradient(135deg, var(--accent-gold-light), #f0d875) !important;
  transform: translateY(-2px);
  box-shadow: 0 10px 22px rgba(212, 175, 55, 0.45);
}

.dialog-content {
  padding: 0;
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

  .report-generators {
    grid-template-columns: 1fr;
  }

  .documentos-grid {
    grid-template-columns: 1fr;
  }
}
</style>
