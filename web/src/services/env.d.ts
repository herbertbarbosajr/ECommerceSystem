/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  // Adicione outras variáveis de ambiente aqui, se houver
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}