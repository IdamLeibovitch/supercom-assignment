import { useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Avatar, Typography, IconButton, Menu, MenuItem, ListItemIcon, ListItemText, } from '@mui/material';
import { MoreVert, Visibility, Edit } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { usePrivileges } from '../../hooks';
import { UserPrivileges } from '../../constants';
import ViewUserDialog from './ViewUserDialog';

function UsersTableCompact({ users }) {
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
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>User</TableCell>
              <TableCell>Details</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.length === 0 ? (
              <TableRow>
                <TableCell colSpan={3} align="center">
                  <Typography variant="body2" color="text.secondary">
                    No users found
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              users.map((user) => (
                <TableRow key={user.id} hover>
                  <TableCell>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                      <Avatar
                        src={`https://robohash.org/${user.userName}`}
                        alt={user.userName}
                        sx={{ width: 36, height: 36 }}
                      />
                      <div>
                        <Typography variant="body2" fontWeight={500}>
                          {user.fullName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          @{user.userName}
                        </Typography>
                      </div>
                    </div>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2">
                      {user.email || '-'}
                    </Typography>
                    <Typography variant="caption" color="text.secondary" noWrap>
                      {user.phoneNumber || '-'}
                    </Typography>
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      onClick={(e) => handleMenuOpen(e, user)}
                      aria-label="user actions"
                    >
                      <MoreVert fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

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

export default UsersTableCompact;
