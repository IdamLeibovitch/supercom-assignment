import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Card, CardContent, Avatar, Typography, IconButton, Menu, MenuItem, ListItemIcon, ListItemText, Stack, } from '@mui/material';
import { MoreVert, Visibility, Edit } from '@mui/icons-material';
import { usePrivileges } from '../../hooks';
import { UserPrivileges } from '../../constants';
import ViewUserDialog from './ViewUserDialog';

function UsersTableMobile({ users }) {
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState(null);
  const [selectedUser, setSelectedUser] = useState(null);
  const [viewUser, setViewUser] = useState(null);

  const [canWrite] = usePrivileges(UserPrivileges.UsersWrite);

  const handleMenuOpen = (event, user) => {
    setAnchorEl(event.currentTarget);
    setSelectedUser(user);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedUser(null);
  };

  const handleView = () => {
    setViewUser(selectedUser);
    handleMenuClose();
  };

  const handleEdit = () => {
    navigate(`/users/${selectedUser.id}`);
    handleMenuClose();
  };

  return (
    <>
      <Stack spacing={2}>
        {users.length === 0 ? (
          <Card>
            <CardContent>
              <Typography variant="body2" color="text.secondary" align="center">
                No users found
              </Typography>
            </CardContent>
          </Card>
        ) : (
          users.map((user) => (
            <Card key={user.id}>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Box sx={{ display: 'flex', gap: 2, alignItems: 'flex-start', flex: 1 }}>
                    <Avatar
                      src={`https://robohash.org/${user.userName}`}
                      alt={user.userName}
                      sx={{ width: 48, height: 48 }}
                    />
                    <Box sx={{ flex: 1 }}>
                      <Typography variant="subtitle1" fontWeight={600}>
                        {user.fullName}
                      </Typography>
                      <Typography variant="caption" color="text.secondary" display="block">
                        @{user.userName}
                      </Typography>
                      <Typography variant="body2" sx={{ mt: 1 }}>
                        {user.email || '-'}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {user.phoneNumber || '-'}
                      </Typography>
                    </Box>
                  </Box>
                  <IconButton
                    size="small"
                    onClick={(e) => handleMenuOpen(e, user)}
                    aria-label="user actions"
                  >
                    <MoreVert fontSize="small" />
                  </IconButton>
                </Box>
              </CardContent>
            </Card>
          ))
        )}
      </Stack>

      <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
        <MenuItem onClick={handleView}>
          <ListItemIcon>
            <Visibility fontSize="small" />
          </ListItemIcon>
          <ListItemText>View</ListItemText>
        </MenuItem>
        {canWrite && (
          <MenuItem onClick={handleEdit}>
            <ListItemIcon>
              <Edit fontSize="small" />
            </ListItemIcon>
            <ListItemText>Edit</ListItemText>
          </MenuItem>
        )}
      </Menu>

      <ViewUserDialog
        open={Boolean(viewUser)}
        onClose={() => setViewUser(null)}
        user={viewUser}
      />
    </>
  );
}

export default UsersTableMobile;
