import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
// import { logout } from './slices/authSlice';

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

const baseQueryWithReauth = async (args, api, extraOptions) => {
  const result = await baseQuery(args, api, extraOptions);

  if (result.error && result.error.status === 401) {
    // Logout and redirect to login
    // api.dispatch(logout());
    window.location.href = '/login';
  }

  return result;
};

export const api = createApi({
  reducerPath: 'api',
  baseQuery: baseQueryWithReauth,
  tagTypes: ['User', 'Users', 'Tasks'],
  endpoints: () => ({}),
});
