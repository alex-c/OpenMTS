import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);

// Import modules
import ui from './modules/ui.js';

// Load user data from local storage
const token = localStorage.getItem('token');
const user = localStorage.getItem('user');
const role = localStorage.getItem('role');
const name = localStorage.getItem('name');

// Define store
export default new Vuex.Store({
  state: {
    token: token,
    user: user,
    role: role,
    name: name,
  },
  mutations: {
    login(state, token, user, role, name) {
      state.token = token;
      state.user = user;
      state.role = role;
      state.name = name;
      localStorage.setItem('token', token);
      localStorage.setItem('user', user);
      localStorage.setItem('role', role);
      localStorage.setItem('name', name);
    },
    logout(state) {
      state.token = null;
      state.user = null;
      state.role = null;
      state.name = null;
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      localStorage.removeItem('role');
      localStorage.removeItem('name');
    },
  },
  actions: {},
  modules: { ui },
});
