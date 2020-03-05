<template>
  <div id="create-material">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{$t('materials.create')}}</div>
        <div class="right">
          <router-link to="/private/materials">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{$t('general.back')}}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Create Material Form -->
      <el-form
        :model="createMaterialForm"
        :rules="validationRules"
        ref="createMaterialForm"
        label-position="left"
        label-width="140px"
        size="mini"
      >
        <div class="content-row">
          <!-- Material Name -->
          <el-form-item prop="name" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="createMaterialForm.name"></el-input>
          </el-form-item>

          <!-- Manufacturer -->
          <el-form-item prop="manufacturer" :label="$t('materials.manufacturer')">
            <el-select
              v-model="createMaterialForm.manufacturer"
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
          <el-form-item prop="manufacturerId" :label="$t('materials.manufacturerId')">
            <el-input
              :placeholder="$t('materials.manufacturerId')"
              v-model="createMaterialForm.manufacturerId"
            ></el-input>
          </el-form-item>

          <!-- Material Type & Save -->
          <el-form-item prop="type" :label="$t('materials.type')">
            <el-select
              v-model="createMaterialForm.type"
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
                @click="createMaterial"
                icon="el-icon-check"
              >{{$t('general.save')}}</el-button>
            </div>
          </el-form-item>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'CreateMaterial',
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      createMaterialForm: {
        name: '',
        manufacturer: '',
        manufacturerId: '',
        type: null,
      },
      manufacturers: [],
      materialTypes: [],
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('materials.validation.name'), trigger: 'blur' },
        manufacturer: { required: true, message: this.$t('materials.validation.manufacturer'), trigger: ['change', 'blur', 'clear'] },
        manufacturerId: { required: true, message: this.$t('materials.validation.manufacturerId'), trigger: 'blur' },
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
      Api.getMaterialTypes()
        .then(response => {
          this.materialTypes = response.body.data;
        })
        .catch(this.handleHttpError);
    },
    createMaterial: function() {
      this.$refs['createMaterialForm'].validate(valid => {
        if (valid) {
          // TODO: call API, redirect on success
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