import { getValidToken } from "../../auth/keycloak";

/**
 * Enhanced fetch wrapper that automatically attaches the Keycloak Bearer token
 * and refreshes it proactively if close to expiry.
 */
export async function apiFetch(
  input: RequestInfo | URL,
  init?: RequestInit
): Promise<Response> {
  const headers = new Headers(init?.headers);

  const token = await getValidToken();
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  return fetch(input, {
    ...init,
    headers,
  });
}

export default apiFetch;
