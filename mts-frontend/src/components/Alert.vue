<template>
  <transition name="el-zoom-in-top">
    <el-alert
      :closable="false"
      :title="title"
      :type="type"
      :effect="effect"
      show-icon
      v-show="show"
      class="alert"
    >{{ description }}</el-alert>
  </transition>
</template>

<script>
export default {
  name: 'Alert',
  props: ['type', 'title', 'description', 'show', 'dark'],
  data() {
    return {
      effect: this.dark ? 'dark' : 'light',
    };
  },
};
</script>

<style lang="scss" scoped>
.alert {
  margin-top: 16px;
}

.content-row .alert {
  margin-top: 8px;
}
</style>