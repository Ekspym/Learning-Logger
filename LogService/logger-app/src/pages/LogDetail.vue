<template>
  <q-page padding>
    <div v-if="loading" class="row justify-center">
      <q-spinner-dots size="50px" />
    </div>
  
    <div v-else-if="!record">
      <q-banner dense class="bg-red-2 text-white">
        Záznam s ID {{ id }} nebyl nalezen.
      </q-banner>
      <q-btn
        flat
        icon="arrow_back"
        label="Zpět"
        @click="goBack"
      />
    </div>
  
    <div v-else>
      <q-card>
        <q-card-section>
          <div class="text-h5">Detail logu #{{ record.id }}</div>
          <div class="text-subtitle2 q-mt-sm">{{ formattedDate }}</div>
        </q-card-section>
        <q-separator />
        <q-card-section>
          <div class="text-h6">Message</div>
          <div class="message-block">{{ record.message }}</div>
        </q-card-section>
        <q-card-section>
          <div class="text-h6">INFO</div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn
            flat
            icon="arrow_back"
            label="Zpět"
            @click="goBack"
          />
        </q-card-actions>
      </q-card>
    </div>
  </q-page>
</template>
  
<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { api } from 'src/boot/axios';
  
const route = useRoute();
const router = useRouter();
  
const id = ref(route.params.id);
const record = ref(null);
const loading = ref(true);
  
const fetch = async () => {
  loading.value = true;
  try {
    // POST request na konkrétní záznam
    const response = await api.post(`/log/GetLogByID?id=${id.value}`);
    record.value = response.data;
  } catch (e) {
    record.value = null;
  } finally {
    loading.value = false;
  }
};
  
const goBack = () => router.back();
  
onMounted(fetch);

watch(() => route.params.id, newId => {
  id.value = newId;
  fetch();
});
  
const formattedDate = computed(() => {
  if (!record.value?.createDate) return '';
  return new Date(record.value.createDate)
    .toLocaleString('cs-CZ', { dateStyle: 'medium', timeStyle: 'short' });
});
</script>
<style scoped>
.message-block {
    white-space: pre-wrap;
}
</style>
