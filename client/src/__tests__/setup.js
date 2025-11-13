import '@testing-library/jest-dom';
import { cleanup } from '@testing-library/react';
import { afterEach } from 'vitest';

// Cleanup after each test
afterEach(() => {
  cleanup();
});

// Mock environment variables
if (!import.meta.env.VITE_BACKEND_URL) {
  import.meta.env.VITE_BACKEND_URL = 'http://localhost:5042';
}
