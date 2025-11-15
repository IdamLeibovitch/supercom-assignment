import { useState } from 'react';
import { Accordion, AccordionSummary, AccordionDetails, FormControl, InputLabel, Select, MenuItem, Chip, Box, OutlinedInput, FormControlLabel, Switch, Typography, Autocomplete, TextField, Avatar, ListItem, ListItemAvatar, ListItemText, } from '@mui/material';
import { ExpandMore, FilterList, Clear } from '@mui/icons-material';
import { TaskPriority, TaskPriorityLabels } from '../../constants';
import { useGetAllUsersQuery } from '../../store/slices/usersSlice';

function TasksFilters({
  sortBy,
  ascending,
  priorities,
  users,
  onSortByChange,
  onAscendingChange,
  onPrioritiesChange,
  onUsersChange,
}) {
  const [expanded, setExpanded] = useState(false);
  const { data } = useGetAllUsersQuery({ page: 1, pageSize: 100 });

  const selectedUsers = (data?.items || []).filter((user) => users.includes(user.id));

  return (
    <Accordion
      expanded={expanded}
      onChange={() => setExpanded(!expanded)}
      elevation={0}
      sx={{ border: 'none', '&:before': { display: 'none' } }}
    >
      <AccordionSummary expandIcon={<ExpandMore />}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <FilterList />
          <Typography>Filters</Typography>
          {(priorities.length > 0 || users.length > 0) && (
            <Chip
              label={`${priorities.length + users.length} active`}
              size="small"
              color="primary"
            />
          )}
        </Box>
      </AccordionSummary>
      <AccordionDetails>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {/* Sort By */}
          <FormControl fullWidth>
            <InputLabel>Sort By</InputLabel>
            <Select value={sortBy} onChange={(e) => onSortByChange(e.target.value)} label="Sort By">
              <MenuItem value="dueDate">Due Date</MenuItem>
              <MenuItem value="title">Title</MenuItem>
              <MenuItem value="priority">Priority</MenuItem>
            </Select>
          </FormControl>

          {/* Sort Direction */}
          <FormControlLabel
            control={
              <Switch
                checked={ascending}
                onChange={(e) => onAscendingChange(e.target.checked)}
                inputProps={{ 'aria-label': 'Ascending' }}
              />
            }
            label="Ascending"
          />

          {/* Priority Filter */}
          <FormControl fullWidth>
            <InputLabel>Priorities</InputLabel>
            <Select
              multiple
              value={priorities}
              onChange={(e) => onPrioritiesChange(e.target.value)}
              input={<OutlinedInput label="Priorities" />}
              renderValue={(selected) => (
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                  {selected.map((value) => (
                    <Chip key={value} label={TaskPriorityLabels[value]} size="small" />
                  ))}
                </Box>
              )}
              endAdornment={
                priorities.length > 0 && (
                  <Clear
                    sx={{ mr: 2, cursor: 'pointer' }}
                    onClick={(e) => {
                      e.stopPropagation();
                      onPrioritiesChange([]);
                    }}
                  />
                )
              }
            >
              {Object.values(TaskPriority).map((priority) => (
                <MenuItem key={priority} value={priority}>
                  {TaskPriorityLabels[priority]}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          {/* Users Filter */}
          <Autocomplete
            multiple
            fullWidth
            options={data?.items || []}
            value={selectedUsers}
            onChange={(event, newValue) => {
              onUsersChange(newValue.map((user) => user.id));
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
                label="Users"
                placeholder="Select users..."
              />
            )}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => {
                const { key, ...tagProps } = getTagProps({ index });
                return (
                  <Chip
                    key={key}
                    avatar={
                      <Avatar
                        src={`https://robohash.org/${option.userName}`}
                        alt={option.userName}
                      />
                    }
                    label={option.userName}
                    size="small"
                    {...tagProps}
                  />
                );
              })
            }
            ChipProps={{
              deleteIcon: <Clear />,
            }}
          />
        </Box>
      </AccordionDetails>
    </Accordion>
  );
}

export default TasksFilters;
