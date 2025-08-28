<template>
  <q-card ref="scrollContainer">
    <q-infinite-scroll
      :scroll-target="scrollContainer"
      :offset="150"
      spinner
      @load="onLoadScroll"
    />

    <q-table
      :rows-per-page-options="[0]"
      :rows="logStore.logs"
      :columns="columns"
      row-key="id"
      :loading="logStore.loading"
      hide-bottom
    >
      <template #body-cell-id="props">
        <q-td :props="props">
          <router-link
            style="cursor:pointer; color: var(--q-primary);"
            :to="{ name: 'log-detail', params: {id: props.row.id}}"
          >
            {{ props.row.id }}
          </router-link>
        </q-td>
      </template>
      <template #body-cell-message="props">
        <q-td :props="props">
          <div class="message-cell">
            <router-link 
              class="ellipsis" 
              style="max-width: 200px; display: inline-block; cursor:pointer; color: var(--q-primary);" 
              :to="{ name: 'log-detail', params: {id: props.row.id}}"
            >
              {{ props.row.message }}
            </router-link>
            <q-tooltip>{{ props.row.message }}</q-tooltip>
          </div>
        </q-td>
      </template>
    </q-table>
  </q-card>
</template>
  
<script setup>
import { ref } from 'vue';
import { date } from 'quasar';
import { useLogStore } from 'src/stores/log.store';
import { useFilterStore } from 'src/stores/filter.store';

const filterStore = useFilterStore();
const logStore = useLogStore();

// columns for table
const columns = [
  { name: 'id', required: true, label: 'ID', align: 'left', field: row => row.id, format: val => `${val}` },
  { name: 'virtualMachineName', label: 'Server Name', align: 'left', field: 'virtualMachineName' },
  { name: 'logType', label: 'Type', align: 'left', field: 'logType' },
  { name: 'message', label: 'Message', align: 'left', field: 'message' },
  { name: 'applicationName', label: 'App Name', align: 'left', field: 'applicationName' },
  { name: 'createDate', label: 'Creation Time', align: 'right', field: 'createDate', format: val => date.formatDate(val, 'DD. MM. YYYY HH:mm') },
];

// const formattedDate = computed(() => {
//   if (!record.value?.createDate) return '';
//   return new Date(record.value.createDate)
//     .toLocaleString('cs-CZ', { dateStyle: 'medium', timeStyle: 'short' });
// });

function onLoadScroll(index, done) {
  if (!filterStore.canNextScroll) {
    done(true);
    console.log('return');
    return;
  }

  filterStore.nextScroll()
    .then(() => done())
    .catch(() => done());
}

</script>
<style lang="sass">

.my-sticky-dynamic
  /* height or max-height is important */
  max-height: 89vh
  overflow-y: visible

  .q-table__top,
  .q-table__bottom,
  thead tr:first-child th /* bg color is important for th; just specify one */
    background-color: $secondary

  thead tr th
    position: sticky
    z-index: 1
  /* this will be the loading indicator */
  thead tr:last-child th
    /* height of all previous header rows */
    top: 48px
  thead tr:first-child th
    top: 0

  /* prevent scrolling behind sticky top row on focus */
  tbody
    /* height of all previous header rows */
    scroll-margin-top: 48px
</style>
<style scoped>
.scroll-container {
  flex: 1;            /* roztáhne se na výšku karty */
  min-height: 0;      /* dovolí flexu zmenšit se */
  overflow-y: auto;   /* tady je jediný scrollbar */
}

/* sticky header, if you want */

.my-sticky-dynamic table thead tr {
  position: sticky;
  top: 0;
  background: white;
  z-index: 1;
}
</style>
