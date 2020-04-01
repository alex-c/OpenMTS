<template>
  <div id="create-material-prop">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{ $t('config.createBatchProp') }}</div>
        <div class="right">
          <router-link to="/private/config">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{ $t('general.back') }}</el-button>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Create Prop Form -->
    <div class="content-section">
      <el-form :model="createPropForm" :rules="validationRules" ref="createPropForm" label-position="left" label-width="120px" size="mini">
        <div class="content-row">
          <el-form-item prop="name" :label="$t('general.name')">
            <el-input v-model="createPropForm.name" :placeholder="$t('general.name')"></el-input>
          </el-form-item>
        </div>
        <div class="content-row-nopad">
          <div class="right">
            <el-button type="primary" size="mini" @click="create" icon="el-icon-check">{{ $t('general.save') }}</el-button>
          </div>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'CreateBatchProp',
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      createPropForm: {
        name: '',
      },
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('config.validation.propName'), trigger: 'blur' },
      };
    },
  },
  methods: {
    create: function() {
      this.$refs['createPropForm'].validate(valid => {
        if (valid) {
          Api.createCustomBatchProp(this.createPropForm.name)
            .then(result => {
              this.$router.push({ name: 'configuration', params: { feedbackBatchProps: this.$t('config.batchPropCreated', { ...result.body }) } });
            })
            .catch(error => this.handleHttpError(error));
        }
      });
    },
  },
};
</script>
