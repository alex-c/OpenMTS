import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

// Components
import Public from '../views/Public.vue';
import Private from '../views/Private.vue';
import Dashboard from '../views/private/Dashboard.vue';
import Account from '../views/private/Account.vue';
import Configuration from '../views/private/Configuration.vue';
import UserAdministration from '../views/private/UserAdministration.vue';
import CreateUser from '../views/private/UserAdministration/CreateUser.vue';
import EditUser from '../views/private/UserAdministration/EditUser.vue';

// Store
import store from '../store';

// Administration router guard
function userIsAdministrator(_to, _from, next) {
  if (store.state.token === null) {
    next({ path: '/login' });
  } else if (store.state.role === 0) {
    next();
  } else {
    next(_from);
  }
}

// Routes
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
        path: 'config',
        component: Configuration,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'users',
        name: 'users',
        component: UserAdministration,
        props: true,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'users/create',
        component: CreateUser,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'users/edit',
        name: 'editUser',
        component: EditUser,
        props: true,
        beforeEnter: userIsAdministrator,
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
