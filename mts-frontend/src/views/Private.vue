<template>
  <div id="private">
    <Navbar />
    <Sidebar />
    <div id="content">Content</div>
  </div>
</template>

<script>
import Navbar from '@/components/Navbar.vue';
import Sidebar from '@/components/Sidebar.vue';

export default {
  name: 'private',
  components: { Navbar, Sidebar },
};
</script>

<style lang="scss" scoped>
#content {
  margin-left: 230px;
}
</style>
