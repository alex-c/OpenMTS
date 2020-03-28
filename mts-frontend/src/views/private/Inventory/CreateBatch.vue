<template>
  <div id="view-create-batch">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{ $t('inventory.create') }}</div>
        <div class="right">
          <router-link to="/private/inventory">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Create batch form -->
      <el-form :model="createBatchForm" :rules="validationRules" ref="createBatchForm" label-position="left" label-width="140px" size="mini">
        <div class="content-row">
          <!-- Material -->
          <el-form-item prop="material" :label="$t('general.material')">
            <el-select v-model="createBatchForm.material" :placeholder="$t('general.material')" :no-data-text="$t('general.noData')" size="mini" filterable>
              <el-option v-for="material in materials" :key="material.id" :label="`${material.id} - ${material.name}`" :value="material.id"></el-option>
            </el-select>
          </el-form-item>

          <!-- Expiration date -->
          <el-form-item prop="expirationDate" :label="$t('inventory.expirationDate')">
            <el-date-picker v-model="createBatchForm.expirationDate" type="date" :placeholder="$t('inventory.expirationDate')" />
          </el-form-item>

          <!-- Storage site & area -->
          <el-form-item prop="storageArea" :label="$t('storage.location')">
            <el-select
              v-model="createBatchForm.storageSite"
              :placeholder="$t('storage.site')"
              :no-data-text="$t('general.noData')"
              size="mini"
              filterable
              @change="selectStorageSite"
            >
              <el-option v-for="storageSite in storageSites" :key="storageSite.id" :label="storageSite.name" :value="storageSite.id"></el-option>
            </el-select>
            <el-select
              v-model="createBatchForm.storageArea"
              :placeholder="$t('storage.area')"
              :no-data-text="$t('general.noData')"
              size="mini"
              filterable
              :disabled="selectedStorageSite.id === null"
              style="margin-left: 16px;"
            >
              <el-option v-for="storageArea in selectedStorageSite.areas" :key="storageArea.id" :label="storageArea.name" :value="storageArea.id"></el-option>
            </el-select>
          </el-form-item>
        </div>

        <div class="content-row-nopad">
          <div class="right">
            <el-button type="primary" size="mini" icon="el-icon-check" @click="createBatch">{{ $t('general.save') }}</el-button>
          </div>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'CreateBatch',
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      createBatchForm: {
        material: '',
        expirationDate: '',
        storageSite: '',
        storageArea: '',
        batchNumber: '',
        quantity: '',
      },
      materials: [],
      storageSites: [],
      selectedStorageSite: { id: null, areas: [] },
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('materials.validation.name'), trigger: 'blur' },
        material: { required: true, message: this.$t('inventory.validation.material'), trigger: ['change', 'blur'] },
        expirationDate: { required: true, message: this.$t('inventory.validation.expirationDate'), trigger: ['change', 'blur'] },
        storageArea: { required: true, message: this.$t('inventory.validation.location'), trigger: ['change', 'blur'] },
      };
    },
  },
  methods: {
    getMaterials: function() {
      Api.getAllMaterials()
        .then(response => {
          this.materials = response.body.data;
        })
        .catch(this.handleHttpError);
    },
    getStorageSites: function() {
      Api.getAllStorageSites()
        .then(response => {
          this.storageSites = response.body.data;
        })
        .catch(this.handleHttpError);
    },
    selectStorageSite: function(siteId) {
      this.selectedStorageSite = this.storageSites.find(s => s.id == siteId);
      this.createBatchForm.storageArea = '';
    },
    createBatch: function() {
      this.$refs['createBatchForm'].validate();
    },
  },
  mounted() {
    this.getMaterials();
    this.getStorageSites();
  },
};
</script>
