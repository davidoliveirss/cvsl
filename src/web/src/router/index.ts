import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'home',
    component: () => import('../views/HomeView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/pesquisa',
    name: 'pesquisa',
    component: () => import('../views/PesquisaView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/aluno/:id',
    name: 'aluno',
    component: () => import('../views/AlunoView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/resumos',
    name: 'resumos',
    component: () => import('../views/ResumosView.vue'),
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

// Guarda o estado de login (simples com localStorage)
/*router.beforeEach((to, from, next) => {
  const loggedIn = localStorage.getItem('user') // exemplo: "user" guardado no login
  if (to.meta.requiresAuth && !loggedIn) {
    next('/login') // redireciona para login se não estiver autenticado
  } else {
    next() // permite acesso
  }
})
*/
export default router

