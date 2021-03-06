<template>
  <div id="environment" class="page-large">
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

      <!-- History & Extrema - Title, Picker -->
      <div class="content-row content-subtitle">{{ $t('environment.history') }} &amp; {{ $t('environment.extrema') }}</div>
      <div class="content-row content-row-inputs flex">
        <div class="grow" id="picker-wrapper">
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
        <div class="density-setting" style="padding-top: 4px;">Reduce data density: <el-switch v-model="reduceDataDensity" size="mini" plain /></div>
        <div class="density-setting">Max. data points: <el-input-number v-model="maxDataPoints" :min="100" size="mini" :disabled="!reduceDataDensity" /></div>
        <div class="density-setting"><el-button icon="el-icon-refresh" size="mini" plain @click="getAllData" /></div>
      </div>

      <!-- History -->
      <div class="content-row" id="graph-container">
        <div class="graph">
          <apexchart type="line" :options="temperatureChartOptions" :series="temperature" />
        </div>
        <div class="graph">
          <apexchart type="line" :options="humidityChartOptions" :series="humidity" />
        </div>
      </div>

      <!-- Extrema -->
      <div class="content-row">
        <el-table :data="extrema" border size="mini" :empty-text="$t('general.noData')">
          <el-table-column prop="factor" :label="$t('environment.factor')" />
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
    const end = new Date();
    const start = new Date();
    start.setTime(start.getTime() - 3600 * 1000 * 24);
    return {
      site: [],
      range: [start, end],
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
      reduceDataDensity: true,
      maxDataPoints: 1000,
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
          labels: {
            datetimeUTC: false,
          },
        },
        tooltip: {
          x: {
            format: 'dd MMM yyyy - HH:mm:ss',
          },
        },
        colors: ['#057D13'],
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
          labels: {
            datetimeUTC: false,
          },
        },
        tooltip: {
          x: {
            format: 'dd MMM yyyy - HH:mm:ss',
          },
        },
        colors: ['#057D13'],
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
              if (site.temperature == null) {
                site.temperature = this.$t('general.noData');
              }
              Api.getStorageSiteLatestEnvironmentValue(this.siteId, 'humidity')
                .then(response => {
                  site.humidity = response.body ? response.body.value : this.$t('general.noData');
                  if (site.humidity == null) {
                    site.humidity = this.$t('general.noData');
                  }
                  this.site = [site];
                })
                .catch(this.handleHttpError);
            })
            .catch(this.handleHttpError);
        })
        .catch(this.handleHttpError);
    },
    getTemperatureHistory: function() {
      const maxPoints = this.reduceDataDensity ? this.maxDataPoints : null;
      Api.getStorageSiteEnvironmentHistory(this.siteId, 'temperature', this.range[0].toUTCString(), this.range[1].toUTCString(), maxPoints)
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
      const maxPoints = this.reduceDataDensity ? this.maxDataPoints : null;
      Api.getStorageSiteEnvironmentHistory(this.siteId, 'humidity', this.range[0].toUTCString(), this.range[1].toUTCString(), maxPoints)
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
            minValue: response.body.minValue == null ? this.$t('general.noData') : response.body.minValue,
            maxValue: response.body.maxValue == null ? this.$t('general.noData') : response.body.maxValue,
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

.density-setting {
  margin-left: 16px;
}

#graph-container {
  display: flex;
  .graph {
    width: 50%;
    overflow: hidden;
  }
}
</style>
