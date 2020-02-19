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
          Show archived users:
          <el-switch v-model="query.showArchivedUsers" @change="switchArchived" />
          <el-button icon="el-icon-refresh" type="primary" size="mini" @click="refresh"></el-button>
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
          <el-table-column prop="id" label="ID"></el-table-column>
          <el-table-column prop="name" label="Name"></el-table-column>
          <el-table-column prop="role" :label="$t('users.role')" :formatter="roleIdToText"></el-table-column>
          <el-table-column
            prop="isArchived"
            :label="$t('users.status.label')"
            :formatter="archivedText"
            v-if="query.showArchivedUsers"
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
            @current-change="changePage"
          ></el-pagination>
        </div>
        <div class="right">
          <el-button
            icon="el-icon-s-check"
            type="success"
            size="mini"
            v-if="this.selected.archived === true"
            @click="restore"
          >{{$t('users.restore')}}</el-button>
          <el-button
            icon="el-icon-takeaway-box"
            type="warning"
            size="mini"
            v-if="this.selected.archived === false"
            @click="archive"
          >{{$t('users.archive')}}</el-button>
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
  props: ['successMessage'],
  data() {
    return {
      search: '',
      query: {
        page: 1,
        usersPerPage: 10,
        search: '',
        showArchivedUsers: false,
      },
      users: [],
      totalUsers: 0,
      selected: {
        id: null,
        name: null,
        role: null,
        archived: null,
      },
      feedback: {
        successMessage: this.successMessage,
      },
    };
  },
  methods: {
    getUsers: function() {
      this.resetSelectedUser();
      Api.getUsers(this.query.page, this.query.usersPerPage, this.query.search, this.query.showArchivedUsers)
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
    switchArchived: function(value) {
      this.getUsers();
    },
    selectUser: function(user) {
      this.selected = {
        id: user.id,
        name: user.name,
        role: user.role,
        archived: user.isArchived,
      };
    },
    resetSelectedUser: function() {
      this.$refs['userTable'].setCurrentRow(1);
      this.selected.id = null;
      this.selected.name = null;
      this.selected.role = null;
      this.selected.archived = null;
    },
    edit: function() {
      const params = { id: this.selected.id, name: this.selected.name, role: this.selected.role };
      this.$router.push({ name: 'editUser', params });
    },
    archive: function() {
      this.$confirm(this.$t('users.archiveConfirm', { id: this.selected.id }), {
        confirmButtonText: this.$t('users.archive'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'warning',
      })
        .then(() => {
          Api.updateUserStatus(this.selected.id, true)
            .then(response => {
              this.feedback.successMessage = this.$t('users.archived', { id: this.selected.id });
              this.getUsers();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    restore: function() {
      this.$confirm(this.$t('users.restoreConfirm', { id: this.selected.id }), {
        confirmButtonText: this.$t('users.restore'),
        cancelButtonText: this.$t('general.cancel'),
        type: 'success',
      })
        .then(() => {
          Api.updateUserStatus(this.selected.id, false)
            .then(response => {
              this.feedback.successMessage = this.$t('users.restored', { id: this.selected.id });
              this.getUsers();
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        })
        .catch(() => {});
    },
    archivedText: function(value) {
      if (value.isArchived) {
        return this.$t('users.status.archived');
      } else {
        return this.$t('users.status.active');
      }
    },
    refresh: function() {
      this.$router.go();
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