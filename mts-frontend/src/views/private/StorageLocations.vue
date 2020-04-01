<template>
  <div id="storage-locations" class="page-small">
    <div class="content-section">
      <!-- Page Title -->
      <div class="content-row">
        <div class="left content-title">{{ $t('storage.sites') }}</div>
        <div class="right">
          <el-button icon="el-icon-plus" type="primary" size="mini" @click="createStorageSite">{{ $t('storage.createSite') }}</el-button>
        </div>
      </div>

      <!-- Search Bar -->
      <div class="content-row content-row-inputs">
        <el-input :placeholder="$t('storage.filter')" prefix-icon="el-icon-search" v-model="search" size="mini" clearable @change="setSearch"></el-input>
      </div>

      <!-- Table -->
      <div class="content-row">
        <el-table :data="sites" stripe border size="mini" :empty-text="$t('general.noData')" highlight-current-row @current-change="selectStorageSite" ref="siteTable" row-key="id">
          <el-table-column type="expand">
            <template slot-scope="props">
              <p id="area-tags">
                <b>{{ $t('storage.areas') }}:</b>
                <el-tag v-for="area in props.row.areas" :key="area.id" size="mini">{{ area.name }}</el-tag>
              </p>
            </template>
          </el-table-column>
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column prop="areas" :label="$t('storage.areas')" width="120" :formatter="areaCountFormatter"></el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Buttons -->
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalSites"
            :page-size="query.sitesPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button icon="el-icon-edit" type="info" size="mini" :disabled="selected.id === null" @click="editStorageSite">{{ $t('general.edit') }}</el-button>
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
  name: 'StorageLocations',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  data() {
    return {
      search: '',
      query: {
        page: 1,
        sitesPerPage: 10,
        search: '',
      },
      sites: [],
      totalSites: 0,
      selected: {
        id: null,
        name: null,
        areas: null,
      },
    };
  },
  methods: {
    getStorageSites: function() {
      this.resetSelectedStorageSite();
      Api.getStorageSites(this.query.page, this.query.elementsPerPage, this.query.search)
        .then(result => {
          this.sites = result.body.data;
          this.totalSites = result.body.totalElements;
        })
        .catch(error => this.handleHttpError(error));
    },
    changePage: function(page) {
      this.query.page = page;
      this.getStorageSites();
    },
    setSearch: function(value) {
      this.query.search = value;
      this.query.page = 1;
      this.getStorageSites();
    },
    selectStorageSite: function(site) {
      this.selected = {
        id: site.id,
        name: site.name,
        areas: site.areas,
      };
    },
    resetSelectedStorageSite: function() {
      this.$refs['siteTable'].setCurrentRow(1);
      this.selected.id = null;
      this.selected.name = null;
      this.selected.areas = null;
    },
    createStorageSite: function() {
      this.$prompt(this.$t('storage.createSitePrompt'), this.$t('storage.createSite'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('storage.createSiteInputError'),
      })
        .then(({ value }) => {
          Api.createStorageSite(value)
            .then(result => {
              this.getStorageSites();
            })
            .catch(error => this.handleHttpError(error));
        })
        .catch(() => {});
    },
    editStorageSite: function() {
      const params = { id: this.selected.id, name: this.selected.name, areas: this.selected.areas };
      this.$router.push({ name: 'editStoragSite', params });
    },
    areaCountFormatter: function(site) {
      return site.areas.length;
    },
  },
  mounted() {
    this.getStorageSites();
  },
};
</script>

<style lang="scss" scoped>
#area-tags .el-tag {
  margin-left: 4px;
}
</style>
