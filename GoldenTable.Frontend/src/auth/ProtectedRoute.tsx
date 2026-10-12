import { useEffect, type ReactNode } from "react";
import { useAuth } from "./useAuth";

interface Props {
  children: ReactNode;
  role?: string; // optional: require a specific realm role
}

export function ProtectedRoute({ children, role }: Props) {
  const { authenticated, hasRole, login } = useAuth();

  useEffect(() => {
    if (!authenticated) login(); // send to Keycloak, come back to this page
  }, [authenticated, login]);

  if (!authenticated) return <p>Redirecting to login...</p>;
  if (role && !hasRole(role)) return <p>403: you don't have access to this page.</p>;

  return <>{children}</>;
}