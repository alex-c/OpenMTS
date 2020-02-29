<template>
  <div id="configuration">
    <Alert
      type="success"
      :description="feedback.successMessage"
      :show="feedback.successMessage !== undefined"
    />
    <div class="content-section">
      <div class="content-row content-title">{{$t('general.configuration')}}</div>
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
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'configuration',
  components: { Alert },
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      feedback: {
        successMessage: undefined,
      },
      config: {
        allowGuestLogin: false,
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
              this.feedback.successMessage = allowGuestLogin ? this.$t('config.guestLoginAllowed') : this.$t('config.guestLoginDisallowed');
            })
            .catch(error => {
              this.config.allowGuestLogin = !allowGuestLogin;
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
  },
  mounted() {
    this.getConfiguration();
  },
};
</script>