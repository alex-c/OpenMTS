import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);

const ui = {
  state: {
    menuCollapsed: false,
  },
  mutations: {
    toggleSidebar(state) {
      state.menuCollapsed = !state.menuCollapsed;
    },
  },
  actions: {},
  getters: {},
};

export default new Vuex.Store({
  state: {},
  mutations: {},
  actions: {},
  modules: { ui },
});
