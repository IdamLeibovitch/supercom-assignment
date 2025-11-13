import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import TasksTableFull from '@/components/tasks/TasksTableFull';

describe('TasksTableFull', () => {
  const mockTasks = [
    {
      id: '1',
      title: 'High Priority Task',
      description: 'Important task description',
      dueDate: new Date('2024-12-31').toISOString(),
      priority: 'High',
      user: {
        id: 'user-1',
        userName: 'testuser1',
        fullName: 'Test User 1',
        email: 'test1@example.com',
      },
    },
    {
      id: '2',
      title: 'Low Priority Task',
      description: 'Less important task',
      dueDate: new Date('2024-12-25').toISOString(),
      priority: 'Low',
      user: {
        id: 'user-2',
        userName: 'testuser2',
        fullName: 'Test User 2',
      },
    },
  ];

  const mockProps = {
    tasks: mockTasks,
    sortBy: 'dueDate',
    ascending: true,
    onSortChange: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders table with all columns', () => {
    render(<TasksTableFull {...mockProps} />);

    expect(screen.getByRole('table')).toBeInTheDocument();
    expect(screen.getByText('User')).toBeInTheDocument();
    expect(screen.getByText('Task')).toBeInTheDocument();
    expect(screen.getByText('Due Date')).toBeInTheDocument();
    expect(screen.getByText('Priority')).toBeInTheDocument();
    expect(screen.getByText('Actions')).toBeInTheDocument();
  });

  it('displays all tasks with correct data', () => {
    render(<TasksTableFull {...mockProps} />);

    expect(screen.getByText('High Priority Task')).toBeInTheDocument();
    expect(screen.getByText('Important task description')).toBeInTheDocument();
    expect(screen.getByText('Low Priority Task')).toBeInTheDocument();
    expect(screen.getByText('Less important task')).toBeInTheDocument();
  });

  it('shows priority chips with correct colors', () => {
    render(<TasksTableFull {...mockProps} />);

    const highChip = screen.getByText('High');
    const lowChip = screen.getByText('Low');

    expect(highChip).toBeInTheDocument();
    expect(lowChip).toBeInTheDocument();
  });

  it('displays user avatars', () => {
    render(<TasksTableFull {...mockProps} />);

    const avatar1 = screen.getByAltText('testuser1');
    expect(avatar1).toHaveAttribute('src', 'https://robohash.org/testuser1');
  });

  it('has sortable column headers', () => {
    render(<TasksTableFull {...mockProps} />);

    const sortButtons = screen.getAllByRole('button');
    const taskSortButton = sortButtons.find(btn => btn.textContent?.includes('Task'));
    expect(taskSortButton).toBeInTheDocument();
  });

  it('calls onSortChange when clicking sortable headers', async () => {
    const user = userEvent.setup();
    render(<TasksTableFull {...mockProps} />);

    const sortButtons = screen.getAllByRole('button');
    const taskSortButton = sortButtons.find(btn => btn.textContent?.includes('Task'));

    if (taskSortButton) {
      await user.click(taskSortButton);
      expect(mockProps.onSortChange).toHaveBeenCalledWith('title');
    }
  });

  it('has accessible action buttons', () => {
    render(<TasksTableFull {...mockProps} />);

    const viewButtons = screen.getAllByLabelText('view task');
    const editButtons = screen.getAllByLabelText('edit task');
    const deleteButtons = screen.getAllByLabelText('delete task');

    expect(viewButtons).toHaveLength(2);
    expect(editButtons).toHaveLength(2);
    expect(deleteButtons).toHaveLength(2);
  });

  it('opens view dialog when clicking view button', async () => {
    const user = userEvent.setup();
    render(<TasksTableFull {...mockProps} />);

    const viewButtons = screen.getAllByLabelText('view task');
    await user.click(viewButtons[0]);

    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('High Priority Task', { selector: 'span' })).toBeInTheDocument();
  });

  it('opens delete dialog when clicking delete button', async () => {
    const user = userEvent.setup();
    render(<TasksTableFull {...mockProps} />);

    const deleteButtons = screen.getAllByLabelText('delete task');
    await user.click(deleteButtons[0]);

    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('Delete Task')).toBeInTheDocument();
  });

  it('formats due dates correctly', () => {
    render(<TasksTableFull {...mockProps} />);

    const date1 = new Date('2024-12-31').toLocaleDateString();
    const date2 = new Date('2024-12-25').toLocaleDateString();

    expect(screen.getByText(date1)).toBeInTheDocument();
    expect(screen.getByText(date2)).toBeInTheDocument();
  });

  it('handles empty tasks array', () => {
    render(<TasksTableFull {...mockProps} tasks={[]} />);

    expect(screen.getByRole('table')).toBeInTheDocument();
    const rows = screen.queryAllByRole('row');
    // Only header row should be present
    expect(rows.length).toBe(1);
  });
});
