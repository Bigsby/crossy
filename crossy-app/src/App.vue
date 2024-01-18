<script setup lang="ts">
import { ref, watchEffect } from "vue"
import { Character } from "./models/Character"
import { Category } from "./models/Category"

interface Data {
  characters: Character[]
  categories: Category[]
}

const data = ref<Data | null>(null)

watchEffect(async () => data.value = await ((await fetch("data.json")).json()))

function getCategoryCount(category: Category): number {
  return data.value?.characters.filter(c => c.category == category.name).length ?? 0;
}

</script>

<template>
  <div>
    <p>Crossy Road</p>
    <p>{{ data?.characters.length }}</p>
    <ul v-if="data">
      <li v-for="category in data.categories">{{ category.name }} ({{ getCategoryCount(category)}})</li>

    </ul>
  </div>
</template>

<style scoped></style>
