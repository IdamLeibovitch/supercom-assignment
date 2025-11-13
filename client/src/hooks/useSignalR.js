import { useEffect, useRef } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useDispatch } from 'react-redux';
import { api } from '../store/api';
import { useAppSelector } from '../store/hooks';

/**
 * Custom React hook that establishes and manages a SignalR connection for real-time task and user updates.
 * Automatically handles reconnection, authentication via token, and dispatches RTK Query cache updates
 * based on incoming SignalR events (TaskCreated, TaskUpdated, TaskDeleted, UserUpdated).
 * 
 * @returns {HubConnection | null} The SignalR connection instance or null if not connected
 */
export const useSignalR = () => {
  const dispatch = useDispatch();
  const connectionRef = useRef(null);
  const token = useAppSelector((state) => state.auth.token);
  const currentUserId = useAppSelector((state) => state.auth.user?.id);

  useEffect(() => {
    if (!token) return;

    const backendBaseUrl = import.meta.env.VITE_BACKEND_URL || 'http://localhost:5042';
    const hubUrl = `${backendBaseUrl}/tasksHub`;

    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    connectionRef.current = connection;

    connection.on('TaskCreated', (taskId) => {
      console.log('TaskCreated:', taskId);

      const state = dispatch(api.util.getRunningQueriesThunk());
      const isGetTasksFetching = Object.values(state).some(
        (query) => query?.endpointName === 'getTasks' && query?.status === 'pending'
      );

      if (!isGetTasksFetching) {
        // TODO: Optimize based on rendered content - check if new task matches current filters
        // before invalidating to avoid unnecessary refetches
        dispatch(
          api.util.invalidateTags([{ type: 'Tasks', id: 'LIST' }])
        );
      }
    });

    // TaskUpdated handler
    connection.on('TaskUpdated', async (taskId) => {
      console.log('TaskUpdated:', taskId);

      // Get current getTasks results
      const state = api.endpoints.getTasks.select()(dispatch(api.util.getRunningQueriesThunk()));
      const renderedTasks = state?.data?.items || [];

      const isTaskRendered = renderedTasks.some((task) => task.id === taskId);

      if (isTaskRendered) {
        try {
          const updatedTask = await dispatch(
            api.endpoints.getTaskById.initiate(taskId, { forceRefetch: true })
          ).unwrap();

          dispatch(
            api.util.updateQueryData('getTasks', undefined, (draft) => {
              const taskIndex = draft.items.findIndex((t) => t.id === taskId);
              if (taskIndex !== -1) {
                draft.items[taskIndex] = updatedTask;
              }
            })
          );
        } catch (error) {
          console.error('Failed to fetch updated task:', error);
        }
      }
    });

    connection.on('TaskDeleted', (taskId) => {
      console.log('TaskDeleted:', taskId);

      const state = api.endpoints.getTasks.select()(dispatch(api.util.getRunningQueriesThunk()));
      const renderedTasks = state?.data?.items || [];

      const isTaskRendered = renderedTasks.some((task) => task.id === taskId);

      if (isTaskRendered) {
        dispatch(
          api.util.invalidateTags([
            { type: 'Tasks', id: taskId },
            { type: 'Tasks', id: 'LIST' },
          ])
        );
      }
    });

    connection.on('UserUpdated', async (userId) => {
      console.log('UserUpdated:', userId);

      if (currentUserId === userId) {
        dispatch(api.util.invalidateTags(['User']));
      }

      const state = api.endpoints.getTasks.select()(dispatch(api.util.getRunningQueriesThunk()));
      const renderedTasks = state?.data?.items || [];

      const affectedTasks = renderedTasks.filter((task) => task.user.id === userId);

      if (affectedTasks.length > 0) {
        try {
          const updatedUser = await dispatch(
            api.endpoints.getUserById.initiate(userId, { forceRefetch: true })
          ).unwrap();

          dispatch(
            api.util.updateQueryData('getTasks', undefined, (draft) => {
              draft.items.forEach((task) => {
                if (task.user.id === userId) {
                  task.user = updatedUser;
                }
              });
            })
          );
        } catch (error) {
          console.error('Failed to fetch updated user:', error);
        }
      }
    });

    connection
      .start()
      .then(() => {
        console.log('SignalR Connected');
      })
      .catch((error) => {
        console.error('SignalR Connection Error:', error);
      });

    // Cleanup
    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, [token, currentUserId, dispatch]);

  return connectionRef.current;
};
