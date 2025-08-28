import { api } from 'boot/axios';

const route = 'log';

export const logApi = {
  async getDummyLogs() {
    return api.get(`${route}/dummyLog`);
  },
  async getAllNames() {
    return api.post(`${route}/GetAllNames`);
  },
  async getByFilter(filter) {
    return api.post(`${route}/GetByFilter`, filter);
  },
};
