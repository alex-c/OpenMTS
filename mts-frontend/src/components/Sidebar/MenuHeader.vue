<template>
  <div class="menu-header" v-show="!collapsed">{{label}}</div>
</template>

<script>
export default {
  name: 'MenuHeader',
  props: ['label', 'collapsed'],
};
</script>

<style lang="scss" scoped>
@import '../../theme/colors.scss';

.menu-header {
  padding: 12px;
  background-color: $color-menu-dark-accent;
  color: $color-menu-dark-text;
  overflow: hidden;
}
</style>