<template>
  <div id="edit-user" class="page-small">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{ $t('users.edit', { id }) }}</div>
        <div class="right">
          <router-link to="/private/users">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Edit User Form -->
    <div class="content-section">
      <el-form :model="editUserForm" :rules="validationRules" ref="editUserForm" label-position="left" label-width="120px" size="mini">
        <div class="content-row">
          <el-form-item prop="userName" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="editUserForm.userName"></el-input>
          </el-form-item>
          <el-form-item prop="userRole" :label="$t('users.role')">
            <el-select v-model="editUserForm.userRole" :placeholder="$t('users.role')">
              <el-option value="0" :label="$t('users.roles.admin')" />
              <el-option value="1" :label="$t('users.roles.assistant')" />
              <el-option value="2" :label="$t('users.roles.user')" />
            </el-select>
            <div class="right">
              <el-button type="primary" @click="edit" icon="el-icon-check">{{ $t('general.save') }}</el-button>
            </div>
          </el-form-item>
        </div>
      </el-form>
      <Alert type="error" :description="$t(error)" :show="error !== null" />
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'EditUser',
  components: { Alert },
  mixins: [GenericErrorHandlingMixin],
  props: ['id', 'name', 'role'],
  data() {
    return {
      editUserForm: {
        userName: this.name,
        userRole: this.role.toString(),
      },
      error: null,
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('users.validation.name'), trigger: 'blur' },
        role: { required: true, message: this.$t('users.validation.role'), trigger: ['change', 'blur'] },
      };
    },
  },
  methods: {
    edit: function() {
      this.error = null;
      this.$refs['editUserForm'].validate(valid => {
        if (valid) {
          Api.updateUser(this.id, this.editUserForm.userName, this.editUserForm.userRole)
            .then(response => {
              this.$router.push({ name: 'users', params: { successMessage: this.$t('users.updated', { id: this.id }) } });
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        }
      });
    },
  },
};
</script>
