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
        User Login
      </div>
      <div id="login-box-main">
        <el-form :model="form">
          <el-form-item>
            <el-input placeholder="User name" v-model="form.user"></el-input>
          </el-form-item>
          <el-form-item>
            <el-input placeholder="Password" v-model="form.password" show-password></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="login">Sign in</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div id="login-box-footer">
        Password forgotten? Contact an administrator!
      </div>
    </div>
    <div id="footer">
      <a href="https://www.github.com/alex-c/openmts">OpenMTS</a>
    </div>
    <el-drawer title="Settings" :visible.sync="drawer" direction="rtl">
      <span>Internationalization settings here!</span>
    </el-drawer>
  </div>
</template>

<script>
export default {
  name: 'public',
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
</style>
