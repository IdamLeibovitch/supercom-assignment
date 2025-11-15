import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Avatar, Typography, IconButton, Menu, MenuItem, ListItemIcon, ListItemText, TableSortLabel, } from '@mui/material';
import { MoreVert, Visibility, Edit, Delete } from '@mui/icons-material';
import { useState } from 'react';
import { alpha } from '@mui/material/styles';
import { TaskPriorityColors } from '../../constants/taskPriorities';
import ViewTaskDialog from './ViewTaskDialog';
import EditTaskDialog from './EditTaskDialog';
import DeleteTaskDialog from './DeleteTaskDialog';

function TasksTableCompact({ tasks, sortBy, ascending, onSortChange, isLoading = false }) {
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

  const getRowBackground = (priority, theme) => {
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
      <TableContainer component={Paper} sx={{ position: 'relative', opacity: isLoading ? 0.6 : 1 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>User</TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'title'}
                  direction={sortBy === 'title' && ascending ? 'asc' : 'desc'}
                  onClick={() => !isLoading && onSortChange('title')}
                  disabled={isLoading}
                >
                  Task
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'dueDate'}
                  direction={sortBy === 'dueDate' && ascending ? 'asc' : 'desc'}
                  onClick={() => !isLoading && onSortChange('dueDate')}
                  disabled={isLoading}
                >
                  Due Date
                </TableSortLabel>
              </TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tasks.map((task) => (
              <TableRow
                key={task.id}
                sx={(theme) => ({
                  backgroundColor: getRowBackground(task.priority, theme),
                })}
                aria-label={`Priority: ${task.priority}`}
              >
                <TableCell>
                  <Avatar
                    src={`https://robohash.org/${task.user.userName}`}
                    alt={task.user.userName}
                    sx={{ width: 36, height: 36 }}
                  />
                </TableCell>
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>
                    {task.title}
                  </Typography>
                  <Typography variant="caption" color="text.secondary" noWrap>
                    {task.description}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">
                    {new Date(task.dueDate).toLocaleDateString()}
                  </Typography>
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    size="small"
                    onClick={(e) => !isLoading && handleMenuOpen(e, task)}
                    aria-label="task actions"
                    disabled={isLoading}
                  >
                    <MoreVert fontSize="small" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

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

export default TasksTableCompact;
