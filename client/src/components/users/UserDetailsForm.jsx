import { Form, Field } from 'formik';
import { Box, Button, TextField } from '@mui/material';

function UserDetailsForm({ errors, touched, isSubmitting, dirty, resetForm, isLoading }) {
  return (
    <Form style={{ width: '100%' }}>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <Field name="fullName">
          {({ field }) => (
            <TextField
              {...field}
              label="Full Name"
              fullWidth
              error={touched.fullName && Boolean(errors.fullName)}
              helperText={touched.fullName && errors.fullName}
            />
          )}
        </Field>

        <Field name="phoneNumber">
          {({ field }) => (
            <TextField
              {...field}
              label="Phone Number"
              fullWidth
              error={touched.phoneNumber && Boolean(errors.phoneNumber)}
              helperText={touched.phoneNumber && errors.phoneNumber}
            />
          )}
        </Field>

        <Field name="email">
          {({ field }) => (
            <TextField
              {...field}
              label="Email"
              type="email"
              fullWidth
              error={touched.email && Boolean(errors.email)}
              helperText={touched.email && errors.email}
            />
          )}
        </Field>

        <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2, mt: 2 }}>
          {dirty && (
            <Button
              variant="outlined"
              size="large"
              onClick={() => resetForm()}
              disabled={isSubmitting || isLoading}
            >
              Reset
            </Button>
          )}
          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={isSubmitting || isLoading || !dirty}
          >
            {isLoading ? 'Saving...' : 'Save Changes'}
          </Button>
        </Box>
      </Box>
    </Form>
  );
}

export default UserDetailsForm;
