import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField, Box, MenuItem, Autocomplete, Avatar, ListItem, ListItemAvatar, ListItemText, Typography, } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { TaskPriority, TaskPriorityLabels } from '../../constants';
import { useCreateTaskMutation } from '../../store/slices/tasksSlice';
import { useGetAllUsersQuery } from '../../store/slices/usersSlice';
import { useUser } from '../../contexts';
import { useToast } from '../../hooks';

const CreateTaskSchema = Yup.object().shape({
  title: Yup.string()
    .min(1, 'Title must be at least 1 character')
    .max(200, 'Title cannot exceed 200 characters')
    .required('Title is required'),
  description: Yup.string()
    .min(1, 'Description must be at least 1 character')
    .max(1000, 'Description cannot exceed 1000 characters')
    .required('Description is required'),
  dueDate: Yup.date().required('Due date is required'),
  priority: Yup.string().required('Priority is required'),
  userId: Yup.string().required('User is required'),
});

function CreateTaskDialog({ open, onClose }) {
  const { id: currentUserId } = useUser();
  const toast = useToast();
  const [createTask, { isLoading }] = useCreateTaskMutation();
  const { data: allUsers = [] } = useGetAllUsersQuery({ page: 1, pageSize: 100 });

  const defaultDueDate = new Date();
  defaultDueDate.setDate(defaultDueDate.getDate() + 7);

  const handleSubmit = async (values, { setSubmitting, resetForm }) => {
    try {
      await createTask(values).unwrap();
      toast.success('Task created successfully!');
      resetForm();
      onClose();
    } catch (err) {
      console.error('Create failed:', err);
      toast.error(err.data?.message || 'Failed to create task');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={() => { }} maxWidth="sm" fullWidth>
      <DialogTitle>Create New Task</DialogTitle>
      <Formik
        initialValues={{
          title: '',
          description: '',
          dueDate: defaultDueDate,
          priority: TaskPriority.MEDIUM,
          userId: currentUserId,
        }}
        validationSchema={CreateTaskSchema}
        onSubmit={handleSubmit}
      >
        {({ errors, touched, values, setFieldValue }) => (
          <Form>
            <DialogContent>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <Field name="title">
                  {({ field }) => (
                    <TextField
                      {...field}
                      label="Title"
                      fullWidth
                      autoFocus
                      error={touched.title && Boolean(errors.title)}
                      helperText={touched.title && errors.title}
                    />
                  )}
                </Field>

                <Field name="description">
                  {({ field }) => (
                    <TextField
                      {...field}
                      label="Description"
                      fullWidth
                      multiline
                      rows={4}
                      error={touched.description && Boolean(errors.description)}
                      helperText={touched.description && errors.description}
                    />
                  )}
                </Field>

                <LocalizationProvider dateAdapter={AdapterDateFns}>
                  <DatePicker
                    label="Due Date"
                    value={values.dueDate}
                    onChange={(newValue) => setFieldValue('dueDate', newValue)}
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: touched.dueDate && Boolean(errors.dueDate),
                        helperText: touched.dueDate && errors.dueDate,
                      },
                    }}
                  />
                </LocalizationProvider>

                <Field name="priority">
                  {({ field }) => (
                    <TextField
                      {...field}
                      select
                      label="Priority"
                      fullWidth
                      error={touched.priority && Boolean(errors.priority)}
                      helperText={touched.priority && errors.priority}
                    >
                      {Object.values(TaskPriority).map((priority) => (
                        <MenuItem key={priority} value={priority}>
                          {TaskPriorityLabels[priority]}
                        </MenuItem>
                      ))}
                    </TextField>
                  )}
                </Field>

                <Autocomplete
                  fullWidth
                  options={allUsers}
                  value={allUsers.find((u) => u.id === values.userId) || null}
                  onChange={(event, newValue) => {
                    setFieldValue('userId', newValue?.id || '');
                  }}
                  getOptionLabel={(option) => option.userName}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  renderOption={(props, option) => {
                    const { key, ...restProps } = props;
                    return (
                      <ListItem key={key} {...restProps}>
                        <ListItemAvatar>
                          <Avatar
                            src={`https://robohash.org/${option.userName}`}
                            alt={option.userName}
                            sx={{ width: 32, height: 32 }}
                          />
                        </ListItemAvatar>
                        <ListItemText
                          primary={
                            option.fullName
                              ? `${option.fullName} (${option.userName})`
                              : option.userName
                          }
                          secondary={
                            <Box component="span" sx={{ display: 'flex', flexDirection: 'column' }}>
                              {option.email && (
                                <Typography variant="caption" color="text.secondary">
                                  {option.email}
                                </Typography>
                              )}
                              {option.phoneNumber && (
                                <Typography variant="caption" color="text.secondary">
                                  {option.phoneNumber}
                                </Typography>
                              )}
                            </Box>
                          }
                        />
                      </ListItem>
                    );
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Assign to User"
                      error={touched.userId && Boolean(errors.userId)}
                      helperText={touched.userId && errors.userId}
                    />
                  )}
                />
              </Box>
            </DialogContent>
            <DialogActions>
              <Button onClick={onClose} disabled={isLoading}>
                Cancel
              </Button>
              <Button type="submit" variant="contained" disabled={isLoading}>
                {isLoading ? 'Creating...' : 'Create Task'}
              </Button>
            </DialogActions>
          </Form>
        )}
      </Formik>
    </Dialog>
  );
}

export default CreateTaskDialog;
