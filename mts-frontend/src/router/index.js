import Vue from 'vue';
import VueRouter from 'vue-router';
import Public from '../views/Public.vue';

Vue.use(VueRouter);

const routes = [
  {
    path: '/',
    name: 'public',
    component: Public,
  },
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

export default router;
