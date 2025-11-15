import { useState } from 'react';
import { Box, CircularProgress, Alert, } from '@mui/material';
import { useGetAllUsersQuery } from '../store/slices/usersSlice';
import { usePrivileges } from '../hooks';
import { UserPrivileges } from '../constants';
import PageLayout from '../components/layout/PageLayout';
import UsersTableResponsive from '../components/users/UsersTableResponsive';
import Pagination from '../components/shared/Pagination';
import ErrorBoundary from '../components/shared/ErrorBoundary';
// import { useNavigate } from 'react-router-dom';

const UsersContent = () => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // const navigate = useNavigate();
  const [canRead] = usePrivileges(UserPrivileges.UsersRead);
  // console.log(`canRead: `, canRead);
  const { data: paginatedData, isLoading: loading, error } = useGetAllUsersQuery(
    { page, pageSize },
    { skip: !canRead }
  );

  const handlePageChange = (value) => {
    setPage(value);
  };

  const handlePageSizeChange = (value) => {
    setPageSize(value);
    setPage(1);
  };

  // if (!canRead) {
  //   navigate('/unauthorized', { replace: true });
  //   return null;
  // }

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="80vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="80vh"
      >
        <Alert severity="error">Failed to load users</Alert>
      </Box>
    );
  }

  const users = paginatedData?.items || [];
  const totalCount = paginatedData?.totalCount || 0;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <PageLayout maxWidth="xl" title="Users">
      <Box sx={{ pb: 10 }}>
        <UsersTableResponsive users={users} />

        <Pagination
          page={page}
          pageSize={pageSize}
          totalCount={totalCount}
          totalPages={totalPages}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
        />
      </Box>
    </PageLayout>
  );
};

const Users = () => {
  return (
    <ErrorBoundary
      title="Failed to load users"
      message="There was a problem loading the users page. Please try again."
      onReset={() => window.location.reload()}
    >
      <UsersContent />
    </ErrorBoundary>
  );
};

export default Users;
