import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import type { KeycloakTokenParsed } from "keycloak-js";
import { keycloak, getValidToken } from "./keycloak";
import { AuthContext } from "./AuthContext";
import type { AuthContextValue, UserProfile } from "./types";
import { LoadingScreen } from "../components/LoadingScreen";

interface ExtendedTokenParsed extends KeycloakTokenParsed {
  given_name?: string;
  family_name?: string;
  name?: string;
  email?: string;
}

function extractUserProfile(): UserProfile | null {
  if (!keycloak.authenticated || !keycloak.tokenParsed) {
    return null;
  }

  const parsed = keycloak.tokenParsed as ExtendedTokenParsed;
  const firstName = parsed.given_name;
  const lastName = parsed.family_name;
  const fullName =
    parsed.name ??
    (firstName && lastName ? `${firstName} ${lastName}` : parsed.preferred_username);

  return {
    id: parsed.sub,
    username: parsed.preferred_username,
    email: parsed.email,
    firstName,
    lastName,
    fullName,
  };
}

function extractAllRoles(): string[] {
  if (!keycloak.authenticated) {
    return [];
  }

  const realmRoles = keycloak.realmAccess?.roles ?? [];

  // Extract client roles from all configured resources in resource_access
  const clientRoles = Object.values(keycloak.resourceAccess ?? {}).flatMap(
    (resource) => resource.roles ?? []
  );

  return Array.from(new Set([...realmRoles, ...clientRoles]));
}

// Module-level guard: init must only ever run once
let initPromise: Promise<boolean> | null = null;
function initKeycloak(): Promise<boolean> {
  if (!initPromise) {
    const timeoutPromise = new Promise<boolean>((resolve) => {
      setTimeout(() => {
        console.warn("Keycloak initialization timed out (5s), continuing in guest mode");
        resolve(false);
      }, 5000);
    });

    const kcPromise = keycloak.init({
      onLoad: "check-sso",
      pkceMethod: "S256",
      silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
      checkLoginIframe: false,
    });

    initPromise = Promise.race([kcPromise, timeoutPromise]).catch((err) => {
      console.error("Keycloak initialization failed:", err);
      return false;
    });
  }
  return initPromise;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [initialized, setInitialized] = useState(false);
  const [authenticated, setAuthenticated] = useState(false);
  const [user, setUser] = useState<UserProfile | null>(null);
  const [roles, setRoles] = useState<string[]>([]);
  const [token, setToken] = useState<string | undefined>(undefined);

  const syncAuthState = useCallback(() => {
    const isAuth = !!keycloak.authenticated;
    setAuthenticated(isAuth);
    setToken(keycloak.token);
    setUser(extractUserProfile());
    setRoles(extractAllRoles());
  }, []);

  useEffect(() => {
    // Setup event listeners
    keycloak.onAuthSuccess = () => {
      syncAuthState();
    };

    keycloak.onAuthRefreshSuccess = () => {
      syncAuthState();
    };

    keycloak.onAuthRefreshError = () => {
      console.warn("Keycloak token refresh error, user session expired");
      setAuthenticated(false);
      setToken(undefined);
      setUser(null);
      setRoles([]);
    };

    keycloak.onAuthLogout = () => {
      setAuthenticated(false);
      setToken(undefined);
      setUser(null);
      setRoles([]);
    };

    keycloak.onTokenExpired = () => {
      keycloak
        .updateToken(30)
        .then((refreshed) => {
          if (refreshed) {
            syncAuthState();
          }
        })
        .catch((err) => {
          console.warn("Failed to refresh expired Keycloak token:", err);
        });
    };

    // Initialize Keycloak
    initKeycloak()
      .then(() => {
        syncAuthState();
      })
      .finally(() => {
        setInitialized(true);
      });

    // Proactive background token check every 30 seconds
    const interval = setInterval(() => {
      if (keycloak.authenticated) {
        keycloak
          .updateToken(60)
          .then((refreshed) => {
            if (refreshed) {
              syncAuthState();
            }
          })
          .catch(() => {
            // Error handled by onAuthRefreshError
          });
      }
    }, 30000);

    return () => {
      clearInterval(interval);
    };
  }, [syncAuthState]);

  const hasRole = useCallback(
    (role: string): boolean => {
      return roles.includes(role);
    },
    [roles]
  );

  const hasAnyRole = useCallback(
    (requiredRoles: string[]): boolean => {
      if (roles.includes("admin")) return true;
      return requiredRoles.some((role) => roles.includes(role));
    },
    [roles]
  );

  const hasAllRoles = useCallback(
    (requiredRoles: string[]): boolean => {
      if (roles.includes("admin")) return true;
      return requiredRoles.every((role) => roles.includes(role));
    },
    [roles]
  );

  const hasPermission = useCallback(
    (permission: string): boolean => {
      // Admin has all permissions, or check explicit role/permission
      if (roles.includes("admin")) return true;
      return roles.includes(permission);
    },
    [roles]
  );

  const login = useCallback(async (redirectUri?: string) => {
    await keycloak.login({
      redirectUri: redirectUri ?? window.location.href,
    });
  }, []);

  const logout = useCallback(async (redirectUri?: string) => {
    await keycloak.logout({
      redirectUri: redirectUri ?? window.location.origin,
    });
  }, []);

  const register = useCallback(async (redirectUri?: string) => {
    await keycloak.register({
      redirectUri: redirectUri ?? window.location.href,
    });
  }, []);

  const getToken = useCallback(async () => {
    return getValidToken(30);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      initialized,
      authenticated,
      user,
      username: user?.username,
      email: user?.email,
      roles,
      token,
      hasRole,
      hasAnyRole,
      hasAllRoles,
      hasPermission,
      login,
      logout,
      register,
      getToken,
    }),
    [
      initialized,
      authenticated,
      user,
      roles,
      token,
      hasRole,
      hasAnyRole,
      hasAllRoles,
      hasPermission,
      login,
      logout,
      register,
      getToken,
    ]
  );

  if (!initialized) {
    return <LoadingScreen message="Se inițializează autentificarea..." />;
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export default AuthProvider;