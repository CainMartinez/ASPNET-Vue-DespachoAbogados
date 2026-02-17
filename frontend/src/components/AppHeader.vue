<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const menuItems = ref([
  {
    label: 'Clientes',
    icon: 'pi pi-users',
    route: '/clientes'
  },
  {
    label: 'Expedientes',
    icon: 'pi pi-briefcase',
    route: '/expedientes'
  },
  {
    label: 'Actuaciones',
    icon: 'pi pi-file-edit',
    route: '/actuaciones'
  },
  {
    label: 'Citas',
    icon: 'pi pi-calendar',
    route: '/citas'
  },
  {
    label: 'Documentos',
    icon: 'pi pi-file-pdf',
    route: '/documentos'
  }
])

const isActiveRoute = (itemRoute: string) => {
  return route.path === itemRoute
}

const navigateTo = (itemRoute: string) => {
  router.push(itemRoute)
}
</script>

<template>
  <header class="law-header">
    <!-- Barra superior con información del despacho -->
    <div class="header-top">
      <div class="header-top-content">
        <div class="contact-info">
          <span class="contact-item">
            <i class="pi pi-phone"></i>
            +34 91 123 45 67
          </span>
          <span class="contact-item">
            <i class="pi pi-envelope"></i>
            contacto@despacholegal.com
          </span>
        </div>
      </div>
    </div>

    <!-- Barra principal con logo y navegación -->
    <div class="header-main">
      <div class="header-main-content">
        <!-- Logo y nombre del despacho -->
        <div class="logo-section" @click="router.push('/clientes')">
          <div class="logo-icon">
            <i class="pi pi-shield"></i>
            <div class="logo-scales">
              <i class="pi pi-star-fill"></i>
            </div>
          </div>
          <div class="logo-text">
            <h1 class="firm-name">Despacho Legal</h1>
            <p class="firm-tagline">Excelencia Jurídica desde 1985</p>
          </div>
        </div>

        <!-- Navegación principal -->
        <nav class="main-nav">
          <button
            v-for="item in menuItems"
            :key="item.route"
            :class="['nav-item', { 'active': isActiveRoute(item.route) }]"
            @click="navigateTo(item.route)"
          >
            <i :class="item.icon"></i>
            <span>{{ item.label }}</span>
            <div v-if="isActiveRoute(item.route)" class="active-indicator"></div>
          </button>
        </nav>
      </div>
    </div>

    <!-- Línea decorativa dorada -->
    <div class="header-divider"></div>
  </header>
</template>

<style scoped>
.law-header {
  position: sticky;
  top: 0;
  z-index: 1000;
  background: white;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  border-bottom: 3px solid #D4AF37;
}

/* Barra superior */
.header-top {
  background: linear-gradient(135deg, var(--primary-brown) 0%, var(--secondary-brown) 100%);
  border-bottom: 1px solid rgba(212, 175, 55, 0.3);
  font-family: 'Inter', sans-serif;
}

.header-top-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0.5rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.contact-info {
  display: flex;
  gap: 2rem;
}

.contact-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: white;
  font-size: 0.875rem;
  transition: color 0.3s;
}

.contact-item:hover {
  color: #D4AF37;
}

.contact-item i {
  color: #D4AF37;
  font-size: 0.875rem;
}

.user-section {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
  transition: transform 0.3s;
}

.user-section:hover {
  transform: translateY(-2px);
}

.user-avatar {
  width: 32px;
  height: 32px;
}

.user-name {
  color: white;
  font-weight: 600;
  font-size: 0.875rem;
}

/* Barra principal */
.header-main {
  background: white;
}

.header-main-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem 2rem;
  display: flex;
  align-items: center;
  gap: 3rem;
}

/* Logo */
.logo-section {
  display: flex;
  align-items: center;
  gap: 1rem;
  cursor: pointer;
  transition: all 0.3s;
  flex-shrink: 0;
}

.logo-section:hover {
  transform: translateY(-2px);
}

.logo-section:hover .logo-icon {
  transform: scale(1.05);
}

.logo-icon {
  position: relative;
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #D4AF37 0%, #E8C961 100%);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(212, 175, 55, 0.4);
  transition: transform 0.3s;
}

.logo-icon > .pi-shield {
  font-size: 2rem;
  color: var(--primary-brown);
}

.logo-scales {
  position: absolute;
  top: 5px;
  right: 5px;
  width: 18px;
  height: 18px;
  background: var(--primary-brown);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.logo-scales i {
  font-size: 0.6rem;
  color: #D4AF37;
}

.logo-text {
  font-family: 'Playfair Display', serif;
}

.firm-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0;
  line-height: 1.2;
  letter-spacing: 0.5px;
}

.firm-tagline {
  font-size: 0.75rem;
  color: #D4AF37;
  margin: 0;
  font-style: italic;
  letter-spacing: 1px;
}

/* Navegación */
.main-nav {
  display: flex;
  gap: 0.5rem;
  flex: 1;
  justify-content: center;
  font-family: 'Inter', sans-serif;
}

.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: transparent;
  border: 2px solid transparent;
  border-radius: 6px;
  color: #475569;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  transition: all 0.2s ease;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.nav-item i {
  font-size: 1rem;
}

.nav-item:hover {
  background: rgba(212, 175, 55, 0.1);
  border-color: var(--accent-gold);
  color: var(--accent-gold);
  transform: translateY(-1px);
}

.nav-item.active {
  background: linear-gradient(135deg, var(--primary-brown), var(--secondary-brown));
  border-color: var(--accent-gold);
  color: white;
  box-shadow: 0 4px 12px rgba(93, 78, 55, 0.3);
}

.active-indicator {
  position: absolute;
  bottom: -1px;
  left: 50%;
  transform: translateX(-50%);
  width: 60%;
  height: 3px;
  background: #D4AF37;
  border-radius: 3px 3px 0 0;
}

/* Línea divisoria */
.header-divider {
  height: 3px;
  background: linear-gradient(
    90deg,
    transparent 0%,
    #D4AF37 10%,
    #E8C961 50%,
    #D4AF37 90%,
    transparent 100%
  );
  box-shadow: 0 2px 8px rgba(212, 175, 55, 0.3);
}

/* Responsivo */
@media (max-width: 1200px) {
  .header-main-content {
    gap: 2rem;
  }

  .nav-item {
    padding: 0.75rem 1rem;
    font-size: 0.875rem;
  }

  .firm-name {
    font-size: 1.25rem;
  }
}

@media (max-width: 768px) {
  .contact-info {
    display: none;
  }

  .main-nav {
    gap: 0.25rem;
  }

  .nav-item span {
    display: none;
  }

  .nav-item {
    padding: 0.75rem;
  }
}
</style>
