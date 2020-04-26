<template>
  <div class="public">
    <Navbar />
    <div id="login-box">
      <div id="login-box-logo">
        <i class="el-icon-user-solid"></i>
      </div>
      <div id="login-box-header">{{ $t('login.header') }}</div>
      <div id="login-box-main">
        <el-form :model="loginForm" :rules="validationRules" ref="loginForm">
          <el-form-item prop="user">
            <el-input :placeholder="$t('login.placeholder.user')" v-model="loginForm.user" prefix-icon="el-icon-user-solid" @keyup.enter.native="login" autofocus></el-input>
          </el-form-item>
          <el-form-item prop="password">
            <el-input :placeholder="$t('login.placeholder.password')" v-model="loginForm.password" show-password prefix-icon="el-icon-lock" @keyup.enter.native="login"></el-input>
          </el-form-item>
          <transition name="el-zoom-in-top">
            <el-alert id="bad-login-error" :closable="false" :title="$t('login.fail.title')" type="error" v-show="badLogin" show-icon>{{ $t('login.fail.description') }}</el-alert>
          </transition>
          <el-form-item>
            <el-button type="default" @click="guestLogin" v-if="guestLoginAllowed">{{ $t('login.guestLogin') }}</el-button>
            <el-button type="primary" @click="login">{{ $t('login.button') }}</el-button>
          </el-form-item>
        </el-form>
      </div>
      <div id="login-box-footer">{{ $t('login.notice') }}</div>
    </div>
    <div id="footer">
      <a href="https://www.github.com/alex-c/openmts">OpenMTS on Github</a>
    </div>
  </div>
</template>

<script>
import Api from '../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Navbar from '@/components/Navbar.vue';

export default {
  name: 'public',
  components: { Navbar },
  mixins: [GenericErrorHandlingMixin],
  data() {
    return {
      guestLoginAllowed: false,
      loginForm: {
        user: '',
        password: '',
      },
      badLogin: false,
    };
  },
  computed: {
    validationRules() {
      return {
        user: { required: true, message: this.$t('login.validation.user'), trigger: 'blur' },
        password: { required: true, message: this.$t('login.validation.password'), trigger: 'blur' },
      };
    },
  },
  methods: {
    login: function() {
      this.badLogin = false;
      this.$refs['loginForm'].validate(valid => {
        if (valid) {
          Api.login(this.loginForm.user, this.loginForm.password)
            .then(response => {
              const token = response.body.token;
              this.$store.commit('login', token);
              this.$router.push('/private');
            })
            .catch(error => {
              if (error.status === 401) {
                this.badLogin = true;
              } else {
                this.handleHttpError(error);
              }
            });
        } else {
          return false;
        }
      });
    },
    guestLogin: function() {
      Api.guestLogin()
        .then(response => {
          const token = response.body.token;
          this.$store.commit('login', token);
          this.$router.push('/private');
        })
        .catch(error => {
          this.handleHttpError(error);
        });
    },
  },
  mounted() {
    Api.getConfiguration()
      .then(response => {
        if (response.body.allowGuestLogin === true) {
          this.guestLoginAllowed = true;
        }
      })
      .catch(error => {
        this.handleHttpError(error);
      });
  },
};
</script>

<style lang="scss" scoped>
@import '../theme/colors.scss';

#login-box {
  margin: auto;
  margin-top: 140px;
  max-width: 360px;
  min-width: 230px;
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

#bad-login-error {
  text-align: left;
  margin-bottom: 20px;
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
