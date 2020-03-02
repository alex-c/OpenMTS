<template>
  <div id="edit-storage-site">
    <div class="content-section">
      <div class="content-row content-title">{{$t('storage.editSite')}}</div>
      <div class="content-row"></div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'EditStorageSite',
  props: ['id', 'name', 'areas'],
};
</script>