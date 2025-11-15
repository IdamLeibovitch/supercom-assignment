import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Box, Typography, Chip, Divider, Avatar, CircularProgress, Alert, Stack, } from '@mui/material';
import { useGetUserPrivilegesQuery } from '../../store/slices/usersSlice';
import { usePrivileges } from '../../hooks';
import { UserPrivileges } from '../../constants';

function ViewUserDialog({ open, onClose, user }) {
  const [canReadPrivileges] = usePrivileges(UserPrivileges.UserPrivilegesRead);

  const {
    data: privilegesData,
    isLoading: loadingPrivileges,
    error: privilegesError
  } = useGetUserPrivilegesQuery(user?.id, {
    skip: !canReadPrivileges || !user
  });

  const privileges = privilegesData?.privileges;

  if (!user) return null;

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Avatar
            src={`https://robohash.org/${user.userName}`}
            alt={user.userName}
            sx={{ width: 48, height: 48 }}
          />
          <Box sx={{ flex: 1 }}>
            <Typography variant="h6" component="span">
              {user.fullName}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              @{user.userName}
            </Typography>
          </Box>
        </Box>
      </DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
          <Box>
            <Typography variant="caption" color="text.secondary">
              Email
            </Typography>
            <Typography variant="body1">
              {user.email || '-'}
            </Typography>
          </Box>

          <Box>
            <Typography variant="caption" color="text.secondary">
              Phone Number
            </Typography>
            <Typography variant="body1">
              {user.phoneNumber || '-'}
            </Typography>
          </Box>

          {canReadPrivileges && (
            <>
              <Divider />
              <Box>
                <Typography variant="caption" color="text.secondary" gutterBottom>
                  Privileges
                </Typography>
                {loadingPrivileges ? (
                  <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
                    <CircularProgress size={24} />
                  </Box>
                ) : privilegesError ? (
                  <Alert severity="error" sx={{ mt: 1 }}>
                    Failed to load privileges
                  </Alert>
                ) : privileges && privileges.length > 0 ? (
                  <Stack direction="row" spacing={1} flexWrap="wrap" sx={{ mt: 1, gap: 1 }}>
                    {privileges.map(privilege => (
                      <Chip
                        key={privilege}
                        label={privilege}
                        size="small"
                        color="primary"
                        variant="outlined"
                      />
                    ))}
                  </Stack>
                ) : (
                  <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                    No privileges assigned
                  </Typography>
                )}
              </Box>
            </>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
}

export default ViewUserDialog;
