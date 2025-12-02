const API_URL = 'http://localhost:8080';

// Users
export const getAllUsers = async () => {
  const response = await fetch(`${API_URL}/api/users`);
  return response.json();
};

export const getUserById = async (id) => {
  const response = await fetch(`${API_URL}/api/users/${id}`);
  return response.json();
};

export const getUsersByCity = async (city) => {
  const response = await fetch(`${API_URL}/api/users/city/${encodeURIComponent(city)}`);
  return response.json();
};

export const createUser = async (userData) => {
  const response = await fetch(`${API_URL}/api/users`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(userData)
  });
  return response.json();
};

// Regions
export const getCityStatistics = async () => {
  const response = await fetch(`${API_URL}/api/regions/statistics`);
  return response.json();
};

// Notifications
export const sendMessageToUser = async (userId, message) => {
  const response = await fetch(`${API_URL}/api/notifications/user`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, message })
  });
  return response.json();
};

export const sendMessageToCity = async (city, message) => {
  const response = await fetch(`${API_URL}/api/notifications/city`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ city, message })
  });
  return response.json();
};

// Auth
export const login = async (username, password) => {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password })
  });
  return response.json();
};

export const register = async (userData) => {
  const response = await fetch(`${API_URL}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(userData)
  });
  return response.json();
};

// Logs
export const getSystemLog = async () => {
  const response = await fetch(`${API_URL}/api/logs/system`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
};

export const getUserLogsList = async () => {
  const response = await fetch(`${API_URL}/api/logs/users`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
};

export const getUserLog = async (fileName) => {
  const response = await fetch(`${API_URL}/api/logs/users/${encodeURIComponent(fileName)}`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
};
