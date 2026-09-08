import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../lib/useAuth";

const SessionWatcher = () => {
  const { endedReason } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (endedReason === "expired" || endedReason === "unauthorized") {
      navigate("/login", { replace: true });
    }
  }, [endedReason, navigate]);

  return null;
};

export default SessionWatcher;
