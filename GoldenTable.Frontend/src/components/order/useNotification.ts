import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

/**
 * A short-lived message. Starts from `state.message` passed by the page that navigated here,
 * and can be shown again with `notify`.
 */
export default function useNotification(durationMs = 3000) {
  const location = useLocation();
  const navigate = useNavigate();
  const initial = (location.state as { message?: string } | null)?.message;
  const [message, setMessage] = useState<string | undefined>(initial);

  // Drop the message from history so a refresh does not show it again
  useEffect(() => {
    if (initial) {
      navigate(location.pathname, { replace: true, state: null });
    }
  }, [initial, location.pathname, navigate]);

  useEffect(() => {
    if (!message) return;
    const timer = setTimeout(() => setMessage(undefined), durationMs);
    return () => clearTimeout(timer);
  }, [message, durationMs]);

  return { message, notify: setMessage };
}
