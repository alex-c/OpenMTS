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
  getUsers: (page, elementsPerPage, search, showDisabled) => {
    return fetch(`http://localhost:5000/api/users?page=${page}&elementsPerPage=${elementsPerPage}&search=${search}&showDisabled=${showDisabled}`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
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
};
