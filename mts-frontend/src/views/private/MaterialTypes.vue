<template>
  <div id="material-types">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{$t('general.materialTypes')}}</div>
        <div class="right">
          <router-link to="/private/material-types/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">{{$t('materialTypes.create')}}</el-button>
          </router-link>
        </div>
      </div>

      <Alert type="success" :description="feedback" :show="feedback !== null" />

      <!-- Materiyl Type Table -->
      <div class="content-row">
        <el-table
          :data="materialTypes"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectMaterialType"
          ref="materialTypesTable"
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
            :total="totalMaterialTypes"
            :page-size="query.elementsPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button
            icon="el-icon-edit"
            type="info"
            size="mini"
            :disabled="selectedMaterialType.id === null"
            @click="editMaterialType"
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
  name: 'MaterialTypes',
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
      materialTypes: [],
      totalMaterialTypes: 0,
      selectedMaterialType: { id: null },
      feedback: this.successMessage || null,
    };
  },
  methods: {
    getMaterialTypes: function() {
      this.resetSelectedMaterialType();
      Api.getMaterialTypes(this.query.page, this.query.elementsPerPage, this.query.search)
        .then(response => {
          this.materialTypes = response.body.data;
          this.totalMaterialTypes = response.body.totalElements;
        })
        .catch(error => this.handleHttpError(error));
    },
    selectMaterialType: function(materialType) {
      this.selectedMaterialType = { ...materialType };
    },
    resetSelectedMaterialType: function() {
      this.$refs['materialTypesTable'].setCurrentRow(1);
      this.selectedMaterialType = { id: null };
    },
    changePage: function(page) {
      this.query.page = page;
      this.getMaterialTypes();
    },
    editMaterialType: function() {
      this.$prompt(this.$t('materialTypes.editPrompt'), this.$t('materialTypes.edit'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('materialTypes.editInputError'),
      })
        .then(({ value }) => {
          Api.updateMaterialType(this.selectedMaterialType.id, value)
            .then(result => {
              this.feedback = this.$t('materialTypes.updated', result.body);
              for (let i = 0; i < this.materialTypes.length; i++) {
                if (this.materialTypes[i].id === this.selectedMaterialType.id) {
                  this.materialTypes[i].name = value;
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
    this.getMaterialTypes();
  },
};
</script>