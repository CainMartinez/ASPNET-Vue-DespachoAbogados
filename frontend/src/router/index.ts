import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      redirect: '/clientes'
    },
    {
      path: '/clientes',
      name: 'clientes',
      component: () => import('@/views/ClientesView.vue')
    },
    {
      path: '/clientes/:id',
      name: 'cliente-detalle',
      component: () => import('@/views/ClienteDetalleView.vue'),
      props: true
    },
    {
      path: '/expedientes',
      name: 'expedientes',
      component: () => import('@/views/ExpedientesView.vue')
    },
    {
      path: '/actuaciones',
      name: 'actuaciones',
      component: () => import('@/views/ActuacionesView.vue')
    },
    {
      path: '/citas',
      name: 'citas',
      component: () => import('@/views/CitasView.vue')
    },
    {
      path: '/documentos',
      name: 'documentos',
      component: () => import('@/views/DocumentosView.vue')
    }
  ]
})

export default router
