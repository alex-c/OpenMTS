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

export default {
  login: (id, password) => {
    return fetch('http://localhost:5000/api/auth?method=userLogin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Id: id, Password: password }),
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
};
