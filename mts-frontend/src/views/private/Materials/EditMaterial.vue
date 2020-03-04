<template>
  <div id="edit-material">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{$t('materials.material')}}</div>
        <div class="right">
          <router-link to="/private/materials">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{$t('general.back')}}</el-button>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Master Data -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{$t('general.masterData')}}</div>
    </div>

    <!-- Custom Props -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{$t('materials.props')}}</div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'EditMaterial',
};
</script>