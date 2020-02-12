<template>
  <el-drawer :title="$t('login.settings')" :visible.sync="drawerOpen" direction="rtl" size="200">
    <div id="drawer-body">
      <el-form label-position="top">
        <el-form-item :label="$t('login.language')">
          <el-select v-model="$i18n.locale">
            <el-option v-for="(lang, i) in languages" :key="`Lang${i}`" :label="lang" :value="lang">
              <span style="float: left">{{ lang }}</span>
              <span style="float: right; color: #8492a6; font-size: 13px">{{ lang }}</span>
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
    </div>
  </el-drawer>
</template>

<script>
export default {
  name: 'settings',
  props: ['drawer'],
  data() {
    return {
      drawerOpen: this.drawer,
      languages: ['en', 'de'],
    };
  },
  watch: {
    drawer(drawer) {
      this.drawerOpen = drawer;
    },
    drawerOpen(drawerOpen) {
      this.$emit('update:drawer', drawerOpen);
    },
  },
};
</script>

<style lang="scss" scoped>
#drawer-body {
  padding: 16px;
  text-align: left;
  min-width: 200px;
}

#drawer-body .el-select {
  width: 100%;
}
</style>
