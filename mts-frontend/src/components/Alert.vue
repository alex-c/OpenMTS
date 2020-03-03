<template>
  <transition name="el-zoom-in-top">
    <div class="content-row" v-show="show">
      <el-alert
        :closable="false"
        :title="title"
        :type="type"
        :effect="effect"
        show-icon
        class="alert"
      >{{ description }}</el-alert>
    </div>
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