<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const email = ref('')
const password = ref('')
const errorMessage = ref('')
const isLoading = ref(false)

const submitLogin = async () => {
  errorMessage.value = ''
  isLoading.value = true
  try {
    const response = await api.post('/api/v1/auth/login', {
      email: email.value,
      password: password.value
    })

    if (response.data?.success && response.data?.token) {
      localStorage.setItem('token', response.data.token)
      router.push('/chat')
      return
    }

    errorMessage.value = response.data?.error || 'Login failed'
  } catch (error) {
    errorMessage.value = 'Login failed'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <section class="page fade-in">
    <div class="card auth-card">
      <div class="card-header">
        <p class="eyebrow">Welcome back</p>
        <h1>Sign in</h1>
        <p class="subtitle">Use your account to access chats.</p>
      </div>

      <form class="form" @submit.prevent="submitLogin">
        <div class="field">
          <label for="email">Email</label>
          <input id="email" v-model="email" type="email" class="input" autocomplete="email" required />
        </div>

        <div class="field">
          <label for="password">Password</label>
          <input
            id="password"
            v-model="password"
            type="password"
            class="input"
            autocomplete="current-password"
            required
          />
        </div>

        <p v-if="errorMessage" class="notice error">{{ errorMessage }}</p>

        <button class="button primary" type="submit" :disabled="isLoading">
          {{ isLoading ? 'Signing in...' : 'Sign in' }}
        </button>
      </form>

      <div class="card-footer">
        <span>New here?</span>
        <RouterLink class="text-link" to="/register">Create an account</RouterLink>
      </div>
    </div>
  </section>
</template>
