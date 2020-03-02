<template>
  <div id="edit-keys" class="content-section">
    <div class="content-row content-title">{{$t('apiKeys.editTitle', {key: this.id})}}</div>
    <el-form
      :model="editKeyForm"
      :rules="validationRules"
      ref="editKeyForm"
      label-position="left"
      label-width="120px"
      size="mini"
    >
      <div class="content-row">
        <el-form-item prop="name" label="Name">
          <el-input placeholder="Name" v-model="editKeyForm.name"></el-input>
        </el-form-item>
      </div>
      <div class="content-row">
        <el-form-item prop="rights" label="Rights" style="text-align:center;">
          <el-transfer
            style="margin: auto;text-align: left;"
            v-model="selectedRights"
            :data="availableRights"
            :titles="[$t('apiKeys.rightsAvailable'), $t('apiKeys.rightsSelected')]"
            filterable
            :filter-placeholder="$t('apiKeys.filterPlaceholder')"
            empty-text="TEST"
          ></el-transfer>
        </el-form-item>
      </div>
      <div class="content-row">
        <el-form-item class="right">
          <router-link to="/private/keys">
            <el-button type="warning" plain icon="el-icon-arrow-left">{{$t('general.cancel')}}</el-button>
          </router-link>
          <el-button type="primary" @click="edit" icon="el-icon-check">{{$t('general.save')}}</el-button>
        </el-form-item>
      </div>
      <Alert type="error" :description="$t(error)" :show="error !== null" />
    </el-form>
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
.el-transfer-panel__empty {
  display: none;
}
</style>