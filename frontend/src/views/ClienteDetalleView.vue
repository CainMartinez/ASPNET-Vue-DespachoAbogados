<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { clienteService } from '@/services/clienteService'
import type { ClienteDto, ExpedienteDto } from '@/types/api.types'

import AppHeader from '@/components/AppHeader.vue'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Tag from 'primevue/tag'

const route = useRoute()
const router = useRouter()
const toast = useToast()

const cliente = ref<ClienteDto | null>(null)
const expedientes = ref<ExpedienteDto[]>([])
const loading = ref(false)

const estadoSeverity: Record<number, string> = {
  0: 'info',     // Abierto
  1: 'success',  // EnTramite
  2: 'warning',  // Suspendido
  3: 'secondary',// Archivado
  4: 'danger'    // Cerrado
}

const estadoLabel: Record<number, string> = {
  0: 'Abierto',
  1: 'En Trámite',
  2: 'Suspendido',
  3: 'Archivado',
  4: 'Cerrado'
}

onMounted(async () => {
  await cargarDatos()
})

async function cargarDatos() {
  loading.value = true
  try {
    const id = parseInt(route.params.id as string)
    cliente.value = await clienteService.getById(id)
    expedientes.value = await clienteService.getExpedientes(id)
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'No se pudo cargar la información del cliente',
      life: 3000
    })
    router.push('/clientes')
  } finally {
    loading.value = false
  }
}

function volver() {
  router.push('/clientes')
}
</script>

<template>
  <div class="cliente-detalle-view">
    <AppHeader />

    <div class="container">
      <Button
        label="Volver a Clientes"
        icon="pi pi-arrow-left"
        @click="volver"
        class="mb-3"
        text
      />

      <Card v-if="cliente">
        <template #title>
          <div class="card-header">
            <i class="pi pi-user"></i>
            <h1>{{ cliente.nombre }} {{ cliente.apellidos }}</h1>
          </div>
        </template>

        <template #content>
          <!-- Información del cliente -->
          <div class="cliente-info">
            <div class="info-grid">
              <div class="info-item">
                <label>DNI/CIF</label>
                <span>{{ cliente.dniCif }}</span>
              </div>
              <div class="info-item">
                <label>Email</label>
                <span>{{ cliente.email || '-' }}</span>
              </div>
              <div class="info-item">
                <label>Teléfono</label>
                <span>{{ cliente.telefono || '-' }}</span>
              </div>
              <div class="info-item">
                <label>Ciudad</label>
                <span>{{ cliente.ciudad || '-' }}</span>
              </div>
              <div class="info-item">
                <label>Dirección</label>
                <span>{{ cliente.direccion || '-' }}</span>
              </div>
              <div class="info-item">
                <label>Código Postal</label>
                <span>{{ cliente.codigoPostal || '-' }}</span>
              </div>
            </div>

            <div v-if="cliente.observaciones" class="observaciones">
              <label>Observaciones</label>
              <p>{{ cliente.observaciones }}</p>
            </div>
          </div>

          <!-- Expedientes del cliente -->
          <div class="expedientes-section">
            <h2>
              <i class="pi pi-folder"></i>
              Expedientes ({{ expedientes.length }})
            </h2>

            <DataTable
              :value="expedientes"
              :loading="loading"
              stripedRows
              class="mt-3"
            >
              <template #empty>
                <div class="empty-state">
                  <i class="pi pi-folder-open" style="font-size: 3rem; color: #94a3b8"></i>
                  <p>No hay expedientes registrados para este cliente</p>
                </div>
              </template>

              <Column field="numeroExpediente" header="Número" sortable></Column>
              <Column field="asunto" header="Asunto" sortable></Column>
              <Column field="tipoExpediente" header="Tipo" sortable></Column>
              <Column field="estado" header="Estado" sortable>
                <template #body="{ data }">
                  <Tag :value="estadoLabel[data.estado]" :severity="estadoSeverity[data.estado]" />
                </template>
              </Column>
              <Column field="fechaApertura" header="Fecha Apertura" sortable>
                <template #body="{ data }">
                  {{ new Date(data.fechaApertura).toLocaleDateString('es-ES') }}
                </template>
              </Column>
              <Column field="totalActuaciones" header="Actuaciones" sortable>
                <template #body="{ data }">
                  <Tag :value="data.totalActuaciones" severity="info" />
                </template>
              </Column>
              <Column field="totalCitas" header="Citas" sortable>
                <template #body="{ data }">
                  <Tag :value="data.totalCitas" severity="warning" />
                </template>
              </Column>
              <Column field="totalDocumentos" header="Documentos" sortable>
                <template #body="{ data }">
                  <Tag :value="data.totalDocumentos" severity="success" />
                </template>
              </Column>
            </DataTable>
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>

<style scoped>
.cliente-detalle-view {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
}

.mb-3 {
  margin-bottom: 1rem;
}

.mt-3 {
  margin-top: 1rem;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.card-header h1 {
  font-size: 1.75rem;
  color: #1e293b;
  margin: 0;
}

.cliente-info {
  margin-bottom: 2rem;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.info-item label {
  font-weight: 600;
  color: #64748b;
  font-size: 0.875rem;
  text-transform: uppercase;
}

.info-item span {
  color: #1e293b;
  font-size: 1rem;
}

.observaciones {
  background-color: #f1f5f9;
  padding: 1rem;
  border-radius: 8px;
}

.observaciones label {
  font-weight: 600;
  color: #64748b;
  font-size: 0.875rem;
  text-transform: uppercase;
  display: block;
  margin-bottom: 0.5rem;
}

.observaciones p {
  color: #475569;
  margin: 0;
}

.expedientes-section {
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 2px solid #e2e8f0;
}

.expedientes-section h2 {
  font-size: 1.5rem;
  color: #1e293b;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.empty-state {
  text-align: center;
  padding: 3rem;
}

.empty-state p {
  color: #64748b;
  margin-top: 1rem;
  font-size: 1.1rem;
}
</style>
