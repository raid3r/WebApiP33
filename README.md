# WebApiP33

Messenger web app: **ASP.NET Core 8 API** + **Vue 3 / TypeScript** frontend + **MS SQL Server 2022**.

## Quick start with Docker

This is the **local / non-production** Docker workflow and uses the default `docker-compose.yml` file.

1. Create a local `.env` from the example.
2. Fill in the required secrets.
3. Start the stack.

```bash
cp .env.example .env

docker compose up --build
```

Equivalent explicit command:

```bash
docker compose --env-file .env -f docker-compose.yml up --build
```

Default published ports come from `.env`:
- `DB_EXTERNAL_PORT=1433`
- `API_EXTERNAL_PORT=8082`
- `FRONTEND_EXTERNAL_PORT=8081`

## Environment files

### Local / default
Use `.env.example` as the base for local Docker runs.

Start the local / non-production stack with:

```bash
docker compose --env-file .env -f docker-compose.yml up --build
```

Required values:
- `DB_PASSWORD`
- `JWT_SECRET`

Published host ports:
- `DB_EXTERNAL_PORT`
- `API_EXTERNAL_PORT`
- `FRONTEND_EXTERNAL_PORT`

### Production
Use `.env.production.example` as the base for a production server:

```bash
cp .env.production.example .env
```

Start the production stack with:

```bash
docker compose --env-file .env -f docker-compose.production.yml up -d --build
```

Production-oriented variables:
- `DB_PASSWORD` — SQL Server `sa` password
- `JWT_SECRET` — JWT signing key
- `FRONTEND_EXTERNAL_PORT` — published host port for HTTP nginx frontend
- `APP_DOMAIN` / `APP_DOMAIN_WWW` — public domain names for nginx
- `FRONTEND_HTTPS_EXTERNAL_PORT` — published host port for HTTPS frontend
- `SSL_CERTS_DIR` — host directory mounted into nginx with certificates
- `SSL_CERT_FULLCHAIN_PATH` / `SSL_CERT_PRIVKEY_PATH` — certificate paths inside the nginx container

## Production deployment

### 1. Prepare the server
Install Docker Engine and Docker Compose plugin on the production host.

Make sure DNS records for your domain point to the server IP:
- `A example.com -> <server-ip>`
- `A www.example.com -> <server-ip>`

### 2. Prepare configuration
Create the production `.env` file:

```bash
cp .env.production.example .env
```

Edit `.env` and replace all example values with real ones.

At minimum, configure:
- strong `DB_PASSWORD`
- long random `JWT_SECRET`
- published ports for your host
- domain names and certificate paths if you plan to enable HTTPS

### 3. Start the stack
Run the application in the project root:

```bash
docker compose --env-file .env -f docker-compose.production.yml up -d --build
```

Check containers and logs:

```bash
docker compose --env-file .env -f docker-compose.production.yml ps

docker compose --env-file .env -f docker-compose.production.yml logs -f db

docker compose --env-file .env -f docker-compose.production.yml logs -f api

docker compose --env-file .env -f docker-compose.production.yml logs -f frontend
```

### 4. Current production behavior
`docker-compose.production.yml` is a dedicated production stack:
- frontend serves HTTP on `FRONTEND_EXTERNAL_PORT`
- frontend serves HTTPS on `FRONTEND_HTTPS_EXTERNAL_PORT`
- `vue-front/nginx.conf.template` injects `APP_DOMAIN` / `APP_DOMAIN_WWW` via environment variables
- API and DB stay internal to the Docker network and are not published on host ports

### 5. Enable domain + HTTPS later
The production stack already expects TLS termination in nginx. Before first start, do all of the following:

1. Put certificates on the host inside the directory referenced by `SSL_CERTS_DIR`.
2. Set `APP_DOMAIN` and, optionally, `APP_DOMAIN_WWW` in `.env`.
3. Set `SSL_CERT_FULLCHAIN_PATH` and `SSL_CERT_PRIVKEY_PATH` to the in-container certificate paths used by `vue-front/nginx.conf.template`.
4. Rebuild and restart the containers.

```bash
docker compose --env-file .env -f docker-compose.production.yml up -d --build
```

### 6. Recommended hardening for real production
The dedicated production compose file already exposes only frontend ports.
Additional hardening ideas:
- keep `APP_DOMAIN_WWW` empty if you do not use the `www` alias
- place valid certificate files in `SSL_CERTS_DIR` before container startup
- use firewall rules to allow inbound traffic only on `FRONTEND_EXTERNAL_PORT` / `FRONTEND_HTTPS_EXTERNAL_PORT`

## Useful project paths
- `docker-compose.yml`
- `docker-compose.production.yml`
- `.env.example`
- `.env.production.example`
- `WebApiP33/Program.cs`
- `vue-front/nginx.conf`
- `vue-front/nginx.conf.template`

