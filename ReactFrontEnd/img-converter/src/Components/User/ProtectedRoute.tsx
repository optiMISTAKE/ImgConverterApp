import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../User/AuthContext';

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { token, initializing } = useAuth();
  const location = useLocation();

  // While we are restoring auth from storage, don't redirect — wait until
  // initialization completes so page refresh won't cause a premature redirect.
  if (initializing) {
    // TO-DO: You could return a Mantine <Loader /> here if you wanted a smoother loading state
    return null;
  }

  // If not authenticated, redirect to the login page and include the
  // current location in state so the login page can return the user.
  if (!token) {
    return <Navigate to="/account/login" replace state={{ from: location }} />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;