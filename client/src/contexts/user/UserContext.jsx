import { createContext } from 'react';
import { UserPrivileges } from '../../constants/privileges';

/**
 * Context for user's info.
 * 
 * @type {React.Context<{
 *   id: string|null,
 *   userName: string|null,
 *   fullName: string|null,
 *   phoneNumber: string|null,
 *   email: string|null,
 *   privileges: UserPrivileges[]|null
 *   isLoading: boolean
 * }|null>}
 */
export const UserContext = createContext({});
