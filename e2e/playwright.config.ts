import { defineConfig, devices } from '@playwright/test';

const edgeExecutablePath = 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';

export default defineConfig({
  testDir: './tests',
  workers: 1,
  retries: process.env.CI ? 1 : 0,
  reporter: [['list'], ['html', { open: 'never' }]],
  use: {
    baseURL: 'http://localhost:5000',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        channel: 'msedge',
        headless: true,
        launchOptions: {
          executablePath: edgeExecutablePath,
        },
      }
      
    }
  ],
  webServer: {
    command: 'dotnet run --project ../backend/src/InvoiceApp.Api',
    url: 'http://localhost:5000',
    reuseExistingServer: true,
    timeout: 120_000,
  },
});