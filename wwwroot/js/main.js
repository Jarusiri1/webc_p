import { createApp } from "vue";
import App from "/webc_p/app.vue";
import { initializeKeycloak, keycloak } from "./keycloak.js";

initializeKeycloak()
  .then(() => {
    const app = createApp(App);
    app.config.globalProperties.$keycloak = keycloak;
    app.mount("#app");
  })
  .catch((err) => {
    console.error("[Keycloak] Initialization failed:", err);
    alert("Keycloak failed to initialize. Please check your configuration.");
  });
