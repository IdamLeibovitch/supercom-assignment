import { createContext } from 'react';

/**
 * Context for toast notifications.
 * 
 * @type {React.Context<{
 *   success: (message: string) => void,
 *   error: (message: string) => void,
 *   warning: (message: string) => void,
 *   info: (message: string) => void,
 * }|null>}
 */
export const ToastContext = createContext(null);
