import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen, waitFor } from '@testing-library/react';
import { render } from '../../utils/test-utils';
import TasksTableResponsive from '@/components/tasks/TasksTableResponsive';

// Mock ResizeObserver
global.ResizeObserver = class ResizeObserver {
  constructor(callback) {
    this.callback = callback;
  }
  observe(target) {
    // Simulate different widths for testing
    this.callback([{ contentRect: { width: 1200 } }]);
  }
  unobserve() {}
  disconnect() {}
};

describe('TasksTableResponsive', () => {
  const mockTasks = [
    {
      id: '1',
      title: 'Test Task 1',
      description: 'Description 1',
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
      title: 'Test Task 2',
      description: 'Description 2',
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

  it('renders full table for desktop width', async () => {
    global.ResizeObserver = class ResizeObserver {
      constructor(callback) {
        this.callback = callback;
      }
      observe() {
        this.callback([{ contentRect: { width: 1200 } }]);
      }
      unobserve() {}
      disconnect() {}
    };

    render(<TasksTableResponsive {...mockProps} />);

    await waitFor(() => {
      // Desktop view should show all columns including Priority column
      expect(screen.getByText('Priority')).toBeInTheDocument();
      expect(screen.getByText('High')).toBeInTheDocument();
    });
  });

  it('renders compact table for tablet width', async () => {
    global.ResizeObserver = class ResizeObserver {
      constructor(callback) {
        this.callback = callback;
      }
      observe() {
        this.callback([{ contentRect: { width: 800 } }]);
      }
      unobserve() {}
      disconnect() {}
    };

    render(<TasksTableResponsive {...mockProps} />);

    await waitFor(() => {
      // Compact view should not show Priority column header
      expect(screen.queryByText('Priority')).not.toBeInTheDocument();
      // But tasks should still be visible
      expect(screen.getByText('Test Task 1')).toBeInTheDocument();
    });
  });

  it('renders mobile card view for small width', async () => {
    global.ResizeObserver = class ResizeObserver {
      constructor(callback) {
        this.callback = callback;
      }
      observe() {
        this.callback([{ contentRect: { width: 500 } }]);
      }
      unobserve() {}
      disconnect() {}
    };

    render(<TasksTableResponsive {...mockProps} />);

    await waitFor(() => {
      // Mobile view uses cards, not table
      expect(screen.queryByRole('table')).not.toBeInTheDocument();
      expect(screen.getByText('Test Task 1')).toBeInTheDocument();
    });
  });

  it('disconnects ResizeObserver on unmount', () => {
    const disconnectSpy = vi.fn();
    global.ResizeObserver = class ResizeObserver {
      constructor(callback) {
        this.callback = callback;
      }
      observe() {
        this.callback([{ contentRect: { width: 1200 } }]);
      }
      unobserve() {}
      disconnect = disconnectSpy;
    };

    const { unmount } = render(<TasksTableResponsive {...mockProps} />);
    unmount();

    expect(disconnectSpy).toHaveBeenCalled();
  });
});
