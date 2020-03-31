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
      <el-form :model="createBatchForm" :rules="validationRules" ref="createBatchForm" label-position="left" label-width="180px" size="mini">
        <div class="content-row">
          <!-- Material -->
          <el-form-item prop="material" :label="$t('general.material')">
            <el-select v-model="createBatchForm.material" :placeholder="$t('general.material')" :no-data-text="$t('general.noData')" size="mini" filterable style="width: 300px;">
              <el-option v-for="material in materials" :key="material.id" :label="`${material.id} - ${material.name}`" :value="material.id"></el-option>
            </el-select>
          </el-form-item>

          <!-- Expiration date -->
          <el-form-item prop="expirationDate" :label="$t('inventory.expirationDate')">
            <el-date-picker v-model="createBatchForm.expirationDate" type="date" :placeholder="$t('inventory.expirationDate')" style="width: 300px;" />
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
              style="width: 300px;"
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
              style="width: 300px; margin-left: 16px;"
            >
              <el-option v-for="storageArea in selectedStorageSite.areas" :key="storageArea.id" :label="storageArea.name" :value="storageArea.id"></el-option>
            </el-select>
          </el-form-item>

          <!-- Batch number -->
          <el-form-item prop="batchNumber" :label="$t('inventory.batchNumber')">
            <el-input-number v-model="createBatchForm.batchNumber" size="mini" style="width: 300px;" />
          </el-form-item>

          <!-- Quantity -->
          <el-form-item prop="quantity" :label="$t('inventory.quantity') + ' (kg)'">
            <el-input-number v-model="createBatchForm.quantity" :precision="3" :step="25" size="mini" style="width: 300px;" />
          </el-form-item>

          <!-- Custom props -->
          <el-form-item v-for="prop in customProps" :key="prop.id" :prop="prop.id" :label="prop.name">
            <el-input :placeholder="prop.name" v-model="createBatchForm[prop.id]"></el-input>
          </el-form-item>
        </div>

        <!-- Save button -->
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
        batchNumber: 0,
        quantity: 0,
      },
      customProps: [],
      materials: [],
      storageSites: [],
      selectedStorageSite: { id: null, areas: [] },
    };
  },
  computed: {
    validationRules() {
      let rules = {
        material: { required: true, message: this.$t('inventory.validation.material'), trigger: ['change', 'blur'] },
        expirationDate: [
          { required: true, message: this.$t('inventory.validation.expirationDate'), trigger: ['change', 'blur'] },
          { validator: this.validateExpirationDate, trigger: ['change', 'blur'] },
        ],
        storageArea: { required: true, message: this.$t('inventory.validation.location'), trigger: ['change', 'blur'] },
        batchNumber: { required: true, validator: this.validatePositiveNumber, trigger: ['change', 'blur'] },
        quantity: { required: true, validator: this.validatePositiveNumber, trigger: ['change', 'blur'] },
      };
      for (let prop of this.customProps) {
        rules[prop.id] = { required: true, message: this.$t('inventory.validation.customProp'), trigger: 'blur' };
      }
      return rules;
    },
  },
  methods: {
    getCustomBatchProps: function() {
      Api.getCustomBatchProps()
        .then(result => {
          for (let prop of this.customProps) {
            this.createBatchForm[prop.id] = '';
          }
          this.customProps = result.body;
        })
        .catch(this.handleHttpError);
    },
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
      this.$refs['createBatchForm'].validate(valid => {
        if (valid) {
          let customProps = {};
          for (let prop of this.customProps) {
            customProps[prop.id] = this.createBatchForm[prop.id];
          }
          Api.createBatch(
            this.createBatchForm.material,
            this.createBatchForm.expirationDate,
            this.createBatchForm.storageSite,
            this.createBatchForm.storageArea,
            this.createBatchForm.batchNumber,
            this.createBatchForm.quantity,
            customProps,
          )
            .then(response => {
              this.$router.push({ name: 'inventory', params: { successMessage: this.$t('inventory.created', response.body) } });
            })
            .catch(this.handleHttpError);
        }
      });
    },
    validatePositiveNumber: function(_, value, callback) {
      if (value <= 0) {
        callback(new Error(this.$t('inventory.validation.number')));
      } else {
        callback();
      }
    },
    validateExpirationDate: function(_, value, callback) {
      if (value <= new Date().setHours(0, 0, 0, 0)) {
        callback(new Error(this.$t('inventory.validation.expirationDateFuture')));
      } else {
        callback();
      }
    },
  },
  mounted() {
    this.getCustomBatchProps();
    this.getMaterials();
    this.getStorageSites();
  },
};
</script>

<style lang="scss" scoped>
#view-create-batch {
  width: 800px;
  margin: auto;
}
</style>
