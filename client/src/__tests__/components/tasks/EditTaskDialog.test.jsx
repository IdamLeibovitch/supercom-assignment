import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import EditTaskDialog from '@/components/tasks/EditTaskDialog';

describe('EditTaskDialog', () => {
  const mockOnClose = vi.fn();
  const mockTask = {
    id: '123',
    title: 'Test Task',
    description: 'Test Description',
    dueDate: new Date('2024-12-31').toISOString(),
    priority: 'High',
    user: {
      id: 'user-123',
      userName: 'testuser',
      fullName: 'Test User',
    },
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('does not render when closed', () => {
    render(<EditTaskDialog open={false} onClose={mockOnClose} task={mockTask} />);

    expect(screen.queryByText('Edit Task')).not.toBeInTheDocument();
  });

  it('returns null when task is null', () => {
    const { container } = render(<EditTaskDialog open={true} onClose={mockOnClose} task={null} />);

    expect(container.firstChild).toBeNull();
  });
});
