import { UserPrivileges } from '../constants/privileges';
import { useUser } from '../contexts';

/**
 * Hook to check if the current user has specific privileges
 * @param {...UserPrivileges} privileges - Privilege names to check (see UserPrivileges in constants/privileges.js)
 * @returns {boolean[]} Array of booleans indicating if user has each privilege
 */
export const usePrivileges = (...privileges) => {
  const { privileges: userPrivileges, isLoading } = useUser();

  if (isLoading) {
    return [];
  }

  if (!userPrivileges) {
    return privileges.map(() => false);
  }

  return privileges.map(privilege => userPrivileges.includes(privilege));
};

/**
 * Check if user has any of the specified privileges
 * @param {...UserPrivileges} privileges
 * @returns {boolean}
 */
export const useHasAnyPrivilege = (...privileges) => {
  const results = usePrivileges(...privileges);
  return results.some(result => result === true);
};

/**
 * Check if user has all of the specified privileges
 * @param {...UserPrivileges} privileges
 * @returns {boolean}
 */
export const useHasAllPrivileges = (...privileges) => {
  const results = usePrivileges(...privileges);
  return results.every(result => result === true);
};
