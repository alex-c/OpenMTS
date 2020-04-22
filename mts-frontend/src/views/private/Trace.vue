<template>
  <div id="trace" class="page-small">
    <div class="content-section">
      <div class="content-title">{{ $t('general.trace') }}</div>

      <div class="content-row">
        {{ $t('trace.prompt') }}
      </div>
      <div class="content-row content-row-inputs">
        <el-input v-model="query" :placeholder="$t('trace.placeholder')" size="mini" clearable @change="trace" />
      </div>
      <div v-if="result != null">
        <div class="content-row content-subtitle">{{ $t('trace.result') }}</div>
        <div class="content-row">
          <el-tabs type="border-card">
            <el-tab-pane :label="$t('trace.overview')">
              <el-tree :data="treeResult" :placeholder="$t('trace.result')" />
            </el-tab-pane>
            <el-tab-pane :label="$t('trace.raw')">
              <el-input type="textarea" :rows="20" v-model="rawResult" :placeholder="$t('trace.result')" />
            </el-tab-pane>
          </el-tabs>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'trace',
  mixins: [GenericErrorHandlingMixin],
  props: ['transactionId'],
  data() {
    return {
      query: '',
      result: null,
    };
  },
  computed: {
    rawResult() {
      return JSON.stringify(this.result);
    },
    treeResult() {
      const result = this.result;
      return [
        { label: `${this.$t('inventory.quantity')}: ${result.checkOutTransaction.quantity * -1} kg` },
        {
          label: `${this.$t('trace.checkedOut')}: ${new Date(result.checkOutTransaction.timestamp).toLocaleString(this.$i18n.locale, {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
          })} ${this.$t('trace.by')} ${result.checkOutTransaction.userId}`,
        },
        {
          label: `${this.$t('general.material')} ${result.batch.material.id}`,
          children: [
            { label: `${this.$t('general.name')}: ${result.batch.material.name}` },
            { label: `${this.$t('materials.manufacturer')}: ${result.batch.material.manufacturer}` },
            { label: `${this.$t('materials.manufacturerId')}: ${result.batch.material.manufacturerSpecificId}` },
            { label: `${this.$t('materials.type')}: ${result.batch.material.type.id} - ${result.batch.material.type.name}` },
            {
              label: this.$t('materials.props'),
              children: result.batch.material.customProps.map(prop => {
                return {
                  label: `${prop.propId}: ${prop.value}`,
                };
              }),
            },
          ],
        },
        {
          label: `${this.$t('trace.batch')} ${result.batch.id}`,
          children: [
            { label: `${this.$t('inventory.batchNumber')}: ${result.batch.batchNumber}` },
            { label: `${this.$t('inventory.quantity')}: ${result.batch.quantity} kg` },
            {
              label: `${this.$t('inventory.expirationDate')}: ${new Date(result.batch.expirationDate).toLocaleDateString(this.$i18n.locale, {
                year: 'numeric',
                month: 'long',
                day: 'numeric',
              })}`,
            },
            {
              label: `${this.$t('trace.checkedIn')}: ${new Date(result.checkInTransaction.timestamp).toLocaleString(this.$i18n.locale, {
                year: 'numeric',
                month: 'long',
                day: 'numeric',
                hour: 'numeric',
                minute: 'numeric',
              })} ${this.$t('trace.by')} ${result.checkInTransaction.userId}`,
            },
            {
              label: this.$t('materials.props'),
              children: Object.entries(result.batch.customProps).map(prop => {
                return {
                  label: `${prop[0]}: ${prop[1]}`,
                };
              }),
            },
          ],
        },
        {
          label: this.$t('storage.location'),
          children: [
            { label: `${this.$t('storage.site')}: ${result.batch.storageLocation.storageSiteName} (${result.batch.storageLocation.storageSiteId})` },
            { label: `${this.$t('storage.area')}: ${result.batch.storageLocation.storageAreaName} (${result.batch.storageLocation.storageAreaId})` },
          ],
        },
        {
          label: this.$t('environment.temperatureWithUnit'),
          children: [
            { label: `${this.$t('environment.minimum')}: ${result.temperature.minValue}` },
            { label: `${this.$t('environment.maximum')}: ${result.temperature.maxValue}` },
          ],
        },
        {
          label: this.$t('environment.humidityWithUnit'),
          children: [{ label: `${this.$t('environment.minimum')}: ${result.humidity.minValue}` }, { label: `${this.$t('environment.maximum')}: ${result.humidity.maxValue}` }],
        },
      ];
    },
  },
  methods: {
    trace: function() {
      if (this.query != '') {
        this.result = null;
        Api.trace(this.query)
          .then(result => {
            this.result = result.body;
          })
          .catch(this.handleHttpError);
      }
    },
  },
  mounted() {
    if (this.transactionId !== undefined && this.transactionId !== null) {
      this.query = this.transactionId;
      this.trace();
    }
  },
};
</script>
