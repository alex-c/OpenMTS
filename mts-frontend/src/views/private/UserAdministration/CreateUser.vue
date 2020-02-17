<template>
  <div class="content-section">
    <div class="content-row content-title">Create User</div>
    <el-form :model="createUserForm" :rules="validationRules" ref="loginForm">
      <div class="content-row">
        <el-form-item prop="id">
          <el-input :placeholder="$t('users.id')" v-model="createUserForm.id" autofocus></el-input>
        </el-form-item>
        <el-form-item prop="name">
          <el-input :placeholder="$t('users.name')" v-model="createUserForm.name"></el-input>
        </el-form-item>
      </div>
      <div class="content-row">
        Administrator:
        <el-switch v-model="createUserForm.isAdmin"></el-switch>
        <el-form-item class="right">
          <el-button type="primary" @click="create">Create</el-button>
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
  name: 'CreateUser',
  data() {
    return {
      createUserForm: {
        id: '',
        name: '',
        isAdmin: false,
      },
    };
  },
};
</script>