import { useNavigate } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { Box, Typography, Button, Paper, Grid } from '@mui/material';
import { logout } from '../store/slices/authSlice';
import { UserContext } from '../contexts';
import PageLayout from '../components/PageLayout';
import { use } from 'react';

function Dashboard() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { id, userName, fullName, phoneNumber, email } = use(UserContext);

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  return (
    <PageLayout>
      <Box>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 4 }}>
          <Typography variant="h4" component="h1">
            Dashboard
          </Typography>
          <Button variant="outlined" color="error" onClick={handleLogout}>
            Logout
          </Button>
        </Box>

        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Welcome, {fullName || userName || 'User'}!
              </Typography>
              <Typography variant="body1" color="text.secondary">
                Username: {userName}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                ID: {id}
              </Typography>
              {email && (
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  Email: {email}
                </Typography>
              )}
              {phoneNumber && (
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  Phone: {phoneNumber}
                </Typography>
              )}
            </Paper>
          </Grid>

          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Quick Stats
              </Typography>
              <Typography variant="body2" color="text.secondary">
                This is your protected dashboard area.
              </Typography>
            </Paper>
          </Grid>
        </Grid>
      </Box>
    </PageLayout>
  );
}

export default Dashboard;
