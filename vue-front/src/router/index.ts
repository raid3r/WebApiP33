import { createRouter, createWebHistory } from 'vue-router'
import LoginPage from '../pages/LoginPage.vue'
import RegisterPage from '../pages/RegisterPage.vue'
import ChatPage from '../pages/ChatPage.vue'

const routes = [
  { path: '/', redirect: '/chat' },
  { path: '/login', name: 'login', component: LoginPage, meta: { guest: true } },
  { path: '/register', name: 'register', component: RegisterPage, meta: { guest: true } },
  { path: '/chat', name: 'chat', component: ChatPage, meta: { requiresAuth: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }
  if (token && to.meta.guest) {
    return { path: '/chat' }
  }
  return true
})

export default router
