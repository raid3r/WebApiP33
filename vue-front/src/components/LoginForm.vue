<script lang="ts" setup>
import { ref } from 'vue'

interface LoginForm {
  email: string
  password: string
}

const loginForm = ref<LoginForm>({
  email: '',
  password: ''
})


const errorMessage = ref<string>('')
const successMessage = ref<string>('')

const profile = ref<any>(null)

const getProfile = async (token: string) => {
  await fetch('/api/v1/profile', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    }
  }).then(res => res.json())
    .then(data => {
      console.log(data)
      profile.value = data
    })
    .catch(err => {
      console.error(err)
    })
}



const clickLogin = async () => {
  await fetch('/api/v1/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(loginForm.value)
  }).then(res => res.json())
    .then(async data => {
      console.log(data)
      if (data.success) {
        successMessage.value = 'Login successful'
        await getProfile(data.token)
      } else {
        errorMessage.value = data.error || 'Login failed'
      }

      setTimeout(() => {
        successMessage.value = ''
        errorMessage.value = ''
      }, 3000)
    })
    .catch(err => {
      console.error(err)
      errorMessage.value = 'Login failed'
      setTimeout(() => {
        errorMessage.value = ''
      }, 3000)
    })
}

</script>

<template>

<div>
  {{ JSON.stringify(profile) }}
</div>
<h2>Login</h2>
<div>
  <div class="form-group">
    <label for="email">Email:</label>
    <input type="email" id="email" name="email" v-model="loginForm.email" required>
  </div>

  <div class="form-group">
    <label for="password">Password:</label>
    <input type="password" id="password" name="password" v-model="loginForm.password" required>
  </div>

  <div v-if="errorMessage.length > 0" class="error-message">{{ errorMessage }}</div>
  <div v-if="successMessage.length > 0" class="success-message">{{ successMessage }}</div>

  <button type="submit" @click="clickLogin">Login</button>

</div>


</template>



<style scoped>

.error-message {
  color: red;
  margin-top: 10px;
}
.success-message {
  color: green;
  margin-top: 10px;
}

</style>
