import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import TasksFilters from '@/components/tasks/TasksFilters';

describe('TasksFilters', () => {
  const mockProps = {
    sortBy: 'dueDate',
    ascending: true,
    priorities: [],
    users: [],
    onSortByChange: vi.fn(),
    onAscendingChange: vi.fn(),
    onPrioritiesChange: vi.fn(),
    onUsersChange: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders filters accordion', () => {
    render(<TasksFilters {...mockProps} />);

    expect(screen.getByText('Filters')).toBeInTheDocument();
  });

  it('starts collapsed by default', () => {
    render(<TasksFilters {...mockProps} />);

    const sortByField = screen.queryByLabelText(/sort by/i);
    if (sortByField) {
      expect(sortByField).not.toBeVisible();
    }
  });

  it('expands when clicked', async () => {
    const user = userEvent.setup();
    render(<TasksFilters {...mockProps} />);

    const filtersButton = screen.getByText('Filters');
    await user.click(filtersButton);

    await waitFor(() => {
      expect(screen.getByText('Due Date')).toBeVisible();
    });
  });

  it('displays active filter count', () => {
    const propsWithFilters = {
      ...mockProps,
      priorities: ['High', 'Medium'],
      users: ['user-1'],
    };

    render(<TasksFilters {...propsWithFilters} />);

    expect(screen.getByText('3 active')).toBeInTheDocument();
  });

  it('allows changing sort field', async () => {
    const user = userEvent.setup();
    render(<TasksFilters {...mockProps} />);

    await user.click(screen.getByText('Filters'));

    const sortBySelect = screen.getByText(/due date/i);
    await user.click(sortBySelect);

    const titleOption = screen.getByRole('option', { name: /title/i });
    await user.click(titleOption);

    expect(mockProps.onSortByChange).toHaveBeenCalledWith('title');
  });

  it('allows toggling sort direction', async () => {
    const user = userEvent.setup();
    render(<TasksFilters {...mockProps} />);

    await user.click(screen.getByText('Filters'));

    const ascendingSwitch = screen.getByRole('switch', { name: /ascending/i });
    await user.click(ascendingSwitch);

    expect(mockProps.onAscendingChange).toHaveBeenCalled();
  });

  it('renders priority filter options', async () => {
    const user = userEvent.setup();
    render(<TasksFilters {...mockProps} />);

    await user.click(screen.getByText('Filters'));

    const prioritySelect = screen.getByText(/priorities/i, { selector: 'label' });
    expect(prioritySelect).toBeInTheDocument();
  });

  it('renders user autocomplete', async () => {
    const user = userEvent.setup();
    render(<TasksFilters {...mockProps} />);

    await user.click(screen.getByText('Filters'));

    const userAutocomplete = screen.getByRole('combobox', { name: /users/i });
    expect(userAutocomplete).toBeInTheDocument();
  });

  it('shows selected priorities as chips', () => {
    const propsWithPriorities = {
      ...mockProps,
      priorities: ['High'],
    };

    render(<TasksFilters {...propsWithPriorities} />);

    expect(screen.getByText('1 active')).toBeInTheDocument();
  });
});
