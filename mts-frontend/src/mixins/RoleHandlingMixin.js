export default {
  methods: {
    roleIdToText: function(user) {
      switch (user.role) {
        case 0:
          return this.$t('users.roles.admin');
        case 1:
          return this.$t('users.roles.assistant');
        case 2:
          return this.$t('users.roles.user');
        default:
          return 'error';
      }
    },
  },
};
