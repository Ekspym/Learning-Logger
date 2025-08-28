<template>
  <q-card flat bordered class="q-pa-md">
    <q-form @submit.prevent="applyFilter">
      <!-- Vše v jednom sloupci -->
      <div class="column q-gutter-md">
        <!-- SERVER SELECT (full-width) -->
        <q-select
          v-model="localFilter.virtualMachineName"
          label="Server Name"
          use-chips
          use-input
          multiple
          emit-value
          map-options
          filled
          dense
          :options="options"
          @filter="filterFn"
        >
          <template #no-option>
            <q-item><q-item-section class="text-grey">No results</q-item-section></q-item>
          </template>
        </q-select>

        <!-- LOG TYPE (hned pod) -->
        <q-select
          v-model="localFilter.logType"
          :options="logTypeOptions"
          label="Log Type"
          use-chips
          multiple
          emit-value
          map-options
          filled
          dense
        />

        <q-separator spaced />

        <!-- DATE FILTER RADIO -->
        <q-option-group
          v-model="localFilter.dateFilterType"
          type="radio"
          inline
          :options="[
            { label: 'Range (From–To)', value: 'range' },
            { label: 'From (to now)', value: 'from' },
            { label: 'To (since start)', value: 'to' }
          ]"
        />

        <!-- DATE INPUTS -->
        <div class="column q-gutter-md">
          <q-input
            v-if="localFilter.dateFilterType === 'range' || localFilter.dateFilterType === 'from'"
            v-model="localFilter.fromDate"
            label="From"
            type="datetime-local"
            filled
            dense
          />
          <q-input
            v-if="localFilter.dateFilterType === 'range' || localFilter.dateFilterType === 'to'"
            v-model="localFilter.toDate"
            label="To"
            type="datetime-local"
            filled
            dense
          />
        </div>

        <!-- APPLY BUTTON -->
        <q-btn
          color="primary"
          label="Apply"
          type="submit"
        />
      </div>
    </q-form>

    <q-separator spaced />
    <q-select
      v-model="filterStore.logCount"
      :options="[10, 15, 20]"
      label="Logs per page"
      dense
      filled
      style="max-width: 50%; align-items: center;"
      @update:model-value="filterStore.setLogCount"
    />
    <!-- PAGINATION CONTROLS -->
    <div class="row items-center justify-between q-gutter-sm">
      <q-btn
        label="« Previous"
        :disable="!filterStore.canPrev"
        @click="filterStore.prevPage"
      />
      <div class="text-h6">Page: {{ filterStore.page + 1 }}</div>
      <q-btn
        label="Next »"
        :disable="!filterStore.canNext"
        @click="filterStore.nextPage"
      />
    </div>
  </q-card>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { useFilterStore } from 'src/stores/filter.store';
import { api } from 'src/boot/axios';

const filterStore = useFilterStore();

// Lokální stav filtru
const localFilter = reactive({
  fromDate: null,
  toDate: null,
  virtualMachineName: null,
  logType: ['Information', 'Debug', 'Warning', 'Error', 'Critical'],
  applicationName: null,
  dateFilterType: 'range',
});

// Zdroje pro selecty
const searchOptions = ref([]);
const options = ref([]);

// Možnosti logType
const logTypeOptions = [
  { label: 'Information', value: 'Information' },
  { label: 'Debug', value: 'Debug' },
  { label: 'Warning', value: 'Warning' },
  { label: 'Error', value: 'Error' },
  { label: 'Critical', value: 'Critical' },
];

// Načtení všech názvů serverů pro select
async function fetchSearchOptions() {
  try {
    const resp = await api.get('/log/GetAllNames');
    searchOptions.value = resp.data.map(item => ({ label: item, value: item }));
    options.value = searchOptions.value;
  } catch (err) {
    console.error('Chyba fetchSearchOptions:', err);
  }
}
onMounted(fetchSearchOptions);

// Filtr pro q-select s use-input
//const options = ref(searchOptions.value);

function filterFn(val, update) {
  if (val === '') {
    update(() => {
      options.value = searchOptions.value;
    });
    return;
  }

  update(() => {
    const needle = val.toLowerCase();
    options.value = searchOptions.value.filter(v => v && v.label && v.label.toLowerCase().indexOf(needle) > -1);
  });
}

function orNull(arr) {
  return Array.isArray(arr) && arr.length === 0 ? null : arr;
}

// Filter will be saved to a pinia store and fetch for new logs
function applyFilter() {
  const params = {
    virtualMachineName: orNull(localFilter.virtualMachineName),
    logType: orNull(localFilter.logType),
    applicationName: orNull(localFilter.applicationName),
  };

  filterStore.setFilterParams(params);
  
  if (localFilter.dateFilterType === 'range') {
    filterStore.setFilterParams({
      fromDate: localFilter.fromDate,
      toDate: localFilter.toDate,
    });
  } else if (localFilter.dateFilterType === 'from') {
    filterStore.setFilterParams({
      fromDate: localFilter.fromDate,
      toDate: null,
    });
  } else if (localFilter.dateFilterType === 'to') {
    filterStore.setFilterParams({
      toDate: localFilter.toDate,
      fromDate: null,
    });
  }

  // Reset stránky a velikosti
  filterStore.setPage(0);
  filterStore.setLogCount(filterStore.logCount);

  filterStore.fetchLogs();
}
</script>
