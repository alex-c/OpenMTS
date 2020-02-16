import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);

// Import modules
import ui from './modules/ui.js';

// Load user data from local storage
const token = localStorage.getItem('token');
const user = localStorage.getItem('user');
const name = localStorage.getItem('name');

// Define store
export default new Vuex.Store({
  state: {
    token: token,
    user: user,
    name: name,
  },
  mutations: {
    logout(state) {
      state.token = null;
      state.user = null;
      state.name = null;
    },
  },
  actions: {},
  modules: { ui },
});
