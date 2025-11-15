import { useEffect, useRef } from 'react';
import { Box, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Checkbox, Typography, CircularProgress, Alert, } from '@mui/material';
import { useGetUserPrivilegesQuery, useUpdateUserPrivilegesMutation } from '../../store/slices/usersSlice';
import { UserPrivileges } from '../../constants';
import { useToast } from '../../hooks';

const PRIVILEGE_CATEGORIES = {
  'User Management': [
    {
      name: 'Users',
      description: 'View and manage user accounts',
      privileges: [
        { key: UserPrivileges.UsersRead, label: 'Read' },
        { key: UserPrivileges.UsersWrite, label: 'Write' },
      ]
    },
    {
      name: 'User Privileges',
      description: 'View and manage user permissions',
      privileges: [
        { key: UserPrivileges.UserPrivilegesRead, label: 'Read' },
        { key: UserPrivileges.UserPrivilegesWrite, label: 'Write' },
      ]
    },
  ],
  'Task Management': [
    {
      name: 'My Tasks',
      description: 'Manage your own tasks',
      privileges: [
        { key: UserPrivileges.TasksRead, label: 'Read' },
        { key: UserPrivileges.TasksCreate, label: 'Create' },
        { key: UserPrivileges.TasksWrite, label: 'Write' },
        { key: UserPrivileges.TasksDelete, label: 'Delete' },
      ]
    },
    {
      name: 'All Tasks',
      description: 'Manage tasks for all users',
      privileges: [
        { key: UserPrivileges.AllTasksRead, label: 'Read' },
        { key: UserPrivileges.AllTasksCreate, label: 'Create' },
        { key: UserPrivileges.AllTasksWrite, label: 'Write' },
        { key: UserPrivileges.AllTasksDelete, label: 'Delete' },
      ]
    },
  ],
};

function UserPermissionsForm({ userId }) {
  const abortControllerRef = useRef(null);
  const toast = useToast();

  const { data: privilegesData, isLoading, error } = useGetUserPrivilegesQuery(userId);
  const [updatePrivileges] = useUpdateUserPrivilegesMutation();

  const currentPrivileges = privilegesData?.privileges || [];

  useEffect(() => {
    return () => {
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, []);

  const handlePrivilegeToggle = async (privilege) => {
    // Cancel previous request
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }

    abortControllerRef.current = new AbortController();

    const newPrivileges = currentPrivileges.includes(privilege)
      ? currentPrivileges.filter(p => p !== privilege)
      : [...currentPrivileges, privilege];

    try {
      await updatePrivileges({
        userId,
        privileges: newPrivileges
      }).unwrap();
      toast.success('Privileges updated successfully');
    } catch (err) {
      if (err.name !== 'AbortError') {
        console.error('Failed to update privileges:', err);
        toast.error('Failed to update privileges');
      }
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" p={3}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error">
        Failed to load permissions
      </Alert>
    );
  }

  return (
    <Box>
      {Object.entries(PRIVILEGE_CATEGORIES).map(([category, items]) => (
        <Box key={category} sx={{ mb: 4 }}>
          <Typography variant="h6" gutterBottom>
            {category}
          </Typography>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Permission</TableCell>
                  <TableCell align="right" sx={{ width: 80 }}>Read</TableCell>
                  <TableCell align="right" sx={{ width: 80 }}>Create</TableCell>
                  <TableCell align="right" sx={{ width: 80 }}>Write</TableCell>
                  <TableCell align="right" sx={{ width: 80 }}>Delete</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {items.map((item) => (
                  <TableRow key={item.name}>
                    <TableCell>
                      <Typography variant="subtitle2">{item.name}</Typography>
                      <Typography variant="caption" color="text.secondary">
                        {item.description}
                      </Typography>
                    </TableCell>
                    {['Read', 'Create', 'Write', 'Delete'].map((action) => {
                      const privilege = item.privileges.find(p => p.label === action);
                      return (
                        <TableCell key={action} align="right" sx={{ width: 80 }}>
                          {privilege ? (
                            <Checkbox
                              checked={currentPrivileges.includes(privilege.key)}
                              onChange={() => handlePrivilegeToggle(privilege.key)}
                            />
                          ) : null}
                        </TableCell>
                      );
                    })}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Box>
      ))}
    </Box>
  );
}

export default UserPermissionsForm;
