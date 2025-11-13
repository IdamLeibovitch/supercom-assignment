import {
  Box,
  Card,
  CardContent,
  Avatar,
  Typography,
  IconButton,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Stack,
} from '@mui/material';
import { MoreVert, Visibility, Edit, Delete, CalendarToday } from '@mui/icons-material';
import { useState } from 'react';
import { alpha } from '@mui/material/styles';
import { TaskPriorityColors } from '../../constants/taskPriorities';
import ViewTaskDialog from './ViewTaskDialog';
import EditTaskDialog from './EditTaskDialog';
import DeleteTaskDialog from './DeleteTaskDialog';

function TasksTableMobile({ tasks, isLoading = false }) {
  const [anchorEl, setAnchorEl] = useState(null);
  const [selectedTask, setSelectedTask] = useState(null);
  const [viewTask, setViewTask] = useState(null);
  const [editTask, setEditTask] = useState(null);
  const [deleteTask, setDeleteTask] = useState(null);

  const handleMenuOpen = (event, task) => {
    setAnchorEl(event.currentTarget);
    setSelectedTask(task);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedTask(null);
  };

  const handleView = () => {
    setViewTask(selectedTask);
    handleMenuClose();
  };

  const handleEdit = () => {
    setEditTask(selectedTask);
    handleMenuClose();
  };

  const handleDelete = () => {
    setDeleteTask(selectedTask);
    handleMenuClose();
  };

  const getPriorityColor = (priority) => {
    return TaskPriorityColors[priority] || 'default';
  };

  const getCardBackground = (priority, theme) => {
    const color = getPriorityColor(priority);
    const baseColors = {
      success: theme.palette.success.main,
      warning: theme.palette.warning.main,
      error: theme.palette.error.main,
    };
    return alpha(baseColors[color] || theme.palette.grey[500], 0.08);
  };

  return (
    <>
      <Stack spacing={2} sx={{ position: 'relative', opacity: isLoading ? 0.6 : 1 }}>
        {tasks.map((task) => (
          <Card
            key={task.id}
            sx={(theme) => ({
              backgroundColor: getCardBackground(task.priority, theme),
            })}
            aria-label={`Priority: ${task.priority}`}
          >
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', flex: 1 }}>
                  <Avatar
                    src={`https://robohash.org/${task.user.userName}`}
                    alt={task.user.userName}
                    sx={{ width: 48, height: 48 }}
                  />
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="subtitle1" fontWeight={600}>
                      {task.title}
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {task.user.userName}
                    </Typography>
                  </Box>
                </Box>
                <IconButton
                  size="small"
                  onClick={(e) => !isLoading && handleMenuOpen(e, task)}
                  aria-label="task actions"
                  disabled={isLoading}
                >
                  <MoreVert fontSize="small" />
                </IconButton>
              </Box>

              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                {task.description}
              </Typography>

              <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                <CalendarToday fontSize="small" color="action" />
                <Typography variant="caption" color="text.secondary">
                  {new Date(task.dueDate).toLocaleDateString()}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        ))}
      </Stack>

      <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
        <MenuItem onClick={handleView}>
          <ListItemIcon>
            <Visibility fontSize="small" />
          </ListItemIcon>
          <ListItemText>View</ListItemText>
        </MenuItem>
        <MenuItem onClick={handleEdit}>
          <ListItemIcon>
            <Edit fontSize="small" />
          </ListItemIcon>
          <ListItemText>Edit</ListItemText>
        </MenuItem>
        <MenuItem onClick={handleDelete}>
          <ListItemIcon>
            <Delete fontSize="small" />
          </ListItemIcon>
          <ListItemText>Delete</ListItemText>
        </MenuItem>
      </Menu>

      <ViewTaskDialog
        open={Boolean(viewTask)}
        onClose={() => setViewTask(null)}
        task={viewTask}
      />
      <EditTaskDialog
        open={Boolean(editTask)}
        onClose={() => setEditTask(null)}
        task={editTask}
      />
      <DeleteTaskDialog
        open={Boolean(deleteTask)}
        onClose={() => setDeleteTask(null)}
        task={deleteTask}
      />
    </>
  );
}

export default TasksTableMobile;
