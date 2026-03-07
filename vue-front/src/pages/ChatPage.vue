<script setup lang="ts">
import { computed, nextTick, onMounted, onUnmounted, ref } from 'vue'
import api from '../services/api'
import UserListItem from '../components/UserListItem.vue'

interface RecipientDto {
  id: number
  name: string
}

interface UserDto {
  id: number
  recipientId: number
  email: string
  recipient?: RecipientDto | null
}

interface MessageDto {
  id: number
  from: RecipientDto
  to: RecipientDto
  text: string
  timestamp: string
}

const profile = ref<UserDto | null>(null)
const profileError = ref('')
const searchTerm = ref('')
const searchResults = ref<UserDto[]>([])
const searchError = ref('')
const chats = ref<UserDto[]>([])
const chatsError = ref('')
const isLoadingChats = ref(false)
const selectedUser = ref<UserDto | null>(null)
const messages = ref<MessageDto[]>([])
const messageText = ref('')
const isLoadingMessages = ref(false)
const isSending = ref(false)
const pollDelayMs = 5000
const pollTimeoutId = ref<number | null>(null)
const messageListRef = ref<HTMLElement | null>(null)
const lastMessageId = ref<number | null>(null)

const currentRecipientId = computed(() => profile.value?.recipientId ?? 0)

const displayName = (user: UserDto) => user.recipient?.name || user.email
const secondaryLabel = (user: UserDto) => ''

const scrollToBottom = async () => {
  await nextTick()
  if (messageListRef.value) {
    messageListRef.value.scrollTop = messageListRef.value.scrollHeight
  }
}

const loadProfile = async () => {
  profileError.value = ''
  try {
    const response = await api.get('/api/v1/profile')
    profile.value = response.data
  } catch (error) {
    profileError.value = 'Unable to load profile'
  }
}

const loadChats = async () => {
  chatsError.value = ''
  isLoadingChats.value = true
  try {
    const response = await api.get('/api/v1/chat/chats')
    chats.value = response.data || []
  } catch (error) {
    chatsError.value = 'Unable to load chats'
  } finally {
    isLoadingChats.value = false
  }
}

const findUsers = async () => {
  searchError.value = ''
  searchResults.value = []
  if (!searchTerm.value.trim()) {
    searchError.value = 'Enter a name to search'
    return
  }

  try {
    const response = await api.post('/api/v1/chat/find-users', { name: searchTerm.value.trim() })
    searchResults.value = response.data || []
  } catch (error) {
    searchError.value = 'Search failed'
  }
}

const selectUser = async (user: UserDto) => {
  selectedUser.value = user
  lastMessageId.value = null
  await loadMessages()
}

const loadMessages = async () => {
  if (!selectedUser.value) {
    messages.value = []
    return
  }

  isLoadingMessages.value = true
  try {
    const response = await api.get(`/api/v1/chat/messages/${selectedUser.value.recipientId}`)
    messages.value = response.data || []
    const newLastId = messages.value.length ? messages.value[messages.value.length - 1].id : null
    if (newLastId !== null && newLastId !== lastMessageId.value) {
      lastMessageId.value = newLastId
      await scrollToBottom()
    }
  } catch (error) {
    messages.value = []
  } finally {
    isLoadingMessages.value = false
  }
}

const sendMessage = async () => {
  if (!selectedUser.value || !messageText.value.trim()) {
    return
  }

  isSending.value = true
  try {
    await api.post('/api/v1/chat/send', {
      recipientId: selectedUser.value.recipientId,
      text: messageText.value.trim()
    })
    messageText.value = ''
    await loadMessages()
    await loadChats()
  } finally {
    isSending.value = false
  }
}

const formatTimestamp = (value: string) => {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) {
    return value
  }
  return date.toLocaleString()
}

const isMine = (message: MessageDto) => message.from?.id === currentRecipientId.value

const schedulePoll = () => {
  if (pollTimeoutId.value !== null) {
    window.clearTimeout(pollTimeoutId.value)
  }
  pollTimeoutId.value = window.setTimeout(async () => {
    if (selectedUser.value && !isLoadingMessages.value) {
      await loadMessages()
    }
    schedulePoll()
  }, pollDelayMs)
}

onMounted(async () => {
  await loadProfile()
  await loadChats()
  schedulePoll()
})

onUnmounted(() => {
  if (pollTimeoutId.value !== null) {
    window.clearTimeout(pollTimeoutId.value)
  }
})
</script>

<template>
  <section class="page fade-in">
    <header class="chat-header">
      <div>
        <p class="eyebrow">Chat dashboard</p>
        <h1>Messages</h1>
      </div>
      <div class="profile-chip" v-if="profile">
        <span class="dot"></span>
        <div>
          <strong>{{ profile.email }}</strong>
          <!-- <span class="muted">Recipient #{{ profile.recipientId }}</span> -->
        </div>
      </div>
    </header>

    <p v-if="profileError" class="notice error">{{ profileError }}</p>

    <div class="chat-layout">
      <aside class="card panel">
        <div>
          <h2>Your chats</h2>
          <p class="muted">People you have already messaged.</p>
        </div>
        <div class="list">
          <p v-if="isLoadingChats" class="muted">Loading chats...</p>
          <p v-else-if="chatsError" class="notice error">{{ chatsError }}</p>
          <p v-else-if="!chats.length" class="muted">No chats yet.</p>
          <button
            v-for="user in chats"
            :key="user.recipientId"
            class="list-item"
            :class="{ active: selectedUser?.recipientId === user.recipientId }"
            type="button"
            @click="selectUser(user)"
          >
            <UserListItem :label="displayName(user)" :secondary="secondaryLabel(user)" />
          </button>
        </div>

        <div class="divider"></div>

        <h2>Find people</h2>
        <form class="form" @submit.prevent="findUsers">
          <div class="field">
            <label for="search">Name</label>
            <input id="search" v-model="searchTerm" class="input" placeholder="Search by name" />
          </div>
          <button class="button ghost" type="submit">Search</button>
          <p v-if="searchError" class="notice error">{{ searchError }}</p>
        </form>

        <div class="list">
          <p v-if="!searchResults.length && !searchError" class="muted">No results yet.</p>
          <button
            v-for="user in searchResults"
            :key="user.recipientId"
            class="list-item"
            :class="{ active: selectedUser?.recipientId === user.recipientId }"
            type="button"
            @click="selectUser(user)"
          >
            <UserListItem :label="displayName(user)" :secondary="secondaryLabel(user)" />
          </button>
        </div>
      </aside>

      <section class="card panel">
        <div class="conversation-head">
          <div>
            <h2>{{ selectedUser ? displayName(selectedUser) : 'Select a user' }}</h2>
            <p class="muted">{{ selectedUser ? 'Conversation history' : 'Choose a person to start' }}</p>
          </div>
          <button class="button ghost" type="button" :disabled="!selectedUser" @click="loadMessages">
            Refresh
          </button>
        </div>

        <div class="message-list" ref="messageListRef">
          <p v-if="isLoadingMessages" class="muted">Loading messages...</p>
          <p v-else-if="!messages.length" class="muted">No messages yet.</p>
          <div
            v-for="message in messages"
            :key="message.id"
            class="message-bubble"
            :class="{ mine: isMine(message) }"
          >
            <div class="message-meta">
              <span>{{ message.from?.name }}</span>
              <span>{{ formatTimestamp(message.timestamp) }}</span>
            </div>
            <p>{{ message.text }}</p>
          </div>
        </div>

        <form class="composer" @submit.prevent="sendMessage">
          <input
            v-model="messageText"
            class="input"
            placeholder="Write a message"
            :disabled="!selectedUser"
          />
          <button class="button primary" type="submit" :disabled="!selectedUser || isSending">
            {{ isSending ? 'Sending...' : 'Send' }}
          </button>
        </form>
      </section>
    </div>
  </section>
</template>
