<template>
  <div id="api-keys">
    <Alert
      type="success"
      :description="feedback.successMessage"
      :show="feedback.successMessage !== undefined"
    />
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{$t('general.apiKeys')}}</div>
        <div class="right">
          <el-button
            icon="el-icon-plus"
            type="primary"
            size="mini"
            @click="create"
          >{{$t('apiKeys.create')}}</el-button>
        </div>
      </div>

      <!-- Keys Table -->
      <div class="content-row">
        <el-table
          :data="keys"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectKey"
          ref="keysTable"
          row-key="id"
        >
          <el-table-column type="expand">
            <template slot-scope="props">
              <p id="rights-tags">
                <b>{{$t('apiKeys.rights')}}:</b>
                <el-tag v-for="right in props.row.rights" :key="right" size="mini">{{ right }}</el-tag>
              </p>
            </template>
          </el-table-column>
          <el-table-column prop="id" :label="$t('apiKeys.key')"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column
            prop="enabled"
            :label="$t('general.status.label')"
            :formatter="enabledText"
          ></el-table-column>
        </el-table>
      </div>

      <!-- Pagination & Buttons -->
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalKeys"
            :page-size="query.keysPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button
            icon="el-icon-unlock"
            type="success"
            size="mini"
            v-if="this.selected.enabled === false"
            @click="enable"
          >{{$t('general.enable')}}</el-button>
          <el-button
            icon="el-icon-lock"
            type="warning"
            size="mini"
            v-if="this.selected.enabled === true"
            @click="disable"
          >{{$t('general.disable')}}</el-button>
          <el-button
            icon="el-icon-edit"
            type="info"
            size="mini"
            :disabled="selected.id === null"
            @click="edit"
          >{{$t('general.edit')}}</el-button>
          <el-button
            icon="el-icon-delete"
            type="danger"
            size="mini"
            :disabled="selected.id === null"
            @click="deleteKey"
          >{{$t('general.delete')}}</el-button>
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
  name: 'ApiKey',
  components: { Alert },
  mixins: [GenericErrorHandlingMixin],
  props: ['successMessage'],
  data() {
    return {
      query: {
        page: 1,
        keysPerPage: 10,
      },
      keys: [],
      totalKeys: 0,
      selected: {
        id: null,
        name: null,
        enabled: null,
        rights: null,
      },
      feedback: {
        successMessage: this.successMessage,
      },
    };
  },
  methods: {
    getKeys: function() {
      Api.getApiKeys(this.query.page, this.query.keysPerPage)
        .then(result => {
          this.keys = result.body.data;
          this.totalKeys = result.body.totalElements;
        })
        .catch(error => {
          this.handleHttpError(error);
        });
    },
    create: function() {
      this.$prompt(this.$t('apiKeys.createPrompt'), this.$t('apiKeys.create'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('apiKeys.createInputError'),
      })
        .then(({ value }) => {
          Api.createApiKey(value)
            .then(result => this.$router.push({ name: 'editKey', params: { id: result.body.id, name: result.body.name, rights: result.body.rights } }))
            .catch(error => this.handleHttpError(error));
        })
        .catch(() => {});
    },
    selectKey: function(key) {
      this.selected = {
        id: key.id,
        name: key.name,
        enabled: key.enabled,
        rights: key.rights,
      };
    },
    resetSelectedKey: function() {
      this.$refs['keysTable'].setCurrentRow(1);
      this.selected.id = null;
      this.selected.name = null;
      this.selected.enabled = null;
      this.selected.rights = null;
    },
    changePage: function(page) {
      this.query.page = page;
      this.getKeys();
    },
    edit: function() {
      const params = { id: this.selected.id, name: this.selected.name, rights: this.selected.rights };
      this.$router.push({ name: 'editKey', params });
    },
    deleteKey: function() {
      this.$confirm(this.$t('apiKeys.deleteConfirm', { id: this.selected.id, name: this.selected.name }), {
        confirmButtonText: this.$t('general.delete'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'error',
      })
        .then(() => {
          Api.deleteApiKey(this.selected.id)
            .then(result => {
              this.feedback.successMessage = this.$t('apiKeys.deleted', { id: this.selected.id, name: this.selected.name });
              this.query.page = 1;
              this.resetSelectedKey();
              this.getKeys();
            })
            .catch(error => this.handleHttpError(error));
        })
        .catch(() => {});
    },
    disable: function() {
      this.$confirm(this.$t('apiKeys.disableConfirm', { id: this.selected.id, name: this.selected.name }), {
        confirmButtonText: this.$t('general.disable'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          Api.updateApiKeyStatus(this.selected.id, false)
            .then(response => {
              this.feedback.successMessage = this.$t('apiKeys.disabled', { id: this.selected.id, name: this.selected.name });
              this.resetSelectedKey();
              this.getKeys();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    enable: function() {
      this.$confirm(this.$t('apiKeys.enableConfirm', { id: this.selected.id, name: this.selected.name }), {
        confirmButtonText: this.$t('general.enable'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          Api.updateApiKeyStatus(this.selected.id, true)
            .then(response => {
              this.feedback.successMessage = this.$t('apiKeys.enabled', { id: this.selected.id, name: this.selected.name });
              this.resetSelectedKey();
              this.getKeys();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    enabledText: function(key) {
      if (key.enabled) {
        return this.$t('general.status.enabled');
      } else {
        return this.$t('general.status.disabled');
      }
    },
  },
  mounted: function() {
    this.getKeys();
  },
};
</script>

<style lang="scss" scoped>
#rights-tags .el-tag {
  margin-left: 4px;
}
</style>