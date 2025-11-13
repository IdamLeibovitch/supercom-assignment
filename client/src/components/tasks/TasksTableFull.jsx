import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Avatar,
  Typography,
  Chip,
  IconButton,
  TableSortLabel,
} from '@mui/material';
import { Visibility, Edit, Delete } from '@mui/icons-material';
import { TaskPriorityLabels, TaskPriorityColors } from '../../constants/taskPriorities';
import { alpha } from '@mui/material/styles';
import ViewTaskDialog from './ViewTaskDialog';
import EditTaskDialog from './EditTaskDialog';
import DeleteTaskDialog from './DeleteTaskDialog';

function TasksTableFull({ tasks, sortBy, ascending, onSortChange, isLoading = false }) {
  const [viewTask, setViewTask] = useState(null);
  const [editTask, setEditTask] = useState(null);
  const [deleteTask, setDeleteTask] = useState(null);

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
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'priority'}
                  direction={sortBy === 'priority' && ascending ? 'asc' : 'desc'}
                  onClick={() => !isLoading && onSortChange('priority')}
                  disabled={isLoading}
                >
                  Priority
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
              >
                <TableCell>
                  <Avatar
                    src={`https://robohash.org/${task.user.userName}`}
                    alt={task.user.userName}
                    sx={{ width: 40, height: 40 }}
                  />
                </TableCell>
                <TableCell>
                  <Typography variant="body1" fontWeight={500}>
                    {task.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {task.description}
                  </Typography>
                </TableCell>
                <TableCell>
                  {new Date(task.dueDate).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  <Chip
                    label={TaskPriorityLabels[task.priority]}
                    color={getPriorityColor(task.priority)}
                    size="small"
                  />
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    size="small"
                    aria-label="view task"
                    onClick={() => setViewTask(task)}
                    disabled={isLoading}
                  >
                    <Visibility fontSize="small" />
                  </IconButton>
                  <IconButton
                    size="small"
                    aria-label="edit task"
                    onClick={() => setEditTask(task)}
                    disabled={isLoading}
                  >
                    <Edit fontSize="small" />
                  </IconButton>
                  <IconButton
                    size="small"
                    aria-label="delete task"
                    onClick={() => setDeleteTask(task)}
                    disabled={isLoading}
                  >
                    <Delete fontSize="small" />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

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

export default TasksTableFull;
