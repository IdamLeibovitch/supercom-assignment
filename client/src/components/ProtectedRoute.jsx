import { Navigate } from 'react-router-dom';
// import { useAppSelector } from '../store/hooks';

function ProtectedRoute({ children }) {
  const isAuthenticated = !!localStorage.getItem('token');
  // const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

export default ProtectedRoute;
