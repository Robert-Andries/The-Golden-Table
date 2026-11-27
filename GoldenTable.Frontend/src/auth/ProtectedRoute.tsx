import { useEffect, type ReactNode } from "react";
import { Outlet } from "react-router-dom";
import { useAuth } from "./useAuth";
import { LoadingScreen } from "../components/LoadingScreen";
import { Forbidden } from "../components/Forbidden";

interface ProtectedRouteProps {
  children?: ReactNode;
  role?: string;
  roles?: string[];
  permission?: string;
  permissions?: string[];
  requireAll?: boolean;
  redirectUri?: string;
}

export function ProtectedRoute({
  children,
  role,
  roles,
  permission,
  permissions,
  requireAll = false,
  redirectUri,
}: ProtectedRouteProps): ReactNode {
  const {
    initialized,
    authenticated,
    hasRole,
    hasAnyRole,
    hasAllRoles,
    hasPermission,
    login,
  } = useAuth();

  useEffect(() => {
    if (initialized && !authenticated) {
      login(redirectUri ?? window.location.href);
    }
  }, [initialized, authenticated, login, redirectUri]);

  if (!initialized) {
    return <LoadingScreen message="Initialising..." />;
  }

  if (!authenticated) {
    return <LoadingScreen message="Redirecting to login page..." />;
  }

  if (role && !hasRole(role)) {
    return <Forbidden requiredRole={role} />;
  }

  if (roles && roles.length > 0) {
    const rolesMatch = requireAll ? hasAllRoles(roles) : hasAnyRole(roles);
    if (!rolesMatch) {
      return <Forbidden requiredRoles={roles} />;
    }
  }

  if (permission && !hasPermission(permission)) {
    return <Forbidden requiredPermission={permission} />;
  }

  if (permissions && permissions.length > 0) {
    const permissionsMatch = requireAll
      ? permissions.every((p) => hasPermission(p))
      : permissions.some((p) => hasPermission(p));
    if (!permissionsMatch) {
      return <Forbidden requiredPermission={permissions.join(", ")} />;
    }
  }

  return children ? <>{children}</> : <Outlet />;
}

export default ProtectedRoute;