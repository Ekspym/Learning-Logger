//---- Store for filter params and filtering logic ----//
import { defineStore } from 'pinia';
import { useLogStore } from 'src/stores/log.store';
import { api } from 'src/boot/axios';

export const useFilterStore = defineStore('filterStore', {
  //default state of filter for API call
  state: () => ({
    fromDate: null,
    toDate: null,
    sortType: 'Id',
    virtualMachineName: null,
    logType: null,
    applicationName: null,
    isAscending: true,

    page: 0,
    logCount: 20,
  }),

  getters: {
    // Check if you can go to prev page
    canPrev: (state) => state.page > 0,

    // Check if you can go to next page (Button'll be enabled if dispayed logs eq to wanted count)
    canNext: (state) => {
      const logStore = useLogStore();
      return logStore.logs?.length === state.logCount;
    },
    canNextScroll: (state) => {
      const logStore = useLogStore();
      return logStore.logs?.length % state.logCount === 0;
    },
  },

  actions: {
    setFilterParams(params) {
      Object.entries(params).forEach(([k, v]) => {
        if (!(k in this)) {
          return;
        }
    
        // If the value is an array, treat an empty array as reset (null), otherwise use the array
        if (Array.isArray(v)) { 
          this[k] = v.length > 0 ? v : null; 
        } else if (v !== undefined) { // If the value is any other type (string, number, boolean, or null), do not ignore null values
          this[k] = v;
        }
      });
    },

    // Go to prev page
    async prevPage() {
      if (!this.canPrev) {
        return;
      }
      this.page--;
      await this.fetchLogs();
    },

    // Go to next page
    async nextPage() {
      if (!this.canNext) {
        return;
      }
      this.page++;
      await this.fetchLogs();
    },
    async nextScroll() {
      if (!this.canNextScroll) {
        return;
      }
      this.page++;
      await this.fetchLogs({ append: true });
    },

    // Go to specific page - not currently used
    async setPage(p) {
      if (p < 0) {
        p = 0;
      }
      this.page = p;
      await this.fetchLogs();
    },

    // Setting wanted count of logs per page
    setLogCount(n) {
      this.logCount = n;
      // Change will "reload" page
      this.page = 0;
      this.fetchLogs();
    },

    // Func for loading new data
    async fetchLogs({ append = false } = {}) {
      const f = this;
      const logStore = useLogStore();
      logStore.setLoading(true);

      const payload = {
        fromDate: f.fromDate,
        toDate: f.toDate,
        sortType: f.sortType,
        isAscending: f.isAscending,
        virtualMachines: f.virtualMachines,
        virtualMachineName: f.virtualMachineName,
        logType: f.logType,
        applicationName: f.applicationName,
        page: f.page,
        logCount: f.logCount,
      };
      /*
      const payload = Object.fromEntries(
        Object.entries(raw)
          .filter(([_, v]) => {
            if (v == null) return false;
            if (Array.isArray(v)) return v.length > 0;
            if (typeof v === 'string') return v !== '';
            return true;
          }),
      );*/

      try {
        const resp = await api.post('/log/GetByFilter', payload);
        const newBatch = resp.data;

        if (append) {
          // append to what’s already there
          logStore.setLogs([...logStore.logs, ...newBatch]);
        } else {
          // first load or manual refresh
          logStore.setLogs(newBatch);
        }
      } finally {
        logStore.setLoading(false);
      }
    },
  },
});
