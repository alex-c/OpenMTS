<template>
  <div id="material-details">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{$t('materials.materialDetailsWithId', {id})}}</div>
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
      <div class="content-row">
        <el-table :data="tableData" border size="mini" :empty-text="$t('general.noData')">
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column prop="manufacturer" :label="$t('materials.manufacturer')"></el-table-column>
          <el-table-column prop="manufacturerSpecificId" :label="$t('materials.manufacturerId')"></el-table-column>
          <el-table-column
            prop="type"
            :label="$t('materials.type')"
            :formatter="materialTypeFormatter"
          ></el-table-column>
        </el-table>
      </div>
    </div>

    <!-- Custom Props -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{$t('materials.props')}}</div>TODO: Display custom material props, allow download of file.
      <div class="content-row" v-for="prop in customMaterialProps" v-bind:key="prop.id">TODO</div>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import PropTypeHandlingMixin from '@/mixins/PropTypeHandlingMixin.js';

export default {
  name: 'MaterialDetails',
  mixins: [GenericErrorHandlingMixin, PropTypeHandlingMixin],
  props: ['id', 'name', 'manufacturer', 'manufacturerSpecificId', 'type', 'customProps'],
  data() {
    return {
      tableData: [
        {
          name: this.name,
          manufacturer: this.manufacturer,
          manufacturerSpecificId: this.manufacturerSpecificId,
          type: this.type,
        },
      ],
      customMaterialProps: [],
    };
  },
  methods: {
    materialTypeFormatter: function(material) {
      return material.type.id + ' - ' + material.type.name;
    },
  },
};
</script>