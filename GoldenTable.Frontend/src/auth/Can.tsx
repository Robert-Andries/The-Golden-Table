import type { ReactNode } from "react";
import { useAuth } from "./useAuth";

export interface CanProps {
  /** Required single role */
  role?: string;
  /** Array of roles */
  roles?: string[];
  /** Required single permission */
  permission?: string;
  /** Array of permissions */
  permissions?: string[];
  /** When multiple roles or permissions are specified, whether ALL must match (default: false, matches ANY) */
  requireAll?: boolean;
  /** Content to render if authorized */
  children: ReactNode;
  /** Optional fallback content if not authorized */
  fallback?: ReactNode;
}

export function Can({
  role,
  roles,
  permission,
  permissions,
  requireAll = false,
  children,
  fallback = null,
}: CanProps): ReactNode {
  const { authenticated, hasRole, hasAnyRole, hasAllRoles, hasPermission } = useAuth();

  if (!authenticated) {
    return <>{fallback}</>;
  }

  // Check single role
  if (role && !hasRole(role)) {
    return <>{fallback}</>;
  }

  // Check multiple roles
  if (roles && roles.length > 0) {
    const rolesMatch = requireAll ? hasAllRoles(roles) : hasAnyRole(roles);
    if (!rolesMatch) {
      return <>{fallback}</>;
    }
  }

  // Check single permission
  if (permission && !hasPermission(permission)) {
    return <>{fallback}</>;
  }

  // Check multiple permissions
  if (permissions && permissions.length > 0) {
    const permissionsMatch = requireAll
      ? permissions.every((p) => hasPermission(p))
      : permissions.some((p) => hasPermission(p));
    if (!permissionsMatch) {
      return <>{fallback}</>;
    }
  }

  return <>{children}</>;
}

export default Can;
