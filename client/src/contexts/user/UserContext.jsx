import { createContext } from 'react';

/**
 * Context for user's info.
 * 
 * @type {React.Context<{id: string|null, userName: string|null, fullName: string|null, phoneNumber: string|null, email: string|null}|null>}
 */
export const UserContext = createContext({});
