<template>
  <div id="edit-storage-site">
    <!-- Page Title -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-title">{{$t('storage.site')}}</div>
        <div class="right">
          <router-link to="/private/locations">
            <el-button type="warning" size="mini" icon="el-icon-arrow-left">{{$t('general.back')}}</el-button>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Master Data Form -->
    <div class="content-section">
      <el-form
        :model="editStorageSiteForm"
        :rules="validationRules"
        ref="editStorageSiteForm"
        label-position="left"
        label-width="120px"
        size="mini"
      >
        <div class="content-row content-subtitle">{{$t('general.masterData')}}</div>
        <Alert
          type="success"
          :description="feedback.masterData"
          :show="feedback.masterData !== null"
        />
        <div class="content-row">
          <el-form-item prop="id" :label="$t('general.id')">
            <el-input :placeholder="$t('general.id')" v-model="id" disabled></el-input>
          </el-form-item>
          <el-form-item prop="siteName" :label="$t('general.name')">
            <el-input :placeholder="$t('general.name')" v-model="editStorageSiteForm.siteName"></el-input>
          </el-form-item>
        </div>
        <div class="content-row-nopad">
          <el-button
            class="right"
            type="primary"
            icon="el-icon-check"
            size="mini"
            @click="editStorageSite"
          >{{$t('general.save')}}</el-button>
        </div>
      </el-form>
    </div>

    <!-- Storage Areas Form -->
    <div class="content-section">
      <div class="content-row">
        <div class="left content-subtitle">{{$t('storage.areas')}}</div>
        <div class="right">
          <el-button
            icon="el-icon-plus"
            type="primary"
            size="mini"
            @click="createStorageArea"
          >{{$t('storage.createArea')}}</el-button>
        </div>
      </div>
      <Alert
        type="success"
        :description="feedback.storageAreas"
        :show="feedback.storageAreas !== null"
      />
      <div class="content-row">
        <el-table
          :data="editStorageSiteForm.siteAreas"
          stripe
          border
          size="mini"
          :empty-text="$t('general.noData')"
          @current-change="selectStorageArea"
          highlight-current-row
          ref="areaTable"
          row-key="id"
        >
          <el-table-column prop="id" :label="$t('general.id')" width="300"></el-table-column>
          <el-table-column prop="name" :label="$t('general.name')"></el-table-column>
        </el-table>
        <div class="content-row">
          <div class="right">
            <el-button
              icon="el-icon-edit"
              type="info"
              size="mini"
              :disabled="selectedArea.id === null"
              @click="editStorageArea"
            >{{$t('general.edit')}}</el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from '../../../Api.js';
import GenericErrorHandlingMixin from '@/mixins/GenericErrorHandlingMixin.js';
import Alert from '@/components/Alert.vue';

export default {
  name: 'EditStorageSite',
  mixins: [GenericErrorHandlingMixin],
  components: { Alert },
  props: ['id', 'name', 'areas'],
  data() {
    return {
      editStorageSiteForm: {
        siteName: this.name,
        siteAreas: this.areas,
      },
      selectedArea: {
        id: null,
        name: null,
      },
      feedback: {
        masterData: null,
        storageAreas: null,
      },
    };
  },
  computed: {
    validationRules() {
      return {
        siteId: { required: true, message: '' },
        siteName: { required: true, message: this.$t('storage.validation.name'), trigger: 'blur' },
      };
    },
  },
  methods: {
    editStorageSite: function() {
      this.error = null;
      this.$refs['editStorageSiteForm'].validate(valid => {
        if (valid) {
          Api.updateStorageSite(this.id, this.editStorageSiteForm.siteName)
            .then(response => {
              this.feedback.masterData = this.$t('storage.siteUpdated', response.body);
            })
            .catch(error => {
              this.handleHttpError(error);
            });
        }
      });
    },
    selectStorageArea: function(area) {
      this.selectedArea = {
        id: area.id,
        name: area.name,
      };
    },
    createStorageArea: function() {
      this.$prompt(this.$t('storage.createAreaPrompt'), this.$t('storage.createArea'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('storage.createAreaInputError'),
      })
        .then(({ value }) => {
          Api.createStorageArea(this.id, value)
            .then(result => {
              this.editStorageSiteForm.siteAreas.push(result.body);
              this.feedback.storageAreas = this.$t('storage.areaCreated', result.body);
            })
            .catch(this.handleHttpError);
        })
        .catch(() => {});
    },
    editStorageArea: function() {
      this.$prompt(this.$t('storage.editAreaPrompt'), this.$t('storage.editArea'), {
        confirmButtonText: this.$t('general.save'),
        cancelButtonText: this.$t('general.cancel'),
        inputPattern: /^(?!\s*$).+/,
        inputErrorMessage: this.$t('storage.editAreaInputError'),
      })
        .then(({ value }) => {
          Api.updateStorageArea(this.id, this.selectedArea.id, value)
            .then(result => {
              this.feedback.storageAreas = this.$t('storage.areaUpdated', result.body);
              for (let i = 0; i < this.editStorageSiteForm.siteAreas.length; i++) {
                if (this.editStorageSiteForm.siteAreas[i].id === this.selectedArea.id) {
                  this.editStorageSiteForm.siteAreas[i].name = result.body.name;
                  this.selectedArea.name = result.body.name;
                  break;
                }
              }
            })
            .catch(this.handleHttpError);
        })
        .catch(() => {});
    },
  },
};
</script>