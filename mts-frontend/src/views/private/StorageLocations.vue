<template>
  <div id="storage-locations">
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{$t('storage.sites')}}</div>
        <div class="right">
          <el-button
            icon="el-icon-plus"
            type="primary"
            size="mini"
            @click="createStorageSite"
          >{{$t('storage.createSite')}}</el-button>
        </div>
      </div>
      <div class="content-row">
        <el-table
          :data="sites"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectStorageSite"
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
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column
            prop="areas"
            :label="$t('storage.areas')"
            :formatter="areaCountFormatter"
          ></el-table-column>
        </el-table>
      </div>
      <div class="content-row">
        <!--div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalKeys"
            :page-size="query.keysPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div-->
        <div class="right">
          <el-button
            icon="el-icon-edit"
            type="info"
            size="mini"
            :disabled="selected.id === null"
            @click="editStorageSite"
          >{{$t('general.edit')}}</el-button>
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
      sites: [],
      selected: {
        id: null,
        name: null,
        areas: null,
      },
    };
  },
  methods: {
    getStorageSites: function() {
      Api.getStorageSites()
        .then(result => {
          this.sites = result.body;
        })
        .catch(error => this.handleHttpError(error));
    },
    selectStorageSite: function(site) {
      this.selected = {
        id: site.id,
        name: site.name,
        areas: site.areas,
      };
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