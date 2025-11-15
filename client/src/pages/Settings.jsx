import { useState } from 'react';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { Box, Avatar, Tabs, Tab } from '@mui/material';
import { useUser } from '../contexts';
import { useUpdateCurrentUserMutation, useGetCurrentUserQuery } from '../store/slices/usersSlice';
import { usePrivileges } from '../hooks';
import { UserPrivileges } from '../constants';
import { useToast } from '../hooks';
import PageLayout from '../components/layout/PageLayout';
import UserDetailsForm from '../components/users/UserDetailsForm';
import UserPermissionsForm from '../components/users/UserPermissionsForm';
import ErrorBoundary from '../components/shared/ErrorBoundary';

const SettingsSchema = Yup.object().shape({
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

const SettingsContent = () => {
  const [activeTab, setActiveTab] = useState(0);
  const { userName, fullName, phoneNumber, email, id } = useUser();
  const { refetch } = useGetCurrentUserQuery();
  const [updateUser, { isLoading }] = useUpdateCurrentUserMutation();
  const [canReadPrivileges] = usePrivileges(UserPrivileges.UserPrivilegesRead);
  const toast = useToast();

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      await updateUser(values).unwrap();
      await refetch();
      resetForm({ values });
      toast.success('Settings updated successfully!');
    } catch (err) {
      console.error('Update failed:', err);
      toast.error(err.data?.message || 'Failed to update settings');
    } finally {
      setSubmitting(false);
    }
  };

  const avatarUrl = `https://robohash.org/${userName}`;

  return (
    <PageLayout maxWidth="md" title="Settings">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          gap: 3,
          py: 3,
        }}
      >
        <Avatar
          src={avatarUrl}
          alt={userName}
          sx={{ width: 120, height: 120 }}
        />

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
                    fullName: fullName || '',
                    phoneNumber: phoneNumber || '',
                    email: email || '',
                  }}
                  validationSchema={SettingsSchema}
                  onSubmit={handleSubmit}
                  enableReinitialize
                >
                  {(formikProps) => (
                    <UserDetailsForm {...formikProps} isLoading={isLoading} />
                  )}
                </Formik>
              )}

              {activeTab === 1 && (
                <UserPermissionsForm userId={id} />
              )}
            </Box>
          </>
        ) : (
          <Formik
            initialValues={{
              fullName: fullName || '',
              phoneNumber: phoneNumber || '',
              email: email || '',
            }}
            validationSchema={SettingsSchema}
            onSubmit={handleSubmit}
            enableReinitialize
          >
            {(formikProps) => (
              <UserDetailsForm {...formikProps} isLoading={isLoading} />
            )}
          </Formik>
        )}
      </Box>
    </PageLayout>
  );
};

function Settings() {
  return (
    <ErrorBoundary
      title="Failed to load settings"
      message="There was a problem loading your settings. Please try again."
      onReset={() => window.location.reload()}
    >
      <SettingsContent />
    </ErrorBoundary>
  );
}

export default Settings;
