export default {
  methods: {
    materialTypeIdToText: function(material) {
      switch (material.type) {
        case 0:
        case '0':
          return this.$t('materials.types.pp');
        case 1:
        case '1':
          return this.$t('materials.types.spice');
        default:
          return 'error';
      }
    },
  },
};
