import { unstable_usePrompt as usePrompt } from 'react-router-dom';

/**
 * Custom hook to prompt user before navigating away from a page with unsaved changes
 * @param {boolean} when - Condition to show the prompt (e.g., form is dirty)
 * @param {string} message - Custom message to show in the prompt
 */
export function useConfirmNavigation(when, message = 'You have unsaved changes. Are you sure you want to leave?') {
  usePrompt({
    when,
    message,
  });
}
