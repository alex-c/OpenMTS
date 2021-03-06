<template>
  <div id="materials" class="page-medium">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{ $t('general.materials') }}</div>
        <div class="right">
          <router-link to="/private/materials/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">{{ $t('materials.create') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Filtering Options -->
      <div class="content-row content-row-inputs flex">
        <!-- Manufacturer Filter -->
        <div>
          <el-select
            v-model="query.manufacturer"
            :placeholder="$t('materials.manufacturer')"
            :no-data-text="$t('general.noData')"
            @change="setManufacturer"
            size="mini"
            allow-create
            clearable
            filterable
          >
            <el-option v-for="manufacturer in manufacturers" :key="manufacturer" :label="manufacturer" :value="manufacturer"></el-option>
          </el-select>
        </div>

        <!-- Plastics Filter -->
        <div style="margin: 0px 8px">
          <el-select v-model="query.type" :placeholder="$t('materials.type')" :no-data-text="$t('general.noData')" @change="setPlastic" size="mini" clearable filterable>
            <el-option v-for="plastic in plastics" :key="plastic.id" :label="plastic.id + ' - ' + plastic.name" :value="plastic.id"></el-option>
          </el-select>
        </div>

        <!-- Name Search -->
        <div class="grow">
          <el-input :placeholder="$t('materials.filter')" prefix-icon="el-icon-search" v-model="search" size="mini" clearable @change="setSearch"></el-input>
        </div>

        <!-- Reset Filters -->
        <div style="margin-left: 8px;">
          <el-button icon="el-icon-close" size="mini" plain @click="resetFilters" />
        </div>
      </div>

      <!-- Materials Table -->
      <div class="content-row">
        <el-table
          :data="materials"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectMaterial"
          ref="materialsTable"
          row-key="id"
        >
          <el-table-column prop="id" :label="$t('general.id')" width="60"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column prop="manufacturer" :label="$t('materials.manufacturer')"></el-table-column>
          <el-table-column prop="type" :label="$t('materials.type')" width="60">
            <template slot-scope="material">
              <el-popover trigger="hover" placement="left">
                {{ material.row.type.name }}
                <div slot="reference">
                  <el-tag type="primary" plain size="mini">{{ material.row.type.id }}</el-tag>
                </div>
              </el-popover>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Options -->
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalMaterials"
            :page-size="query.elementsPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <router-link :to="{ name: 'inventory', params: { materialFilter: selectedMaterial.id } }">
            <el-button icon="el-icon-box" type="success" size="mini" :disabled="selectedMaterial.id === null">{{ $t('general.inventory') }}</el-button>
          </router-link>
          <router-link :to="{ name: 'editMaterial', params: { id: selectedMaterial.id } }">
            <el-button icon="el-icon-edit" type="info" size="mini" :disabled="selectedMaterial.id === null">{{ $t('general.edit') }}</el-button>
          </router-link>
          <router-link :to="{ name: 'materialDetails', params: { ...selectedMaterial } }">
            <el-button icon="el-icon-view" type="primary" size="mini" :disabled="selectedMaterial.id === null">{{ $t('general.details') }}</el-button>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'Materials',
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      search: '',
      query: {
        page: 1,
        elementsPerPage: 10,
        manufacturer: '',
        type: '',
        search: '',
      },
      materials: [],
      totalMaterials: 0,
      manufacturers: [],
      plastics: [],
      selectedMaterial: { id: null },
    };
  },
  methods: {
    getMaterials: function() {
      this.resetSelectedMaterial();
      Api.getMaterials(this.query.page, this.query.elementsPerPage, this.query.search, this.query.manufacturer, this.query.type)
        .then(response => {
          this.materials = response.body.data;
          this.totalMaterials = response.body.totalElements;
        })
        .catch(error => this.handleHttpError(error));
    },
    getPlastics: function() {
      Api.getAllPlastics()
        .then(response => {
          this.plastics = response.body.data;
        })
        .catch(error => this.handleHttpError(error));
    },
    getManufacturers: function() {
      Api.getManufacturers()
        .then(response => {
          this.manufacturers = response.body;
        })
        .catch(this.handleHttpError);
    },
    setManufacturer: function(value) {
      this.query.page = 1;
      this.getMaterials();
    },
    setPlastic: function(value) {
      this.query.page = 1;
      this.getMaterials();
    },
    setSearch: function(value) {
      this.query.search = value;
      this.query.page = 1;
      this.getMaterials();
    },
    resetFilters: function() {
      this.query.manufacturer = '';
      this.query.type = '';
      this.search = '';
      this.query.search = '';
      this.query.page = 1;
      this.getMaterials();
    },
    selectMaterial: function(material) {
      this.selectedMaterial = { ...material };
    },
    resetSelectedMaterial: function() {
      this.$refs['materialsTable'].setCurrentRow(1);
      this.selectedMaterial = { id: null };
    },
    changePage: function(page) {
      this.query.page = page;
      this.getMaterials();
    },
  },
  mounted() {
    this.getManufacturers();
    this.getPlastics();
    this.getMaterials();
  },
};
</script>
