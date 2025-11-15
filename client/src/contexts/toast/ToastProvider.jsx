import { useSnackbar } from 'notistack';
import { ToastContext } from './ToastContext';

export function ToastProvider({ children }) {
  const { enqueueSnackbar } = useSnackbar();

  const toast = {
    success: (message) => {
      enqueueSnackbar(message, { variant: 'success' });
    },
    error: (message) => {
      enqueueSnackbar(message, { variant: 'error' });
    },
    warning: (message) => {
      enqueueSnackbar(message, { variant: 'warning' });
    },
    info: (message) => {
      enqueueSnackbar(message, { variant: 'info' });
    },
  };

  return (
    <ToastContext.Provider value={toast}>
      {children}
    </ToastContext.Provider>
  );
}
