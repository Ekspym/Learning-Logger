//---- Store for only data storing ----//
import { defineStore } from 'pinia';

export const useLogStore = defineStore('logStore', {
  state: () => ({
    logs: [],
    loading: false,
  }),

  actions: {
    // Setter for data(loaded logs)
    setLogs(logs) {
      this.logs = logs;
    },

    // Setter for loading
    setLoading(isLoading) {
      this.loading = isLoading;
    },
  },
});
