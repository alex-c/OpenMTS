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
          <router-link :to="{ name: 'editMaterial', params: {id: this.id} }">
            <el-button type="info" size="mini" icon="el-icon-edit">{{$t('general.edit')}}</el-button>
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
      <div class="content-row content-subtitle">{{$t('materials.props')}}</div>
      <div class="content-row" v-for="prop in customMaterialProps" v-bind:key="prop.id">
        <CustomProp :prop="prop">
          <div v-if="propIsTextProp(prop)">{{prop.value}}</div>
          <div v-else-if="propIsFileProp(prop)">TODO</div>
          <Alert type="error" :description="$t('materials.invalidProp')" :show="true" v-else />
        </CustomProp>
      </div>
      <div class="content-row" v-if="customMaterialProps.length==0">
        <Empty>{{$t('materials.noPropsSet')}}</Empty>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import PropTypeHandlingMixin from '@/mixins/PropTypeHandlingMixin.js';
import Alert from '@/components/Alert.vue';
import CustomProp from '@/components/CustomProp.vue';
import Empty from '@/components/Empty.vue';

export default {
  name: 'MaterialDetails',
  mixins: [GenericErrorHandlingMixin, PropTypeHandlingMixin],
  components: { Alert, CustomProp, Empty },
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
    getCustomMaterialProps: function() {
      Api.getCustomMaterialProps()
        .then(result => {
          let props = [];
          for (let i = 0; i < result.body.length; i++) {
            const customProp = result.body[i];
            const propValue = this.customProps.find(pv => pv.propId == customProp.id);
            if (propValue !== undefined) {
              props.push({
                ...customProp,
                value: propValue.value,
              });
            }
          }
          this.customMaterialProps = props;
        })
        .catch(this.handleHttpError);
    },
  },
  mounted() {
    this.getCustomMaterialProps();
  },
};
</script>

<style lang="scss" scoped>
textarea {
  margin-bottom: 8px;
}
</style>