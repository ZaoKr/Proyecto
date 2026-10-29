const esServidorAngular = window.location.port === '4200' || window.location.port === '4201';

export const API_URL = esServidorAngular
  ? 'http://localhost:5196/api'
  : '/api';
