<template>
  <q-page padding>
    <!-- Table without :id -->
    <LogTable v-if="!hasId" />

    <!-- Detail (child-route) with :id -->
    <router-view v-else />
  </q-page>
</template>

<script setup>
import { onMounted, computed } from 'vue';
import LogTable from 'src/components/LogTable.vue';
import { useFilterStore } from 'src/stores/filter.store';
import { useRoute } from 'vue-router';
// import { useLogStore } from 'src/stores/log.store';

// const logStore = useLogStore();
const filterStore = useFilterStore();
const route = useRoute();
const hasId = computed(() => !!route.params.id);

onMounted(() => {
  filterStore.fetchLogs();
});

</script>
