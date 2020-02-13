<template>
  <el-drawer :title="$t('login.settings')" :visible.sync="drawerOpen" direction="rtl" size="200">
    <div id="drawer-body">
      <el-form label-position="top">
        <el-form-item :label="$t('login.language')">
          <el-select v-model="$i18n.locale">
            <el-option
              v-for="(lang, i) in languages"
              :key="`Lang${i}`"
              :label="lang.label"
              :value="lang.value"
            >
              <span style="float: left">{{ lang.label }}</span>
              <span style="float: right; color: #8492a6; font-size: 13px">{{ lang.value }}</span>
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
    </div>
  </el-drawer>
</template>

<script>
import languageIndex from '../i18n/index.json';

export default {
  name: 'settings',
  props: ['drawer'],
  data() {
    return {
      languages: languageIndex.languages,
    };
  },
  computed: {
    drawerOpen: {
      get() {
        return this.drawer;
      },
      set(value) {
        this.$emit('update:drawer', value);
      },
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
