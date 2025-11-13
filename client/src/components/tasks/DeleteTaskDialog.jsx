import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
} from '@mui/material';
import { Warning } from '@mui/icons-material';
import { useDeleteTaskMutation } from '../../store/slices/tasksSlice';

function DeleteTaskDialog({ open, onClose, task }) {
  const [deleteTask, { isLoading }] = useDeleteTaskMutation();

  if (!task) return null;

  const handleDelete = async () => {
    try {
      await deleteTask(task.id).unwrap();
      onClose();
    } catch (err) {
      console.error('Delete task failed:', err);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Warning color="error" />
          <Typography variant="h6" component="span">
            Delete Task
          </Typography>
        </Box>
      </DialogTitle>
      <DialogContent>
        <Typography variant="body1" gutterBottom>
          Are you sure you want to delete this task?
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
          This task belongs to{' '}
          <strong>
            {task.user.fullName || task.user.userName}
          </strong>
        </Typography>
        <Typography variant="body2" color="error" sx={{ mt: 2 }}>
          This action cannot be undone.
        </Typography>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isLoading}>
          Cancel
        </Button>
        <Button
          onClick={handleDelete}
          color="error"
          variant="contained"
          disabled={isLoading}
        >
          {isLoading ? 'Deleting...' : 'Delete'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export default DeleteTaskDialog;
