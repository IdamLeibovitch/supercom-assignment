import { useState, useDeferredValue } from 'react';
import { Box, TextField, Typography, InputAdornment, CircularProgress, Button, } from '@mui/material';
import { Search, Add } from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { useGetTasksQuery } from '../store/slices/tasksSlice';
import PageLayout from '../components/PageLayout';
import TasksFilters from '../components/tasks/TasksFilters';
import TasksTableResponsive from '../components/tasks/TasksTableResponsive';
import TasksPagination from '../components/tasks/TasksPagination';
import CreateTaskDialog from '../components/tasks/CreateTaskDialog';

const SearchContainer = styled('div')(({ theme }) => ({
  padding: theme.spacing(2),
  paddingBottom: 0,
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.shape.borderRadius,
  [theme.breakpoints.down('sm')]: {
    padding: 0,
    backgroundColor: 'transparent',
  },
}));

function Tasks() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [sortBy, setSortBy] = useState('dueDate');
  const [ascending, setAscending] = useState(true);
  const [priorities, setPriorities] = useState([]);
  const [users, setUsers] = useState([]);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);

  const deferredSearch = useDeferredValue(search);

  const { data, isLoading, isFetching } = useGetTasksQuery({
    pageNumber: page,
    pageSize,
    search: deferredSearch || null,
    sortBy,
    ascending,
    priorities: priorities.length > 0 ? priorities : null,
    users: users.length > 0 ? users : null,
  });

  const handlePageChange = (newPage) => {
    setPage(newPage);
  };

  const handlePageSizeChange = (newPageSize) => {
    setPageSize(newPageSize);
    setPage(1);
  };

  return (
    <PageLayout maxWidth="xl">
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pb: 10 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h4" component="h1">
            Tasks
          </Typography>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => setCreateDialogOpen(true)}
          >
            Create Task
          </Button>
        </Box>

        {/* Search Bar */}
        <SearchContainer>
          <TextField
            fullWidth
            placeholder="Search tasks..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
                </InputAdornment>
              ),
              endAdornment: isFetching && (
                <InputAdornment position="end">
                  <CircularProgress size={20} />
                </InputAdornment>
              ),
            }}
          />

          <TasksFilters
            sortBy={sortBy}
            ascending={ascending}
            priorities={priorities}
            users={users}
            onSortByChange={setSortBy}
            onAscendingChange={setAscending}
            onPrioritiesChange={setPriorities}
            onUsersChange={setUsers}
          />
        </SearchContainer>

        {/* Tasks Table */}
        {isLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <TasksTableResponsive
            isLoading={isLoading}
            tasks={data?.items || []}
            sortBy={sortBy}
            ascending={ascending}
            onSortChange={(field) => {
              if (sortBy === field) {
                setAscending(!ascending);
              } else {
                setSortBy(field);
                setAscending(true);
              }
            }}
          />
        )}

        {/* Pagination */}
        <TasksPagination
          page={page}
          pageSize={pageSize}
          totalCount={data?.totalCount || 0}
          totalPages={data?.totalPages || 0}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
        />

        {/* Create Dialog */}
        <CreateTaskDialog
          open={createDialogOpen}
          onClose={() => setCreateDialogOpen(false)}
        />
      </Box>
    </PageLayout>
  );
}

export default Tasks;
