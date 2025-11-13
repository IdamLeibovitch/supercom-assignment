import { Box, AppBar, Toolbar, Typography, Container, Button } from '@mui/material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '../store/hooks';
import UserMenu from './UserMenu';

function Layout({ children }) {
  const navigate = useNavigate();
  const location = useLocation();
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);

  const isActive = (path) => location.pathname === path;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
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
            sx={{
              flexGrow: 1,
              fontFamily: 'monospace',
              fontWeight: 'bold'
            }}
          >
            Tasky
          </Typography>
          {isAuthenticated && (
            <>
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
              <UserMenu />
            </>
          )}
        </Toolbar>
      </AppBar>
      <Container component="main" sx={{ flex: 1, py: 3 }}>
        {children}
      </Container>
    </Box>
  );
}

export default Layout;
