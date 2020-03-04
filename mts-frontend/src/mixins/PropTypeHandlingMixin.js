export default {
  methods: {
    propTypeIdToText: function(prop) {
      switch (prop.type) {
        case 0:
        case '0':
          return this.$t('types.text');
        case 1:
        case '1':
          return this.$t('types.file');
        default:
          return 'error';
      }
    },
  },
};
