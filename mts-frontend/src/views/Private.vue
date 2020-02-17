<template>
  <div id="private">
    <Navbar />
    <Sidebar />
    <div id="content" :class="{collapsed : menuCollapsed}">
      <router-view></router-view>
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

<style lang="scss" scoped>
#content {
  margin-left: 230px;
  text-align: left;
  transition: all 0.5s ease-in-out;
  &.collapsed {
    margin-left: 48px;
  }
}
</style>
