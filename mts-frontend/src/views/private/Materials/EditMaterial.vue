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
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'EditMaterial',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  props: ['id', 'name', 'manufacturer', 'manufacturerSpecificId', 'type'],
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
  },
};
</script>