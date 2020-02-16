import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

// Components
import Public from '../views/Public.vue';
import Private from '../views/Private.vue';
import Dashboard from '../views/private/Dashboard.vue';

// Store
import store from '../store';

const routes = [
  {
    path: '/',
    name: 'home',
    beforeEnter: (_to, _from, next) => {
      if (store.state.token === null) {
        next({ path: '/login' });
      } else {
        next({ path: '/private' });
      }
    },
  },
  {
    path: '/login',
    name: 'public',
    component: Public,
  },
  {
    path: '/private',
    name: 'private',
    component: Private,
    beforeEnter: function(_to, _from, next) {
      if (store.state.token === null) {
        next({ path: '/' });
      } else {
        next();
      }
    },
    children: [
      {
        path: '/',
        component: Dashboard,
      },
    ],
  },
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
