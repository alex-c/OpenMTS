<template>
  <div class="content-section">
    <div class="content-row content-title">Edit User {{id}}</div>
    <el-form :model="editUserForm" :rules="validationRules" ref="loginForm">
      <div class="content-row">
        <el-form-item prop="name">
          <el-input :placeholder="$t('users.name')" v-model="editUserForm.name"></el-input>
        </el-form-item>
      </div>
      <div class="content-row">
        Administrator:
        <el-switch v-model="editUserForm.isAdmin"></el-switch>
        <el-form-item class="right">
          <el-button type="primary" @click="save">Save</el-button>
        </el-form-item>
      </div>
      <div class="content-row">
        <transition name="el-zoom-in-top">
          <el-alert
            id="bad-login-error"
            :closable="false"
            :title="$t('login.fail.title')"
            type="error"
            v-show="badLogin"
            show-icon
          >{{ $t('login.fail.description') }}</el-alert>
        </transition>
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
        userIsAdmin: false,
      },
    };
  },
  methods: {
    save: function() {},
  },
};
</script>