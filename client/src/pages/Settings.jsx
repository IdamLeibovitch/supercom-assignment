import { Formik } from 'formik';
import * as Yup from 'yup';
import { Box, Typography, Avatar, Alert } from '@mui/material';
import { useUser } from '../contexts';
import { useUpdateCurrentUserMutation } from '../store/slices/usersSlice';
import PageLayout from '../components/PageLayout';
import SettingsForm from '../components/SettingsForm';

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

function Settings() {
  const { userName, fullName, phoneNumber, email, refetchUser } = useUser();
  const [updateUser, { isLoading, error, isSuccess }] = useUpdateCurrentUserMutation();

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      await updateUser(values).unwrap();
      await refetchUser();
      resetForm({ values });
    } catch (err) {
      console.error('Update failed:', err);
    } finally {
      setSubmitting(false);
    }
  };

  const avatarUrl = `https://robohash.org/${userName}`;

  return (
    <PageLayout maxWidth="sm">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          gap: 3,
          py: 3,
        }}
      >
        <Typography variant="h4" component="h1" gutterBottom>
          Settings
        </Typography>

        <Avatar
          src={avatarUrl}
          alt={userName}
          sx={{ width: 120, height: 120 }}
        />

        {isSuccess && (
          <Alert severity="success" sx={{ width: '100%' }}>
            Settings updated successfully!
          </Alert>
        )}

        {error && (
          <Alert severity="error" sx={{ width: '100%' }}>
            {error.data?.message || 'Failed to update settings. Please try again.'}
          </Alert>
        )}

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
            <SettingsForm {...formikProps} isLoading={isLoading} />
          )}
        </Formik>
      </Box>
    </PageLayout>
  );
}

export default Settings;
