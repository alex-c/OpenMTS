export default {
  state: {
    menuCollapsed: false,
  },
  mutations: {
    toggleSidebar(state) {
      state.menuCollapsed = !state.menuCollapsed;
    },
    collapseSidebar(state) {
      state.menuCollapsed = true;
    },
    expandSidebar(state) {
      state.menuCollapsed = false;
    },
  },
  actions: {},
  getters: {},
};
