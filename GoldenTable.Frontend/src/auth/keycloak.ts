import Keycloak from "keycloak-js";

const keycloakUrl = import.meta.env.VITE_KEYCLOAK_URL || "http://localhost:18080";
const keycloakRealm = import.meta.env.VITE_KEYCLOAK_REALM || "goldentable";
const keycloakClientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "goldentable-public-client";

export const keycloak = new Keycloak({
  url: keycloakUrl,
  realm: keycloakRealm,
  clientId: keycloakClientId,
});

export async function getValidToken(minValidity = 5): Promise<string | undefined> {
  if (!keycloak.authenticated) {
    return undefined;
  }

  try {
    await keycloak.updateToken(minValidity);
    return keycloak.token;
  } catch (error) {
    console.warn("Failed to refresh Keycloak token:", error);
    return undefined;
  }
}

export default keycloak;