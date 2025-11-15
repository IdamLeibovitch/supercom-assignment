import { useContext } from 'react';
import { ToastContext } from '../contexts/toast/ToastContext';

/**
 * Hook to access toast notifications
 * @returns {{
 *   success: (message: string) => void,
 *   error: (message: string) => void,
 *   warning: (message: string) => void,
 *   info: (message: string) => void,
 * }}
 */
export const useToast = () => {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error('useToast must be used within ToastProvider');
  }
  return context;
};
