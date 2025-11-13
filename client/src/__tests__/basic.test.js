import { describe, it, expect } from 'vitest';

describe('Basic Setup Test', () => {
  it('should pass a simple assertion', () => {
    expect(true).toBe(true);
  });

  it('should perform arithmetic', () => {
    expect(2 + 2).toBe(4);
  });

  it('should handle strings', () => {
    expect('hello').toBe('hello');
  });
});
