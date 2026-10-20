# DormFlow Web

React 19 + Vite interface in Thai for the DormFlow API. The UI uses the signed-in user's API permissions and organization/dormitory scope; server authorization remains authoritative.

## Run locally

1. Start PostgreSQL and the API using the repository's development instructions in the root [README](../README.md), with the new `20261015033521_CompleteDormitoryWorkflows` migration applied.
2. From `web`, run `npm ci` once and then `npm run dev`.
3. Open http://127.0.0.1:5173. Vite proxies `/api` to `http://127.0.0.1:5080` by default. Set `API_TARGET` to the API URL if it listens elsewhere (PowerShell: `$env:API_TARGET='http://127.0.0.1:5089'; npm run dev`).

For a single-origin deployment, `src/DormFlow.Api/Dockerfile` builds this app and copies its static bundle into the API image. Configure the repository's `.env` as documented in the root README, then use `docker compose up --build`. The UI and API are served together at `http://localhost:8081` by default.

For separate public domains, set `VITE_API_BASE=https://api.flowerdev.online/api/v1` in the repository's `.env`, allow `https://test.flowerdev.online` in the API's `Cors__AllowedOrigins__0`, and rebuild with `docker compose up -d --build dormflow-api`. The API URL is embedded in the frontend bundle, so changing it requires rebuilding. Local development keeps using `/api/v1` unless `VITE_API_BASE` is set in the web environment.

## Verify

- `npm test` runs the UI unit tests.
- `npm run build` creates the production bundle in `dist/`.
- `npm run test:e2e` runs the isolated end-to-end flow in `e2e/workflows.spec.js`. It needs a running API with a disposable PostgreSQL database and `QA_EMAIL` / `QA_PASSWORD` for a test account. Playwright uses Microsoft Edge on this workstation; install a supported browser if running elsewhere.
- From the repository root, run `dotnet test DormFlow.sln` for the API/integration suite (the integration tests need Docker).

The end-to-end flow creates test records, so point it only at a disposable test database. It exercises tenant, reservation, contract, move-in, meter, invoice, payment, receipt, and desktop/mobile dashboard screens.

## Operational notes

- Amounts are displayed without a currency symbol until the property's currency is confirmed.
- QR is a payment method recorded by staff; a bank/PromptPay payment gateway has not been configured.
- Notifications are in-app for system users. Tenant-facing accounts and external SMS/email delivery are outside the current setup.
- The API's existing role and permission model controls actions and data scope; hiding a UI action does not replace API authorization.
