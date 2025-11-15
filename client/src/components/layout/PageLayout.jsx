import { useEffect } from 'react';
import { Box, Typography, IconButton } from '@mui/material';
import { ArrowBack } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

function PageLayout({ children, maxWidth = 'md', title, showBackButton = false, backTo }) {
  const navigate = useNavigate();

  useEffect(() => {
    if (title) {
      document.title = `${title} - Tasky`;
    }
    return () => {
      document.title = 'Tasky';
    };
  }, [title]);

  const handleBack = () => {
    if (backTo) {
      navigate(backTo);
    } else {
      navigate(-1);
    }
  };

  return (
    <Box
      sx={{
        maxWidth: (theme) => theme.breakpoints.values[maxWidth],
        mx: 'auto',
        width: '100%',
      }}
    >
      {title && (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
          {showBackButton && (
            <IconButton onClick={handleBack} aria-label="go back">
              <ArrowBack />
            </IconButton>
          )}
          <Typography variant="h4" component="h1">
            {title}
          </Typography>
        </Box>
      )}
      {children}
    </Box>
  );
}

export default PageLayout;
