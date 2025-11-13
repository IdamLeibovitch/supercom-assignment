import { test, expect } from '@playwright/test';

test.describe('Tasks Page', () => {
  test.beforeEach(async ({ page }) => {
    // Mock authentication
    await page.goto('/login');
    // Add auth token to localStorage if needed
    await page.evaluate(() => {
      localStorage.setItem('token', 'mock-token');
    });
    await page.goto('/tasks');
  });

  test('should display tasks page with header', async ({ page }) => {
    await expect(page.getByRole('heading', { name: /tasks/i })).toBeVisible();
    await expect(page.getByRole('button', { name: /create task/i })).toBeVisible();
  });

  test('should display search bar', async ({ page }) => {
    await expect(page.getByPlaceholder(/search tasks/i)).toBeVisible();
  });

  test('should open create dialog when clicking create button', async ({ page }) => {
    await page.getByRole('button', { name: /create task/i }).click();
    
    await expect(page.getByText('Create New Task')).toBeVisible();
    await expect(page.getByLabel(/title/i)).toBeVisible();
    await expect(page.getByLabel(/description/i)).toBeVisible();
  });

  test('should allow typing in create dialog fields', async ({ page }) => {
    await page.getByRole('button', { name: /create task/i }).click();
    
    await page.getByLabel(/title/i).fill('New Test Task');
    await page.getByLabel(/description/i).fill('Test task description');
    
    await expect(page.getByLabel(/title/i)).toHaveValue('New Test Task');
    await expect(page.getByLabel(/description/i)).toHaveValue('Test task description');
  });

  test('should show validation errors in create dialog', async ({ page }) => {
    await page.getByRole('button', { name: /create task/i }).click();
    await page.getByRole('button', { name: /create task/i }).nth(1).click();
    
    await expect(page.getByText(/title is required/i)).toBeVisible();
  });

  test('should close create dialog when clicking cancel', async ({ page }) => {
    await page.getByRole('button', { name: /create task/i }).click();
    await page.getByRole('button', { name: /cancel/i }).click();
    
    await expect(page.getByText('Create New Task')).not.toBeVisible();
  });

  test('should expand filters accordion', async ({ page }) => {
    await page.getByText('Filters').click();
    
    await expect(page.getByLabel(/sort by/i)).toBeVisible();
    await expect(page.getByLabel(/ascending/i)).toBeVisible();
  });

  test('should display pagination controls', async ({ page }) => {
    await expect(page.getByText(/rows per page/i)).toBeVisible();
  });
});
