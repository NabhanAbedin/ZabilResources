import { Navigate } from "react-router-dom";
import { getIsAdmin, getToken } from "../../lib/authToken";
import type { RequireAdminProps } from "../../types/interfaces";

const RequireAdmin = ({ children }: RequireAdminProps) => {
  if (!getToken()) {
    return <Navigate to="/login" replace />;
  }

  if (!getIsAdmin()) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

export default RequireAdmin;
