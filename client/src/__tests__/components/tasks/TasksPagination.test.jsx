import { describe, it, expect, vi, beforeEach } from 'vitest';
import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '../../utils/test-utils';
import TasksPagination from '@/components/tasks/TasksPagination';

describe('TasksPagination', () => {
  const mockProps = {
    page: 1,
    pageSize: 10,
    totalCount: 100,
    totalPages: 10,
    onPageChange: vi.fn(),
    onPageSizeChange: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders pagination controls', () => {
    render(<TasksPagination {...mockProps} />);

    expect(screen.getByText(/rows per page/i)).toBeInTheDocument();
  });

  it('displays current page information', () => {
    render(<TasksPagination {...mockProps} />);

    expect(screen.getByText('1-10 of 100')).toBeInTheDocument();
  });

  it('allows changing page size', async () => {
    const user = userEvent.setup();
    render(<TasksPagination {...mockProps} />);

    const pageSizeSelect = screen.getByRole('combobox');
    await user.click(pageSizeSelect);

    const option25 = screen.getByRole('option', { name: '25' });
    await user.click(option25);

    expect(mockProps.onPageSizeChange).toHaveBeenCalledWith(25);
  });

  it('shows correct range for last page', () => {
    const lastPageProps = {
      ...mockProps,
      page: 10,
      totalCount: 95,
    };

    render(<TasksPagination {...lastPageProps} />);

    expect(screen.getByText('91-95 of 95')).toBeInTheDocument();
  });

  it('displays page size options', async () => {
    const user = userEvent.setup();
    render(<TasksPagination {...mockProps} />);

    const pageSizeSelect = screen.getByRole('combobox');
    await user.click(pageSizeSelect);

    expect(screen.getByRole('option', { name: '5' })).toBeInTheDocument();
    expect(screen.getByRole('option', { name: '10' })).toBeInTheDocument();
    expect(screen.getByRole('option', { name: '25' })).toBeInTheDocument();
    expect(screen.getByRole('option', { name: '50' })).toBeInTheDocument();
  });

  it('has first and last page buttons', () => {
    render(<TasksPagination {...mockProps} />);

    const buttons = screen.getAllByRole('button');
    const firstButton = buttons.find(btn => btn.getAttribute('aria-label')?.includes('first'));
    const lastButton = buttons.find(btn => btn.getAttribute('aria-label')?.includes('last'));

    expect(firstButton).toBeInTheDocument();
    expect(lastButton).toBeInTheDocument();
  });
});
