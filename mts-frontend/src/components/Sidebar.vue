<template>
  <aside id="sidebar">
    <div id="menu">
      <div class="menu-header">Header</div>
      <div class="menu-button">
        <i class="el-icon-share"></i> Button
      </div>
    </div>
  </aside>
</template>

<script>
export default {
  name: 'sidebar',
  data() {
    return {
      isCollapsed: false,
    };
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
  top: 62px;
  left: 0;
  min-height: 100%;
  width: 229px;
  background-color: $color-menu-primary;
  border-right: 1px solid black;
  color: $color-menu-light-text;
}

#menu {
  height: auto;
  text-align: left;
}

.menu-header {
  padding: 8px;
  background-color: $color-menu-dark-accent;
  color: $color-menu-dark-text;
}

.menu-button {
  padding: 8px;
  &:hover {
    cursor: pointer;
    background-color: $color-menu-light-accent;
    color: white;
  }
}
</style>