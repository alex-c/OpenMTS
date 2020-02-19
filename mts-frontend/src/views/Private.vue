<template>
  <div id="private">
    <Navbar />
    <Sidebar />
    <div id="content" :class="{collapsed : menuCollapsed}">
      <div id="content-inner">
        <router-view></router-view>
      </div>
    </div>
  </div>
</template>

<script>
import Navbar from '@/components/Navbar.vue';
import Sidebar from '@/components/Sidebar.vue';

export default {
  name: 'private',
  components: { Navbar, Sidebar },
  computed: {
    menuCollapsed() {
      return this.$store.state.ui.menuCollapsed;
    },
  },
  methods: {
    fitToScreen: function() {
      if (window.innerWidth < 700) {
        this.$store.commit('collapseSidebar');
      } else {
        this.$store.commit('expandSidebar');
      }
    },
  },
  created: function() {
    window.addEventListener('resize', this.fitToScreen);
    this.fitToScreen();
  },
  destroyed() {
    window.removeEventListener('resize', this.fitToScreen);
  },
};
</script>

<style lang="scss">
@import '../theme/colors.scss';

#content {
  margin-left: 230px;
  padding: 0px 16px;
  text-align: center;
  transition: all 0.5s ease-in-out;
  &.collapsed {
    margin-left: 48px;
  }
}

#content-inner {
  max-width: 900px;
  margin: auto;
  text-align: left;
}

.content-row {
  padding-top: 16px;
  overflow: auto;
  .left {
    float: left;
  }
  .right {
    float: right;
    button {
      margin-left: 8px;
    }
  }
}

.content-section {
  padding-top: 8px;
  & .content-row {
    padding-top: 8px;
  }
}

.content-title {
  font-size: 16px;
  font-weight: bold;
  margin-top: 4px;
}

.content-subtitle {
  font-size: 14px;
  font-weight: 600;
}

table td {
  cursor: pointer;
}

.current-row > td {
  background-color: $color-info !important;
  color: white !important;
}

.input-button {
  color: $color-primary !important;
}

.el-select-dropdown {
  text-align: left;
}
</style>
