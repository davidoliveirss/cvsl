import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  {
    path: '/',
    name: 'home',
    component: () => import('../views/HomeView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/clientes',
    name: 'clientes',
    component: () => import('../views/ClientesView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/produtos',
    name: 'produtos',
    component: () => import('../views/ProdutosView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/funcionarios',
    name: 'funcionarios',
    component: () => import('../views/FuncionariosView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/animais',
    name: 'animais',
    component: () => import('../views/AnimaisView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('../views/LoginView.vue')
  },
  {
    path: '/avaliar',
    name: 'avaliar',
    component: () => import('../views/AvaliacaoView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/conta',
    name: 'conta',
    component: () => import('../views/AccountView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/calendario',
    name: 'calendario',
    component: () => import('../views/CalendarioView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/status',
    name: 'test',
    component: () => import('../views/StatusView.vue'),
    meta: { requiresAuth: true }
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login');
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/');
  } else {
    next();
  }
})

export default router

