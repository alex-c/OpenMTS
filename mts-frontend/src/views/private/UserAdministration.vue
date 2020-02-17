<template>
  <div id="user-administration">
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">Users</div>
        <div class="right">
          <router-link to="/private/users/create">
            <el-button icon="el-icon-plus" type="primary" size="mini">New User</el-button>
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
          <el-table-column prop="role" label="Role"></el-table-column>
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
          >Edit</el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../Api.js';

export default {
  name: 'UserAdministration',
  data() {
    return {
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
      this.selected = user.id;
    },
    edit: function() {
      this.$router.push({ path: '/private/users/edit', params: { id: this.selected.id, name: this.selected.name } });
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