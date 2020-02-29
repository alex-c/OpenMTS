<template>
  <router-link :to="to" tag="span">
    <div class="menu-button">
      <i :class="icon"></i>
      <div>{{label}}</div>
    </div>
  </router-link>
</template>

<script>
export default {
  name: 'MenuButton',
  props: ['label', 'icon', 'to'],
};
</script>

<style lang="scss" scoped>
@import '../../theme/colors.scss';

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
    max-height: 20px;
    overflow: hidden;
  }
  i {
    float: left;
    margin: 1px;
    margin-right: 8px;
  }
}
</style>