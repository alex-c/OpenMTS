import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

// Components
import Public from '../views/Public.vue';
import Private from '../views/Private.vue';
import Dashboard from '../views/private/Dashboard.vue';
import Account from '../views/private/Account.vue';
import UserAdministration from '../views/private/UserAdministration.vue';
import CreateUser from '../views/private/UserAdministration/CreateUser.vue';
import EditUser from '../views/private/UserAdministration/EditUser.vue';

// Store
import store from '../store';

const routes = [
  {
    path: '/',
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
    component: Public,
  },
  {
    path: '/private',
    component: Private,
    beforeEnter: function(_to, _from, next) {
      if (store.state.token === null) {
        next({ path: '/login' });
      } else {
        next();
      }
    },
    children: [
      {
        path: '',
        component: Dashboard,
      },
      {
        path: 'account',
        component: Account,
      },
      {
        path: 'users',
        component: UserAdministration,
      },
      {
        path: 'users/create',
        component: CreateUser,
      },
      {
        path: 'users/edit',
        component: EditUser,
        props: true,
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
