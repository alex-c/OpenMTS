<template>
  <div id="create-plastic" class="page-small">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{ $t('plastics.create') }}</div>
        <div class="right">
          <router-link to="/private/plastics">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>

      <!-- Form -->
      <div class="content-row">
        <el-form :model="createPlasticForm" :rules="validationRules" ref="createPlasticForm" label-position="left" label-width="140px" size="mini">
          <el-form-item prop="id" :label="$t('general.id')">
            <el-input :placeholder="$t('general.id')" v-model="createPlasticForm.id"></el-input>
          </el-form-item>
          <el-form-item prop="name" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="createPlasticForm.name"></el-input>
          </el-form-item>
        </el-form>
      </div>

      <!-- Save Button -->
      <div class="content-row-nopad">
        <div class="right">
          <el-button type="primary" @click="createPlastic" icon="el-icon-check" size="mini">{{ $t('general.save') }}</el-button>
        </div>
      </div>

      <!-- Error -->
      <Alert type="error" :description="$t(error)" :show="error !== null" />
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'CreatePlastic',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  data() {
    return {
      createPlasticForm: {
        id: '',
        name: '',
      },
      error: null,
    };
  },
  computed: {
    validationRules() {
      return {
        id: { required: true, message: this.$t('plastics.validation.id'), trigger: 'blur' },
        name: { required: true, message: this.$t('plastics.validation.name'), trigger: 'blur' },
      };
    },
  },
  methods: {
    createPlastic: function() {
      this.$refs['createPlasticForm'].validate(valid => {
        if (valid) {
          Api.createPlastic(this.createPlasticForm.id, this.createPlasticForm.name)
            .then(response => {
              this.$router.push({ name: 'plastics', params: { successMessage: this.$t('plastics.created', { ...response.body }) } });
            })
            .catch(error => {
              if (error.status == 409) {
                this.error = this.$t('plastics.conflict', { id: this.createPlasticForm.id });
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
