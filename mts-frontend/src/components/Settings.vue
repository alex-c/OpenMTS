<template>
  <el-drawer :title="$t('login.settings')" :visible.sync="drawerOpen" direction="rtl">
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
