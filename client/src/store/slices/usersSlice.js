import { api } from '../api';

export const usersSlice = api.injectEndpoints({
  endpoints: (builder) => ({
    getCurrentUser: builder.query({
      query: () => '/me',
      providesTags: ['User'],
    }),
    getAllUsers: builder.query({
      query: ({ page = 1, pageSize = 10 } = {}) => ({
        url: '/users',
        params: { page, pageSize },
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({ type: 'Users', id })),
              { type: 'Users', id: 'LIST' },
            ]
          : [{ type: 'Users', id: 'LIST' }],
    }),
    getUserById: builder.query({
      query: (userId) => `/users/${userId}`,
      providesTags: (result, error, userId) => [{ type: 'Users', id: userId }],
    }),
    updateCurrentUser: builder.mutation({
      query: (userData) => ({
        url: '/me',
        method: 'PUT',
        body: userData,
      }),
      invalidatesTags: ['User'],
    }),
    // updateUserById: builder.mutation({
    //   query: ({ userId, userData }) => ({
    //     url: `/users/${userId}`,
    //     method: 'PUT',
    //     body: userData,
    //   }),
    //   invalidatesTags: (result, error, { userId }) => [
    //     { type: 'Users', id: userId },
    //     { type: 'Users', id: 'LIST' },
    //   ],
    // }),
  }),
});

export const {
  useGetCurrentUserQuery,
  useGetAllUsersQuery,
  useGetUserByIdQuery,
  useUpdateCurrentUserMutation,
  useUpdateUserByIdMutation,
} = usersSlice;
