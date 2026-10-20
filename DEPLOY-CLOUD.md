# Deploy DormFlow on a free cloud plan

Deployment is prepared but has not been provisioned. Keep the existing local
Cloudflare Tunnel routes until the cloud service and database pass verification.

## Layout

- `test.flowerdev.online`: the React website.
- `api.flowerdev.online`: API calls, using `/api/v1` as the prefix.
- Render: one free Docker web service, with both custom domains attached.
- Neon: PostgreSQL, independent of the local computer.

The same container serves both hostnames. The frontend bundle calls the API
hostname explicitly. One service avoids dividing Render's free instance hours
between two running web services. `render.yaml` contains no secret values.

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

## Cut over the existing subdomains

1. In Render, add/verify both custom domains. Use the exact DNS target and any
   verification records shown in that service's dashboard.
2. Save the current Cloudflare CNAME values and Tunnel routes for rollback.
3. Change only `test` and `api` records under `flowerdev.online` to the assigned
   Render hostname. Do not change `kindee.online` or other applications' records.
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
