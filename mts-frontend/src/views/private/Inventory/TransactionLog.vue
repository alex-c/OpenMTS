<template>
  <div id="transaction-log" class="page-large">
    <!-- Transaction log -->
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
        <el-table
          :data="transactions"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectTransaction"
          ref="transactionLog"
          row-key="id"
        >
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="quantity" :label="$t('inventory.quantity') + ' (kg)'"></el-table-column>
          <el-table-column prop="timestamp" :label="$t('inventory.timestamp')" :formatter="formatTimestamp"></el-table-column>
          <el-table-column prop="userId" :label="$t('inventory.userId')"></el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Trace Button-->
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalTransactions"
            :page-size="query.elementsPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button :disabled="selectedTransaction == null || selectedTransaction.quantity > 0" @click="trace" type="primary" size="mini" icon="el-icon-s-promotion">{{
            $t('general.trace')
          }}</el-button>
        </div>
      </div>
    </div>

    <!-- Log entry amending -->
    <div class="content-section">
      <div class="content-row content-title">{{ $t('inventory.amendTitle') }}</div>
      <div class="content-row">{{ $t('inventory.amendDescription') }}</div>
      <div class="content-row">
        <el-table :data="lastTransaction" border size="mini" :empty-text="$t('general.noData')" ref="lastTransaction" row-key="id">
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="quantity" :label="$t('inventory.quantity') + ' (kg)'"></el-table-column>
          <el-table-column prop="timestamp" :label="$t('inventory.timestamp')" :formatter="formatTimestamp"></el-table-column>
          <el-table-column prop="userId" :label="$t('inventory.userId')"></el-table-column>
        </el-table>
      </div>
      <div class="content-row">
        <div class="right">
          <el-button type="info" icon="el-icon-edit" size="mini" @click="amend">{{ $t('inventory.amend') }}</el-button>
        </div>
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
      lastTransaction: [],
      selectedTransaction: null,
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
    getLastTransaction: function() {
      Api.getLastTransaction(this.id)
        .then(result => {
          this.lastTransaction = [result.body];
        })
        .catch(this.handleHttpError);
    },
    changePage: function(page) {
      this.query.page = page;
      this.getTransactionLog();
    },
    selectTransaction: function(transaction) {
      this.selectedTransaction = transaction;
    },
    formatTimestamp: function(transaction) {
      return new Date(transaction.timestamp).toLocaleString(this.$i18n.locale, { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' });
    },
    trace: function() {
      this.$router.push({ name: 'trace', params: { transactionId: this.selectedTransaction.id } });
    },
    amend: function() {
      this.$prompt(this.$t('inventory.amendPrompt', { oldValue: this.lastTransaction[0].quantity }), this.$t('inventory.amendTitle'), {
        confirmButtonText: this.$t('inventory.amend'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^\-?[0-9]+(\.[0-9]+)?$/,
        inputErrorMessage: this.$t('inventory.transactionInputError'),
      })
        .then(({ value }) => {
          Api.amendLastTransaction(this.id, this.lastTransaction[0].id, value)
            .then(result => {
              this.$alert(this.$t('inventory.amendSuccessMessage'), this.$t('inventory.amendSuccessTitle'), {
                confirmButtonText: this.$t('general.ok'),
                showClose: false,
                type: 'success',
              }).then(() => {
                this.getTransactionLog();
                this.getLastTransaction();
              });
            })
            .catch(error => {
              if (error.status === 400) {
                this.$alert(this.$t('inventory.amendFailMessageLastTransaction'), this.$t('inventory.amendFailTitle'), {
                  confirmButtonText: this.$t('general.ok'),
                  type: 'error',
                });
              } else if (error.status === 403) {
                this.$alert(this.$t('inventory.amendFailMessageWrongUser'), this.$t('inventory.amendFailTitle'), {
                  confirmButtonText: this.$t('general.ok'),
                  type: 'error',
                });
              } else {
                this.handleHttpError(error);
              }
            });
        })
        .catch(() => {});
    },
  },
  mounted() {
    this.getTransactionLog();
    this.getLastTransaction();
  },
};
</script>
