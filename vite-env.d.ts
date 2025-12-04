/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  // add more custom variables here...
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}