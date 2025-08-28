<template>
  <q-layout view="hHh Lpr fFf">
    <q-header elevated class="bg-primary text-white" height-hint="98">
      <q-toolbar>
        <q-toolbar-title>
          <div class="flex q-m">
            <q-btn
              dense
              flat
              round
              icon="filter_list"
              @click="toggleLeftDrawer"
            />
            <div>
              GAIA APP LOGS
            </div>
          </div>
        </q-toolbar-title>
        <q-tabs align="left">
          <q-route-tab to="/" label="Home Page" />
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="leftDrawerOpen"
      side="left"
      :overlay="false"
      behavior="desktop"
      elevated
      :width="500"
    >
      <!-- drawer content -->
      <LogFilter />
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref } from 'vue';
import LogFilter from 'src/components/LogFilter.vue';

const leftDrawerOpen = ref(false);

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}
</script>
