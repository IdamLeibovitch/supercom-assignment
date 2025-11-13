import { api } from '../api';

export const tasksSlice = api.injectEndpoints({
  endpoints: (builder) => ({
    getTasks: builder.query({
      query: ({
        pageNumber = 1,
        pageSize = 10,
        search = null,
        sortBy = null,
        ascending = true,
        priorities = null,
        users = null,
      } = {}) => {
        const params = new URLSearchParams();
        params.append('pageNumber', pageNumber.toString());
        params.append('pageSize', pageSize.toString());
        params.append('ascending', ascending.toString());
        
        if (search) params.append('search', search);
        if (sortBy) params.append('sortBy', sortBy);
        if (priorities) params.append('priorities', Array.isArray(priorities) ? priorities.join(',') : priorities);
        if (users) params.append('users', Array.isArray(users) ? users.join(',') : users);

        return {
          url: '/tasks',
          params: Object.fromEntries(params),
        };
      },
      providesTags: (result) =>
        result?.items
          ? [
              ...result.items.map(({ id }) => ({ type: 'Tasks', id })),
              { type: 'Tasks', id: 'LIST' },
            ]
          : [{ type: 'Tasks', id: 'LIST' }],
    }),
    getTaskById: builder.query({
      query: (taskId) => `/tasks/${taskId}`,
      providesTags: (result, error, taskId) => [{ type: 'Tasks', id: taskId }],
    }),
    createTask: builder.mutation({
      query: (taskData) => ({
        url: '/tasks',
        method: 'POST',
        body: taskData,
      }),
      invalidatesTags: [{ type: 'Tasks', id: 'LIST' }],
    }),
    updateTask: builder.mutation({
      query: ({ taskId, taskData }) => ({
        url: `/tasks/${taskId}`,
        method: 'PUT',
        body: taskData,
      }),
      invalidatesTags: (result, error, { taskId }) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
    deleteTask: builder.mutation({
      query: (taskId) => ({
        url: `/tasks/${taskId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (result, error, taskId) => [
        { type: 'Tasks', id: taskId },
        { type: 'Tasks', id: 'LIST' },
      ],
    }),
  }),
});

export const {
  useGetTasksQuery,
  useGetTaskByIdQuery,
  useCreateTaskMutation,
  useUpdateTaskMutation,
  useDeleteTaskMutation,
} = tasksSlice;
