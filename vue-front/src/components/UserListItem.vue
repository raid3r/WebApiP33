<script setup lang="ts">
interface Props {
  label: string
  secondary?: string
}

const props = defineProps<Props>()

const avatarLetter = props.label.trim().slice(0, 1).toUpperCase()
</script>

<template>
  <span class="avatar">{{ avatarLetter }}</span>
  <div>
    <strong>{{ label }}</strong>
    <span v-if="secondary" class="muted">{{ secondary }}</span>
  </div>
</template>
