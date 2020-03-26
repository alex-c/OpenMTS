<template>
  <div id="transaction-log">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{ $t('inventory.log') }}</div>
        <div class="right">
          <router-link to="/private/inventory">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Transaction Log -->
      <div class="content-row">
        <el-table :data="transactions" stripe border size="mini" :empty-text="$t('general.noData')" ref="transactionLog" row-key="id">
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="quantity" :label="$t('inventory.quantity') + ' (kg)'"></el-table-column>
          <el-table-column prop="timestamp" :label="$t('inventory.timestamp')" :formatter="formatTimestamp"></el-table-column>
          <el-table-column prop="userId" :label="$t('inventory.userId')"></el-table-column>
        </el-table>
      </div>

      <!-- Pagination -->
      <div class="content-row">
        <el-pagination
          background
          layout="prev, pager, next"
          :total="totalTransactions"
          :page-size="query.elementsPerPage"
          :current-page.sync="query.page"
          @current-change="changePage"
        ></el-pagination>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'TransactionLog',
  mixins: [GenericErrorHandlingMixin],
  props: ['id'],
  data() {
    return {
      query: {
        page: 1,
        elementsPerPage: 10,
      },
      transactions: [],
      totalTransactions: 0,
    };
  },
  methods: {
    getTransactionLog: function() {
      Api.getTransactionLog(this.query.page, this.query.elementsPerPage, this.id)
        .then(result => {
          this.transactions = result.body.data;
          this.totalTransactions = result.body.totalElements;
        })
        .catch(this.handleHttpError);
    },
    changePage: function(page) {
      this.query.page = page;
      this.getTransactionLog();
    },
    formatTimestamp: function(transaction) {
      return new Date(transaction.timestamp).toLocaleString(this.$i18n.locale, { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' });
    },
  },
  mounted() {
    this.getTransactionLog();
  },
};
</script>
