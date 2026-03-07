<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const email = ref('')
const password = ref('')
const errorMessage = ref('')
const isLoading = ref(false)

const submitRegister = async () => {
  errorMessage.value = ''
  isLoading.value = true
  try {
    const response = await api.post('/api/v1/auth/register', {
      email: email.value,
      password: password.value
    })

    if (response.data?.success && response.data?.token) {
      localStorage.setItem('token', response.data.token)
      router.push('/chat')
      return
    }

    errorMessage.value = response.data?.error || 'Registration failed'
  } catch (error) {
    errorMessage.value = 'Registration failed'
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <section class="page fade-in">
    <div class="card auth-card">
      <div class="card-header">
        <p class="eyebrow">Start here</p>
        <h1>Create account</h1>
        <p class="subtitle">Register to start new conversations.</p>
      </div>

      <form class="form" @submit.prevent="submitRegister">
        <div class="field">
          <label for="email">Email</label>
          <input id="email" v-model="email" type="email" class="input" autocomplete="email" required />
        </div>

        <div class="field">
          <label for="password">Password</label>
          <input id="password" v-model="password" type="password" class="input" autocomplete="new-password" required />
        </div>

        <p v-if="errorMessage" class="notice error">{{ errorMessage }}</p>

        <button class="button primary" type="submit" :disabled="isLoading">
          {{ isLoading ? 'Creating...' : 'Create account' }}
        </button>
      </form>

      <div class="card-footer">
        <span>Already have an account?</span>
        <RouterLink class="text-link" to="/login">Sign in</RouterLink>
      </div>
    </div>
  </section>
</template>
