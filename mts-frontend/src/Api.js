function processResponse(response) {
  return new Promise((resolve, reject) => {
    if (response.status === 204) {
      resolve({ status: response.status });
    } else if (response.status === 401 || response.status === 403) {
      reject({ status: response.status });
    } else {
      let handler;
      response.status < 400 ? (handler = resolve) : (handler = reject);
      response.json().then(json => handler({ status: response.status, body: json }));
    }
  });
}

function processFileDownload(response) {
  return new Promise((resolve, reject) => {
    if (response.status === 200) {
      const contentDisposition = response.headers.get('Content-Disposition');
      let offset = contentDisposition.indexOf('filename=');
      let fileName = 'unknown';
      if (offset != -1) {
        offset += 9;
        fileName = contentDisposition.substring(offset, contentDisposition.indexOf(';', offset));
      }
      response.blob().then(blob => resolve({ blob, fileName }));
    } else {
      reject(response);
    }
  });
}

function catchNetworkError(response) {
  return new Promise((_, reject) => {
    reject({ status: null, message: 'general.networkError' });
  });
}

function getAuthorizationHeader() {
  return 'Bearer ' + localStorage.getItem('token');
}

const SERVER_ENDPOINT = process.env.VUE_APP_SERVER_ENDPOINT;

export default {
  getRights: () => {
    return fetch(`${SERVER_ENDPOINT}/api/auth/rights`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  login: (id, password) => {
    return fetch(`${SERVER_ENDPOINT}/api/auth?method=userLogin`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ id, password }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  guestLogin: () => {
    return fetch(`${SERVER_ENDPOINT}/api/auth?method=guestLogin`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: '{}',
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  changePassword: (oldPassword, newPassword) => {
    return fetch(`${SERVER_ENDPOINT}/api/self/password`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ old: oldPassword, new: newPassword }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getBatches: (page, elementsPerPage, materialId, siteId) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory?page=${page}&elementsPerPage=${elementsPerPage}&materialId=${materialId}&siteId=${siteId}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getBatch: id => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${id}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createBatch: (materialId, expirationDate, storageSiteId, storageAreaId, batchNumber, quantity, customProps, isLocked) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ materialId, expirationDate, storageSiteId, storageAreaId, batchNumber, quantity, customProps, isLocked }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateBatch: (batchId, materialId, expirationDate, storageSiteId, storageAreaId, batchNumber, customProps) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${batchId}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ materialId, expirationDate, storageSiteId, storageAreaId, batchNumber, customProps }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateBatchStatus: (batchId, isLocked) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${batchId}/status`, {
      method: 'PUT',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ isLocked }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getTransactionLog: (page, elementsPerPage, batchId) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${batchId}/log?page=${page}&elementsPerPage=${elementsPerPage}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getLastTransaction: batchId => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${batchId}/last-entry`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  amendLastTransaction: (batchId, transactionId, quantity) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${batchId}/log/${transactionId}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ quantity }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  checkOut: function(batchId, quantity) {
    return this.performMaterialTransaction(batchId, quantity, true);
  },
  checkIn: function(batchId, quantity) {
    return this.performMaterialTransaction(batchId, quantity, false);
  },
  performMaterialTransaction: (batchId, quantity, isCheckout) => {
    return fetch(`${SERVER_ENDPOINT}/api/inventory/${batchId}/log`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ quantity, isCheckout }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getAllPlastics: function() {
    return this.getPlastics(0, 0, '', true);
  },
  getPlastics: (page, elementsPerPage, search, getAll = false) => {
    return fetch(`${SERVER_ENDPOINT}/api/plastics?getAll=${getAll}&page=${page}&elementsPerPage=${elementsPerPage}&search=${search}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createPlastic: (id, name) => {
    return fetch(`${SERVER_ENDPOINT}/api/plastics`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ id, name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updatePlastic: (id, name) => {
    return fetch(`${SERVER_ENDPOINT}/api/plastics/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getManufacturers: () => {
    return fetch(`${SERVER_ENDPOINT}/api/manufacturers`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getAllMaterials: function() {
    return this.getMaterials(0, 0, '', '', '', true);
  },
  getMaterials: (page, elementsPerPage, search, manufacturer, type, getAll = false) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials?getAll=${getAll}&page=${page}&elementsPerPage=${elementsPerPage}&search=${search}&manufacturer=${manufacturer}&type=${type}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getMaterial: id => {
    return fetch(`${SERVER_ENDPOINT}/api/materials/${id}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createMaterial: (name, manufacturer, manufacturerId, type) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name, manufacturer, manufacturerId, type }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateMaterial: (id, name, manufacturer, manufacturerId, type) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name, manufacturer, manufacturerId, type }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  setMaterialCustomTextProp: (materialId, propId, text) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials/${materialId}/text-props/${propId}`, {
      method: 'PUT',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ materialId, propId, text }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  deleteMaterialCustomTextProp: (materialId, propId) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials/${materialId}/text-props/${propId}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  setMaterialCustomFileProp: (materialId, propId, file) => {
    const formData = new FormData();
    formData.append('file', file);
    return fetch(`${SERVER_ENDPOINT}/api/materials/${materialId}/file-props/${propId}`, {
      method: 'PUT',
      withCredentials: true,
      credentials: 'include',
      headers: { Authorization: getAuthorizationHeader() },
      body: formData,
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  deleteMaterialCustomFileProp: (materialId, propId) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials/${materialId}/file-props/${propId}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  downloadFile: (materialId, propId) => {
    return fetch(`${SERVER_ENDPOINT}/api/materials/${materialId}/file-props/${propId}/download`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { Authorization: getAuthorizationHeader(), 'Access-Control-Request-Headers': 'Content-Disposition' },
    })
      .catch(catchNetworkError)
      .then(processFileDownload);
  },
  trace: transactionId => {
    return fetch(`${SERVER_ENDPOINT}/api/trace/${transactionId}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getConfiguration: () => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  setConfiguration: allowGuestLogin => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ allowGuestLogin }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getCustomMaterialProps: () => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/material-props`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createCustomMaterialProp: (name, type) => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/material-props/`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name, type }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateCustomMaterialProp: (id, name) => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/material-props/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  deleteCustomMaterialProp: id => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/material-props/${id}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getCustomBatchProps: () => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/batch-props`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createCustomBatchProp: name => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/batch-props/`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateCustomBatchProp: (id, name) => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/batch-props/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  deleteCustomBatchProp: id => {
    return fetch(`${SERVER_ENDPOINT}/api/configuration/batch-props/${id}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getUsers: (page, elementsPerPage, search, showDisabled) => {
    return fetch(`${SERVER_ENDPOINT}/api/users?page=${page}&elementsPerPage=${elementsPerPage}&search=${search}&showDisabled=${showDisabled}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createUser: (id, name, password, role) => {
    return fetch(`${SERVER_ENDPOINT}/api/users`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ id, name, password, role }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateUser: (id, name, role) => {
    return fetch(`${SERVER_ENDPOINT}/api/users/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ id, name, role }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateUserStatus: (id, disabled) => {
    return fetch(`${SERVER_ENDPOINT}/api/users/${id}/status`, {
      method: 'PUT',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ disabled }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getApiKeys: (page, elementsPerPage) => {
    return fetch(`${SERVER_ENDPOINT}/api/keys?page=${page}&elementsPerPage=${elementsPerPage}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createApiKey: name => {
    return fetch(`${SERVER_ENDPOINT}/api/keys`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateApiKey: (id, name, rights) => {
    return fetch(`${SERVER_ENDPOINT}/api/keys/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name, rights }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateApiKeyStatus: (id, enabled) => {
    return fetch(`${SERVER_ENDPOINT}/api/keys/${id}/status`, {
      method: 'PUT',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ enabled }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  deleteApiKey: id => {
    return fetch(`${SERVER_ENDPOINT}/api/keys/${id}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getAllStorageSites: function() {
    return this.getStorageSites(0, 0, '', true);
  },
  getStorageSites: (page, elementsPerPage, search, getAll = false) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites?getAll=${getAll}&page=${page}&elementsPerPage=${elementsPerPage}&search=${search}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getStorageSite: id => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${id}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getStorageSiteLatestEnvironmentValue: (id, factor) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${id}/${factor}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getStorageSiteEnvironmentHistory: (id, factor, startTime, endTime, maxPoints = null) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${id}/${factor}/history?startTime=${startTime}&endTime=${endTime}&maxPoints=${maxPoints}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getStorageSiteEnvironmentExtrema: (id, factor, startTime, endTime) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${id}/${factor}/extrema?startTime=${startTime}&endTime=${endTime}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createStorageSite: name => {
    return fetch(`${SERVER_ENDPOINT}/api/sites`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateStorageSite: (id, name) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createStorageArea: (siteId, areaName) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${siteId}/areas`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name: areaName }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateStorageArea: (siteId, areaId, areaName) => {
    return fetch(`${SERVER_ENDPOINT}/api/sites/${siteId}/areas/${areaId}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name: areaName }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getStatsSitesOverview: () => {
    return fetch(`${SERVER_ENDPOINT}/api/stats/sites/overview`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
};
