<template>
  <div id="user-administration">
    <Alert type="success" description="User TEST was created." :show="selected.id !== null" />
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{$t('general.users')}}</div>
        <!--el-input
          placeholder="User name"
          prefix-icon="el-icon-search"
          v-model="input2"
          size="mini"
        ></el-input-->
        <div class="right">
          <router-link to="/private/users/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">{{$t('users.create')}}</el-button>
          </router-link>
        </div>
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
        >
          <el-table-column prop="id" label="ID"></el-table-column>
          <el-table-column prop="name" label="Name"></el-table-column>
          <el-table-column prop="role" :label="$t('users.role')" :formatter="roleIdToText"></el-table-column>
        </el-table>
      </div>
      <div class="content-row">
        <div class="left">
          <el-pagination
            background
            layout="prev, pager, next"
            :total="totalUsers"
            :page-size="query.usersPerPage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button
            icon="el-icon-edit"
            type="info"
            size="mini"
            :disabled="selected.id === null"
            @click="edit"
          >{{$t('users.edit')}}</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';
import RoleHandlingMixin from '@/mixins/RoleHandlingMixin.js';
import CreateUser from './UserAdministration/CreateUser.vue';
import Alert from '@/components/Alert.vue';

export default {
  name: 'UserAdministration',
  components: { CreateUser, Alert },
  mixins: [RoleHandlingMixin],
  data() {
    return {
      alert: {},
      query: {
        page: 1,
        usersPerPage: 10,
        search: '',
      },
      users: [],
      totalUsers: 0,
      selected: {
        id: null,
        name: null,
        role: null,
      },
    };
  },
  methods: {
    selectUser: function(user) {
      this.selected = {
        id: user.id,
        name: user.name,
        role: user.role,
      };
    },
    edit: function() {
      this.$router.push({ name: 'editUser', params: { id: this.selected.id, name: this.selected.name, role: this.selected.role } });
    },
  },
  mounted() {
    Api.getUsers(this.page, this.usersPerPage, '')
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
};
</script>

<style lang="scss" scoped>
</style>