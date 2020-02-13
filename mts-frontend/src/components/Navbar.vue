<template>
  <div id="navbar">
    <div id="navbar-title">OpenMTS</div>
    <div id="navbar-right">
      <div class="navbar-button" @click="drawer = true">
        <i class="el-icon-s-tools"></i>
      </div>
    </div>
    <Settings :drawer.sync="drawer" />
  </div>
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
};
</script>

<style lang="scss" scoped>
@import '../theme/colors.scss';

#navbar {
  background-color: $color-primary;
  color: white;
  overflow: auto;
  border-bottom: 1px solid $color-dark-accent;
  box-shadow: 0px 0px 5px gray;
}

#navbar-title {
  float: left;
  padding: 16px;
  font-size: 24px;
}

#navbar-right {
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
</style>