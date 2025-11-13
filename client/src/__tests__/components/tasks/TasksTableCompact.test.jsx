import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import TasksTableCompact from '@/components/tasks/TasksTableCompact';

describe('TasksTableCompact', () => {
  const mockTasks = [
    {
      id: '1',
      title: 'Task 1',
      description: 'Description 1',
      dueDate: new Date('2024-12-31').toISOString(),
      priority: 'High',
      user: {
        id: 'user-1',
        userName: 'testuser1',
        fullName: 'Test User 1',
      },
    },
    {
      id: '2',
      title: 'Task 2',
      description: 'Description 2',
      dueDate: new Date('2024-12-25').toISOString(),
      priority: 'Medium',
      user: {
        id: 'user-2',
        userName: 'testuser2',
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

  it('renders compact table without priority column', () => {
    render(<TasksTableCompact {...mockProps} />);

    expect(screen.getByRole('table')).toBeInTheDocument();
    expect(screen.getByText('User')).toBeInTheDocument();
    expect(screen.getByText('Task')).toBeInTheDocument();
    expect(screen.getByText('Due Date')).toBeInTheDocument();
    expect(screen.getByText('Actions')).toBeInTheDocument();
    expect(screen.queryByText('Priority')).not.toBeInTheDocument();
  });

  it('shows actions in menu instead of individual buttons', () => {
    render(<TasksTableCompact {...mockProps} />);

    const actionMenuButtons = screen.getAllByLabelText('task actions');
    expect(actionMenuButtons).toHaveLength(2);

    // Individual action buttons should not be visible
    expect(screen.queryByLabelText('view task')).not.toBeInTheDocument();
    expect(screen.queryByLabelText('edit task')).not.toBeInTheDocument();
    expect(screen.queryByLabelText('delete task')).not.toBeInTheDocument();
  });

  it('opens menu when clicking actions button', async () => {
    const user = userEvent.setup();
    render(<TasksTableCompact {...mockProps} />);

    const actionMenuButtons = screen.getAllByLabelText('task actions');
    await user.click(actionMenuButtons[0]);

    await waitFor(() => {
      expect(screen.getByRole('menu')).toBeInTheDocument();
      expect(screen.getByText('View')).toBeInTheDocument();
      expect(screen.getByText('Edit')).toBeInTheDocument();
      expect(screen.getByText('Delete')).toBeInTheDocument();
    });
  });

  it('opens view dialog from menu', async () => {
    const user = userEvent.setup();
    render(<TasksTableCompact {...mockProps} />);

    const actionMenuButtons = screen.getAllByLabelText('task actions');
    await user.click(actionMenuButtons[0]);

    const viewMenuItem = await screen.findByText('View');
    await user.click(viewMenuItem);

    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('Task 1', { selector: 'p' })).toBeInTheDocument();
  });

  // it('opens edit dialog from menu', async () => {
  //   const user = userEvent.setup();
  //   render(<TasksTableCompact {...mockProps} />);

  //   const actionMenuButtons = screen.getAllByLabelText('task actions');
  //   await user.click(actionMenuButtons[0]);

  //   const editMenuItem = await screen.findByText('Edit');
  //   await user.click(editMenuItem);

  //   expect(screen.getByRole('dialog')).toBeInTheDocument();
  //   expect(screen.getByText('Edit Task')).toBeInTheDocument();
  // });

  it('opens delete dialog from menu', async () => {
    const user = userEvent.setup();
    render(<TasksTableCompact {...mockProps} />);

    const actionMenuButtons = screen.getAllByLabelText('task actions');
    await user.click(actionMenuButtons[0]);

    const deleteMenuItem = await screen.findByText('Delete');
    await user.click(deleteMenuItem);

    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByText('Delete Task')).toBeInTheDocument();
  });

  it('displays task information correctly', () => {
    render(<TasksTableCompact {...mockProps} />);

    expect(screen.getByText('Task 1')).toBeInTheDocument();
    expect(screen.getByText('Description 1')).toBeInTheDocument();
  });

  it('formats due dates correctly', () => {
    render(<TasksTableCompact {...mockProps} />);

    const date1 = new Date('2024-12-31').toLocaleDateString();
    expect(screen.getByText(date1)).toBeInTheDocument();
  });

  it('has keyboard navigation support for menu', async () => {
    const user = userEvent.setup();
    render(<TasksTableCompact {...mockProps} />);

    const actionMenuButtons = screen.getAllByLabelText('task actions');
    actionMenuButtons[0].focus();

    await user.keyboard('{Enter}');

    await waitFor(() => {
      expect(screen.getByRole('menu')).toBeInTheDocument();
    });
  });
});
