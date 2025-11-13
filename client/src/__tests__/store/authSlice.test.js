import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import authReducer, { setCredentials, logout } from '@/store/slices/authSlice';

describe('authSlice', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should handle initial state', () => {
    expect(authReducer(undefined, { type: 'unknown' })).toEqual({
      user: null,
      token: null,
      isAuthenticated: false,
    });
  });

  it('should handle setCredentials', () => {
    const actual = authReducer(
      undefined,
      setCredentials({ user: { id: '123', userName: 'test' }, token: 'test-token' })
    );
    
    expect(actual.user).toEqual({ id: '123', userName: 'test' });
    expect(actual.token).toBe('test-token');
    expect(actual.isAuthenticated).toBe(true);
  });

  it('should handle logout', () => {
    const initialState = {
      user: { id: '123', userName: 'test' },
      token: 'test-token',
      isAuthenticated: true,
    };
    
    const actual = authReducer(initialState, logout());
    
    expect(actual.user).toBeNull();
    expect(actual.token).toBeNull();
    expect(actual.isAuthenticated).toBe(false);
  });
});
