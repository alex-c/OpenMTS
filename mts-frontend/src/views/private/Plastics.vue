<template>
  <div id="plastics" class="page-small">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{ $t('general.plastics') }}</div>
        <div class="right">
          <router-link to="/private/plastics/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">{{ $t('plastics.create') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Feedback -->
      <Alert type="success" :description="feedback" :show="feedback !== null" />

      <!-- Search Bar -->
      <div class="content-row content-row-inputs">
        <el-input :placeholder="$t('plastics.filter')" prefix-icon="el-icon-search" v-model="search" size="mini" clearable @change="setSearch"></el-input>
      </div>

      <!-- Materiyl Type Table -->
      <div class="content-row">
        <el-table
          :data="plastics"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectPlastic"
          ref="plasticsTable"
          row-key="id"
        >
          <el-table-column prop="id" :label="$t('general.id')" width="100"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Options -->
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalPlastics"
            :page-size="query.elementsPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button icon="el-icon-edit" type="info" size="mini" :disabled="selectedPlastic.id === null" @click="editPlastic">{{ $t('general.edit') }}</el-button>
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
  name: 'Plastics',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  props: ['successMessage'],
  data() {
    return {
      search: '',
      query: {
        page: 1,
        elementsPerPage: 10,
        search: '',
      },
      plastics: [],
      totalPlastics: 0,
      selectedPlastic: { id: null },
      feedback: this.successMessage || null,
    };
  },
  methods: {
    getPlastics: function() {
      this.resetSelectedPlastic();
      Api.getPlastics(this.query.page, this.query.elementsPerPage, this.query.search)
        .then(response => {
          this.plastics = response.body.data;
          this.totalPlastics = response.body.totalElements;
        })
        .catch(error => this.handleHttpError(error));
    },
    selectPlastic: function(plastic) {
      this.selectedPlastic = { ...plastic };
    },
    resetSelectedPlastic: function() {
      this.$refs['plasticsTable'].setCurrentRow(1);
      this.selectedPlastic = { id: null };
    },
    setSearch: function(value) {
      this.query.search = value;
      this.query.page = 1;
      this.getPlastics();
    },
    changePage: function(page) {
      this.query.page = page;
      this.getPlastics();
    },
    editPlastic: function() {
      this.$prompt(this.$t('plastics.editPrompt'), this.$t('plastics.edit'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('plastics.editInputError'),
      })
        .then(({ value }) => {
          Api.updatePlastic(this.selectedPlastic.id, value)
            .then(result => {
              this.feedback = this.$t('plastics.updated', result.body);
              for (let i = 0; i < this.plastics.length; i++) {
                if (this.plastics[i].id === this.selectedPlastic.id) {
                  this.plastics[i].name = value;
                  this.selectedPlastic.name = value;
                  break;
                }
              }
            })
            .catch(this.handleHttpError);
        })
        .catch(() => {});
    },
  },
  mounted() {
    this.getPlastics();
  },
};
</script>
