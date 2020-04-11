<template>
  <div id="environment" class="page-medium">
    <div class="content-section">
      <!-- Page Title & Back Button -->
      <div class="content-row">
        <div class="left content-title">{{ $t('general.environment') }}</div>
        <div class="right">
          <router-link to="/private/sites">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Base Data & Latest Values -->
      <div class="content-row">
        <el-table :data="site" border size="mini" :empty-text="$t('general.noData')">
          <el-table-column prop="id" :label="$t('general.id')" />
          <el-table-column prop="name" :label="$t('general.name')" />
          <el-table-column prop="temperature" :label="$t('environment.temperatureWithUnit')" />
          <el-table-column prop="humidity" :label="$t('environment.humidityWithUnit')" />
        </el-table>
      </div>

      <!-- History & Extrema - Ttitle, Picker -->
      <div class="content-row content-subtitle">{{ $t('environment.history') }} &amp; {{ $t('environment.extrema') }}</div>
      <div class="content-row" id="picker-wrapper">
        <el-date-picker
          v-model="range"
          type="datetimerange"
          :picker-options="pickerOptions"
          :range-separator="$t('environment.rangeSeparator')"
          :start-placeholder="$t('environment.startTime')"
          :end-placeholder="$t('environment.endTime')"
          size="mini"
          @change="getAllData"
        />
      </div>

      <!-- History -->
      <div class="content-row" id="graph-container">
        <div class="graph"><apexchart type="line" :options="temperatureChartOptions" :series="temperature" /></div>
        <div class="graph"><apexchart type="line" :options="humidityChartOptions" :series="humidity" /></div>
      </div>

      <!-- Extrema -->
      <div class="content-row content-subtitle"></div>
      <div class="content-row">
        <el-table :data="extrema" border size="mini" :empty-text="$t('general.noData')">
          <el-table-column prop="factor" :label="$t('general.id')" />
          <el-table-column prop="minValue" :label="$t('environment.minimum')" />
          <el-table-column prop="maxValue" :label="$t('environment.maximum')" />
        </el-table>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'environment',
  mixins: [GenericErrorHandlingMixin],
  props: ['siteId'],
  data() {
    const today = new Date();
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(today.getMonth() - 1);
    return {
      site: [],
      range: [oneMonthAgo, today],
      pickerOptions: {
        shortcuts: [
          {
            text: 'Last day',
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24);
              picker.$emit('pick', [start, end]);
            },
          },
          {
            text: 'Last week',
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
              picker.$emit('pick', [start, end]);
            },
          },
          {
            text: 'Last month',
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
              picker.$emit('pick', [start, end]);
            },
          },
        ],
      },
      temperatureChartOptions: {
        chart: {
          id: 'temperature-chart',
          zoom: {
            enabled: false,
          },
        },
        title: {
          text: this.$t('environment.temperatureWithUnit'),
          align: 'left',
        },
        xaxis: {
          type: 'datetime',
        },
        tooltip: {
          x: {
            format: 'dd MMM yyyy - HH:mm:ss',
          },
        },
        noData: {
          text: this.$t('general.noData'),
        },
      },
      humidityChartOptions: {
        chart: {
          id: 'humidity-chart',
          zoom: {
            enabled: false,
          },
        },
        title: {
          text: this.$t('environment.humidityWithUnit'),
          align: 'left',
        },
        xaxis: {
          type: 'datetime',
        },
        tooltip: {
          x: {
            format: 'dd MMM yyyy - HH:mm:ss',
          },
        },
        noData: {
          text: this.$t('general.noData'),
        },
      },
      temperature: [],
      humidity: [],
      extrema: [],
    };
  },
  methods: {
    getAllData: function() {
      this.extrema = [];
      this.getSite();
      this.getTemperatureHistory();
      this.getHumidityHistory();
      this.getExtrema('temperature');
      this.getExtrema('humidity');
    },
    getSite: function() {
      Api.getStorageSite(this.siteId)
        .then(response => {
          let site = response.body;
          Api.getStorageSiteLatestEnvironmentValue(this.siteId, 'temperature')
            .then(response => {
              site.temperature = response.body ? response.body.value : this.$t('general.noData');
              Api.getStorageSiteLatestEnvironmentValue(this.siteId, 'humidity')
                .then(response => {
                  site.humidity = response.body ? response.body.value : this.$t('general.noData');
                  this.site = [site];
                })
                .catch(this.handleHttpError);
            })
            .catch(this.handleHttpError);
        })
        .catch(this.handleHttpError);
    },
    getTemperatureHistory: function() {
      Api.getStorageSiteEnvironmentHistory(this.siteId, 'temperature', this.range[0].toUTCString(), this.range[1].toUTCString())
        .then(response => {
          this.temperature = [
            {
              name: this.$t('environment.temperature'),
              data: response.body.map(dp => [dp.timestamp, dp.value]),
            },
          ];
        })
        .catch(this.handleHttpError);
    },
    getHumidityHistory: function() {
      Api.getStorageSiteEnvironmentHistory(this.siteId, 'humidity', this.range[0].toUTCString(), this.range[1].toUTCString())
        .then(response => {
          this.humidity = [
            {
              name: this.$t('environment.humidity'),
              data: response.body.map(dp => [dp.timestamp, dp.value]),
            },
          ];
        })
        .catch(this.handleHttpError);
    },
    getExtrema: function(factor) {
      Api.getStorageSiteEnvironmentExtrema(this.siteId, factor, this.range[0].toUTCString(), this.range[1].toUTCString())
        .then(response => {
          this.extrema.push({
            factor: this.$t(`environment.${factor}WithUnit`),
            ...response.body,
          });
        })
        .catch(this.handleHttpError);
    },
  },
  mounted() {
    this.getAllData();
  },
};
</script>

<style lang="scss" scoped>
#picker-wrapper > div {
  width: 100%;
}

#graph-container {
  display: flex;
  .graph {
    width: 50%;
    overflow: hidden;
  }
}
</style>
