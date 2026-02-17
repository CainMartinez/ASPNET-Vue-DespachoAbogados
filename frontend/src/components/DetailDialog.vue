<script setup lang="ts">
import Dialog from 'primevue/dialog'

export interface DetailField {
  label: string
  value: string | number | null | undefined
  fallback?: string
}

defineProps<{
  visible: boolean
  header: string
  fields: DetailField[]
}>()

defineEmits<{
  'update:visible': [value: boolean]
}>()
</script>

<template>
  <Dialog
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    :header="header"
    :modal="true"
    :style="{ width: '720px', maxWidth: '95vw' }"
    :dismissableMask="true"
    class="professional-dialog"
    :contentStyle="{ padding: '2rem' }"
  >
    <div class="dialog-content">
      <div
        v-for="(field, index) in fields"
        :key="index"
        class="detail-block"
      >
        <span class="detail-label">{{ field.label }}</span>
        <span class="detail-value">{{ field.value || field.fallback || '-' }}</span>
      </div>
    </div>
  </Dialog>
</template>

<style scoped>
.dialog-content {
  padding: 0;
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
</style>
