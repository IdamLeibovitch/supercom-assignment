import { Provider } from 'react-redux';
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { SnackbarProvider } from 'notistack';
import { store } from './store/store';
import Layout from './components/layout/Layout';
import { ToastProvider, UserProvider, useUser } from './contexts';
import { UserPrivileges } from './constants';
import { usePrivileges, useSignalR } from './hooks';
import { useAppSelector } from './store';

import Login from './pages/Login';
import Signup from './pages/Signup';
import Settings from './pages/Settings';
import Tasks from './pages/Tasks';
import Users from './pages/Users';
import UserEdit from './pages/UserEdit';

import Unauthorized from './pages/Unauthorized';
import NotFound from './pages/NotFound';

const theme = createTheme({
  palette: {
    mode: 'light',
  },
});

function ProtectedRoute({ children, privileges }) {
  const location = useLocation();
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (privileges !== undefined && !privileges) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}

function AppContent() {
  useSignalR();
  const { isLoading } = useUser();
  const [canReadUsers, canReadTasks, canReadAllTasks] = usePrivileges(UserPrivileges.UsersRead, UserPrivileges.TasksRead, UserPrivileges.AllTasksRead);

  return (
    <Layout loading={isLoading}>
      <Routes>
        <Route path="/" element={<Navigate to="/tasks" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<Signup />} />
        <Route path="/unauthorized" element={<Unauthorized />} />
        <Route
          path="/tasks"
          element={
            <ProtectedRoute privileges={canReadTasks || canReadAllTasks}>
              <Tasks />
            </ProtectedRoute>
          }
        />
        <Route
          path="/users"
          element={
            <ProtectedRoute privileges={canReadUsers}>
              <Users />
            </ProtectedRoute>
          }
        />
        <Route
          path="/users/:userId"
          element={
            <ProtectedRoute privileges={canReadUsers}>
              <UserEdit />
            </ProtectedRoute>
          }
        />
        <Route
          path="/settings"
          element={
            <ProtectedRoute>
              <Settings />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Layout>
  );
}

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <SnackbarProvider
          maxSnack={3}
          anchorOrigin={{
            vertical: 'top',
            horizontal: 'right',
          }}
          autoHideDuration={3000}
        >
          <BrowserRouter>
            <UserProvider>
              <ToastProvider>
                <AppContent />
              </ToastProvider>
            </UserProvider>
          </BrowserRouter>
        </SnackbarProvider>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
