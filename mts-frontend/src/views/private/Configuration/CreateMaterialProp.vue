<template>
  <div id="create-material-prop" class="page-small">
    <!-- Header -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{ $t('config.createMaterialProp') }}</div>
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
          <el-form-item prop="type" :label="$t('general.type')">
            <el-select v-model="createPropForm.type" :placeholder="$t('general.type')" :no-data-text="$t('general.noData')">
              <el-option value="0" :label="$t('types.text')" />
              <el-option value="1" :label="$t('types.file')" />
            </el-select>
            <div class="right">
              <el-button type="primary" @click="create" icon="el-icon-check">{{ $t('general.save') }}</el-button>
            </div>
          </el-form-item>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';

export default {
  name: 'CreateMaterialProp',
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      createPropForm: {
        name: '',
        type: null,
      },
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('config.validation.propName'), trigger: 'blur' },
        type: { required: true, message: this.$t('config.validation.propType'), trigger: ['change', 'blur'] },
      };
    },
  },
  methods: {
    create: function() {
      this.$refs['createPropForm'].validate(valid => {
        if (valid) {
          Api.createCustomMaterialProp(this.createPropForm.name, this.createPropForm.type)
            .then(result => {
              this.$router.push({ name: 'configuration', params: { feedbackMaterialProps: this.$t('config.materialPropCreated', { ...result.body }) } });
            })
            .catch(error => this.handleHttpError(error));
        }
      });
    },
  },
};
</script>
