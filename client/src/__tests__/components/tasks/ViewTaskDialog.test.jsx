import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import ViewTaskDialog from '@/components/tasks/ViewTaskDialog';

describe('ViewTaskDialog', () => {
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
      email: 'test@example.com',
      phoneNumber: '1234567890',
    },
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders task details when open', () => {
    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    expect(screen.getByText('Test Task')).toBeInTheDocument();
    expect(screen.getByText('Test Description')).toBeInTheDocument();
    expect(screen.getByText('High')).toBeInTheDocument();
    expect(screen.getByText('Test User (testuser)')).toBeInTheDocument();
  });

  it('does not render when closed', () => {
    render(<ViewTaskDialog open={false} onClose={mockOnClose} task={mockTask} />);

    expect(screen.queryByText('Test Task')).not.toBeInTheDocument();
  });

  it('returns null when task is null', () => {
    const { container } = render(<ViewTaskDialog open={true} onClose={mockOnClose} task={null} />);

    expect(container.firstChild).toBeNull();
  });

  it('displays user avatar', () => {
    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    const avatar = screen.getByAltText('testuser');
    expect(avatar).toBeInTheDocument();
    expect(avatar).toHaveAttribute('src', 'https://robohash.org/testuser');
  });

  it('displays user email and phone', () => {
    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    expect(screen.getByText('test@example.com')).toBeInTheDocument();
    expect(screen.getByText('1234567890')).toBeInTheDocument();
  });

  it('displays formatted due date', () => {
    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    const dueDate = new Date('2024-12-31').toLocaleDateString();
    expect(screen.getByText(dueDate)).toBeInTheDocument();
  });

  it('has close button that calls onClose', async () => {
    const user = userEvent.setup();
    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    const closeButton = screen.getByRole('button', { name: /close/i });
    await user.click(closeButton);

    expect(mockOnClose).toHaveBeenCalled();
  });

  it('displays username only when fullName is not provided', () => {
    const taskWithoutFullName = {
      ...mockTask,
      user: {
        ...mockTask.user,
        fullName: null,
      },
    };

    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={taskWithoutFullName} />);

    expect(screen.getByText('testuser')).toBeInTheDocument();
    expect(screen.queryByText(/test user/i)).not.toBeInTheDocument();
  });

  it('displays priority chip with correct styling', () => {
    render(<ViewTaskDialog open={true} onClose={mockOnClose} task={mockTask} />);

    const priorityChip = screen.getByText('High');
    expect(priorityChip).toBeInTheDocument();
    expect(priorityChip.closest('.MuiChip-root')).toBeInTheDocument();
  });
});
