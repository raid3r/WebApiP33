# AGENTS.md — WebApiP33

Messenger web app: **ASP.NET Core 8 API** + **Vue 3 / TypeScript** frontend + **MS SQL Server 2022**.

## Architecture Overview

```
vue-front/          Vue 3 + Vite SPA (TypeScript, Axios, Vue Router)
WebApiP33/          ASP.NET Core 8 Web API (.NET 8)
  Controllers/      AuthController, ChatController, ProfileController
  Models/           Domain entities + EF DbContext + DTOs
  Services/         IChatService / ChatService (business logic)
  Migrations/       EF Core migrations (single "Init" migration so far)
docker-compose.yml  Orchestrates db (MSSQL), api (:8080), frontend (:80)
```

### Critical dual-identity pattern
Every `User` (ASP.NET Identity, PK `int`) has an associated `Recipient` (messaging profile).
- `Recipient.Name` is set to `email` on registration.
- All chat operations use **Recipient IDs**, not User IDs.
- `ChatController` extracts `User.Id` from JWT claims → resolves `Recipient` internally via `ChatService`.
- `SendMessageRequestDto.RecipientId` → always a `Recipient.Id`, never a `User.Id`.
- `UserDto` carries both `Id` (User) and `RecipientId` (Recipient) to the frontend.

### Message polling
No WebSockets/SignalR. Frontend (`ChatPage.vue`) polls `GET /api/v1/chat/messages/{recipientId}` every **5000 ms** via `setTimeout`.

## Key API Endpoints

| Method | Route | Auth |
|--------|-------|------|
| POST | `/api/v1/auth/register` | — |
| POST | `/api/v1/auth/login` | — |
| GET  | `/api/v1/profile` | JWT |
| GET  | `/api/v1/chat/chats` | JWT |
| GET  | `/api/v1/chat/messages/{recipientId}` | JWT |
| POST | `/api/v1/chat/send` | JWT |
| POST | `/api/v1/chat/find-users` | JWT |

JWT: symmetric key from `JwtSettings:SecretKey`, no issuer/audience validation, `ClockSkew = 5 min`.

## Developer Workflows

### Local (Docker) — recommended path
```bash
cp .env.example .env          # fill DB_PASSWORD and JWT_SECRET (required)
docker compose up --build
# API → http://localhost:8082   Frontend → http://localhost:8081
```

### Backend only (Rider / dotnet CLI)
```bash
cd WebApiP33
dotnet run                    # uses appsettings.Development.json
# Swagger UI at https://localhost:7025/swagger
```
HTTP scratch files: `WebApiP33.http`, `ChatFlow.http`, `Controllers/auth.http`.

### Frontend only
```bash
cd vue-front
npm install
npm run dev                   # dev server on port 52356
```
Vite proxies `/api` → `https://localhost:7025` (backend must be running).

### Database migrations
```bash
cd WebApiP33
dotnet ef migrations add <Name>
dotnet ef database update
```
`ChatContext.OnConfiguring` has a hardcoded fallback connection string (local SQLEXPRESS) — only activates when no options are injected (e.g., running `dotnet ef` without env vars).

## Project Conventions

- **Constructor primary constructors** used in all controllers and services (C# 12):
  ```csharp
  public class ChatController(IChatService chatService) : ControllerBase { }
  ```
- **Route prefix**: all routes are `api/v1/<resource>`.
- **Service responses**: return domain DTOs directly (no `IActionResult` wrappers). Failures return empty collections or a DTO with `Success = false`.
- **Password policy**: minimal — `RequiredLength = 3`, no uppercase/digit/symbol requirements.
- **CORS**: `AllowAll` policy (any origin/method/header) — configured for development.
- **Frontend auth**: JWT stored in `localStorage['token']`; `src/services/api.ts` attaches it automatically as `Authorization: Bearer`.
- **Frontend type definitions**: DTOs re-declared as local interfaces inside `.vue` files (no shared types package).

## Key Files

| File | Purpose |
|------|---------|
| `WebApiP33/Program.cs` | DI wiring, JWT config, Identity setup, middleware order |
| `WebApiP33/Models/DAL/ChatContext.cs` | EF context — extends `IdentityDbContext<User, IdentityRole<int>, int>` |
| `WebApiP33/Services/ChatService.cs` | All chat business logic; resolves Recipient from User.Id |
| `vue-front/src/services/api.ts` | Axios instance with JWT interceptor |
| `vue-front/src/router/index.ts` | Route guards: `requiresAuth` / `guest` meta flags |
| `vue-front/src/pages/ChatPage.vue` | Main UI; contains polling loop and all chat state |

