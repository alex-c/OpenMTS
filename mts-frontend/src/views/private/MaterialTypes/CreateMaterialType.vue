<template>
  <div id="create-material-type">
    <div class="content-section">
      <!-- Header -->
      <div class="content-row">
        <div class="left content-title">{{$t('materialTypes.create')}}</div>
        <div class="right"></div>
      </div>

      <!-- Form -->
      <div class="content-row">
        <el-form
          :model="createMaterialTypeForm"
          :rules="validationRules"
          ref="createMaterialTypeForm"
          label-position="left"
          label-width="140px"
          size="mini"
        >
          <el-form-item prop="id" :label="$t('general.id')">
            <el-input :placeholder="$t('general.id')" v-model="createMaterialTypeForm.id"></el-input>
          </el-form-item>
          <el-form-item prop="name" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="createMaterialTypeForm.name"></el-input>
          </el-form-item>
        </el-form>
      </div>

      <!-- Save Button -->
      <div class="content-row-nopad">
        <div class="right">
          <el-button
            type="primary"
            @click="createMaterialType"
            icon="el-icon-check"
            size="mini"
          >{{$t('general.save')}}</el-button>
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
  name: 'CreateMaterialType',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  data() {
    return {
      createMaterialTypeForm: {
        id: '',
        name: '',
      },
      error: null,
    };
  },
  computed: {
    validationRules() {
      return {
        id: { required: true, message: this.$t('materialTypes.validation.id'), trigger: 'blur' },
        name: { required: true, message: this.$t('materialTypes.validation.name'), trigger: 'blur' },
      };
    },
  },
  methods: {
    createMaterialType: function() {
      this.$refs['createMaterialTypeForm'].validate(valid => {
        if (valid) {
          Api.createMaterialType(this.createMaterialTypeForm.id, this.createMaterialTypeForm.name)
            .then(response => {
              this.$router.push({ name: 'materialTypes', params: { successMessage: this.$t('materialTypes.created', { ...response.body }) } });
            })
            .catch(error => {
              if (error.status == 409) {
                this.error = this.$t('materialTypes.conflict', { id: this.createMaterialTypeForm.id });
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