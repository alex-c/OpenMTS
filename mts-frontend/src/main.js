import Vue from 'vue';
Vue.config.productionTip = false;

// Element UI & theme
import ElementUI from 'element-ui';
import './element-theme.scss';
Vue.use(ElementUI);

// Router & Vuex
import router from './router';
import store from './store';

// Mount app
import App from './App.vue';
new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app');
