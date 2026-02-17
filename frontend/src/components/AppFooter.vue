<template>
  <footer class="app-footer">
    <div class="footer-content">
      <div class="footer-section">
        <h3 class="footer-title">
          <i class="pi pi-shield"></i>
          Despacho Legal
        </h3>
        <p class="footer-tagline">Excelencia jurídica desde 1985</p>
      </div>

      <div class="footer-section">
        <h4 class="section-title">Contacto</h4>
        <ul class="footer-list">
          <li>
            <i class="pi pi-map-marker"></i>
            <span>Calle Gran Vía 45, 3º A<br>28013 Madrid, España</span>
          </li>
          <li>
            <i class="pi pi-phone"></i>
            <span>+34 91 123 45 67</span>
          </li>
          <li>
            <i class="pi pi-envelope"></i>
            <span>contacto@despacholegal.com</span>
          </li>
        </ul>
      </div>

      <div class="footer-section">
        <h4 class="section-title">Horario</h4>
        <ul class="footer-list">
          <li>
            <i class="pi pi-clock"></i>
            <span>Lunes a Viernes: 9:00 - 19:00</span>
          </li>
          <li>
            <i class="pi pi-calendar"></i>
            <span>Sábados: 10:00 - 14:00</span>
          </li>
        </ul>
      </div>

      <div class="footer-section">
        <h4 class="section-title">Legal</h4>
        <ul class="footer-links">
          <li><a href="#" @click.prevent>Política de Privacidad</a></li>
          <li><a href="#" @click.prevent>Términos de Uso</a></li>
          <li><a href="#" @click.prevent>Política de Cookies</a></li>
          <li><a href="#" @click.prevent>Aviso Legal</a></li>
        </ul>
      </div>
    </div>

    <div class="footer-bottom">
      <div class="footer-copyright">
        <p>
          &copy; {{ new Date().getFullYear() }} Despacho Legal. Todos los derechos reservados.
          <span class="divider">|</span>
          <span class="colegiado">Colegiado ICAM Nº 12345</span>
        </p>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
</script>

<style scoped>
.app-footer {
  background: linear-gradient(135deg, var(--primary-brown) 0%, var(--secondary-brown) 100%);
  color: rgba(255, 255, 255, 0.9);
  margin-top: 4rem;
  border-top: 3px solid var(--accent-gold);
}

.footer-content {
  max-width: 1600px;
  margin: 0 auto;
  padding: 3rem 2rem 2rem 2rem;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2.5rem;
}

.footer-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.footer-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--accent-gold);
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.footer-title i {
  font-size: 1.75rem;
}

.footer-tagline {
  color: rgba(255, 255, 255, 0.7);
  font-style: italic;
  margin: 0;
  font-size: 0.95rem;
}

.section-title {
  font-family: 'Playfair Display', serif;
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--accent-gold);
  margin: 0 0 0.75rem 0;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid rgba(212, 175, 55, 0.3);
}

.footer-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.footer-list li {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  font-size: 0.95rem;
  line-height: 1.6;
}

.footer-list li i {
  color: var(--accent-gold);
  margin-top: 0.25rem;
  flex-shrink: 0;
}

.footer-links {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.footer-links li a {
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  font-size: 0.95rem;
  transition: all 0.2s ease;
  display: inline-block;
}

.footer-links li a:hover {
  color: var(--accent-gold);
  transform: translateX(5px);
}

.footer-bottom {
  background: rgba(0, 0, 0, 0.2);
  padding: 1.5rem 2rem;
  border-top: 1px solid rgba(212, 175, 55, 0.2);
}

.footer-copyright {
  max-width: 1600px;
  margin: 0 auto;
  text-align: center;
}

.footer-copyright p {
  margin: 0;
  font-size: 0.9rem;
  color: rgba(255, 255, 255, 0.7);
}

.divider {
  margin: 0 0.75rem;
  color: var(--accent-gold);
}

.colegiado {
  font-weight: 600;
  color: var(--accent-gold);
}

/* Responsive */
@media (max-width: 768px) {
  .footer-content {
    grid-template-columns: 1fr;
    gap: 2rem;
    padding: 2rem 1rem 1.5rem 1rem;
  }

  .footer-bottom {
    padding: 1rem;
  }

  .footer-copyright p {
    font-size: 0.85rem;
  }

  .divider {
    display: block;
    margin: 0.25rem 0;
  }
}
</style>
