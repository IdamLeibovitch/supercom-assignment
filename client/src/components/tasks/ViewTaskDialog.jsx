import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Box, Typography, Chip, Divider, Avatar, } from '@mui/material';
import { TaskPriorityLabels, TaskPriorityColors } from '../../constants';

function ViewTaskDialog({ open, onClose, task }) {
  if (!task) return null;

  const getPriorityColor = (priority) => {
    return TaskPriorityColors[priority] || 'default';
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Typography variant="h6" component="span" sx={{ flex: 1 }}>
            {task.title}
          </Typography>
          <Chip
            label={TaskPriorityLabels[task.priority]}
            color={getPriorityColor(task.priority)}
            size="small"
          />
        </Box>
      </DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
          <Box>
            <Typography variant="body1" color="text.secondary">
              {task.description}
            </Typography>
          </Box>

          <Box>
            <Typography variant="caption" color="text.secondary">
              Due Date
            </Typography>
            <Typography variant="body1">
              {new Date(task.dueDate).toLocaleDateString()}
            </Typography>
          </Box>

          <Divider />

          <Box>
            <Typography variant="caption" color="text.secondary" gutterBottom>
              Assigned User
            </Typography>
            <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 2, mt: 1 }}>
              <Avatar
                src={`https://robohash.org/${task.user.userName}`}
                alt={task.user.userName}
                sx={{ width: 56, height: 56 }}
              />
              <Box>
                <Typography variant="subtitle1" fontWeight={600}>
                  {task.user.fullName
                    ? `${task.user.fullName} (${task.user.userName})`
                    : task.user.userName}
                </Typography>
                {task.user.email && (
                  <Typography variant="body2" color="text.secondary">
                    {task.user.email}
                  </Typography>
                )}
                {task.user.phoneNumber && (
                  <Typography variant="body2" color="text.secondary">
                    {task.user.phoneNumber}
                  </Typography>
                )}
              </Box>
            </Box>
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
}

export default ViewTaskDialog;
