import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { Box, Typography, Avatar, Tabs, Tab, Alert, CircularProgress } from '@mui/material';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { useGetUserByIdQuery, useUpdateUserByIdMutation } from '../store/slices/usersSlice';
import { usePrivileges, useToast } from '../hooks';
import { UserPrivileges } from '../constants';
import PageLayout from '../components/layout/PageLayout';
import UserDetailsForm from '../components/users/UserDetailsForm';
import UserPermissionsForm from '../components/users/UserPermissionsForm';
import ErrorBoundary from '../components/shared/ErrorBoundary';

const UserDataSchema = Yup.object().shape({
  fullName: Yup.string()
    .max(100, 'Full name cannot exceed 100 characters')
    .nullable(),
  phoneNumber: Yup.string()
    .matches(/^[+]?[(]?[0-9]{1,4}[)]?[-\s.]?[(]?[0-9]{1,4}[)]?[-\s.]?[0-9]{1,9}$/, 'Invalid phone number format')
    .max(20, 'Phone number cannot exceed 20 characters')
    .nullable(),
  email: Yup.string()
    .email('Invalid email address format')
    .max(100, 'Email cannot exceed 100 characters')
    .nullable(),
});

const UserEditContent = () => {
  const { userId } = useParams();
  const [activeTab, setActiveTab] = useState(0);
  const toast = useToast();

  const [canReadPrivileges] = usePrivileges(UserPrivileges.UserPrivilegesRead);

  const { data: user, isLoading, error } = useGetUserByIdQuery(userId);
  const [updateUser, { isLoading: isUpdating, error: updateError, isSuccess }] = useUpdateUserByIdMutation();

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      await updateUser({ userId, userData: values }).unwrap();
      resetForm({ values });
      toast.success('User updated successfully!');
    } catch (err) {
      console.error('Update failed:', err);
      toast.error(err.data?.message || 'Failed to update user');
    } finally {
      setSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error || !user) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <Alert severity="error">Failed to load user</Alert>
      </Box>
    );
  }

  const avatarUrl = `https://robohash.org/${user.userName}`;

  return (
    <PageLayout maxWidth="md" title="Edit User" showBackButton backTo="/users">
      <Box sx={{ py: 3 }}>
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Avatar src={avatarUrl} alt={user.userName} sx={{ width: 100, height: 100 }} />
            <Typography variant="h5" color="text.secondary">
              @{user.userName}
            </Typography>
          </Box>

          {isSuccess && (
            <Alert severity="success" sx={{ width: '100%' }}>
              User updated successfully!
            </Alert>
          )}

          {updateError && (
            <Alert severity="error" sx={{ width: '100%' }}>
              {updateError.data?.message || 'Failed to update user. Please try again.'}
            </Alert>
          )}

          {canReadPrivileges ? (
            <>
              <Tabs value={activeTab} onChange={(e, newValue) => setActiveTab(newValue)} sx={{ width: '100%' }}>
                <Tab label="Details" />
                <Tab label="Permissions" />
              </Tabs>

              <Box sx={{ width: '100%', mt: 2 }}>
                {activeTab === 0 && (
                  <Formik
                    initialValues={{
                      fullName: user.fullName || '',
                      phoneNumber: user.phoneNumber || '',
                      email: user.email || '',
                    }}
                    validationSchema={UserDataSchema}
                    onSubmit={handleSubmit}
                    enableReinitialize
                  >
                    {(formikProps) => (
                      <UserDetailsForm {...formikProps} isLoading={isUpdating} />
                    )}
                  </Formik>
                )}

                {activeTab === 1 && (
                  <UserPermissionsForm userId={userId} />
                )}
              </Box>
            </>
          ) : (
            <Formik
              initialValues={{
                fullName: user.fullName || '',
                phoneNumber: user.phoneNumber || '',
                email: user.email || '',
              }}
              validationSchema={UserDataSchema}
              onSubmit={handleSubmit}
              enableReinitialize
            >
              {(formikProps) => (
                <UserDetailsForm {...formikProps} isLoading={isUpdating} />
              )}
            </Formik>
          )}
        </Box>
      </Box>
    </PageLayout>
  );
};

function UserEdit() {
  return (
    <ErrorBoundary
      title="Failed to load user"
      message="There was a problem loading the user editor. Please try again."
      onReset={() => window.location.href = '/users'}
    >
      <UserEditContent />
    </ErrorBoundary>
  );
}

export default UserEdit;
