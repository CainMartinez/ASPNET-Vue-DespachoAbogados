<script setup lang="ts">
import Button from 'primevue/button'

defineProps<{
  title: string
  subtitle: string
  icon: string
  buttonLabel: string
  buttonIcon: string
}>()

defineEmits<{
  action: []
}>()
</script>

<template>
  <div class="page-hero">
    <div class="hero-content">
      <div class="hero-text">
        <h1 class="page-title">
          <i :class="icon"></i>
          {{ title }}
        </h1>
        <p class="page-subtitle">{{ subtitle }}</p>
      </div>
      <Button
        :label="buttonLabel"
        :icon="buttonIcon"
        class="hero-btn"
        size="large"
        raised
        rounded
        @click="$emit('action')"
      />
    </div>
  </div>
</template>

<style scoped>
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

@media (max-width: 768px) {
  .hero-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
  }

  .page-title {
    font-size: 1.75rem;
  }
}
</style>
