<template>
  <div id="user-administration">
    <div id="breadcrumb">
      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/' }">OpenMTS</el-breadcrumb-item>
        <el-breadcrumb-item>User Administration</el-breadcrumb-item>
      </el-breadcrumb>
    </div>
    <div id="users-view">
      <div id="users-view-header">
        Users
        <el-button icon="el-icon-plus" type="primary" size="mini">New User</el-button>
      </div>
      <div id="users-view-table">
        <el-table
          :data="users"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          highlight-current-row
        >
          <el-table-column prop="id" label="ID"></el-table-column>
          <el-table-column prop="name" label="Name"></el-table-column>
          <el-table-column prop="role" label="Role"></el-table-column>
        </el-table>
      </div>
      <div id="users-view-footer">
        <el-button icon="el-icon-edit" type="info" plain size="mini">Edit</el-button>
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
      users: [],
    };
  },
  mounted() {
    Api.getUsers(1, 10, '')
      .then(response => {
        this.users = response.data;
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
#breadcrumb {
  padding: 16px;
}

#users-view {
  padding: 0px 16px;
}

#users-view-header {
  margin: 8px 0px;
  font-size: 16px;
  overflow: auto;
  button {
    float: right;
  }
}

#users-view-footer {
  margin: 8px 0px;
  overflow: auto;
  button {
    float: right;
  }
}
</style>