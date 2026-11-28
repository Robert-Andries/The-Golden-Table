import type { ReactNode } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import type { UserProfile } from "../auth/types";
import styles from "./Forbidden.module.css";

interface ForbiddenProps {
  requiredRole?: string;
  requiredRoles?: string[];
  requiredPermission?: string;
  user?: UserProfile | null;
}

export function Forbidden({
  requiredRole,
  requiredRoles,
  requiredPermission,
  user: propUser,
}: ForbiddenProps): ReactNode {
  const { user: authUser, logout } = useAuth();
  const user = propUser || authUser;

  const requirements: string[] = [];
  if (requiredRole) requirements.push(`Required role: ${requiredRole}`);
  if (requiredRoles && requiredRoles.length > 0) requirements.push(`Required roles: ${requiredRoles.join(", ")}`);
  if (requiredPermission) requirements.push(`Required permission: ${requiredPermission}`);

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <div className={styles.iconWrapper} aria-hidden="true">
          !
        </div>
        <h1 className={styles.title}>403 - Access Forbidden</h1>
        <p className={styles.message}>
          You do not have the necessary permissions to access this section.
          {requirements.length > 0 && (
            <span className={styles.requirements}>
              {requirements.join(" | ")}
            </span>
          )}
        </p>

        {user && (
          <div className={styles.userInfo}>
            <span>Utilizator curent</span>
            <strong>{user.fullName || user.username || user.email || "Utilizator"}</strong>
            {user.email && <div className={styles.userEmail}>{user.email}</div>}
          </div>
        )}

        <div className={styles.actions}>
          <Link to="/" className={styles.primaryButton}>
            Pagina Principală
          </Link>
          <button type="button" onClick={() => logout()} className={styles.secondaryButton}>
            Deconectare
          </button>
        </div>
      </div>
    </div>
  );
}

export default Forbidden;
