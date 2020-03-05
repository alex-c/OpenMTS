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

function catchNetworkError(response) {
  return new Promise((_, reject) => {
    reject({ status: null, message: 'general.networkError' });
  });
}

function getAuthorizationHeader() {
  return 'Bearer ' + localStorage.getItem('token');
}

export default {
  getRights: () => {
    return fetch('http://localhost:5000/api/auth/rights', {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  login: (id, password) => {
    return fetch('http://localhost:5000/api/auth?method=userLogin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ id, password }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  guestLogin: () => {
    return fetch('http://localhost:5000/api/auth?method=guestLogin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: '{}',
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  changePassword: (oldPassword, newPassword) => {
    return fetch('http://localhost:5000/api/self/password', {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ old: oldPassword, new: newPassword }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getAllMaterialTypes: function() {
    return this.getMaterialTypes(0, 0, '', true);
  },
  getMaterialTypes: (page, elementsPerPage, search, getAll = false) => {
    return fetch(`http://localhost:5000/api/material-types?getAll=${getAll}&page=${page}&elementsPerPage=${elementsPerPage}&search=${search}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createMaterialType: (id, name) => {
    return fetch(`http://localhost:5000/api/material-types`, {
      method: 'POST',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ id, name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  updateMaterialType: (id, name) => {
    return fetch(`http://localhost:5000/api/material-types/${id}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getMaterials: (page, elementsPerPage, search, manufacturer, type) => {
    return fetch(`http://localhost:5000/api/materials?page=${page}&elementsPerPage=${elementsPerPage}&search=${search}&manufacturer=${manufacturer}&type=${type}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getManufacturers: () => {
    return fetch(`http://localhost:5000/api/manufacturers`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getConfiguration: () => {
    return fetch(`http://localhost:5000/api/configuration`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  setConfiguration: allowGuestLogin => {
    return fetch(`http://localhost:5000/api/configuration`, {
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
    return fetch(`http://localhost:5000/api/configuration/material-props`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createCustomMaterialProp: (name, type) => {
    return fetch(`http://localhost:5000/api/configuration/material-props/`, {
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
    return fetch(`http://localhost:5000/api/configuration/material-props/${id}`, {
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
    return fetch(`http://localhost:5000/api/configuration/material-props/${id}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getUsers: (page, elementsPerPage, search, showDisabled) => {
    return fetch(`http://localhost:5000/api/users?page=${page}&elementsPerPage=${elementsPerPage}&search=${search}&showDisabled=${showDisabled}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createUser: (id, name, password, role) => {
    return fetch('http://localhost:5000/api/users', {
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
    return fetch(`http://localhost:5000/api/users/${id}`, {
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
    return fetch(`http://localhost:5000/api/users/${id}/status`, {
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
    return fetch(`http://localhost:5000/api/keys?page=${page}&elementsPerPage=${elementsPerPage}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createApiKey: name => {
    return fetch(`http://localhost:5000/api/keys`, {
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
    return fetch(`http://localhost:5000/api/keys/${id}`, {
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
    return fetch(`http://localhost:5000/api/keys/${id}/status`, {
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
    return fetch(`http://localhost:5000/api/keys/${id}`, {
      method: 'DELETE',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  getStorageSites: (page, elementsPerPage, search) => {
    return fetch(`http://localhost:5000/api/sites?page=${page}&elementsPerPage=${elementsPerPage}&search=${search}`, {
      method: 'GET',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
  createStorageSite: name => {
    return fetch(`http://localhost:5000/api/sites`, {
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
    return fetch(`http://localhost:5000/api/sites/${id}`, {
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
    return fetch(`http://localhost:5000/api/sites/${siteId}/areas`, {
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
    return fetch(`http://localhost:5000/api/sites/${siteId}/areas/${areaId}`, {
      method: 'PATCH',
      withCredentials: true,
      credentials: 'include',
      headers: { 'Content-Type': 'application/json', Authorization: getAuthorizationHeader() },
      body: JSON.stringify({ name: areaName }),
    })
      .catch(catchNetworkError)
      .then(processResponse);
  },
};
