<template>
  <header id="navbar">
    <div id="navbar-title" :class="{collapsed : menuCollapsed}" @click="navigateHome">
      <i class="el-icon-s-home" v-if="menuCollapsed" />
      <span id="navbar-title-text" v-else>OpenMTS</span>
    </div>
    <div id="navbar-left" v-if="userIsAuthenticated">
      <div class="navbar-button" @click="toggleSidebar">
        <i class="el-icon-s-unfold" v-if="menuCollapsed"></i>
        <i class="el-icon-s-fold" v-else></i>
      </div>
    </div>
    <div class="navbar-button navbar-right" @click="drawer = true">
      <i class="el-icon-s-tools"></i>
    </div>
    <el-dropdown
      class="navbar-right"
      style="color:white;"
      trigger="click"
      placement="bottom"
      @command="userMenuAction"
      v-if="userIsAuthenticated && !userIsGuest"
    >
      <div id="user-menu" class="navbar-button">
        <i class="el-icon-user-solid"></i>
        <div id="user-menu-title" :class="{collapsed : menuCollapsed}">{{userName}}</div>
      </div>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item icon="el-icon-user-solid" command="account">{{$t('general.account')}}</el-dropdown-item>
        <el-dropdown-item icon="el-icon-close" command="logout">{{$t('general.logout')}}</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
    <div
      id="user-menu"
      class="navbar-right navbar-button"
      v-if="userIsAuthenticated && userIsGuest"
      @click="userMenuAction('logout')"
    >
      <i class="el-icon-close"></i>
      <div id="user-menu-title">{{$t('general.logout')}}</div>
    </div>
    <Settings :drawer.sync="drawer" />
  </header>
</template>

<script>
import Settings from '@/components/Settings.vue';

export default {
  name: 'navbar',
  components: { Settings },
  data() {
    return {
      drawer: false,
      userName: localStorage.getItem('name'),
    };
  },
  computed: {
    menuCollapsed() {
      return this.$store.state.ui.menuCollapsed;
    },
    userIsAuthenticated() {
      return this.$store.state.token !== null;
    },
    userIsGuest() {
      return this.userName === 'openmts.guest';
    },
  },
  methods: {
    navigateHome: function() {
      this.$router.push({ path: '/' }, () => {});
    },
    toggleSidebar: function() {
      this.$store.commit('toggleSidebar');
    },
    userMenuAction: function(command) {
      switch (command) {
        case 'account':
          this.$router.push({ path: '/private/' + command });
          break;
        case 'logout':
          this.$store.commit('logout');
          this.$store.commit('expandSidebar');
          this.$router.push({ path: '/login' }, () => {});
          break;
        default:
          console.error(`Invalid user menu action '${command}' was triggered.`);
          break;
      }
    },
  },
};
</script>

<style lang="scss" scoped>
@import '../theme/colors.scss';

#navbar {
  max-height: 60px;
  overflow: hidden;
  border-bottom: 1px solid $color-dark-accent;
  background-color: $color-primary;
  color: white;
  box-shadow: 0px 0px 5px gray;
}

#navbar-title {
  float: left;
  width: 230px;
  height: 28px;
  padding: 16px 0px;
  font-size: 24px;
  background-color: $color-dark-accent;
  overflow: hidden;
  transition: all 0.5s ease-in-out;
  &:hover {
    cursor: pointer;
  }
  &.collapsed {
    width: 48px;
  }
  i {
    position: relative;
    top: 2px;
  }
}

#navbar-left {
  float: left;
  font-size: 28px;
}

.navbar-right {
  float: right;
  font-size: 28px;
}

.navbar-button {
  padding: 16px 16px 12px;
}

.navbar-button:hover {
  background-color: $color-dark-accent;
  cursor: pointer;
}

#user-menu {
  width: auto;
  height: 33px;
  overflow: hidden;
}

#user-menu i {
  float: left;
}

#user-menu-title {
  max-width: 320px;
  height: 16px;
  margin: 6px;
  padding-left: 8px;
  font-size: 16px;
  overflow: hidden;
  transition: all 0.5s ease-in-out;
  &.collapsed {
    max-width: 0px;
    padding-left: 0;
    margin-left: 0;
    margin-right: 0;
  }
}

.collapse-enter-active,
.collapse-leave-active {
  transition: all 0.5s ease-in-out;
  overflow: hidden;
}
.collapse-enter,
.collapse-leave-to {
  width: 0;
}

.el-dropdown-menu {
  text-align: left;
}
</style>