import { env } from 'process';

console.log("🔥 Proxy config loaded 🔥");

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7275';

const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/weatherforecast",
      "/avatars",
      "/custom-covers",
      "/mods",
      "/mod-images"
    ],
    target,
    secure: false
  }
]

export default PROXY_CONFIG;
