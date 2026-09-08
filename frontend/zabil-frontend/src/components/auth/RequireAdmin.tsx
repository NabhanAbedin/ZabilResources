import { Navigate } from "react-router-dom";
import { useAuth } from "../../lib/useAuth";
import type { RequireAdminProps } from "../../types/interfaces";

const RequireAdmin = ({ children }: RequireAdminProps) => {
  const { isLoggedIn, isAdmin } = useAuth();

  if (!isLoggedIn) {
    return <Navigate to="/login" replace />;
  }

  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

export default RequireAdmin;
