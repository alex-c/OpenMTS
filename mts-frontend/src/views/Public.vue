<template>
  <div class="public">
    <div id="navbar">
      <div id="navbar-left">OpenMTS</div>
      <div id="navbar-right">
        <el-button icon="el-icon-s-tools" size="mini" circle @click="drawer = true"></el-button>
      </div>
    </div>
    <div id="login-box">
      <div id="login-box-logo">
        <i class="el-icon-user-solid"></i>
      </div>
      <div id="login-box-header">
        {{ $t('login.header') }}
      </div>
      <div id="login-box-main">
        <el-form :model="form">
          <el-form-item>
            <el-input :placeholder="$t('login.placeholder.user')" v-model="form.user"></el-input>
          </el-form-item>
          <el-form-item>
            <el-input :placeholder="$t('login.placeholder.password')" v-model="form.password" show-password></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="login">{{ $t('login.button') }}</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div id="login-box-footer">
        {{ $t('login.notice') }}
      </div>
    </div>
    <div id="footer">
      <a href="https://www.github.com/alex-c/openmts">OpenMTS</a>
    </div>
    <Settings :drawer.sync="drawer" />
  </div>
</template>

<script>
import Settings from '@/components/Settings.vue';

export default {
  name: 'public',
  components: { Settings },
  data() {
    return {
      drawer: false,
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

#navbar {
  padding: 16px;
  background-color: $color-primary;
  color: white;
  overflow: auto;
}

#navbar-left {
  float: left;
  font-size: 24px;
}

#navbar-right {
  float: right;
}

#login-box {
  margin: auto;
  margin-top: 96px;
  max-width: 320px;
  min-width: 240px;
  border: 1px solid gray;
  border-radius: 5px;
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
}

#drawer-body {
  padding: 16px;
  text-align: left;
}

#drawer-body .el-select {
  width: 100%;
}
</style>
