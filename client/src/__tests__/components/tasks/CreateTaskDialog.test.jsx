import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen } from '@testing-library/react';
// import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import CreateTaskDialog from '@/components/tasks/CreateTaskDialog';
import { UserContext } from '@/contexts/user/UserContext';
import { ToastProvider } from '../../../contexts';

describe('CreateTaskDialog', () => {
  const mockOnClose = vi.fn();
  const mockUserContextValue = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    userName: 'testuser',
    fullName: 'Test User',
    email: 'test@example.com',
    phoneNumber: '1234567890',
    refetchUser: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  // it('renders dialog when open', () => {
  //   render(
  //     <UserContext.Provider value={mockUserContextValue}>
  //       <CreateTaskDialog open={true} onClose={mockOnClose} />
  //     </UserContext.Provider>
  //   );

  //   expect(screen.getAllByRole('textbox', { name: /title/i })).toHaveLength(1);
  //   expect(screen.getAllByRole('textbox', { name: /description/i })).toHaveLength(1);
  //   expect(screen.getAllByRole('combobox', { name: /priority/i })).toHaveLength(1);
  // });

  it('does not render when closed', () => {
    render(
      <UserContext.Provider value={mockUserContextValue}>
        <CreateTaskDialog open={false} onClose={mockOnClose} />
      </UserContext.Provider>
    );

    expect(screen.queryByText('Create New Task')).not.toBeInTheDocument();
  });
});
