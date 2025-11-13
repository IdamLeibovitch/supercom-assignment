import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { setCredentials, logout, useGetCurrentUserQuery } from '../../store';
import { useAppSelector } from '../../store/hooks';
import { UserContext } from './UserContext';

export function UserProvider({ children }) {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const token = useAppSelector((state) => state.auth.token);

  const { data, error,
    // isLoading, 
  } = useGetCurrentUserQuery(undefined, {
    skip: !token,
  });

  useEffect(() => {
    if (data) {
      dispatch(setCredentials({ user: data, token }));
    }
  }, [data, dispatch, token]);

  useEffect(() => {
    if (error) {
      console.error('Failed to fetch user:', error);
      dispatch(logout());
      navigate('/login');
    }
  }, [error, dispatch, navigate]);

  const contextValue = {
    id: data?.id || null,
    userName: data?.userName || null,
    fullName: data?.fullName || null,
    phoneNumber: data?.phoneNumber || null,
    email: data?.email || null,
  };

  return (
    <UserContext.Provider value={contextValue}>
      {children}
    </UserContext.Provider>
  );
}
