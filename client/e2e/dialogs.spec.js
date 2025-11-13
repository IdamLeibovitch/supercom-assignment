import { test, expect } from '@playwright/test';

test.describe('Task Dialogs', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
    await page.evaluate(() => {
      localStorage.setItem('token', 'mock-token');
    });
    await page.goto('/tasks');
  });

  test.describe('Create Task Dialog', () => {
    test('should autofocus title field', async ({ page }) => {
      await page.getByRole('button', { name: /create task/i }).click();
      
      const titleInput = page.getByLabel(/title/i);
      await expect(titleInput).toBeFocused();
    });

    test('should allow selecting priority', async ({ page }) => {
      await page.getByRole('button', { name: /create task/i }).click();
      
      await page.getByLabel(/priority/i).click();
      await page.getByRole('option', { name: /high/i }).click();
      
      await expect(page.getByLabel(/priority/i)).toHaveValue('High');
    });

    test('should handle multiline description', async ({ page }) => {
      await page.getByRole('button', { name: /create task/i }).click();
      
      const description = page.getByLabel(/description/i);
      await description.fill('Line 1\nLine 2\nLine 3');
      
      await expect(description).toHaveValue('Line 1\nLine 2\nLine 3');
    });
  });

  test.describe('Filters', () => {
    test('should allow changing sort order', async ({ page }) => {
      await page.getByText('Filters').click();
      
      await page.getByLabel(/sort by/i).click();
      await page.getByRole('option', { name: /title/i }).click();
      
      await expect(page.getByLabel(/sort by/i)).toHaveValue('title');
    });

    test('should toggle ascending switch', async ({ page }) => {
      await page.getByText('Filters').click();
      
      const ascendingSwitch = page.getByLabel(/ascending/i);
      const initialState = await ascendingSwitch.isChecked();
      
      await ascendingSwitch.click();
      
      await expect(ascendingSwitch).toHaveAttribute('aria-checked', String(!initialState));
    });

    test('should display filter count badge', async ({ page }) => {
      await page.getByText('Filters').click();
      
      await page.getByLabel(/priorities/i).click();
      await page.getByRole('option', { name: /high/i }).click();
      
      // Close the dropdown
      await page.keyboard.press('Escape');
      
      await expect(page.getByText(/1 active/i)).toBeVisible();
    });
  });
});
