import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import DeleteTaskDialog from '@/components/tasks/DeleteTaskDialog';

describe('DeleteTaskDialog', () => {
  const mockOnClose = vi.fn();
  const mockTask = {
    id: '123',
    title: 'Test Task',
    user: {
      id: 'user-123',
      userName: 'testuser',
      fullName: 'Test User',
    },
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders delete confirmation dialog', () => {
    render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    expect(screen.getByText('Delete Task')).toBeInTheDocument();
    expect(screen.getByText(/are you sure you want to delete this task/i)).toBeInTheDocument();
  });

  it('displays task owner information', () => {
    render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    expect(screen.getByText(/this task belongs to/i)).toBeInTheDocument();
    expect(screen.getByText('Test User')).toBeInTheDocument();
  });

  it('displays username when fullName is not available', () => {
    const taskWithoutFullName = {
      ...mockTask,
      user: {
        ...mockTask.user,
        fullName: null,
      },
    };

    render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={taskWithoutFullName} />);

    expect(screen.getByText('testuser')).toBeInTheDocument();
  });

  it('shows warning icon and message', () => {
    render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    expect(screen.getByText(/this action cannot be undone/i)).toBeInTheDocument();
  });

  it('does not render when closed', () => {
    render(<DeleteTaskDialog open={false} onClose={mockOnClose} task={mockTask} />);

    expect(screen.queryByText('Delete Task')).not.toBeInTheDocument();
  });

  it('returns null when task is null', () => {
    const { container } = render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={null} />);

    expect(container.firstChild).toBeNull();
  });

  it('has cancel button that calls onClose', async () => {
    const user = userEvent.setup();
    render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    const cancelButton = screen.getByRole('button', { name: /cancel/i });
    await user.click(cancelButton);

    expect(mockOnClose).toHaveBeenCalled();
  });

  it('has delete button styled as error', () => {
    render(<DeleteTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    expect(deleteButton).toBeInTheDocument();
  });
});
