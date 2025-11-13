import { Box } from '@mui/material';

function PageLayout({ children, maxWidth = 'md' }) {
  return (
    <Box
      sx={{
        maxWidth: (theme) => theme.breakpoints.values[maxWidth],
        mx: 'auto',
        width: '100%',
      }}
    >
      {children}
    </Box>
  );
}

export default PageLayout;
