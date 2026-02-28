<script setup lang="ts">
  import HelloWorld from './components/HelloWorld.vue'
import LoginForm from './components/LoginForm.vue'
  import TheWelcome from './components/TheWelcome.vue'


  async function testProfile() {
    await fetch('/api/v1/profile', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    }).then(res => res.json())
      .then(data => {
        console.log(data)
      })
      .catch(err => {
        console.error(err)
      })
  }

  async function test() {
    const login = {
      "email": "test1@test.com",
      "password": "1111"
    };

    await fetch('/api/v1/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(login)
    }).then(res => res.json())
      .then(data => {
        console.log(data)
      })
      .catch(err => {
        console.error(err)
      })
  }



</script>

<template>
  <header>


  </header>

  <main>
    <LoginForm />

  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
