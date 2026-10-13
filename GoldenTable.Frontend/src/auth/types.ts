export interface UserProfile {
  id?: string;
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
}

export interface AuthContextValue {
  initialized: boolean;
  authenticated: boolean;
  user: UserProfile | null;
  username?: string;
  email?: string;
  roles: string[];
  token?: string;
  hasRole: (role: string) => boolean;
  hasAnyRole: (roles: string[]) => boolean;
  hasAllRoles: (roles: string[]) => boolean;
  hasPermission: (permission: string) => boolean;
  login: (redirectUri?: string) => Promise<void>;
  logout: (redirectUri?: string) => Promise<void>;
  register: (redirectUri?: string) => Promise<void>;
  getToken: () => Promise<string | undefined>;
}
