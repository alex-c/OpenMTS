<template>
  <div id="user-administration">
    <Alert
      type="success"
      :description="feedback.successMessage"
      :show="feedback.successMessage !== undefined"
    />
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{$t('general.users')}}</div>
        <div class="right">
          {{$t('users.showDisabledSwitchLabel')}}
          <el-switch v-model="query.showDisabledUsers" @change="switchDisabled" />
          <router-link to="/private/users/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">{{$t('users.create')}}</el-button>
          </router-link>
        </div>
      </div>
      <div class="content-row" id="search-bar">
        <el-input
          :placeholder="$t('users.filter')"
          prefix-icon="el-icon-search"
          v-model="search"
          size="mini"
          clearable
          @change="setSearch"
        ></el-input>
      </div>
      <div class="content-row">
        <el-table
          :data="users"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
          @current-change="selectUser"
          ref="userTable"
          row-key="id"
        >
          <el-table-column prop="id" :label="$t('general.id')"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
          <el-table-column prop="role" :label="$t('users.role')" :formatter="roleIdToText"></el-table-column>
          <el-table-column
            prop="disabled"
            :label="$t('general.status.label')"
            :formatter="disabledText"
            v-if="query.showDisabledUsers"
          ></el-table-column>
        </el-table>
      </div>
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalUsers"
            :page-size="query.usersPerPage"
            :current-page.sync="query.page"
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button
            icon="el-icon-unlock"
            type="success"
            size="mini"
            v-if="this.selected.disabled === true"
            @click="enable"
          >{{$t('general.enable')}}</el-button>
          <el-button
            icon="el-icon-lock"
            type="warning"
            size="mini"
            v-if="this.selected.disabled === false"
            @click="disable"
          >{{$t('general.disable')}}</el-button>
          <el-button
            icon="el-icon-edit"
            type="info"
            size="mini"
            :disabled="selected.id === null"
            @click="edit"
          >{{$t('general.edit')}}</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import RoleHandlingMixin from '@/mixins/RoleHandlingMixin.js';
import CreateUser from './UserAdministration/CreateUser.vue';
import Alert from '@/components/Alert.vue';

export default {
  name: 'UserAdministration',
  components: { CreateUser, Alert },
  mixins: [GenericErrorHandlingMixin, RoleHandlingMixin],
  props: ['successMessage'],
  data() {
    return {
      search: '',
      query: {
        page: 1,
        usersPerPage: 10,
        search: '',
        showDisabledUsers: false,
      },
      users: [],
      totalUsers: 0,
      selected: {
        id: null,
        name: null,
        role: null,
        disabled: null,
      },
      feedback: {
        successMessage: this.successMessage,
      },
    };
  },
  methods: {
    getUsers: function() {
      this.resetSelectedUser();
      Api.getUsers(this.query.page, this.query.usersPerPage, this.query.search, this.query.showDisabledUsers)
        .then(response => {
          this.users = response.body.data;
          this.totalUsers = response.body.totalElements;
        })
        .catch(error => {
          this.$message({
            message: this.$t(error.message),
            type: 'error',
            showClose: true,
          });
        });
    },
    changePage: function(page) {
      this.query.page = page;
      this.getUsers();
    },
    setSearch: function(value) {
      this.query.search = value;
      this.query.page = 1;
      this.getUsers();
    },
    switchDisabled: function(value) {
      this.query.page = 1;
      this.getUsers();
    },
    selectUser: function(user) {
      this.selected = {
        id: user.id,
        name: user.name,
        role: user.role,
        disabled: user.disabled,
      };
    },
    resetSelectedUser: function() {
      this.$refs['userTable'].setCurrentRow(1);
      this.selected.id = null;
      this.selected.name = null;
      this.selected.role = null;
      this.selected.disabled = null;
    },
    edit: function() {
      const params = { id: this.selected.id, name: this.selected.name, role: this.selected.role };
      this.$router.push({ name: 'editUser', params });
    },
    disable: function() {
      this.$confirm(this.$t('users.disableConfirm', { id: this.selected.id }), {
        confirmButtonText: this.$t('users.disable'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          this.query.page = 1;
          Api.updateUserStatus(this.selected.id, true)
            .then(response => {
              this.feedback.successMessage = this.$t('users.disabled', { id: this.selected.id });
              this.getUsers();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    enable: function() {
      this.$confirm(this.$t('users.enableConfirm', { id: this.selected.id }), {
        confirmButtonText: this.$t('users.enable'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          this.query.page = 1;
          Api.updateUserStatus(this.selected.id, false)
            .then(response => {
              this.feedback.successMessage = this.$t('users.enabled', { id: this.selected.id });
              this.getUsers();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    disabledText: function(value) {
      if (value.disabled) {
        return this.$t('general.status.disabled');
      } else {
        return this.$t('general.status.enabled');
      }
    },
  },
  mounted() {
    this.getUsers();
  },
};
</script>

<style lang="scss" scoped>
#search-bar {
  overflow: hidden;
}
</style>