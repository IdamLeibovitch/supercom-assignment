import { useNavigate, Link as RouterLink } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Box, Button, TextField, Typography, Paper, Link, Alert, } from '@mui/material';
import { useSignupMutation } from '../store/slices/authSlice';
import { setCredentials } from '../store/slices/authSlice';

const SignupSchema = Yup.object().shape({
  username: Yup.string()
    .min(3, 'Username must be at least 3 characters')
    .required('Username is required'),
  password: Yup.string()
    .min(6, 'Password must be at least 6 characters')
    .required('Password is required'),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref('password'), null], 'Passwords must match')
    .required('Confirm password is required'),
});

function Signup() {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const [signup, { isLoading, error }] = useSignupMutation();

  const handleSubmit = async (values, { setSubmitting }) => {
    try {
      // eslint-disable-next-line no-unused-vars
      const { confirmPassword, ...signupData } = values;
      const result = await signup(signupData).unwrap();
      dispatch(setCredentials({ user: null, token: result.token }));
      navigate('/tasks');
    } catch (err) {
      console.error('Signup failed:', err);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '80vh',
      }}
    >
      <Paper elevation={3} sx={{ p: 4, maxWidth: 400, width: '100%' }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Sign Up
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error.data?.message || 'Signup failed. Please try again.'}
          </Alert>
        )}

        <Formik
          initialValues={{
            username: '',
            password: '',
            confirmPassword: '',
          }}
          validationSchema={SignupSchema}
          onSubmit={handleSubmit}
        >
          {({ errors, touched, isSubmitting }) => (
            <Form>
              <Field name="username">
                {({ field }) => (
                  <TextField
                    {...field}
                    label="Username"
                    fullWidth
                    margin="normal"
                    error={touched.username && Boolean(errors.username)}
                    helperText={touched.username && errors.username}
                  />
                )}
              </Field>

              <Field name="password">
                {({ field }) => (
                  <TextField
                    {...field}
                    label="Password"
                    type="password"
                    fullWidth
                    margin="normal"
                    error={touched.password && Boolean(errors.password)}
                    helperText={touched.password && errors.password}
                  />
                )}
              </Field>

              <Field name="confirmPassword">
                {({ field }) => (
                  <TextField
                    {...field}
                    label="Confirm Password"
                    type="password"
                    fullWidth
                    margin="normal"
                    error={touched.confirmPassword && Boolean(errors.confirmPassword)}
                    helperText={touched.confirmPassword && errors.confirmPassword}
                  />
                )}
              </Field>

              <Button
                type="submit"
                variant="contained"
                fullWidth
                size="large"
                disabled={isSubmitting || isLoading}
                sx={{ mt: 3, mb: 2 }}
              >
                {isLoading ? 'Signing up...' : 'Sign Up'}
              </Button>

              <Box sx={{ textAlign: 'center' }}>
                <Typography variant="body2">
                  Already have an account?{' '}
                  <Link component={RouterLink} to="/login">
                    Login
                  </Link>
                </Typography>
              </Box>
            </Form>
          )}
        </Formik>
      </Paper>
    </Box>
  );
}

export default Signup;
