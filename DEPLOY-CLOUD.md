# Deploy DormFlow on a free cloud plan

Cloud deployment was provisioned on 20 October 2026 from branch
`codex/cloud-deploy`. The current provider resources are:

- Render service: `srv-dbbd19e7bikc7383ssm0`, free, Singapore.
- Render fallback URL: `https://dormflow-flowerdev.onrender.com`.
- Cloudflare Pages project: `dormflow-flowerdev`.
- Pages fallback URL: `https://dormflow-flowerdev.pages.dev`.
- Neon project: `cool-dew-33787110`, PostgreSQL 17, Singapore, free organization.

Frontend integration uses `https://api.flowerdev.online/api/v1`. API reference:
`https://api.flowerdev.online/scalar/v1`; downloadable schema:
`https://api.flowerdev.online/openapi/v1.json`. The deployment explicitly sets
`Api__EnableDocumentation=true` to publish API metadata in Production. Data
endpoints continue to require their existing JWT and permission checks.

The original 78 application tables were backed up and restored to Neon. Every
application table's row count matched before cloud traffic was enabled. Private
backups and connection settings are in the Git-ignored `.deployment-backups/`.

## Layout

- `test.flowerdev.online`: the React website.
- `api.flowerdev.online`: API calls, using `/api/v1` as the prefix.
- Cloudflare Pages: free static hosting for `test.flowerdev.online`.
- Render: one free Docker web service for `api.flowerdev.online`.
- Neon: PostgreSQL, independent of the local computer.

The frontend bundle calls the API hostname explicitly. Pages hosts the frontend
without using another Render custom-domain slot. This workspace already has
`api.kindee.online`, so adding both DormFlow hostnames on Render would exceed its
two included custom domains. `render.yaml` contains no secret values and is a
reproducible template; the existing Render service was created directly and is
not linked to a Blueprint sync.

## Accounts and database

1. Connect Render and Neon, and grant Render access to the DormFlow GitHub repo.
2. Create a Neon free project near the Render Singapore region where available.
3. Use a direct (non-pooler) PostgreSQL connection for EF migrations and Hangfire.
   Supply a .NET/Npgsql connection string, not a `postgresql://` URI:

   ```text
   Host=<neon-host>;Port=5432;Database=<database>;Username=<role>;Password=<password>;SSL Mode=VerifyFull;Pooling=true;Maximum Pool Size=10
   ```

4. Decide whether to migrate the current data or start an empty database before
   provisioning. For an existing database, take a fresh backup and restore it
   into an empty Neon database; keep passwords and backups out of Git and chat.
   Verify row counts and login before switching DNS. Do not overwrite a populated
   Neon database. If starting empty, set `SEED_ADMIN_EMAIL` and
   `SEED_ADMIN_PASSWORD` in Render's secret environment settings to create the
   first administrator; remove the bootstrap password after verifying login.

## Render setup

1. Push the reviewed deployment changes to the GitHub branch Render will use.
2. Create a Blueprint from this repository's `render.yaml`, selecting the free
   plan. Confirm no paid service or persistent disk is included.
3. Enter the database setting in Render's secret environment fields.
   The blueprint generates a new JWT signing key; existing browser sessions
   will require signing in again after migration.
4. Outbound email is explicitly disabled for this initial deployment, as requested.
   Password-reset requests return 503 before looking up any account and do not
   generate, save, or log reset emails. To enable email later, set
   `Email__Provider=Smtp` plus sender, host, port, username, and password in Render.
   SMTP must support STARTTLS on port 2525 (or another permitted port); the free
   plan blocks ports 25, 465, and 587. Use a verified sender address.
5. First startup applies EF migrations and seeds roles. Automatic migrations are
   enabled because this free deployment has one instance and no separate
   pre-deploy migration step. Back up data before subsequent schema deployments;
   use a dedicated migration step before scaling to multiple instances.
6. Wait for `/health/ready` to return 200 at the assigned `onrender.com` hostname.
   Verify the website, login, a permitted data read, and the API's CORS response
   for `https://test.flowerdev.online`.

## Frontend updates

Build from `web/` with `VITE_API_BASE=https://api.flowerdev.online/api/v1` set in
the environment, then zip the *contents* of `web/dist/` with `index.html` at the
archive root. In Cloudflare Pages, open project `dormflow-flowerdev` and create
a production deployment by uploading the zip. This project uses Direct Upload;
Git commits do not automatically deploy the frontend. Do not upload `.env`,
source database backups, or private deployment settings.

## Cut over the existing subdomains

1. In Render, add/verify the API custom domain. Use the exact DNS target and any
   verification records shown in that service's dashboard.
2. Save the current Cloudflare CNAME values and Tunnel routes for rollback.
3. Point `api.flowerdev.online` to `dormflow-flowerdev.onrender.com` (DNS-only)
   and associate `test.flowerdev.online` with the Pages project. Pages updates
   its CNAME to `dormflow-flowerdev.pages.dev`. Do not change `kindee.online` or
   other applications' records.
4. Use DNS-only mode while Render verifies the domain and issues its certificate.
   Verify HTTPS before enabling any Cloudflare proxy feature. Retain Full
   (strict) TLS if using the proxy; do not use Flexible TLS.
5. Check both public hostnames via normal DNS and verify authenticated frontend
   calls reach `https://api.flowerdev.online/api/v1/...`.
6. Only after successful cloud verification, stop the local DormFlow containers
   and confirm the website and a database-backed API request still work. The
   shared `mukjaiy-test` tunnel may serve other apps; do not stop or delete it.

Rollback: restore the saved `test` and `api` CNAME targets, start the local
DormFlow containers, and verify the old database. Keep the cloud database intact
while reconciling any writes made after cutover; do not overwrite either copy.

## Free-plan behavior

Render free web services sleep after 15 minutes without inbound traffic. The
first request after sleeping can take time. Background invoice and reservation
jobs do not run while the service is asleep; scheduling is not guaranteed.
Hangfire database polling can also consume Neon's compute allowance while the
service is awake. Check both providers' usage dashboards and remain on free
plans; do not add artificial keep-alive traffic or paid resources.

Render local files are ephemeral: file logs and ASP.NET Data Protection keys do
not persist across replacement. This app's main data lives in PostgreSQL, uses
JWT access tokens, and does not use a local email pickup directory in production.
Review any future file-upload feature for external persistent storage.

Limits and availability can change. Official references:

- https://render.com/docs/free
- https://render.com/docs/blueprint-spec
- https://render.com/docs/docker
- https://render.com/docs/configure-cloudflare-dns
- https://neon.com/pricing
