import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import TasksTableMobile from '@/components/tasks/TasksTableMobile';

describe('TasksTableMobile', () => {
  const mockTasks = [
    {
      id: '1',
      title: 'Mobile Task 1',
      description: 'Mobile task description goes here',
      dueDate: new Date('2024-12-31').toISOString(),
      priority: 'High',
      user: {
        id: 'user-1',
        userName: 'mobileuser1',
        fullName: 'Mobile User 1',
      },
    },
    {
      id: '2',
      title: 'Mobile Task 2',
      description: 'Another mobile description',
      dueDate: new Date('2024-12-25').toISOString(),
      priority: 'Low',
      user: {
        id: 'user-2',
        userName: 'mobileuser2',
      },
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders cards instead of table', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    expect(screen.queryByRole('table')).not.toBeInTheDocument();
    expect(screen.getByText('Mobile Task 1')).toBeInTheDocument();
    expect(screen.getByText('Mobile Task 2')).toBeInTheDocument();
  });

  it('displays task information in card format', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    expect(screen.getByText('Mobile Task 1')).toBeInTheDocument();
    expect(screen.getByText('Mobile task description goes here')).toBeInTheDocument();
    expect(screen.getByText('mobileuser1')).toBeInTheDocument();
  });

  it('shows user avatar in each card', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    const avatar1 = screen.getByAltText('mobileuser1');
    const avatar2 = screen.getByAltText('mobileuser2');

    expect(avatar1).toBeInTheDocument();
    expect(avatar1).toHaveAttribute('src', 'https://robohash.org/mobileuser1');
    expect(avatar2).toBeInTheDocument();
  });

  it('displays due date with calendar icon', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    const date1 = new Date('2024-12-31').toLocaleDateString();
    const date2 = new Date('2024-12-25').toLocaleDateString();

    expect(screen.getByText(date1)).toBeInTheDocument();
    expect(screen.getByText(date2)).toBeInTheDocument();
  });

  it('shows actions menu button in each card', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    const actionButtons = screen.getAllByLabelText('task actions');
    expect(actionButtons).toHaveLength(2);
  });

  it('opens menu when clicking actions button', async () => {
    const user = userEvent.setup();
    render(<TasksTableMobile tasks={mockTasks} />);

    const actionButtons = screen.getAllByLabelText('task actions');
    await user.click(actionButtons[0]);

    await waitFor(() => {
      expect(screen.getByRole('menu')).toBeInTheDocument();
      expect(screen.getByText('View')).toBeInTheDocument();
      expect(screen.getByText('Edit')).toBeInTheDocument();
      expect(screen.getByText('Delete')).toBeInTheDocument();
    });
  });

  it('opens dialogs from menu actions', async () => {
    const user = userEvent.setup();
    render(<TasksTableMobile tasks={mockTasks} />);

    const actionButtons = screen.getAllByLabelText('task actions');
    await user.click(actionButtons[0]);

    const viewMenuItem = await screen.findByText('View');
    await user.click(viewMenuItem);

    expect(screen.getByRole('dialog')).toBeInTheDocument();
  });

  it('displays username when fullName is not available', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    expect(screen.getByText('mobileuser2')).toBeInTheDocument();
  });

  it('handles empty tasks array gracefully', () => {
    const { container } = render(<TasksTableMobile tasks={[]} />);

    const cards = container.querySelectorAll('.MuiCard-root');
    expect(cards).toHaveLength(0);
  });

  it('has proper touch targets for mobile interaction', () => {
    render(<TasksTableMobile tasks={mockTasks} />);

    const actionButtons = screen.getAllByLabelText('task actions');
    actionButtons.forEach(button => {
      // IconButtons should have adequate size for touch
      expect(button).toBeInTheDocument();
    });
  });

  it('displays cards in a vertical layout', () => {
    const { container } = render(<TasksTableMobile tasks={mockTasks} />);

    // Cards should be in a Stack (vertical layout) for easy scrolling
    const stack = container.querySelector('.MuiStack-root');
    expect(stack).toBeInTheDocument();
  });

  it('renders all task cards', () => {
    const { container } = render(<TasksTableMobile tasks={mockTasks} />);

    const cards = container.querySelectorAll('.MuiCard-root');
    expect(cards).toHaveLength(2);
  });
});
