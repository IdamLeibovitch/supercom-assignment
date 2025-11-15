import { Box, Pagination as MuiPagination, Select, MenuItem, FormControl, Typography, Paper, } from '@mui/material';

function Pagination({ page, pageSize, totalCount, totalPages, onPageChange, onPageSizeChange }) {
  return (
    <Paper
      sx={{
        position: 'fixed',
        bottom: 0,
        left: 0,
        right: 0,
        p: 2,
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        flexWrap: 'wrap',
        gap: 2,
        zIndex: 1000,
        boxShadow: 3,
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
        <Typography variant="body2" color="text.secondary">
          Rows per page:
        </Typography>
        <FormControl size="small">
          <Select value={pageSize} onChange={(e) => onPageSizeChange(e.target.value)}>
            <MenuItem value={5}>5</MenuItem>
            <MenuItem value={10}>10</MenuItem>
            <MenuItem value={25}>25</MenuItem>
            <MenuItem value={50}>50</MenuItem>
          </Select>
        </FormControl>
      </Box>

      <MuiPagination
        count={totalPages}
        page={page}
        onChange={(e, value) => onPageChange(value)}
        color="primary"
        showFirstButton
        showLastButton
      />

      <Typography variant="body2" color="text.secondary">
        {`${(page - 1) * pageSize + 1}-${Math.min(page * pageSize, totalCount)} of ${totalCount}`}
      </Typography>
    </Paper>
  );
}

export default Pagination;
