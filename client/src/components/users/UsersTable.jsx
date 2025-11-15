import { useState } from 'react';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Avatar, Typography, Box, IconButton, useMediaQuery, useTheme, } from '@mui/material';
import { Visibility, Edit } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { usePrivileges } from '../../hooks';
import { UserPrivileges } from '../../constants';
import ViewUserDialog from './ViewUserDialog';

function UsersTable({ users }) {
  const theme = useTheme();
  const navigate = useNavigate();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [viewUser, setViewUser] = useState(null);

  const [canWrite] = usePrivileges(UserPrivileges.UsersWrite);

  const handleViewUser = (user) => {
    setViewUser(user);
  };

  const handleEditUser = (user) => {
    navigate(`/users/${user.id}`);
  };

  return (
    <>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              {!isMobile && <TableCell>Avatar</TableCell>}
              <TableCell>Name</TableCell>
              <TableCell>Details</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.length === 0 ? (
              <TableRow>
                <TableCell colSpan={isMobile ? 3 : 4} align="center">
                  <Typography variant="body2" color="text.secondary">
                    No users found
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              users.map((user) => (
                <TableRow key={user.id} hover>
                  {!isMobile && (
                    <TableCell>
                      <Avatar
                        src={`https://robohash.org/${user.userName}`}
                        alt={user.userName}
                        sx={{ width: 40, height: 40 }}
                      />
                    </TableCell>
                  )}
                  <TableCell>
                    <Box>
                      <Typography variant="body1" fontWeight={500}>
                        {user.fullName}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        @{user.userName}
                      </Typography>
                    </Box>
                  </TableCell>
                  <TableCell>
                    <Box>
                      <Typography variant="body2">
                        {user.email || '-'}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {user.phoneNumber || '-'}
                      </Typography>
                    </Box>
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      aria-label="view user"
                      onClick={() => handleViewUser(user)}
                    >
                      <Visibility fontSize="small" />
                    </IconButton>
                    {canWrite && (
                      <IconButton
                        size="small"
                        aria-label="edit user"
                        onClick={() => handleEditUser(user)}
                      >
                        <Edit fontSize="small" />
                      </IconButton>
                    )}
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <ViewUserDialog
        open={Boolean(viewUser)}
        onClose={() => setViewUser(null)}
        user={viewUser}
      />
    </>
  );
}

export default UsersTable;
