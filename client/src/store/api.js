import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const backendBaseUrl = import.meta.env.VITE_BACKEND_URL || 'http://localhost:5025';

const baseQuery = fetchBaseQuery({
  baseUrl: `${backendBaseUrl}/api`,
  prepareHeaders: (headers, { getState }) => {
    const token = getState().auth.token;
    headers.set('Content-Type', 'application/json');
    // headers.set('Access-Control-Allow-Origin', '*');

    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }

    return headers;
  },
  credentials: 'include',
});

export const api = createApi({
  reducerPath: 'api',
  baseQuery: baseQuery,
  tagTypes: ['User', 'Users', 'Tasks'],
  endpoints: () => ({}),
});
