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
      <Alert
        type="success"
        :description="feedback.masterData"
        :show="feedback.masterData !== null"
      />
      <el-form
        :model="updateMaterialForm"
        :rules="validationRules"
        ref="updateMaterialForm"
        label-position="left"
        label-width="140px"
        size="mini"
      >
        <div class="content-row">
          <!-- Material ID -->
          <el-form-item prop="id" :label="$t('general.id')">
            <el-input :placeholder="$t('general.id')" v-model="this.id" disabled></el-input>
          </el-form-item>

          <!-- Material Name -->
          <el-form-item prop="name" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="updateMaterialForm.name"></el-input>
          </el-form-item>

          <!-- Manufacturer -->
          <el-form-item prop="manufacturer" :label="$t('materials.manufacturer')">
            <el-select
              v-model="updateMaterialForm.manufacturer"
              :placeholder="$t('materials.manufacturer')"
              :no-data-text="$t('general.noData')"
              size="mini"
              allow-create
              clearable
              filterable
            >
              <el-option
                v-for="manufacturer in manufacturers"
                :key="manufacturer"
                :label="manufacturer"
                :value="manufacturer"
              ></el-option>
            </el-select>
          </el-form-item>

          <!-- Manufacturer's ID -->
          <el-form-item prop="manufacturerSpecificId" :label="$t('materials.manufacturerId')">
            <el-input
              :placeholder="$t('materials.manufacturerId')"
              v-model="updateMaterialForm.manufacturerSpecificId"
            ></el-input>
          </el-form-item>

          <!-- Material Type & Save -->
          <el-form-item prop="type" :label="$t('materials.type')">
            <el-select
              v-model="updateMaterialForm.type"
              :placeholder="$t('materials.type')"
              :no-data-text="$t('general.noData')"
              filterable
            >
              <el-option
                v-for="type in materialTypes"
                :key="type.id"
                :label="type.id + ' - ' + type.name"
                :value="type.id"
              ></el-option>
            </el-select>
            <div class="right">
              <el-button
                type="primary"
                @click="editMaterial"
                icon="el-icon-check"
              >{{$t('general.save')}}</el-button>
            </div>
          </el-form-item>
        </div>
      </el-form>
    </div>

    <!-- Custom Props -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{$t('materials.props')}}</div>
      <Alert
        type="success"
        :description="feedback.customProps"
        :show="feedback.customProps !== null"
      />
      <div class="content-row" v-for="prop in customMaterialProps" v-bind:key="prop.id">
        <div class="prop">
          <div class="prop-header">
            <div class="prop-title">
              {{prop.name}}
              <span class="prop-id">({{prop.id}})</span>
            </div>
            <div class="right">
              <el-tag type="danger" effect="dark">Status: Not Set</el-tag>
            </div>
          </div>
          <div class="prop-content">
            <div class="prop-t-text" v-if="propIsTextProp(prop)">Text</div>
            <div class="prop-t-file" v-else-if="propIsFileProp(prop)">Datei</div>
            <div class="prop-t-error" v-else>Error</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import PropTypeHandlingMixin from '@/mixins/PropTypeHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'EditMaterial',
  mixins: [GenericErrorHandlingMixin, PropTypeHandlingMixin],
  components: { Alert },
  props: ['id', 'name', 'manufacturer', 'manufacturerSpecificId', 'type', 'customProps'],
  data() {
    return {
      updateMaterialForm: {
        id: this.id,
        name: this.name,
        manufacturer: this.manufacturer,
        manufacturerSpecificId: this.manufacturerSpecificId,
        type: this.type.id,
      },
      manufacturers: [],
      materialTypes: [],
      customMaterialProps: [],
      feedback: {
        masterData: null,
        customProps: null,
      },
    };
  },
  computed: {
    validationRules() {
      return {
        id: { required: true },
        name: { required: true, message: this.$t('materials.validation.name'), trigger: 'blur' },
        manufacturer: { required: true, message: this.$t('materials.validation.manufacturer'), trigger: ['change', 'blur', 'clear'] },
        manufacturerSpecificId: { required: true, message: this.$t('materials.validation.manufacturerId'), trigger: 'blur' },
        type: { required: true, message: this.$t('materials.validation.type'), trigger: ['change', 'blur'] },
      };
    },
  },
  methods: {
    getManufacturers: function() {
      Api.getManufacturers()
        .then(response => {
          this.manufacturers = response.body;
        })
        .catch(this.handleHttpError);
    },
    getMaterialTypes: function() {
      Api.getAllMaterialTypes()
        .then(response => {
          this.materialTypes = response.body.data;
        })
        .catch(this.handleHttpError);
    },
    getCustomMaterialProps: function() {
      Api.getCustomMaterialProps()
        .then(result => {
          this.customMaterialProps = result.body;
        })
        .catch(this.handleHttpError);
    },
    editMaterial: function() {
      this.$refs['updateMaterialForm'].validate(valid => {
        if (valid) {
          Api.updateMaterial(
            this.updateMaterialForm.id,
            this.updateMaterialForm.name,
            this.updateMaterialForm.manufacturer,
            this.updateMaterialForm.manufacturerSpecificId,
            this.updateMaterialForm.type,
          )
            .then(result => {
              this.feedback.masterData = this.$t('materials.updated', { id: this.updateMaterialForm.id });
            })
            .catch(this.handleHttpError);
        }
      });
    },
  },
  mounted() {
    this.getManufacturers();
    this.getMaterialTypes();
    this.getCustomMaterialProps();
  },
};
</script>

<style lang="scss" scoped>
@import '../../../theme/colors.scss';

.prop {
  margin: 8px 4px;
  border: 1px solid $color-menu-light-accent;
  border-radius: 4px;
  box-shadow: 0 0 4px 0 rgba(0, 0, 0, 0.3);
}

.prop-header {
  padding: 8px;
  border-bottom: 1px solid $color-menu-light-accent;
  overflow: auto;
}

.prop-title {
  float: left;
  margin-top: 8px;
  margin-left: 8px;
  font-size: 16px;
}

.prop-id {
  font-style: italic;
  color: darkgray;
}

.prop-content {
  padding: 16px;
}

.prop-t-file {
  text-align: center;
}
</style>