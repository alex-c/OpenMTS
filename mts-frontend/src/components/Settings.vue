<template>
  <div id="settings">
    <div id="settings-overlay" v-if="drawerOpen" @click="drawerOpen = false"></div>
    <div id="settings" :class="{ shown: drawerOpen }">
      <div id="settings-header">
        <div id="settings-header-title">{{ $t('login.settings') }}</div>
        <div id="settings-header-button" @click="drawerOpen = false">
          <i class="el-icon-s-tools"></i>
        </div>
      </div>
      <div class="setting">
        <div class="setting-title">{{ $t('login.language') }}</div>
        <div>
          <el-select v-model="$i18n.locale" @change="selectLanguage" style="width: 100%">
            <el-option v-for="(lang, i) in languages" :key="`Lang${i}`" :label="lang.label" :value="lang.value">
              <span style="float: left">{{ lang.label }}</span>
              <span style="float: right; color: #8492a6; font-size: 13px">{{ lang.value }}</span>
            </el-option>
          </el-select>
        </div>
      </div>
    </div>
  </div>
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
  methods: {
    selectLanguage: function(language) {
      localStorage.setItem('language', language);
    },
  },
};
</script>

<style lang="scss" scoped>
@import '../theme/colors.scss';

#settings-overlay {
  position: fixed;
  top: 0px;
  left: 0px;
  right: 0px;
  bottom: 0px;
  background-color: black;
  opacity: 0.5;
}

#settings {
  position: fixed;
  top: 0px;
  right: 0px;
  bottom: 0px;
  width: 0px;
  overflow: hidden;
  transition: all 0.25s ease-in-out;
  background-color: white;
  color: black;
  &.shown {
    width: 300px;
  }
  text-align: left;
}

#settings-header {
  height: 61px;
  background-color: $color-primary;
  color: white;
  #settings-header-title {
    float: left;
    font-size: 20px;
    padding: 18px;
  }
  #settings-header-button {
    float: right;
    font-size: 28px;
    padding: 16px 16px 12px;
    &:hover {
      background-color: $color-dark-accent;
      cursor: pointer;
    }
  }
}

.setting {
  margin: 16px 16px 0px;
}

.setting > div {
  padding: 8px 0px;
}

.setting-title {
  border-bottom: 1px solid darkgray;
}
</style>
