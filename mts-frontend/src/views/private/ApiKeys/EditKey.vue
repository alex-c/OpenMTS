<template>
  <div id="edit-keys" class="page-small">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{ $t('apiKeys.editTitle', { key: this.id }) }}</div>
        <div class="right">
          <router-link to="/private/keys">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>
    </div>

    <!-- API Key Edit Form -->
    <div class="content-section">
      <!-- Form -->
      <div class="content-row">
        <el-form :model="editKeyForm" :rules="validationRules" ref="editKeyForm" label-position="left" label-width="120px" size="mini">
          <el-form-item prop="name" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="editKeyForm.name"></el-input>
          </el-form-item>
          <el-form-item prop="rights" :label="$t('apiKeys.rights')">
            <el-transfer
              v-model="selectedRights"
              :data="availableRights"
              :titles="[$t('apiKeys.rightsAvailable'), $t('apiKeys.rightsSelected')]"
              filterable
              :filter-placeholder="$t('apiKeys.filterPlaceholder')"
              :empty-text="$t('general.noData')"
            ></el-transfer>
          </el-form-item>
        </el-form>
      </div>

      <!-- Buttons -->
      <div class="content-row-nopad">
        <div class="right">
          <el-button type="primary" icon="el-icon-check" size="mini" @click="edit">{{ $t('general.save') }}</el-button>
        </div>
      </div>
      <Alert type="error" :description="$t(error)" :show="error !== null" />
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'EditKey',
  components: { Alert },
  mixins: [GenericErrorHandlingMixin],
  props: ['id', 'name', 'rights'],
  data() {
    return {
      editKeyForm: {
        name: this.name,
      },
      availableRights: [],
      selectedRights: this.rights,
      error: null,
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('apiKeys.validation.name'), trigger: 'blur' },
      };
    },
  },
  methods: {
    edit: function() {
      Api.updateApiKey(this.id, this.editKeyForm.name, this.selectedRights)
        .then(result => {
          this.$router.push({ name: 'keys', params: { successMessage: this.$t('apiKeys.updated', { id: this.id, name: this.editKeyForm.name }) } });
        })
        .catch(error => this.handleHttpError(error));
    },
  },
  mounted() {
    Api.getRights()
      .then(result => {
        let rights = [];
        for (let i = 0; i < result.body.length; i++) {
          rights.push({ key: result.body[i], label: result.body[i], disabled: false });
        }
        this.availableRights = rights;
      })
      .catch(error => this.handleHttpError(error));
  },
};
</script>

<style lang="scss">
.el-transfer-panel {
  width: 258px !important;
}
</style>
