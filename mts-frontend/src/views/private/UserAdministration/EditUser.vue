<template>
  <div class="content-section">
    <div class="content-row content-title">{{$t('users.edit') + " " + id}}</div>
    <el-form
      :model="editUserForm"
      :rules="validationRules"
      ref="editUserForm"
      label-position="left"
      label-width="120px"
      size="mini"
    >
      <div class="content-row">
        <el-form-item prop="name" label="Name">
          <el-input placeholder="Name" v-model="editUserForm.userName"></el-input>
        </el-form-item>
        <el-form-item prop="role" :label="$t('users.role')">
          <el-select v-model="editUserForm.userRole" :placeholder="$t('users.role')">
            <el-option value="0" :label="$t('users.roles.admin')" />
            <el-option value="1" :label="$t('users.roles.assistant')" />
            <el-option value="2" :label="$t('users.roles.user')" />
          </el-select>
          <div class="right">
            <router-link to="/private/users">
              <el-button type="warning" plain icon="el-icon-arrow-left">{{$t('general.cancel')}}</el-button>
            </router-link>
            <el-button type="primary" @click="edit" icon="el-icon-check">{{$t('general.save')}}</el-button>
          </div>
        </el-form-item>
      </div>
    </el-form>
  </div>
</template>

<script>
export default {
  name: 'EditUser',
  props: ['id', 'name', 'role'],
  data() {
    return {
      editUserForm: {
        userName: name,
        userRole: null,
      },
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
    edit: function() {},
  },
};
</script>