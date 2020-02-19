<template>
  <div>
    <div class="content-section">
      <Alert type="success" :description="$t('account.passwordChanged')" :show="passwordChanged" />
      <div class="content-row content-title">{{$t('general.account')}}</div>
    </div>
    <div class="content-section">
      <el-form
        :model="changePasswordForm"
        :rules="validationRules"
        ref="changePasswordForm"
        label-position="left"
        label-width="150px"
        size="mini"
      >
        <div class="content-row content-subtitle">{{$t('account.changePassword')}}</div>
        <div class="content-row">
          <el-form-item prop="oldPassword" :label="$t('account.oldPassword')">
            <el-input
              type="password"
              :placeholder="$t('account.oldPassword')"
              v-model="changePasswordForm.oldPassword"
            ></el-input>
          </el-form-item>
          <el-form-item prop="newPassword" :label="$t('account.newPassword')">
            <el-input
              type="password"
              :placeholder="$t('account.newPassword')"
              v-model="changePasswordForm.newPassword"
            ></el-input>
          </el-form-item>
          <el-form-item prop="newPasswordRepeat" :label="$t('general.repeat')">
            <el-input
              type="password"
              :placeholder="$t('account.newPassword')"
              v-model="changePasswordForm.newPasswordRepeat"
            ></el-input>
          </el-form-item>
          <el-form-item>
            <div class="right">
              <el-button
                type="primary"
                @click="changePassword"
                icon="el-icon-check"
              >{{$t('general.save')}}</el-button>
            </div>
          </el-form-item>
        </div>
        <Alert type="error" :description="$t(error)" :show="error !== null" />
      </el-form>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'Account',
  components: { Alert },
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      error: null,
      passwordChanged: false,
      changePasswordForm: {
        oldPassword: '',
        newPassword: '',
        newPasswordRepeat: '',
      },
    };
  },
  computed: {
    validationRules() {
      return {
        oldPassword: { required: true, message: this.$t('account.validation.oldPassword'), trigger: 'blur' },
        newPassword: { required: true, message: this.$t('account.validation.newPassword'), trigger: 'blur' },
        newPasswordRepeat: { required: true, message: this.$t('account.validation.newPasswordRepeat'), trigger: 'blur' },
      };
    },
  },
  methods: {
    changePassword: function() {
      this.error = null;
      this.$refs['changePasswordForm'].validate(valid => {
        if (valid) {
          Api.changePassword(this.changePasswordForm.oldPassword, this.changePasswordForm.newPassword)
            .then(response => {
              this.passwordChanged = true;
            })
            .catch(error => {
              if (error.status === 400) {
                this.error = 'account.badPassword';
              } else {
                this.handleHttpError(error);
              }
            });
        }
      });
    },
  },
};
</script>