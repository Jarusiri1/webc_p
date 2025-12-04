import Keycloak from "keycloak-js";

export const keycloak = new Keycloak({
  url: "https://sso2.pea.co.th/",
  // url: "https://sso2.pea.co.th/realms/pea-users",
  // https://sso2.pea.co.th/realms/pea-users
  realm: "pea-users",
  clientId: "pea-ics",
});

export const initializeKeycloak = () => {
  return new Promise((resolve, reject) => {
    keycloak
      .init({
        onLoad: "check-sso",
        silentCheckSsoRedirectUri: window.location.origin + "/silent-check-sso.html",
        pkceMethod: "S256",
        flow: "standard",
      })
      .then((authenticated) => {
        if (!authenticated) {
          keycloak.login({ redirectUri: window.location.href });
        } else {
          console.log("[Keycloak] Authenticated ✅");
        }
        resolve(authenticated);
      })
      .catch((err) => {
        console.error("[Keycloak] Init failed ❌", err);
        keycloak.login({ redirectUri: window.location.href });
        reject(err);
      });
  });
};
