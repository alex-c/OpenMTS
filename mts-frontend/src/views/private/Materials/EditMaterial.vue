<template>
  <div id="edit-material" class="page-small">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{ $t('materials.editMaterialWithId', { id }) }}</div>
        <div class="right">
          <router-link to="/private/materials">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Master Data -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{ $t('general.masterData') }}</div>
      <Alert type="success" :description="feedback.masterData" :show="feedback.masterData !== null" />
      <el-form :model="updateMaterialForm" :rules="validationRules" ref="updateMaterialForm" label-position="left" label-width="140px" size="mini">
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
              <el-option v-for="manufacturer in manufacturers" :key="manufacturer" :label="manufacturer" :value="manufacturer"></el-option>
            </el-select>
          </el-form-item>

          <!-- Manufacturer's ID -->
          <el-form-item prop="manufacturerSpecificId" :label="$t('materials.manufacturerId')">
            <el-input :placeholder="$t('materials.manufacturerId')" v-model="updateMaterialForm.manufacturerSpecificId"></el-input>
          </el-form-item>

          <!-- Material Type & Save -->
          <el-form-item prop="type" :label="$t('materials.type')">
            <el-select v-model="updateMaterialForm.type" :placeholder="$t('materials.type')" :no-data-text="$t('general.noData')" filterable>
              <el-option v-for="type in materialTypes" :key="type.id" :label="type.id + ' - ' + type.name" :value="type.id"></el-option>
            </el-select>
            <div class="right">
              <el-button type="primary" @click="editMaterial" icon="el-icon-check">{{ $t('general.save') }}</el-button>
            </div>
          </el-form-item>
        </div>
      </el-form>
    </div>

    <!-- Custom Props -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{ $t('materials.props') }}</div>
      <Alert type="success" :description="feedback.customProps" :show="feedback.customProps !== null" />
      <div class="content-row" v-for="(prop, i) in customMaterialProps" v-bind:key="prop.id">
        <CustomProp :prop="prop">
          <template #right>
            <div class="right">
              <el-tag type="success" effect="dark" size="mini" v-if="prop.set">{{ $t('materials.propSet') }}</el-tag>
              <el-tag type="danger" effect="dark" size="mini" v-else>{{ $t('materials.propNotSet') }}</el-tag>
            </div>
          </template>

          <!-- Text props -->
          <div v-if="propIsTextProp(prop)">
            <div v-if="prop.set">
              <div class="row">{{ prop.value }}</div>
              <div class="row center">
                <el-button @click="deleteMaterialCustomTextProp(prop)" icon="el-icon-close" type="danger" theme="dark" size="mini">{{ $t('general.delete') }}</el-button>
              </div>
            </div>
            <div v-else>
              <el-form :model="prop" :rules="propValidationRules">
                <el-form-item prop="value">
                  <el-input type="textarea" v-model="customMaterialProps[i].value"></el-input>
                </el-form-item>
                <div class="row center">
                  <el-button @click="setMaterialCustomTextProp(prop)" icon="el-icon-check" type="primary" theme="dark" size="mini">{{ $t('general.save') }}</el-button>
                </div>
              </el-form>
            </div>
          </div>

          <!-- File props -->
          <div v-else-if="propIsFileProp(prop)">
            <div v-if="prop.set">
              <div class="row center">
                <el-button @click="deleteMaterialCustomFileProp(prop)" icon="el-icon-close" type="danger" theme="dark" size="mini">{{ $t('general.delete') }}</el-button>
              </div>
            </div>
            <div v-else>
              <el-upload
                :http-request="
                  function(data) {
                    uploadFile(data, prop);
                  }
                "
                :ref="'upload-' + prop.id"
                class="upload-container"
                :auto-upload="false"
                :multiple="false"
                :limit="1"
                action
              >
                <el-button slot="trigger" size="small" type="info" icon="el-icon-document">{{ $t('general.selectFile') }}</el-button>
                <el-button style="margin-left: 10px;" size="small" type="primary" icon="el-icon-upload" @click="submitUpload(prop.id)">{{ $t('general.upload') }}</el-button>
              </el-upload>
            </div>
          </div>
          <Alert type="error" :description="$t('materials.invalidProp')" :show="true" v-else />
        </CustomProp>
      </div>
      <div class="content-row" v-if="customMaterialProps.length == 0">
        <Empty>{{ $t('materials.noPropsDefined') }}</Empty>
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
  name: 'EditMaterial',
  mixins: [GenericErrorHandlingMixin, PropTypeHandlingMixin],
  components: { Alert, CustomProp, Empty },
  props: ['id'],
  data() {
    return {
      updateMaterialForm: {
        id: this.id,
        name: null,
        manufacturer: null,
        manufacturerSpecificId: null,
        type: null,
      },
      customProps: [],
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
    propValidationRules() {
      return {
        value: { required: true, message: this.$t('materials.validation.propText'), trigger: 'blur' },
      };
    },
  },
  methods: {
    getMaterial: function(callback) {
      Api.getMaterial(this.id)
        .then(response => {
          this.updateMaterialForm = { ...response.body, type: response.body.type.id };
          this.customProps = response.body.customProps;
          if (callback !== undefined) {
            callback();
          }
        })
        .catch(this.handleHttpError);
    },
    getManufacturers: function() {
      Api.getManufacturers()
        .then(response => {
          this.manufacturers = response.body;
        })
        .catch(this.handleHttpError);
    },
    getMaterialTypes: function() {
      Api.getAllPlastics()
        .then(response => {
          this.materialTypes = response.body.data;
        })
        .catch(this.handleHttpError);
    },
    getCustomMaterialProps: function() {
      Api.getCustomMaterialProps()
        .then(result => {
          let props = [];
          for (let i = 0; i < result.body.length; i++) {
            const customProp = result.body[i];
            const propValue = this.customProps.find(pv => pv.propId == customProp.id);
            const prop = { ...customProp, set: false, value: '' };
            if (propValue !== undefined) {
              prop.set = true;
              prop.value = propValue.value;
            }
            props.push(prop);
          }
          this.customMaterialProps = props;
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
    setMaterialCustomTextProp: function(prop) {
      Api.setMaterialCustomTextProp(this.id, prop.id, prop.value)
        .then(result => {
          this.getMaterial(() => {
            this.getCustomMaterialProps();
          });
        })
        .catch(error => {
          if (error.status != 400) {
            this.handleHttpError(error);
          }
        });
    },
    deleteMaterialCustomTextProp: function(prop) {
      Api.deleteMaterialCustomTextProp(this.id, prop.id)
        .then(result => {
          this.getMaterial(() => {
            this.getCustomMaterialProps();
          });
        })
        .catch(this.handleHttpError);
    },
    submitUpload: function(id) {
      this.$refs['upload-' + id][0].submit();
    },
    uploadFile: function(data, prop) {
      Api.setMaterialCustomFileProp(this.id, prop.id, data.file)
        .then(response => {
          this.getMaterial(() => {
            this.getCustomMaterialProps();
          });
        })
        .catch(this.handleHttpError);
    },
    deleteMaterialCustomFileProp: function(prop) {
      Api.deleteMaterialCustomFileProp(this.id, prop.id)
        .then(result => {
          this.getMaterial(() => {
            this.getCustomMaterialProps();
          });
        })
        .catch(this.handleHttpError);
    },
  },
  mounted() {
    this.getMaterial(() => {
      this.getCustomMaterialProps();
    });
    this.getManufacturers();
    this.getMaterialTypes();
  },
};
</script>

<style lang="scss" scoped>
.upload-container {
  text-align: center;
}

.center {
  text-align: center;
}
</style>
