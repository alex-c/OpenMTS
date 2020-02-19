<template>
  <div class="content-section">
    <div class="content-row content-title">{{$t('users.create')}}</div>
    <el-form
      :model="createUserForm"
      :rules="validationRules"
      ref="createUserForm"
      label-position="left"
      label-width="120px"
      size="mini"
    >
      <div class="content-row">
        <el-form-item prop="name" label="Name">
          <el-input placeholder="Name" v-model="createUserForm.name"></el-input>
        </el-form-item>
        <el-form-item prop="id" label="ID">
          <el-input placeholder="ID" v-model="createUserForm.id" autofocus>
            <el-button
              slot="append"
              icon="el-icon-refresh-right"
              class="input-button"
              @click="generateId"
              :title="$t('users.generateId')"
            ></el-button>
          </el-input>
        </el-form-item>
        <el-form-item prop="password" :label="$t('login.placeholder.password')">
          <el-input
            :placeholder="$t('login.placeholder.password')"
            v-model="createUserForm.password"
            autofocus
          >
            <el-button
              slot="append"
              icon="el-icon-refresh-right"
              class="input-button"
              @click="generatePassword"
              :title="$t('users.generatePassword')"
            ></el-button>
          </el-input>
        </el-form-item>
        <el-form-item prop="role" :label="$t('users.role')">
          <el-select v-model="createUserForm.role" :placeholder="$t('users.role')">
            <el-option value="0" :label="$t('users.roles.admin')" />
            <el-option value="1" :label="$t('users.roles.assistant')" />
            <el-option value="2" :label="$t('users.roles.user')" />
          </el-select>
          <div class="right">
            <router-link to="/private/users">
              <el-button type="warning" plain icon="el-icon-arrow-left">{{$t('general.cancel')}}</el-button>
            </router-link>
            <el-button type="primary" @click="create" icon="el-icon-check">{{$t('general.save')}}</el-button>
          </div>
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
  name: 'CreateUser',
  components: { Alert },
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      createUserForm: {
        name: '',
        id: '',
        password: '',
        role: null,
      },
      error: null,
    };
  },
  computed: {
    validationRules() {
      return {
        name: { required: true, message: this.$t('users.validation.name'), trigger: 'blur' },
        id: { required: true, message: this.$t('users.validation.id'), trigger: ['change', 'blur'] },
        password: { required: true, message: this.$t('users.validation.password'), trigger: ['change', 'blur'] },
        role: { required: true, message: this.$t('users.validation.role'), trigger: ['change', 'blur'] },
      };
    },
  },
  methods: {
    generateId: function() {
      if (this.createUserForm.name !== '') {
        this.createUserForm.id = this.createUserForm.name
          .trim()
          .replace(/ /g, '_')
          .toLowerCase();
      }
    },
    generatePassword: function() {
      this.createUserForm.password = Math.random()
        .toString(36)
        .substr(2, 8);
    },
    create: function() {
      this.error = null;
      this.$refs['createUserForm'].validate(valid => {
        if (valid) {
          Api.createUser(this.createUserForm.id, this.createUserForm.name, this.createUserForm.password, this.createUserForm.role)
            .then(response => {
              this.$router.push({ name: 'users', params: { successMessage: this.$t('users.created', { id: this.createUserForm.id }) } });
            })
            .catch(error => {
              if (error.status === 409) {
                this.error = 'users.idTaken';
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

<style lang="scss" scoped>
@import '../../../theme/colors.scss';

.el-input button {
  color: $color-primary;
}
</style>