<template>
  <transition name="el-zoom-in-top">
    <el-alert
      :closable="false"
      :title="title"
      :type="type"
      show-icon
      v-show="show"
      class="alert"
    >{{ description }}</el-alert>
  </transition>
</template>

<script>
export default {
  name: 'Alert',
  props: ['type', 'title', 'description', 'show'],
};
</script>

<style lang="scss" scoped>
.alert {
  margin-top: 16px;
}
</style>