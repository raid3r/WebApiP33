<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { RouterLink, RouterView, useRoute, useRouter } from 'vue-router'

const router = useRouter()
const route = useRoute()
const token = ref(localStorage.getItem('token'))

watch(
  () => route.fullPath,
  () => {
    token.value = localStorage.getItem('token')
  }
)

const isAuthed = computed(() => Boolean(token.value))

const logout = () => {
  localStorage.removeItem('token')
  token.value = null
  router.push('/login')
}
</script>

<template>
  <div class="app-shell">
    <header class="app-header">
      <RouterLink class="brand" to="/">
        <span class="brand-mark"></span>
        <span>PulseChat</span>
      </RouterLink>
      <nav class="nav-actions">
        <RouterLink v-if="!isAuthed" class="text-link" to="/login">Sign in</RouterLink>
        <RouterLink v-if="!isAuthed" class="text-link" to="/register">Register</RouterLink>
        <button v-if="isAuthed" class="button ghost" type="button" @click="logout">Logout</button>
      </nav>
    </header>

    <main class="app-main">
      <RouterView />
    </main>
  </div>
</template>
