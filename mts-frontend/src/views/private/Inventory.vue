<template>
  <div id="inventory">
    <Alert type="success" :description="successMessage" :show="successMessage !== undefined" />
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{ $t('general.inventory') }}</div>
        <div class="right">
          <router-link to="/private/inventory/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">{{ $t('inventory.create') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Filtering Options -->
      <div class="content-row content-row-inputs flex">
        <!-- Material Filter -->
        <div class="grow">
          <el-select
            style="width:100%;"
            v-model="query.material"
            :placeholder="$t('general.material')"
            :no-data-text="$t('general.noData')"
            @change="setMaterial"
            size="mini"
            filterable
            clearable
          >
            <el-option v-for="material in materials" :key="material.id" :label="`${material.id} - ${material.name}`" :value="material.id"></el-option>
          </el-select>
        </div>

        <!-- Storage Site Filter -->
        <div class="grow" style="margin: 0px 8px">
          <el-select
            style="width:100%;"
            v-model="query.storageSite"
            :placeholder="$t('storage.site')"
            :no-data-text="$t('general.noData')"
            @change="setStorageSite"
            size="mini"
            clearable
            filterable
          >
            <el-option v-for="storageSite in storageSites" :key="storageSite.id" :label="storageSite.name" :value="storageSite.id"></el-option>
          </el-select>
        </div>

        <!-- Reset Filters -->
        <div>
          <el-button icon="el-icon-close" size="mini" plain @click="resetFilters" />
        </div>
      </div>

      <!-- Material Batch Table -->
      <div class="content-row">
        <el-table
          :data="batches"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectBatch"
          ref="materialBatchTable"
          row-key="id"
        >
          <el-table-column prop="material" :label="$t('general.material')" :formatter="formatMaterial"></el-table-column>
          <el-table-column prop="storageLocation" :label="$t('storage.location')" :formatter="formatLocation"></el-table-column>
          <el-table-column prop="batchNumber" :label="$t('inventory.batchNumber')"></el-table-column>
          <el-table-column prop="expirationDate" :label="$t('inventory.expirationDate')" :formatter="formatDate"></el-table-column>
          <el-table-column v-for="prop in customProps" v-bind:key="prop.id" :prop="prop.id" :label="prop.name"></el-table-column>
          <el-table-column prop="locked" :label="$t('general.status.label')" :formatter="formatStatus"></el-table-column>
          <el-table-column prop="quantity" :label="$t('inventory.quantity') + ' (kg)'"></el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Options -->
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalBatches"
            :page-size="query.elementsPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <span v-if="userMayLockBatches">
            <el-button icon="el-icon-lock" type="warning" size="mini" v-if="selectedBatch.id !== null && selectedBatch.isLocked === false" @click="lockBatch">
              {{ $t('inventory.lock') }}
            </el-button>
            <el-button icon="el-icon-unlock" type="warning" size="mini" v-if="selectedBatch.id !== null && selectedBatch.isLocked === true" @click="unlockBatch">
              {{ $t('inventory.unlock') }}
            </el-button>
          </span>
          <el-button icon="el-icon-download" type="success" size="mini" :disabled="selectedBatch.id === null || selectedBatch.isLocked" @click="checkIn">
            {{ $t('inventory.checkIn') }}
          </el-button>
          <el-button icon="el-icon-upload2" type="success" size="mini" :disabled="selectedBatch.id === null || selectedBatch.isLocked" @click="checkOut">
            {{ $t('inventory.checkOut') }}
          </el-button>
          <router-link :to="{ name: 'editBatch', params: { id: selectedBatch.id } }">
            <el-button icon="el-icon-edit" type="info" size="mini" :disabled="selectedBatch.id === null">{{ $t('general.edit') }}</el-button>
          </router-link>
          <el-button icon="el-icon-view" type="primary" size="mini" :disabled="selectedBatch.id === null" @click="viewLog">{{ $t('inventory.log') }}</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'Inventory',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  props: ['materialFilter', 'storageSiteFilter', 'successMessage'],
  data() {
    return {
      search: '',
      query: {
        page: 1,
        elementsPerPage: 10,
        material: this.materialFilter || '',
        storageSite: this.storageSiteFilter || '',
      },
      customProps: [],
      batches: [],
      totalBatches: 0,
      materials: [],
      storageSites: [],
      selectedBatch: { id: null },
    };
  },
  computed: {
    userMayLockBatches() {
      const role = this.$store.state.role;
      return role == 0 || role == 1;
    },
  },
  methods: {
    getCustomBatchProps: function(callback) {
      Api.getCustomBatchProps()
        .then(result => {
          this.customProps = result.body;
          callback();
        })
        .catch(this.handleHttpError);
    },
    getInventory: function() {
      this.resetSelectedBatch();
      Api.getBatches(this.query.page, this.query.elementsPerPage, this.query.material, this.query.storageSite)
        .then(response => {
          let batches = [];
          this.totalBatches = response.body.totalElements;
          for (let i = 0; i < response.body.data.length; i++) {
            const batch = response.body.data[i];
            for (let j = 0; j < this.customProps.length; j++) {
              const id = this.customProps[j].id;
              const name = this.customProps[j].name;
              batch[id] = batch.customProps[id];
            }
            batches.push(batch);
          }
          this.batches = batches;
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
    setMaterial: function(value) {
      this.query.page = 1;
      this.getInventory();
    },
    setStorageSite: function(value) {
      this.query.page = 1;
      this.getInventory();
    },
    resetFilters: function() {
      this.query.material = '';
      this.query.storageSite = '';
      this.query.page = 1;
      this.getInventory();
    },
    selectBatch: function(batch) {
      this.selectedBatch = { ...batch };
    },
    resetSelectedBatch: function() {
      this.$refs['materialBatchTable'].setCurrentRow(1);
      this.selectedBatch = { id: null };
    },
    changePage: function(page) {
      this.query.page = page;
      this.getInventory();
    },
    editBatch: function() {
      this.$router.push({ name: 'editBatch', params: { ...this.selectedBatch } });
    },
    viewLog: function() {
      this.$router.push({ name: 'transactionLog', params: { id: this.selectedBatch.id } });
    },
    formatMaterial: function(batch) {
      return `${batch.material.id} - ${batch.material.name}`;
    },
    formatLocation: function(batch) {
      return `${batch.storageLocation.storageSiteName} - ${batch.storageLocation.storageAreaName}`;
    },
    formatDate: function(batch) {
      return new Date(batch.expirationDate).toLocaleDateString(this.$i18n.locale, { year: 'numeric', month: 'long', day: 'numeric' });
    },
    formatStatus: function(batch) {
      return batch.isLocked ? this.$t('inventory.status.locked') : this.$t('inventory.status.unlocked');
    },
    checkOut: function() {
      this.$prompt(this.$t('inventory.checkOutPrompt'), this.$t('inventory.checkOut'), {
        confirmButtonText: this.$t('inventory.checkOut'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^[0-9]+(\.[0-9]+)?$/,
        inputErrorMessage: this.$t('inventory.transactionInputError'),
      })
        .then(({ value }) => {
          Api.checkOut(this.selectedBatch.id, value)
            .then(result => {
              this.$alert(this.$t('inventory.checkOutSuccessMessage', { quantity: value, id: result.body.id }), this.$t('inventory.checkOutSuccessTitle'), {
                confirmButtonText: this.$t('general.ok'),
                showClose: false,
                type: 'success',
              }).then(() => {
                this.getInventory();
              });
            })
            .catch(error => {
              if (error.status === 400) {
                this.$alert(this.$t('inventory.checkOutFailMessage'), this.$t('inventory.checkOutFailTitle'), {
                  confirmButtonText: this.$t('general.ok'),
                  type: 'error',
                });
              } else if (error.status === 403) {
                this.$alert(this.$t('inventory.transactionFailLockedMessage'), this.$t('inventory.checkOutFailTitle'), {
                  confirmButtonText: this.$t('general.ok'),
                  type: 'error',
                });
              } else {
                this.handleHttpError(error);
              }
            });
        })
        .catch(() => {});
    },
    checkIn: function() {
      this.$prompt(this.$t('inventory.checkInPrompt'), this.$t('inventory.checkIn'), {
        confirmButtonText: this.$t('inventory.checkIn'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^[0-9]+(\.[0-9]+)?$/,
        inputErrorMessage: this.$t('inventory.transactionInputError'),
      })
        .then(({ value }) => {
          Api.checkIn(this.selectedBatch.id, value)
            .then(result => {
              this.$alert(this.$t('inventory.checkInSuccessMessage', { quantity: value }), this.$t('inventory.checkInSuccessTitle'), {
                confirmButtonText: this.$t('general.ok'),
                showClose: false,
                type: 'success',
              }).then(() => {
                this.getInventory();
              });
            })
            .catch(erorr => {
              if (error.status === 403) {
                this.$alert(this.$t('inventory.transactionFailLockedMessage'), this.$t('inventory.checkInFailTitle'), {
                  confirmButtonText: this.$t('general.ok'),
                  type: 'error',
                });
              } else {
                this.handleHttpError(error);
              }
            });
        })
        .catch(() => {});
    },
    lockBatch: function() {
      this.$confirm(this.$t('inventory.lockConfirm'), this.$t('inventory.updateStatusConfirmTitle'), {
        confirmButtonText: this.$t('general.ok'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          Api.updateBatchStatus(this.selectedBatch.id, true)
            .then(result => {
              this.getInventory();
            })
            .catch(this.handleHttpError);
        })
        .catch(e => {});
    },
    unlockBatch: function() {
      this.$confirm(this.$t('inventory.unlockConfirm'), this.$t('inventory.updateStatusConfirmTitle'), {
        confirmButtonText: this.$t('general.ok'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          Api.updateBatchStatus(this.selectedBatch.id, false)
            .then(result => {
              this.getInventory();
            })
            .catch(this.handleHttpError);
        })
        .catch(e => {});
    },
  },
  mounted() {
    this.getMaterials();
    this.getStorageSites();
    this.getCustomBatchProps(() => {
      this.getInventory();
    });
  },
};
</script>
