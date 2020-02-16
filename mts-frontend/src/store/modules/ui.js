export default {
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
