<template>
  <div class="prop">
    <div class="prop-header">
      <div class="prop-title">
        <span
          class="prop-toggle el-icon-circle-plus-outline"
          @click="collapsed = false"
          v-if="collapsed"
        />
        <span class="prop-toggle el-icon-remove-outline" @click="collapsed = true" v-else />
        {{prop.name}}
        <span class="prop-id">({{prop.id}})</span>
      </div>
      <slot name="right"></slot>
    </div>
    <div class="prop-content" :class="{collapsed}">
      <slot />
    </div>
  </div>
</template>

<script>
export default {
  name: 'CustomProp',
  props: ['prop'],
  data() {
    return {
      collapsed: false,
    };
  },
};
</script>

<style lang="scss" scoped>
@import '@/theme/colors.scss';

.prop {
  margin: 8px 4px;
  border: 1px solid $color-menu-light-accent;
  border-radius: 4px;
  box-shadow: 0 0 4px 0 rgba(0, 0, 0, 0.3);
}

.prop-header {
  padding: 8px;
  border-bottom: 1px solid $color-menu-light-accent;
  overflow: auto;
}

.prop-title {
  float: left;
  margin-top: 2px;
  font-size: 16px;
}

.prop-toggle:hover {
  cursor: pointer;
}

.prop-id {
  font-style: italic;
  color: darkgray;
}

.prop-content {
  padding: 16px;
  overflow: auto;
  &.collapsed {
    display: none;
  }
  .row {
    margin-top: 8px;
    &:first-child {
      margin-top: 0px;
    }
  }
}
</style>