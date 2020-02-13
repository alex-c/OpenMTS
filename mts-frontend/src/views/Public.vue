<template>
  <div class="public">
    <Navbar />
    <div id="login-box">
      <div id="login-box-logo">
        <i class="el-icon-user-solid"></i>
      </div>
      <div id="login-box-header">{{ $t('login.header') }}</div>
      <div id="login-box-main">
        <el-form :model="form">
          <el-form-item>
            <el-input :placeholder="$t('login.placeholder.user')" v-model="form.user"></el-input>
          </el-form-item>
          <el-form-item>
            <el-input
              :placeholder="$t('login.placeholder.password')"
              v-model="form.password"
              show-password
            ></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="login">{{ $t('login.button') }}</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div id="login-box-footer">{{ $t('login.notice') }}</div>
    </div>
    <div id="footer">
      <a href="https://www.github.com/alex-c/openmts">OpenMTS</a>
    </div>
  </div>
</template>

<script>
import Navbar from '@/components/Navbar.vue';

export default {
  name: 'public',
  components: { Navbar },
  data() {
    return {
      form: {
        user: '',
        password: '',
      },
    };
  },
  methods: {
    login: function() {
      this.$message.error({ message: 'Login is not implemented yet.', duration: 2000 });
    },
  },
};
</script>

<style lang="scss" scoped>
@import '../theme/colors.scss';

#login-box {
  margin: auto;
  margin-top: 96px;
  max-width: 320px;
  min-width: 240px;
  border: 1px solid gray;
  border-radius: 5px;
  box-shadow: 0px 0px 3px 0px gray;
}

#login-box-header {
  font-size: 20px;
  position: relative;
  top: -16px;
}

#login-box-logo {
  margin: auto;
  width: 64px;
  height: 64px;
  border-radius: 32px;
  background-color: $color-primary;
  border: 1px solid $color-dark-accent;
  font-size: 52px;
  color: white;
  position: relative;
  top: -32px;
}

#login-box-main {
  padding: 0px 16px;
}

#login-box-footer {
  padding: 8px;
  border-radius: 0px 0px 5px 5px;
  background-color: lightgray;
  border-top: 1px solid darkgray;
  color: gray;
  font-size: 12px;
}

#footer {
  padding: 12px;
  font-size: 10px;
  > a:link {
    color: $color-primary;
  }
  > a:hover {
    text-decoration: underline;
  }
  margin-bottom: 80px;
}
</style>
