<template>
  <div id="configuration">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row content-title">{{$t('general.configuration')}}</div>
    </div>

    <!-- Guest Login -->
    <div class="content-section">
      <div class="content-row content-subtitle">{{$t('config.guestLogin')}}</div>
      <Alert
        type="success"
        :description="feedback.guestLogin"
        :show="feedback.guestLogin !== undefined"
      />
      <div class="content-row">
        {{$t('config.allowGuestLogin')}}
        <el-switch v-model="config.allowGuestLogin" @change="switchAllowGuestLogin" />
        <Alert
          type="warning"
          :description="$t('config.guestLoginWarning')"
          :show="true"
          :dark="true"
        />
      </div>
    </div>

    <!-- Material Custom Props -->
    <div class="content-section">
      <!-- Title & Create Button -->
      <div class="content-row">
        <div class="left content-subtitle">{{$t('config.materialProps')}}</div>
        <div class="right">
          <el-button
            icon="el-icon-plus"
            type="primary"
            size="mini"
            @click="createMaterialProp"
          >{{$t('config.createProp')}}</el-button>
        </div>
      </div>

      <!-- Feedback -->
      <Alert
        type="success"
        :description="feedback.materialProps"
        :show="feedback.materialProps !== undefined"
      />

      <!-- Material Prop Table -->
      <div class="content-row">
        <el-table
          :data="materialProps"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          ref="materialPropTable"
          row-key="id"
          highlight-current-row
          @current-change="selectMaterialProp"
        >
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column prop="type" :label="$t('general.type')" :formatter="propTypeIdToText"></el-table-column>
        </el-table>
      </div>

      <!-- Selected Material Buttons -->
      <div class="content-row">
        <div class="right">
          <el-button
            icon="el-icon-edit"
            type="info"
            size="mini"
            :disabled="selectedMaterialProp.id === null"
            @click="editMaterialProp"
          >{{$t('general.edit')}}</el-button>
          <el-button
            icon="el-icon-delete"
            type="danger"
            size="mini"
            :disabled="selectedMaterialProp.id === null"
            @click="deleteMaterialProp"
          >{{$t('general.delete')}}</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import PropTypeHandlingMixin from '@/mixins/PropTypeHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'configuration',
  mixins: [GenericErrorHandlingMixin, PropTypeHandlingMixin],
  components: { Alert },
  props: ['feedbackMaterialProps'],
  data() {
    return {
      config: {
        allowGuestLogin: false,
      },
      materialProps: [],
      selectedMaterialProp: {
        id: null,
      },
      feedback: {
        guestLogin: undefined,
        materialProps: this.feedbackMaterialProps,
      },
    };
  },
  methods: {
    getConfiguration: function() {
      Api.getConfiguration()
        .then(response => {
          this.config = response.body;
        })
        .catch(error => {
          this.$message({
            message: this.$t(error.message),
            type: 'error',
            showClose: true,
          });
        });
    },
    switchAllowGuestLogin: function(allowGuestLogin) {
      this.$confirm(allowGuestLogin ? this.$t('config.allowGuestLoginConfirm') : this.$t('config.disallowGuestLoginConfirm'), {
        confirmButtonText: allowGuestLogin ? this.$t('config.allow') : this.$t('config.disallow'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          Api.setConfiguration(allowGuestLogin)
            .then(response => {
              this.feedback.guestLogin = allowGuestLogin ? this.$t('config.guestLoginAllowed') : this.$t('config.guestLoginDisallowed');
            })
            .catch(error => {
              this.config.allowGuestLogin = !allowGuestLogin;
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    getMaterialProps: function() {
      this.resetSelecterMaterialProp();
      Api.getCustomMaterialProps()
        .then(result => {
          this.materialProps = result.body;
        })
        .catch(error => this.handleHttpError());
    },
    createMaterialProp: function() {
      this.$router.push({ name: 'createMaterialProp' });
    },
    selectMaterialProp: function(prop) {
      this.selectedMaterialProp = {
        ...prop,
      };
    },
    resetSelecterMaterialProp: function() {
      this.$refs['materialPropTable'].setCurrentRow(1);
      this.selectedMaterialProp = { id: null };
    },
    editMaterialProp: function() {
      this.$prompt(this.$t('config.editPropPrompt'), this.$t('config.editProp'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('config.editPropInputError'),
      })
        .then(({ value }) => {
          Api.updateCustomMaterialProp(this.selectedMaterialProp.id, value)
            .then(result => {
              this.feedback.materialProps = this.$t('config.propUpdated', result.body);
              this.getMaterialProps();
            })
            .catch(this.handleHttpError);
        })
        .catch(() => {});
    },
    deleteMaterialProp: function() {
      this.$confirm(this.$t('config.confirmMaterialPropDeletion', { ...this.selectedMaterialProp }), {
        confirmButtonText: this.$t('general.delete'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'error',
      })
        .then(() => {
          Api.deleteCustomMaterialProp(this.selectedMaterialProp.id)
            .then(response => {
              this.feedback.materialProps = this.$t('config.materialPropDeleted', { ...this.selectedMaterialProp });
              this.resetSelecterMaterialProp();
              this.getMaterialProps();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
  },
  mounted() {
    this.getConfiguration();
    this.getMaterialProps();
  },
};
</script>