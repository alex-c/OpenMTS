<template>
  <aside id="sidebar" :class="{collapsed: menuCollapsed}">
    <div id="menu">
      <div class="menu-header" v-show="!menuCollapsed">Header</div>
      <div class="menu-button">
        <i class="el-icon-share"></i>
        <div>Button</div>
      </div>
    </div>
  </aside>
</template>

<script>
export default {
  name: 'sidebar',
  data() {
    return {};
  },
  computed: {
    menuCollapsed() {
      return this.$store.state.ui.menuCollapsed;
    },
  },
};
</script>

<style lang="scss" scoped>
@import '../theme/colors.scss';

#sidebar {
  position: fixed;
  top: 61px;
  left: 0;
  min-height: 100%;
  width: 229px;
  background-color: $color-menu-primary;
  border-right: 1px solid black;
  color: $color-menu-light-text;
  transition: all 0.5s ease-in-out;
  &.collapsed {
    width: 47px;
  }
}

#menu {
  height: auto;
  text-align: left;
}

.menu-header {
  padding: 12px;
  background-color: $color-menu-dark-accent;
  color: $color-menu-dark-text;
  overflow: hidden;
}

.menu-button {
  padding: 16px;
  overflow: hidden;
  &:hover {
    cursor: pointer;
    background-color: $color-menu-light-accent;
    color: white;
  }
  div {
    max-width: 197px;
    max-height: 16px;
    overflow: hidden;
  }
  i {
    float: left;
    margin: 1px;
    margin-right: 8px;
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
</style>