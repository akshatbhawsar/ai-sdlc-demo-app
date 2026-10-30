import { test, expect } from '@playwright/test';

test('rejects an over-limit manager approval with a clear error and keeps the invoice pending', async ({ page }) => {
  await page.goto('/');

  await page.getByLabel('Username').fill('demo.manager');
  await page.getByLabel('Password').fill('ChangeMe123!');
  await page.getByRole('button', { name: 'Sign in' }).click();

  await expect(page.getByRole('heading', { name: /Invoices/i })).toBeVisible();

  const customerName = 'Over Limit Training';
  await page.getByLabel('Customer name').fill(customerName);
  await page.getByLabel('Amount').fill('50001');
  await page.getByRole('button', { name: 'Create invoice' }).click();

  const createdInvoiceRow = page.locator('tbody tr')
    .filter({ hasText: customerName })
    .filter({ hasText: 'Draft' })
    .last();

  await expect(createdInvoiceRow).toContainText('Draft');
  await createdInvoiceRow.getByRole('button', { name: /Submit invoice .* for approval/i }).click();

  await page.getByRole('tab', { name: 'Approvals' }).click();

  const pendingItem = page.locator('#pending-list > li').filter({ hasText: customerName }).first();
  await expect(pendingItem).toBeVisible();

  await pendingItem.getByRole('button', { name: /Approve invoice/i }).click();

  await expect(page.getByRole('alert')).toContainText(/Director|50,000|500,000|over/i);
  await expect(pendingItem).toContainText(customerName);

  await page.getByRole('tab', { name: 'Invoices' }).click();
  await expect(page.locator('tbody tr').filter({ hasText: customerName }).first()).toContainText('Pending Approval');
});