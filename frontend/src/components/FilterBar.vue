<script setup lang="ts">
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'

defineProps<{
  searchQuery: string
  searchPlaceholder: string
  viewMode: 'grid' | 'table'
  filterLabel?: string | null
}>()

defineEmits<{
  'update:searchQuery': [value: string]
  'update:viewMode': [value: 'grid' | 'table']
  clearFilter: []
}>()
</script>

<template>
  <Card class="filters-card">
    <template #content>
      <div v-if="filterLabel" class="filter-chip-bar">
        <div class="filter-chip">
          <i class="pi pi-filter"></i>
          <span>Filtrando por: <strong>{{ filterLabel }}</strong></span>
          <button class="chip-clear" @click="$emit('clearFilter')" v-tooltip.top="'Quitar filtro'">
            <i class="pi pi-times"></i>
          </button>
        </div>
      </div>
      <div class="filters-bar">
        <div class="search-section">
          <span class="p-input-icon-left search-wrapper">
            <i class="pi pi-search"></i>
            <InputText
              :modelValue="searchQuery"
              @update:modelValue="$emit('update:searchQuery', $event as string)"
              :placeholder="'🔍 ' + searchPlaceholder"
              class="search-input-custom"
            />
          </span>
        </div>

        <div class="view-toggle">
          <Button
            icon="pi pi-th-large"
            :class="{ 'active': viewMode === 'grid' }"
            @click="$emit('update:viewMode', 'grid')"
            :text="viewMode !== 'grid'"
            :raised="viewMode === 'grid'"
            rounded
            v-tooltip.top="'Vista en tarjetas'"
          />
          <Button
            icon="pi pi-list"
            :class="{ 'active': viewMode === 'table' }"
            @click="$emit('update:viewMode', 'table')"
            :text="viewMode !== 'table'"
            :raised="viewMode === 'table'"
            rounded
            v-tooltip.top="'Vista en tabla'"
          />
        </div>
      </div>
    </template>
  </Card>
</template>

<style scoped>
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

@media (max-width: 768px) {
  .filters-bar {
    flex-direction: column;
    align-items: stretch;
  }

  .search-section {
    width: 100%;
  }
}
</style>
