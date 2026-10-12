import { createContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { keycloak } from "./keycloak";

export interface AuthContextValue {
  initialized: boolean;
  authenticated: boolean;
  username?: string;
  email?: string;
  roles: string[];
  hasRole: (role: string) => boolean;
  login: () => Promise<void>;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

// Module-level guard: init must only ever run once per Keycloak instance
let initPromise: Promise<boolean> | null = null;
function initKeycloak(): Promise<boolean> {
  initPromise ??= keycloak.init({
    onLoad: "check-sso",
    pkceMethod: "S256",
    silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
  });
  return initPromise;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [initialized, setInitialized] = useState(false);
  const [authenticated, setAuthenticated] = useState(false);
  const [, forceRender] = useState(0);

  useEffect(() => {
    initKeycloak()
      .then((auth) => setAuthenticated(auth))
      .catch((err) => console.error("Keycloak init failed", err))
      .finally(() => setInitialized(true));

    keycloak.onAuthRefreshError = () => {
      setAuthenticated(false);
      keycloak.login();
    };
    // Re-render when a refreshed token changes the claims (e.g. new roles)
    keycloak.onAuthRefreshSuccess = () => forceRender((n) => n + 1);
    keycloak.onAuthLogout = () => setAuthenticated(false);
  }, []);

  const value = useMemo<AuthContextValue>(() => {
    const roles = keycloak.realmAccess?.roles ?? [];
    return {
      initialized,
      authenticated,
      username: keycloak.tokenParsed?.preferred_username,
      email: keycloak.tokenParsed?.email,
      roles,
      hasRole: (role) => roles.includes(role),
      login: () => keycloak.login({ redirectUri: window.location.origin + window.location.pathname }),
      logout: () => keycloak.logout({ redirectUri: window.location.origin }),
    };
  }, [initialized, authenticated, keycloak.token]);

  if (!initialized) return <p>Loading...</p>;

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}