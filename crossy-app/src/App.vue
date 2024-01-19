<script setup lang="ts">
import { ref, watchEffect } from "vue"
import { Character, UnlockType } from "./models/Character"
import { Category } from "./models/Category"

interface Data {
  characters: Character[]
  categories: Category[]
}

const data = ref<Data | null>(null)

watchEffect(async () => data.value = await ((await fetch("data.json")).json()))

function typeDisplay(type: UnlockType)
{
  switch (type)
  {
    case UnlockType.Prize:
      return "Prize Machine";
    case UnlockType.Free:
      return "Free";
    case UnlockType.Purchase:
      return "Purchase";
    case UnlockType.Secret:
      return "Secret";
    case UnlockType.Token:
      return "Token"
  }
}

</script>

<template>
  <div>
    <p>Crossy Road</p>
    <p>{{ data?.characters.length }}</p>
    <ul v-if="data">
      <li v-for="character in data.characters">{{ character.name }} ({{ typeDisplay(character.type) }})</li>

    </ul>
  </div>
</template>

<style scoped></style>
