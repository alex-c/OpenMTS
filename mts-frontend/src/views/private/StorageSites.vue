<template>
  <div id="storage-sites">
    <div class="content-section">
      <!-- Page Title -->
      <div class="content-row content-title">{{$t('storage.sites')}}</div>

      <!-- Search Bar -->
      <div class="content-row" id="search-bar">
        <el-input
          :placeholder="$t('storage.filter')"
          prefix-icon="el-icon-search"
          v-model="search"
          size="mini"
          clearable
          @change="setSearch"
        ></el-input>
      </div>

      <!-- Table -->
      <div class="content-row">
        <el-table
          :data="sites"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          ref="siteTable"
          row-key="id"
        >
          <el-table-column type="expand">
            <template slot-scope="props">
              <p id="area-tags">
                <b>{{$t('storage.areas')}}:</b>
                <el-tag v-for="area in props.row.areas" :key="area.id" size="mini">{{ area.name }}</el-tag>
              </p>
            </template>
          </el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column
            prop="areas"
            :label="$t('storage.areas')"
            :formatter="areaCountFormatter"
          ></el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Buttons -->
      <div class="content-row">
        <el-pagination
          background
          layout="prev, pager, next"
          :total="totalSites"
          :page-size="query.sitesPerPage"
          :current-page.sync="query.page"
          @current-change="changePage"
        ></el-pagination>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'StorageSites',
  mixins: [GenericErrorHandlingMixin],
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
    };
  },
  methods: {
    getStorageSites: function() {
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
#search-bar {
  overflow: hidden;
}

#area-tags .el-tag {
  margin-left: 4px;
}
</style>