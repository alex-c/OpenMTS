<template>
  <div id="dashboard">
    <div class="content-section">
      <div class="content-row content-title">{{ $t('dashboard.welcome') }}</div>
      <div class="content-row" id="card-container">
        <Empty v-if="sites.length == 0">{{ $t('general.noData') }}</Empty>
        <div class="card" v-else v-for="site in sites" :key="site.site.id">
          <div class="card-header">
            <div>{{ site.totalMaterial }} kg</div>
          </div>
          <div class="card-body">
            <div class="title">{{ site.site.name }}</div>
            <div class="row">
              <div class="left">{{ $t('environment.temperature') }}</div>
              <div class="right">{{ site.temperature == null ? $t('general.noData') : `${site.temperature} °C` }}</div>
            </div>
            <div class="row">
              <div class="left">{{ $t('environment.humidity') }}</div>
              <div class="right">{{ site.humidity == null ? $t('general.noData') : `${site.humidity} %` }}</div>
            </div>
          </div>
          <div class="card-footer">
            <div class="button" @click="toInventory(site.site.id)"><i class="el-icon-box" /> Inventory</div>
            <div class="button" @click="toEnvironment(site.site.id)"><i class="el-icon-view" /> Environemnt</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Empty from '@/components/Empty.vue';

export default {
  name: 'dashboard',
  mixins: [GenericErrorHandlingMixin],
  components: { Empty },
  data() {
    return {
      sites: [],
    };
  },
  methods: {
    getStorageSiteOverview: function() {
      Api.getStatsSitesOverview()
        .then(result => {
          this.sites = result.body;
        })
        .catch(error => this.handleHttpError(error));
    },
    toInventory: function(siteId) {
      this.$router.push({ name: 'inventory', params: { storageSiteFilter: siteId } });
    },
    toEnvironment: function(siteId) {
      this.$router.push({ name: 'environment', params: { siteId } });
    },
  },
  mounted() {
    this.getStorageSiteOverview();
  },
};
</script>

<style lang="scss" scoped>
#card-container {
  padding: 10px 0px;
  display: flex;
  justify-content: space-evenly;
  flex-wrap: wrap;
  & > div {
    flex-grow: 1;
  }
}

.card {
  margin: 0px 10px 10px 10px;
  min-width: 300px;
  height: 200px;
  border-radius: 4px;
  border: 1px solid #ebeef5;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.card-header {
  height: 100px;
  border-radius: 4px 4px 0px 0px;
  background-color: #67c23a;
  color: white;
  font-weight: bold;
  position: relative;
  & > div {
    margin: 0;
    position: absolute;
    top: 50%;
    -ms-transform: translateY(-50%);
    transform: translateY(-50%);
    width: 100%;
    text-align: center;
  }
}

.card-body {
  height: 60px;
  padding: 4px;
  .title {
    font-weight: bold;
  }
  .row {
    margin-top: 4px;
    overflow: auto;
  }
}

.card-footer {
  height: 32px;
  border-radius: 0px 0px 4px 4px;
  display: flex;
  .button {
    width: 50%;
    border-top: 1px solid #ebeef5;
    padding: 8px;
    border-radius: 0px 0px 4px 0px;
    &:first-child {
      border-right: 1px solid #ebeef5;
      border-radius: 0px 0px 0px 4px;
    }
    &:hover {
      cursor: pointer;
      background-color: #057d13;
      color: white;
    }
  }
}
</style>
