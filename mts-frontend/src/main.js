import Vue from 'vue';
Vue.config.productionTip = false;

// Element UI & theme
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/reset.css';
import './theme/element-theme.scss';
import enLocale from 'element-ui/lib/locale/lang/en';
import deLocale from 'element-ui/lib/locale/lang/de';
Vue.use(ElementUI);

// Internationalization
import VueI18n from 'vue-i18n';
Vue.use(VueI18n);

// Load messages
import enMessages from './i18n/en.json';
import deMessages from './i18n/de.json';
const messages = {
  en: Object.assign(enMessages, enLocale),
  de: Object.assign(deMessages, deLocale),
};

// Configure internationalization
const i18n = new VueI18n({
  locale: 'en',
  fallbackLocale: 'en',
  messages,
});

// Router & Vuex
import router from './router';
import store from './store';

// Mount app
import App from './App.vue';
new Vue({
  router,
  store,
  i18n,
  render: h => h(App),
}).$mount('#app');
