import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

// Views - General
import Public from '../views/Public.vue';
import Private from '../views/Private.vue';
import Account from '../views/private/Account.vue';
import Dashboard from '../views/private/Dashboard.vue';

// Views - Material
import StorageSites from '../views/private/StorageSites.vue';
import Inventory from '../views/private/Inventory.vue';
import Materials from '../views/private/Materials.vue';
import CreateMaterial from '../views/private/Materials/CreateMaterial.vue';
import EditMaterial from '../views/private/Materials/EditMaterial.vue';
import MaterialDetails from '../views/private/Materials/MaterialDetails.vue';
import MaterialTypes from '../views/private/MaterialTypes.vue';
import CreateMaterialType from '../views/private/MaterialTypes/CreateMaterialType.vue';

// Views - Administration
import Configuration from '../views/private/Configuration.vue';
import CreateMaterialProp from '../views/private/Configuration/CreateMaterialProp.vue';
import ApiKeys from '../views/private/ApiKeys.vue';
import EditKey from '../views/private/ApiKeys/EditKey.vue';
import UserAdministration from '../views/private/UserAdministration.vue';
import CreateUser from '../views/private/UserAdministration/CreateUser.vue';
import EditUser from '../views/private/UserAdministration/EditUser.vue';
import StorageLocations from '../views/private/StorageLocations.vue';
import EditStorageSite from '../views/private/StorageLocations/EditStorageSite.vue';

// Store
import store from '../store';

// Administration router guard
function userIsAdministrator(_to, _from, next) {
  if (store.state.role === 0 || store.state.role === '0') {
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
        beforeEnter: function(_to, _from, next) {
          if (store.state.user === 'openmts.guest') {
            next(_from);
          } else {
            next();
          }
        },
      },
      {
        path: 'sites',
        component: StorageSites,
      },
      {
        path: 'inventory',
        component: Inventory,
      },
      {
        path: 'materials',
        component: Materials,
      },
      {
        path: 'materials/create',
        component: CreateMaterial,
      },
      {
        path: 'materials/edit',
        name: 'editMaterial',
        component: EditMaterial,
        props: true,
      },
      {
        path: 'materials/details',
        name: 'materialDetails',
        component: MaterialDetails,
        props: true,
      },
      {
        path: 'material-types',
        name: 'materialTypes',
        component: MaterialTypes,
        props: true,
      },
      {
        path: 'material-types/create',
        component: CreateMaterialType,
      },
      {
        path: 'config',
        name: 'configuration',
        component: Configuration,
        props: true,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'config/crate-material-prop',
        name: 'createMaterialProp',
        component: CreateMaterialProp,
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
      {
        path: 'keys',
        name: 'keys',
        component: ApiKeys,
        props: true,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'keys/edit',
        name: 'editKey',
        component: EditKey,
        props: true,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'locations',
        name: 'locations',
        component: StorageLocations,
        props: true,
        beforeEnter: userIsAdministrator,
      },
      {
        path: 'locations/edit',
        name: 'editStoragSite',
        component: EditStorageSite,
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
