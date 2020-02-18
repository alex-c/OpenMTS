function processResponse(response) {
  return new Promise((resolve, reject) => {
    if (response.status === 401 || response.status === 403) {
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
  getUsers: (page, elementsPerPage, search) => {
    return fetch(`http://localhost:5000/api/users?page=${page}&elementsPerPage=${elementsPerPage}&search=${search}`, {
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
};
