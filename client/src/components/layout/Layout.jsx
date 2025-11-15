import { Box, AppBar, Toolbar, Typography, Container, Button, CircularProgress, Backdrop } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../../store/hooks';
import { usePrivileges } from '../../hooks';
import { UserPrivileges } from '../../constants';
import UserMenu from './UserMenu';

function Layout({ children, loading = false }) {
  const navigate = useNavigate();
  const location = useLocation();
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
  const [canViewUsers, canReadTasks, canReadAllTasks] = usePrivileges(UserPrivileges.UsersRead, UserPrivileges.TasksRead, UserPrivileges.AllTasksRead);

  const isActive = (path) => location.pathname === path;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <Backdrop
        sx={{
          color: '#fff',
          zIndex: (theme) => theme.zIndex.drawer + 1,
          position: 'fixed'
        }}
        open={loading}
      >
        <CircularProgress color="inherit" size={60} />
      </Backdrop>

      <AppBar
        position="sticky"
        sx={{
          position: { xs: 'sticky', sm: 'static' },
          top: 0,
          zIndex: (theme) => theme.zIndex.appBar,
        }}
      >
        <Toolbar>
          <Typography
            variant="h3"
            component="div"
            onClick={() => navigate('/')}
            sx={{
              flexGrow: 1,
              fontFamily: 'monospace',
              fontWeight: 'bold',
              cursor: 'pointer',
              '&:hover': {
                opacity: 0.8,
              },
            }}
          >
            Tasky
          </Typography>
          {isAuthenticated && (
            <>
              {(canReadTasks || canReadAllTasks) && (
                <Button
                  color="inherit"
                  onClick={() => navigate('/tasks')}
                  sx={{
                    fontWeight: isActive('/tasks') ? 'bold' : 'normal',
                    textDecoration: isActive('/tasks') ? 'underline' : 'none',
                  }}
                >
                  Tasks
                </Button>
              )}
              {canViewUsers && (
                <Button
                  color="inherit"
                  onClick={() => navigate('/users')}
                  sx={{
                    fontWeight: isActive('/users') ? 'bold' : 'normal',
                    textDecoration: isActive('/users') ? 'underline' : 'none',
                  }}
                >
                  Users
                </Button>
              )}
              <UserMenu />
            </>
          )}
        </Toolbar>
      </AppBar>
      <Container component="main" sx={{ flex: 1, py: 3 }}>
        {!loading && children}
      </Container>
    </Box>
  );
}

export default Layout;
