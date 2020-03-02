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
            @click="createSite"
          >{{$t('storage.createSite')}}</el-button>
        </div>
      </div>
      <div class="content-row">
        <div class="content-row">
          <el-table
            :data="sites"
            stripe
            border
            size="mini"
            :empty-text="$t('general.noData')"
            highlight-current-row
            @current-change="selectSite"
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
            <el-table-column prop="areas" :label="$t('storage.areas')" :formatter="areaCount"></el-table-column>
          </el-table>
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
    };
  },
  methods: {
    getStorageSites: function() {
      Api.getStorageLocations()
        .then(result => {
          this.sites = result.body;
        })
        .catch(error => this.handleHttpError(error));
    },
    selectSite: function() {},
    createSite: function() {
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
    areaCount: function(site) {
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