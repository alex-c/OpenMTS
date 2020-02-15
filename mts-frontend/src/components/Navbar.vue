<template>
  <header id="navbar">
    <div id="navbar-title" :class="{collapsed : menuCollapsed}">
      <i class="el-icon-s-home" v-if="menuCollapsed" />
      <span id="navbar-title-text" v-else>OpenMTS</span>
    </div>
    <div id="navbar-left">
      <div class="navbar-button" @click="toggleSidebar">
        <i class="el-icon-s-unfold" v-if="menuCollapsed"></i>
        <i class="el-icon-s-fold" v-else></i>
      </div>
    </div>
    <div class="navbar-button navbar-right" @click="drawer = true">
      <i class="el-icon-s-tools"></i>
    </div>
    <el-dropdown class="navbar-right" style="color:white;" trigger="click" placement="bottom">
      <div class="navbar-button" id="user-menu">
        <i class="el-icon-user-solid"></i>
        <span id="user-menu-title">User Name</span>
      </div>
      <el-dropdown-menu slot="dropdown">
        <el-dropdown-item icon="el-icon-user-solid">Account</el-dropdown-item>
        <el-dropdown-item icon="el-icon-close">Sign Out</el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
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
    };
  },
  computed: {
    menuCollapsed() {
      return this.$store.state.ui.menuCollapsed;
    },
  },
  methods: {
    toggleSidebar: function() {
      this.$store.commit('toggleSidebar');
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
  &.collapsed {
    width: 47px;
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

#user-menu > i {
  margin-right: 8px;
}

#user-menu-title {
  font-size: 16px;
  position: relative;
  top: -4px;
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
</style>