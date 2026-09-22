# DormFlow — ລະບົບຄຸ້ມຄອງຫໍພັກ (Backend API)

ລະບົບການຄຸ້ມຄອງຫໍພັກ ແລະອາພາດເມັນຫຼາຍອົງກອນ (Multi-Organization SaaS)
ສ້າງດ້ວຍ ASP.NET Core Web API ສຸດ .NET 10 ອີງຕາມຄໍາແນະນໍາ **Modular Monolith + Clean Architecture**

> ສະຖານະປັດຈຸບັນ: **Phase 1 ສຳເລັດແລ້ວ** ແລະ **Phase 2 ສຳເລັດແລ້ວ**
> Phase 1 ສໍາເລັດ Identity, JWT, Refresh Token, Roles & Permissions ແລະ Endpoint ຈັດການ
> Organizations, Dormitories, Buildings, Floors, Room Types, Rooms, Users, User-Dormitory
> Assignment, Roles/Permissions ແລະ Audit Logs
> Phase 2 ສໍາເລັດ Tenants, Reservations, Contracts (ລວມເຊັກອິນ/ກວດເບິ່ງ), Meter Readings,
> Invoices, Payments, Maintenance (ວຽກງານແຈ້ງການບໍາລຸງຮັກສາ), Reports (ບົດລາຍງານລາຍຮັບ/ໃບແຈ້ງໜີ້ເກີນກຳນົດ),
> Deposits (ຝາກ), Receipts (ໃບຮັບເງິນ), Expenses (ຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ),
> Parcels (ພັດສະດຸ), Visitors (ນັກທ່ອງທ່ຽວ), Parking (ບ່ອນຈອດລົດ), Announcements (ປະກາດ),
> Notifications (ການແຈ້ງເຕືອນ) ແລະ Settings (ການຕັ້ງຄ່າອົງກອນ)
> ໂມດູນຂອງ Phase 2 ບ່ອນທີ່ກົດລະບຽບທຸລະກິດບໍ່ມີລາຍລະອຽດຢູ່ໃນເອກະສານຕົ້ນສະບັບ (ທຸກໆໂມດູນຈາກ Deposits ເປັນຕົ້ນໄປ)
> ມັນໄດ້ຖືກອອກແບບດ້ວຍການສົມມຸດຕິຖານທີ່ສົມເຫດສົມຜົນແລະເຫດຜົນທີ່ໄດ້ຮັບໃນຫົວຂໍ້
> [Phase 2 — ໂມດູນເສັ້ນ Contracts](#phase-2--ໂມດູນເສັ້ນ-contracts) **ຄວນໄດ້ຮັບການທົບທວນຄືນກັບເຈົ້າຂອງລະບົບກ່ອນທີ່ຈະໃຊ້ຕົວຈິງ**
> ພາຍໃຕ້ການພັດທະນາ Reports ຊ່ອງໂຫວ່ຂອງການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນບົດບາດຍັງໄດ້ຖືກພົບເຫັນແລະແກ້ໄຂ Owner ໃນ List Endpoint
> ສິ່ງອື່ນ 10 ໂມດູນ, ເບິ່ງລາຍລະອຽດໃນຫົວຂໍ້
> [ການແກ້ໄຂຊ່ອງໂຫວ່ໃນການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນ](#ການແກ້ໄຂຊ່ອງໂຫວ່ໃນການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນ-owner-ໃນ-list-endpoint)
> ເບິ່ງລາຍລະອຽດໃນຫົວຂໍ້ [ຂອບເຂດຂອງຂັ້ນຕອນ 1](#ຂອບເຂດຂອງຂັ້ນຕອນ-1)
> ແລະ [Phase 2 — ໂມດູນເສັ້ນ Contracts](#phase-2--ໂມດູນເສັ້ນ-contracts)

---

## ສາລະບານ

- [ເຕັກໂນໂລຊີທີ່ໃຊ້](#ເຕັກໂນໂລຊີທີ່ໃຊ້)
- [ໂຄງສ້າງ Solution](#ໂຄງສ້າງ-solution)
- [ທິດທາງຂອງການເພິ່ງພາອາໄສລະຫວ່າງ Layer](#ທິດທາງຂອງການເພິ່ງພາອາໄສລະຫວ່າງ-layer)
- [ສິ່ງທີ່ຕ້ອງຕິດຕັ້ງກ່ອນ](#ສິ່ງທີ່ຕ້ອງຕິດຕັ້ງກ່ອນ)
- [ເລີ່ມຕົ້ນໃນເຄື່ອງພັດທະນາຂອງທ່ານ](#ເລີ່ມຕົ້ນໃນເຄື່ອງພັດທະນາຂອງທ່ານ)
- [ແລ່ນມັນຄືກັນ Docker Compose](#ແລ່ນມັນຄືກັນ-docker-compose)
- [ການຕັ້ງຄ່າແລະຕົວແປສະພາບແວດລ້ອມ](#ການຕັ້ງຄ່າແລະຕົວແປສະພາບແວດລ້ອມ)
- [Endpoint ທີ່ສາມາດຖືກນໍາໃຊ້ໃນຂັ້ນຕອນນີ້](#endpoint-ທີ່ສາມາດຖືກນໍາໃຊ້ໃນຂັ້ນຕອນນີ້)
- [ຮູບແບບ API Response](#ຮູບແບບ-api-response)
- [ການທົດສອບ](#ການທົດສອບ)
- [Entity Framework Core ແລະ Migration](#entity-framework-core-ແລະ-migration)
- [Background Jobs](#background-jobs)
- [ສົນທິສັນຍາການເຂົ້າລະຫັດ](#ສົນທິສັນຍາການເຂົ້າລະຫັດ)
- [ຂອບເຂດຂອງຂັ້ນຕອນ 1](#ຂອບເຂດຂອງຂັ້ນຕອນ-1)
- [Phase 2 — ໂມດູນເສັ້ນ Contracts](#phase-2--ໂມດູນເສັ້ນ-contracts)

---

## ເຕັກໂນໂລຊີທີ່ໃຊ້

| ສ່ວນ | ເຕັກໂນໂລຊີ | ຮຸ່ນ |
| --- | --- | --- |
| Runtime | .NET | 10.0 |
| Web | ASP.NET Core Web API | 10.0 |
| ORM | Entity Framework Core | 10.0 |
| ຖານຂໍ້ມູນ | PostgreSQL (Npgsql provider) | 17 / 10.0 |
| ການຕັ້ງຊື່ຖັນ | EFCore.NamingConventions (snake_case) | 10.0 |
| Validation | FluentValidation | 12.1 |
| Logging | Serilog | 10.0 |
| ເອກະສານ API | Microsoft.AspNetCore.OpenApi + Scalar | 10.0 / 2.17 |
| API Versioning | Asp.Versioning | 10.2 |
| Test framework | xUnit v3 + FluentAssertions | 4.0 / 7.2 |
| Integration test | Testcontainers for PostgreSQL | 4.14 |
| Architecture test | NetArchTest.Rules | 1.3 |

> **ຝາຍເຫດກ່ຽວກັບສັນຍາໃບອະນຸຍາດ** — ໂຄງການ Freeze FluentAssertions ທີ່ **7.2.2**
> ເຊິ່ງເປັນລຸ້ນສຸດທ້າຍທີ່ໃຊ້ Apache-2.0 ນັບຕັ້ງແຕ່ 8.x ຕ້ອງຊື້ຕໍ່ໄປ license ສໍາລັບການນໍາໃຊ້ທາງການຄ້າ
> ຖ້າອົງກອນຊື້ license ຫຼັງຈາກນັ້ນ, ການປ່ຽນແປງສະບັບທີ່ `Directory.Packages.props` ບ່ອນດຽວ

---

## ໂຄງສ້າງ Solution

```
DormFlow.sln
├── global.json                  Pin Version SDK ແລະເປີດໃຊ້ Microsoft Testing Platform
├── Directory.Build.props        ການຕັ້ງຄ່າ MSBuild ແບ່ງປັນໂດຍທຸກໆໂຄງການ
├── Directory.Packages.props     Central Package Management (ສະບັບຊຸດແມ່ນຢູ່ບ່ອນດຽວ)
├── .editorconfig                ການຈັດຮູບແບບແລະການຕັ້ງຊື່ສົນທິສັນຍາ
├── docker-compose.yml           dormflow-api + dormflow-postgres
├── .env.example                 ຕົ້ນສະບັບຂອງໄຟລ໌ .env
│
├── src/
│   ├── Directory.Build.props            ເປີດ TreatWarningsAsErrors ລະຫັດເທົ່ານັ້ນ production
│   ├── DormFlow.Domain/                 Entity, Enum, Value Object, Domain Event, Business Rule
│   ├── DormFlow.Contracts/              Request / Response Model ໃຊ້ຮ່ວມກັບ Client
│   ├── DormFlow.Application/            Use Case, Command, Query, DTO, Interface, Validation
│   ├── DormFlow.Infrastructure/         EF Core, PostgreSQL, Repository, External Service
│   └── DormFlow.Api/                    Controller, Middleware, DI, OpenAPI, Health Check
│
└── tests/
    ├── Directory.Build.props
    ├── DormFlow.UnitTests/              ເຫດຜົນອັນບໍລິສຸດ, ບໍ່ມີການສໍາພັດ I/O
    ├── DormFlow.IntegrationTests/       ຍິງຜ່ານ HTTP ແທ້ຈິງແລ້ວ PostgreSQL ໃນ Testcontainers
    └── DormFlow.ArchitectureTests/      ການຂື້ນກັບໂດຍກົງລະຫວ່າງ Layer
```

---

## ທິດທາງຂອງການເພິ່ງພາອາໄສລະຫວ່າງ Layer

```
        Contracts  ◄──────────────┐
            ▲                     │
            │                     │
Domain ◄── Application ◄── Infrastructure ◄── Api
```

- **Domain** ບໍ່ເພິ່ງອາໄສຫຍັງ, ບໍ່ຮູ້ EF Core, ASP.NET Core ຫຼື Npgsql
- **Application** ພຽງແຕ່ສາມາດອີງໃສ່ Domain ແລະ Contracts
- **Infrastructure** ອີງໃສ່ Application ແມ່ນແລ້ວ, ແຕ່ຢ່າອີງໃສ່ມັນ Api
- **Api** ມັນເປັນຊັ້ນນອກທີ່ສຸດ. ສາມາດເອື່ອຍອີງໄດ້ໃນທຸກລະດັບ

ກົດລະບຽບເຫຼົ່ານີ້ຖືກບັງຄັບໃຊ້ໂດຍການທົດສອບຕົວຈິງ `tests/DormFlow.ArchitectureTests`
ການເພີ່ມອຸບັດຕິເຫດ `ProjectReference` ທິດທາງທີ່ບໍ່ຖືກຕ້ອງຈະເຮັດໃຫ້ການທົດສອບລົ້ມເຫຼວໃນທັນທີ

---

## ສິ່ງທີ່ຕ້ອງຕິດຕັ້ງກ່ອນ

| ເຄື່ອງມື | ຮຸ່ນ | ມັນໃຊ້ເພື່ອຫຍັງ? |
| --- | --- | --- |
| .NET SDK | 10.0.400 ຂຶ້ນ | build ແລະດໍາເນີນການໂຄງການ |
| Docker Desktop / Docker Engine | ລ່າສຸດ | ແລ່ນ PostgreSQL, ແລ່ນ compose ແລະ Integration Test |
| PostgreSQL | 17 | ໃຊ້ຜ່ານ Docker ບໍ່ຈໍາເປັນຕ້ອງຕິດຕັ້ງໃນເຄື່ອງ |

ກວດເບິ່ງສະບັບ SDK:

```bash
dotnet --version   # ຕ້ອງໄດ້ຮັບ 10.0.400 ຂຶ້ນ
```

> `global.json` ແຊ່ແຂງ SDK ທີ່ `10.0.400` ຕົວແບບ `rollForward: latestFeature`
> ຖ້າເຄື່ອງມີ SDK ຄໍາສັ່ງເກົ່າ `dotnet` ຈະແຈ້ງໃຫ້ທ່ານປັບປຸງກ່ອນ

---

## ເລີ່ມຕົ້ນໃນເຄື່ອງພັດທະນາຂອງທ່ານ

### 1. ກະກຽມຖານຂໍ້ມູນ

```bash
cp .env.example .env
# ແກ້ໄຂ POSTGRES_PASSWORD ໃນໄຟລ໌ .env ທໍາອິດ
docker compose up -d dormflow-postgres
```

ກວດສອບວ່າຖານຂໍ້ມູນມີຢູ່:

```bash
docker compose ps dormflow-postgres      # STATUS ຕ້ອງເປັນ healthy
```

### 2. Restore ແລະ Build

```bash
dotnet restore
dotnet build
```

### 3. ແລ່ນ API

```bash
dotnet run --project src/DormFlow.Api
```

ຈະໄດ້ຮັບ:

| ປາຍທາງ | URL |
| --- | --- |
| ເອກະສານ API ໂຕ້ຕອບ (Scalar) | http://localhost:5080/scalar/v1 |
| ເອກະສານ OpenAPI (JSON) | http://localhost:5080/openapi/v1.json |
| Health Check | http://localhost:5080/health |

Connection String ຂອງສິ່ງແວດລ້ອມ Development ຢູ່ໃນ `src/DormFlow.Api/appsettings.Development.json`
ແລະຊີ້ໃຫ້ເຫັນ PostgreSQL ຂອງ docker compose ຢູ່ໃນເຄື່ອງແລ້ວ

---

## ແລ່ນມັນຄືກັນ Docker Compose

```bash
cp .env.example .env
# ແກ້ໄຂ POSTGRES_PASSWORD, ConnectionStrings__DefaultConnection ແລະ Cors__AllowedOrigins__0

docker compose up -d --build
docker compose ps
docker compose logs -f dormflow-api
```

ກວດເບິ່ງສຸຂະພາບຂອງລະບົບ:

```bash
curl http://localhost:8080/health
curl http://localhost:8080/health/ready
curl http://localhost:8080/health/live
```

ຢຸດເຮັດວຽກ:

```bash
docker compose down            # stop container ແຕ່ມັນຍັງເກັບຮັກສາຂໍ້ມູນ
docker compose down -v         # ລຶບ volume ຂອງຖານຂໍ້ມູນໄດ້ຖືກຍົກເລີກເຊັ່ນດຽວກັນ
```

**ບໍລິການໃນ compose**

| Service | Image | ທ່າເຮືອ | Health Check |
| --- | --- | --- | --- |
| `dormflow-postgres` | `postgres:17-alpine` | `${POSTGRES_PORT:-5432}` | `pg_isready` |
| `dormflow-api` | build ຈາກ `src/DormFlow.Api/Dockerfile` | `${API_HTTP_PORT:-8080}` | `curl /health/live` |

`dormflow-api` ມັນຈະເລີ່ມເຮັດວຽກເມື່ອໃດ `dormflow-postgres` ບົດລາຍງານສະຖານະການ `healthy` ແລະຫຼັງຈາກນັ້ນເທົ່ານັ້ນ

> Health Check ຂອງ API ຊື່ສະເພາະ `/health/live` ຕັ້ງໃຈ
> ນີ້ຈະປ້ອງກັນບໍ່ໃຫ້ containers ຈາກການເລີ່ມຕົ້ນໃຝ່ພຽງແຕ່ເນື່ອງຈາກວ່າຖານຂໍ້ມູນ crashed ໃນເວລານີ້

---

## ການຕັ້ງຄ່າແລະຕົວແປສະພາບແວດລ້ອມ

ການຕັ້ງຄ່າທັງຝົດອ່ານຜ່ານ configuration ປົກກະຕິຂອງ ASP.NET Core
ກໍານົດກັບ environment variable ການນໍາໃຊ້ `__` ສໍາລັບຕົວຢ່າງ, ແທນທີ່ຈະເປັນຊັ້ນ `ConnectionStrings__DefaultConnection`

| ຕົວແປ | ຄ່າເລີ່ມຕົ້ນ | ລາຍລະອຽດ |
| --- | --- | --- |
| `ConnectionStrings__DefaultConnection` | *(ບໍ່ມີ)* | **ຈໍາເປັນ** ລະບົບຈະບໍ່ເລີ່ມຕົ້ນຖ້າບໍ່ໄດ້ລະບຸ |
| `ASPNETCORE_ENVIRONMENT` | `Production` | `Development` ເປີດເອກະສານ OpenAPI ແລະ Log ລະດັບ Debug |
| `Https__EnableRedirection` | `true` | ຕັ້ງເປັນ `false` ເມື່ອມີ reverse proxy ຈັດການ TLS ມອບໃຫ້ແລ້ວ |
| `Cors__AllowedOrigins__0` … | *(ຟຣີ)* | **ຕ້ອງການໃນ Production** ຖ້າບໍ່ໄດ້ລະບຸ, ລະບົບຈະບໍ່ເລີ່ມຕົ້ນ |
| `RateLimiting__Enabled` | `true` | ເປີດຫຼືປິດ Rate Limiting |
| `RateLimiting__PermitLimit` | `100` | ຈໍານວນການຮ້ອງຂໍຕໍ່ໄລຍະເວລາ |
| `RateLimiting__WindowSeconds` | `60` | ໄລຍະເວລາ |
| `Database__CommandTimeoutSeconds` | `30` | ເວລາສູງສຸດຂອງຄໍາສັ່ງຜຶ່ງ SQL |
| `Database__MaxRetryCount` | `3` | ຈໍານວນຄວາມພະຍາຍາມໃນການເຊື່ອມຕໍ່ຄືນໃຝ່ເມື່ອພົບຂໍ້ຜິດພາດຊົ່ວຄາວ |
| `Database__EnableSensitiveDataLogging` | `false` | **ຢ່າເປີດໃນ Production** ເຮັດໃຫ້ຕົວກໍານົດການ SQL ປະກົດຢູ່ໃນ Log |
| `Database__AutoMigrate` | `false` | ແລ່ນ Migration ທີ່ຕິດຢູ່ໂດຍອັດຕະໂນມັດໃນຕອນເລີ່ມຕົ້ນຂອງລະບົບ ເປີດພຽງແຕ່ຢູ່ໃນເຄື່ອງຂອງນັກພັດທະນາເທົ່ານັ້ນ |
| `Database__SeedOnStartup` | `true` | ສ້າງສິດພິເສດ, ບົດບາດ, ແລະບັນຊີຜູ້ເບິ່ງແຍງລະບົບໃນຕອນເລີ່ມຕົ້ນ (ສາມາດເຮັດວຽກຫຼາຍຄັ້ງໂດຍບໍ່ມີການຊໍ້າກັນ) |
| `Jwt__SigningKey` | *(ບໍ່ມີ)* | **ຈໍາເປັນ** ຢ່າງໜ້ອຍຕ້ອງມີຄວາມຍາວ 32 ຕົວອັກສອນທີ່ໃຊ້ສໍາລັບການເຊັນແລະການກວດສອບ Access Token |
| `Jwt__Issuer` | `DormFlow` | ຜູ້ອອກໂທເຄັນ |
| `Jwt__Audience` | `DormFlow.Client` | ຜູ້ຮັບໂທເຄັນ |
| `Jwt__AccessTokenMinutes` | `15` | ອາຍຸຂອງ Access Token |
| `Jwt__RefreshTokenDays` | `7` | ອາຍຸຂອງ Refresh Token |
| `SEED_ADMIN_EMAIL` | *(ບໍ່ມີ)* | ອີເມວຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນ ຖ້າບໍ່ໄດ້ລະບຸ, ການສ້າງບັນຊີນີ້ຈະຖືກຂ້າມໄປ |
| `SEED_ADMIN_PASSWORD` | *(ບໍ່ມີ)* | ລະຫັດຜ່ານຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນຕ້ອງຜ່ານນະໂຍບາຍລະຫັດຜ່ານດຽວກັນກັບຜູ້ໃຊ້ທົ່ວໄປ |
| `SEED_ADMIN_FIRST_NAME` / `SEED_ADMIN_LAST_NAME` | `System` / `Administrator` | ຊື່ຂອງບັນຊີຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນ |

### ຫຼັກການຂອງຄວາມລັບ

- `appsettings.json` **ບໍ່ມີ** ມູນຄ່າຄວາມລັບຂອງຊ່ອງທາງໃດກໍ່ຕາມ Connection String ແລະ `Jwt:SigningKey` ຖືກປະໄວ້ໂດຍເຈດຕະນາ
- `appsettings.Development.json` ມີພຽງແຕ່ລະຫັດຜ່ານສໍາລັບ PostgreSQL ແລະທີ່ສໍາຄັນ JWT ສໍາລັບເຄື່ອງພັດທະນາເທົ່ານັ້ນ
  ຄຸນຄ່າເຫຼົ່ານີ້ແມ່ນຕົວຈິງແລ້ວບໍ່ແມ່ນຄວາມລັບແລະກົງກັບຄ່າໃນ compose ຢ່າໃຊ້ຄ່າດຽວກັນໃນ Production ຢ່າງແທ້ຈິງ
- ຄວາມຈິງແລ້ວມູນຄ່າລັບໄດ້ຜ່ານໄປ environment variable, Docker Secret ຫຼື Secret Manager ເທົ່ານັ້ນ
- `.env` ເກັບຮັກສາໄວ້ໃນ `.gitignore` ແລະ `.dockerignore` ແລ້ວ
- ບັນຊີຜູ້ເບິ່ງແຍງລະບົບເລີ່ມຕົ້ນແມ່ນສ້າງຂຶ້ນຈາກ `SEED_ADMIN_*` ເທົ່ານັ້ນ, ບໍ່ Hardcode ລະຫັດຜ່ານຕ້ອງຖືກລວມເຂົ້າໃນລະຫັດ

---

## Endpoint ທີ່ສາມາດຖືກນໍາໃຊ້ໃນຂັ້ນຕອນນີ້

| Method | Path | ການອະນຸຍາດທີ່ຕ້ອງການ | ລາຍລະອຽດ |
| --- | --- | --- | --- |
| `GET` | `/health` | - | ຜົນການກວດກາທັງໝົດ |
| `GET` | `/health/live` | - | ກວດສອບວ່າຂະບວນການຍັງເຮັດວຽກຢູ່. ຢ່າແຕະໃສ່ຖານຂໍ້ມູນ |
| `GET` | `/health/ready` | - | ໃຫ້ແນ່ໃຈວ່າມັນເຊື່ອມຕໍ່ແລ້ວ PostgreSQL ເຈົ້າໄດ້ຮັບມັນບໍ? |
| `GET` | `/openapi/v1.json` | - | ເອກະສານ OpenAPI (ພຽງແຕ່ສະພາບແວດລ້ອມແມ່ນບໍ່ Production) |
| `GET` | `/scalar/v1` | - | ເອກະສານ API ໂຕ້ຕອບ (ພຽງແຕ່ສະພາບແວດລ້ອມແມ່ນບໍ່ Production) |
| `POST` | `/api/v1/auth/login` | ບໍ່ຈໍາເປັນຕ້ອງຢືນຢັນຕົວຕົນຂອງທ່ານ | ເຂົ້າສູ່ລະບົບດ້ວຍອີເມວ ແລະລະຫັດຜ່ານ |
| `POST` | `/api/v1/auth/refresh-token` | ບໍ່ຈໍາເປັນຕ້ອງຢືນຢັນຕົວຕົນຂອງທ່ານ | ຕໍ່ອາຍຸ Access Token ຄືກັນ Refresh Token (ມີ Token Rotation) |
| `POST` | `/api/v1/auth/logout` | ຕ້ອງເຂົ້າສູ່ລະບົບ | ຖອນຄືນ Refresh Token ສົ່ງ |
| `POST` | `/api/v1/auth/forgot-password` | ບໍ່ຈໍາເປັນຕ້ອງຢືນຢັນຕົວຕົນຂອງທ່ານ | ຂໍລິ້ງເພື່ອຣີເຊັດລະຫັດຜ່ານຂອງທ່ານຜ່ານທາງອີເມວ |
| `POST` | `/api/v1/auth/reset-password` | ບໍ່ຈໍາເປັນຕ້ອງຢືນຢັນຕົວຕົນຂອງທ່ານ | ຕັ້ງລະຫັດຜ່ານໃໝ່ດ້ວຍໂທເຄັນທີ່ໄດ້ຮັບຜ່ານທາງອີເມວ |
| `POST` | `/api/v1/auth/change-password` | ຕ້ອງເຂົ້າສູ່ລະບົບ | ປ່ຽນລະຫັດຜ່ານ (ຖອດຖອນທຸກເຊດຊັນທີ່ຜ່ານມາ) |
| `GET` | `/api/v1/auth/me` | ຕ້ອງເຂົ້າສູ່ລະບົບ | ຂໍ້ມູນຜູ້ໃຊ້ໃນປະຈຸບັນມີບົດບາດແລະສິດທັງຝົດ |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/organizations` | `organizations.*` | ຈັດການອົງກອນ (ສ້າງ/ສາມາດລຶບໄດ້ເທົ່ານັ້ນ SuperAdmin) |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/dormitories` | `dormitories.*` | ຄຸ້ມຄອງຫໍພັກ |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/buildings` | `buildings.*` | ຄຸ້ມຄອງອາຄານ |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/floors` | `floors.*` | ຈັດການຊັ້ນອາຄານ |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/room-types` | `room-types.*` | ຈັດການປະເພດຫ້ອງ |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/rooms` | `rooms.*` | ຈັດການຫ້ອງ |
| `GET` | `/api/v1/rooms/available` | `rooms.view` | ຫ້ອງຫວ່າງເປົ່າພ້ອມໃຫ້ເຊົ່າ |
| `GET` | `/api/v1/rooms/summary` | `rooms.view` | ສະຫຼຸບຂອງຈໍານວນຫ້ອງທີ່ແຍກຕາມສະຖານະ |
| `GET`/`POST`/`PUT`/`PATCH .../status` | `/api/v1/users` | `users.manage` | ຈັດການບັນຊີຜູ້ໃຊ້ |
| `POST` | `/api/v1/users/{id}/roles` | `users.manage` | ກໍານົດບົດບາດໃຫ້ຜູ້ໃຊ້ (ແທນທີ່ບົດບາດທີ່ຜ່ານມາທັງໝົດ) |
| `POST`/`DELETE` | `/api/v1/users/{id}/dormitories` | `users.manage` | ມອບໝາຍ/ຖອນຫໍພັກທີ່ຮັກສາໄວ້ໂດຍຜູ້ໃຊ້ |
| `GET`/`POST`/`PUT`/`DELETE` | `/api/v1/roles` | `roles.manage` | ຄຸ້ມຄອງບົດບາດ (ບົດບາດມາດຕະຖານປ່ຽນຊື່/ບໍ່ສາມາດລຶບໄດ້) |
| `POST` | `/api/v1/roles/{id}/permissions` | `roles.manage` | ມອບສິດໃຫ້ບົດບາດ (ປ່ຽນແທນສິດຕົ້ນສະບັບທັງໝົດ) |
| `GET` | `/api/v1/permissions` | `roles.manage` | ລາຍຊື່ການອະນຸຍາດລະບົບທັງໝົດ |
| `GET` | `/api/v1/audit-logs` | `audit-logs.view` | ປະຫວັດເຫດການຂອງລະບົບຈັດຮຽງຫຼ້າສຸດກ່ອນໂດຍຄ່າເລີ່ມຕົ້ນ |
| `GET`/`POST`/`PUT`/`PATCH .../status` | `/api/v1/tenants` | `tenants.*` | ຈັດການຂໍ້ມູນຜູ້ເຊົ່າ (ບໍ່ມີ DELETE ຕາມສະເພາະ, ສິດໃນການລົບແມ່ນບໍ່ໄດ້ລະບຸໄວ້) |
| `GET`/`POST` | `/api/v1/reservations` | `reservations.*` | ຈອງຫ້ອງ (ຫ້ອງປ່ຽນເປັນ Reserved ທັນທີ) |
| `POST` | `/api/v1/reservations/{id}/cancel` | `reservations.cancel` | ຍົກເລີກການຈອງ. ຫ້ອງກັບຄືນໄປຫາ Available ຖ້າມັນບໍ່ໄດ້ຖືກໃຊ້ໃນທາງອື່ນ |
| `GET`/`POST` | `/api/v1/contracts` | `contracts.*` | ສ້າງສັນຍາເຊົ່າ (ທ່ານສາມາດໄປໂດຍຜ່ານການຈອງຫຼືເຮັດສັນຍາໂດຍກົງ) |
| `POST` | `/api/v1/contracts/{id}/activate` | `contracts.activate` | ເປີດໃຊ້ສັນຍາ = ເຊັກອິນ. ຫ້ອງປ່ຽນເປັນ Occupied |
| `POST` | `/api/v1/contracts/{id}/terminate` | `contracts.terminate` | ສິ້ນສຸດສັນຍາ = ກວດເບິ່ງ. ຫ້ອງປ່ຽນເປັນ Cleaning |
| `GET`/`POST` | `/api/v1/meter-readings` | `meter-readings.*` | ຂຽນຄ່າເຄື່ອງວັດແທກນໍ້າ/ໄຟຟ້າລາຍເດືອນ (ບໍ່ມີ UPDATE/DELETE ມັນເປັນບັນທຶກປະຫວັດສາດ) |
| `GET`/`POST` | `/api/v1/invoices` | `invoices.*` | ສ້າງໃບແຈ້ງໜີ້ໃນສະຖານະສະບັບຮ່າງ. ຄິດໄລ່ຄ່ານ້ຳ-ໄຟຟ້າຈາກການອ່ານແມັດອັດຕະໂນມັດ |
| `POST` | `/api/v1/invoices/{id}/issue` | `invoices.issue` | ອອກໃບແຈ້ງໜີ້ໃຫ້ຜູ້ເຊົ່າ (Draft → Issued) |
| `POST` | `/api/v1/invoices/{id}/cancel` | `invoices.cancel` | ຍົກເລີກໃບແຈ້ງໜີ້ (ນີ້ສາມາດເຮັດໄດ້ພຽງແຕ່ຖ້າບໍ່ມີການຈ່າຍເງິນເທື່ອ) |
| `GET`/`POST` | `/api/v1/payments` | `payments.*` | ບັນທຶກທຸລະກໍາການຈ່າຍເງິນຢູ່ໃນສະຖານະລໍຖ້າການຢືນຢັນ. ຍັງບໍ່ມີຜົນຕໍ່ກັບຈຳນວນໃບແຈ້ງໜີ້ເທື່ອ |
| `POST` | `/api/v1/payments/{id}/confirm` | `payments.approve` | ຢືນຢັນລາຍການການຈ່າຍເງິນ ເອົາຈໍານວນທີ່ຈະຫັກໃບແຈ້ງຜີ້ແລະປັບສະຖານະ |
| `POST` | `/api/v1/payments/{id}/refund` | `payments.refund` | ເງິນຄືນ (ພຽງແຕ່ລາຍການທີ່ຢືນຢັນ) ຈຳນວນດັ່ງກ່າວຖືກຫັກອອກຈາກໃບແຈ້ງໜີ້ການຄືນເງິນ |
| `GET`/`POST` | `/api/v1/maintenance-requests` | `maintenance.view` / `maintenance.update-status` | ແຈ້ງການບໍາລຸງຮັກສາ (ສ້າງໃຊ້ການອະນຸຍາດດຽວກັນກັບການປ່ຽນແປງສະຖານະ. ບໍ່ມີການອະນຸຍາດ "create" ແຍກຕ່າງຫາກ) |
| `POST` | `/api/v1/maintenance-requests/{id}/assign` | `maintenance.assign` | ມອບໝາຍ/Reassign ຊ່າງບໍາລຸງຮັກສາຫ້ອງປ່ຽນເປັນ Maintenance ຖ້າມັນຍັງມີຢູ່ |
| `PATCH` | `/api/v1/maintenance-requests/{id}/status` | `maintenance.update-status` | ປ່ຽນສະຖານະຕາມລຳດັບທີ່ກຳນົດໄວ້ (ຢ່າຕັ້ງມັນເປັນ Assigned ກົງ) ວຽກປິດແລ້ວ ແລະຫ້ອງຈະກັບມາຫວ່າງເປົ່າ ຖ້າບໍ່ມີວຽກອື່ນຄ້າງຢູ່ |
| `GET` | `/api/v1/reports/revenue` | `reports.view` | ສະຫຼຸບຂອງຈໍານວນໃບບິນ/ສາມາດປະຫຍັດເງິນໄດ້/ເກີນກຳນົດໃນໄລຍະທີ່ກຳນົດ |
| `GET` | `/api/v1/reports/revenue/export` | `reports.export` | ສົ່ງອອກບົດລາຍງານສະຫຼຸບລາຍຮັບເພື່ອຍື່ນ CSV |
| `GET` | `/api/v1/reports/outstanding-invoices` | `reports.view` | ລາຍຊື່ໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງຢູ່ໃນບັນຊີທີ່ຍັງເຫຼືອ ແລະຈຳນວນມື້ທີ່ຄົບກຳນົດ |
| `GET` | `/api/v1/reports/outstanding-invoices/export` | `reports.export` | ສົ່ງອອກບົດລາຍງານໃບແຈ້ງໜີ້ທີ່ຍັງຄ້າງຄາເປັນໄຟລ໌ CSV (ສູງສຸດ 5,000 ແຖວ) |
| `GET`/`POST` | `/api/v1/deposits` | `deposits.view` / `deposits.collect` | ຮັບເງິນຝາກ (ສັນຍາຜຶ່ງສາມາດໄດ້ຮັບພຽງແຕ່ຄັ້ງດຽວ. ສັນຍາຈະຕ້ອງຍັງບໍ່ທັນສິ້ນສຸດເທື່ອ) |
| `POST` | `/api/v1/deposits/{id}/refund` | `deposits.refund` | ຝາກເງິນຄືນ (ເຕັມ/ບາງ/ປະຖິ້ມທັງຝົດ) ນີ້ສາມາດເຮັດໄດ້ພຽງແຕ່ໃນສັນຍາທີ່ໄດ້ສິ້ນສຸດລົງແລ້ວ. ແລະສາມາດສົ່ງຄືນໄດ້ພຽງແຕ່ຄັ້ງດຽວຕໍ່ລາຍການ |
| `GET`/`POST` | `/api/v1/receipts` | `receipts.view` / `receipts.issue` | ອອກໃບຮັບເງິນ (ພຽງແຕ່ຢືນຢັນທຸລະກໍາການຈ່າຍເງິນຜຶ່ງຄັ້ງຕໍ່ລາຍການ) |
| `GET`/`POST` | `/api/v1/expenses` | `expenses.view` / `expenses.create` | ບັນທຶກຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ (ໃບບິນຄ່ານໍ້າ-ແສງກາງ, ບໍາລຸງຮັກສາ, ເງິນເດືອນ ແລະ ອື່ນໆ) |
| `GET`/`POST` | `/api/v1/parcels` | `parcels.view` / `parcels.log` | ບັນທຶກ parcels ທີ່ມາຮອດຫໍພັກ |
| `POST` | `/api/v1/parcels/{id}/pickup` | `parcels.pickup` | ບັນທຶກຂອງການຮັບການພັດທະນາ (ນີ້ສາມາດເຮັດໄດ້ພຽງແຕ່ສໍາລັບ parcels ທີ່ຍັງບໍ່ທັນໄດ້ຮັບ) |
| `GET`/`POST` | `/api/v1/visitors` | `visitors.view` / `visitors.checkin` | ບັນທຶກການເຂົ້າຂອງນັກທ່ອງທ່ຽວ |
| `POST` | `/api/v1/visitors/{id}/checkout` | `visitors.checkout` | ບັນທຶກຜູ້ເຂົ້າຊົມ (ເຈົ້າສາມາດເຮັດໄດ້ພຽງແຕ່ຖ້າມັນຍັງບໍ່ໄດ້ປ່ອຍອອກມາເທື່ອ) |
| `GET`/`POST` | `/api/v1/parking` | `parking.view` / `parking.register` | ການລົງທະບຽນບ່ອນຈອດລົດ (ປ້າຍທະບຽນລົດດຽວກັນບໍ່ສາມາດວາງຊ້ອນກັນໄດ້ ໃນຂະນະທີ່ປ້າຍທະບຽນລົດຍັງບໍ່ທັນໄດ້ສໍາເລັດ) |
| `POST` | `/api/v1/parking/{id}/release` | `parking.release` | ສິ້ນສຸດການລົງທະບຽນບ່ອນຈອດລົດ |
| `GET`/`POST` | `/api/v1/announcements` | `announcements.view` / `announcements.create` | ສ້າງການປະກາດ (ສະເຝີສະຖານະຮ່າງ) |
| `POST` | `/api/v1/announcements/{id}/publish` | `announcements.publish` | ປະກາດ (ມີພຽງແຕ່ການປະກາດທີ່ຍັງຮ່າງເທົ່ານັ້ນທີ່ສາມາດເຮັດໄດ້) |
| `POST` | `/api/v1/announcements/{id}/archive` | `announcements.publish` | ປະກາດການເກັບຮັກສາໃນສາງ (ມີພຽງແຕ່ການປະກາດທີ່ໄດ້ຖືກເຜີຍແຜ່ແລ້ວເທົ່ານັ້ນທີ່ສາມາດເຮັດໄດ້) |
| `GET` | `/api/v1/notifications` | ຕ້ອງເຂົ້າສູ່ລະບົບ | ເບິ່ງການແຈ້ງເຕືອນຂອງທ່ານເອງ (ບໍ່ຈໍາເປັນຕ້ອງມີສິດສະເພາະເຊັ່ນ `/auth/me`) |
| `POST` | `/api/v1/notifications/{id}/read` | ຕ້ອງເຂົ້າສູ່ລະບົບ | ໝາຍການແຈ້ງເຕືອນຂອງທ່ານວ່າອ່ານແລ້ວ |
| `POST` | `/api/v1/notifications` | `notifications.send` | ສົ່ງການແຈ້ງເຕືອນໃຫ້ຜູ້ໃຊ້ໜຶ່ງຄົນໃນອົງກອນດຽວກັນ |
| `GET`/`PUT .../{key}`/`DELETE .../{key}` | `/api/v1/settings` | `settings.manage` | ຈັດການການຕັ້ງຄ່າອົງກອນຂອງທ່ານ Key/Value ເອກະລາດ (ສ້າງ/ແກ້ໄຂຜ່ານ `PUT` ດຽວກັນ) |

ທຸກໆ List Endpoint ສະຜັບສະຜູນ `page`, `pageSize`, `search`, `sortBy`, `sortDirection` ຮ່ວມກັນ
ແລະ Endpoint ບ່ອນທີ່ຂໍ້ມູນຖືກຜູກມັດກັບອົງກອນ/ຫໍພັກສະຜັບສະຜູນການກັ່ນຕອງ `organizationId`/`dormitoryId` ເພີ່ມເຕີມ
ບໍ່ແມ່ນຜູ້ໃຊ້ SuperAdmin ສະເຝີຖືກຈໍາກັດໃຫ້ເຫັນພຽງແຕ່ຂໍ້ມູນອົງກອນຂອງທ່ານໃນຊັ້ນວາງ Application
ໂດຍບໍ່ຄໍານຶງເຖິງມູນຄ່າສິ່ງທີ່ຄໍາຮ້ອງຂໍກໍານົດສໍາລັບການກັ່ນຕອງເຫຼົ່ານີ້

ຕົວຢ່າງຜົນໄດ້ຮັບຂອງ `GET /health`:

```json
{
  "status": "Healthy",
  "totalDurationMs": 12.34,
  "checks": [
    { "name": "self", "status": "Healthy", "durationMs": 0.01, "description": "API ເຮັດວຽກ" },
    { "name": "postgresql", "status": "Healthy", "durationMs": 11.9, "description": null }
  ]
}
```

---

## ຮູບແບບ API Response

ທຸກໆ Endpoint ຂອງ DormFlow ຕອບກັບໂຄງສ້າງດຽວກັນ, ກໍານົດຢູ່ທີ່
`src/DormFlow.Contracts/Common/ApiResponse.cs`

**ປະສົບຜົນສໍາເລັດ**

```json
{ "success": true, "message": "ສຳເລັດແລ້ວ", "data": {} }
```

**ສໍາເລັດດ້ວຍຂໍ້ມູນ pagination**

```json
{
  "success": true,
  "message": "ດຶງຂໍ້ມູນສຳເລັດແລ້ວ",
  "data": {
    "items": [],
    "page": 1,
    "pageSize": 20,
    "totalItems": 100,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

**ຂໍ້ມູນດັ່ງກ່າວຍັງບໍ່ໄດ້ຮັບການຢືນຢັນ (400)**

```json
{
  "success": false,
  "message": "ຂໍ້ມູນບໍ່ຖືກຕ້ອງ",
  "errors": { "name": ["ກະລຸນາລະບຸຊື່ຂອງເຈົ້າ"] },
  "traceId": "00-..."
}
```

**ສະຖານະການຜິດພາດຖືກແປງໂດຍອັດຕະໂນມັດ**

| Exception | HTTP | ຂໍ້ຄວາມ |
| --- | --- | --- |
| `ValidationFailedException`, `FluentValidation.ValidationException` | 400 | ຂໍ້ມູນບໍ່ຖືກຕ້ອງ |
| `UnauthorizedAccessException` | 401 | ກະລຸນາເຂົ້າສູ່ລະບົບ |
| `ForbiddenAccessException` | 403 | ທ່ານບໍ່ໄດ້ຮັບອະນຸຍາດໃຫ້ເຮັດລາຍການນີ້ |
| `NotFoundException` | 404 | ບໍ່ພົບຂໍ້ມູນທີ່ຕ້ອງການ |
| `ConflictException`, `DomainException` | 409 | ຂໍ້ຄວາມຈາກ exception |
| `DbUpdateConcurrencyException` | 409 | ຂໍ້ມູນໄດ້ຖືກແກ້ໄຂໂດຍຜູ້ໃຊ້ອື່ນ |
| ອື່ນໆທັງຝົດ | 500 | ເກີດຄວາມຜິດພາດພາຍໃນລະບົບ |

`traceId` ເຊິ່ງຖືກສົ່ງກັບຄືນໄປຫາການແຂ່ງຂັນ `TraceId` ບັນທຶກໃນ Log ໃຊ້ມັນເພື່ອຊອກຫາສາເຫດຂອງຮາກທັນທີ
ຂໍ້ຄວາມຂອງ exception ລາຍການທີ່ບໍ່ຄາດຄິດຈະປາກົດຢູ່ໃນ Response ສະພາບແວດລ້ອມສະເພາະ Development ເທົ່ານັ້ນ

ຕົວກໍານົດການມາດຕະຖານຂອງ List Endpoint ແຕ່ລະຄົນຂອງພວກເຂົາ: `page`, `pageSize`, `search`, `sortBy`, `sortDirection`
ບວກ `organizationId` ແລະ `dormitoryId` ສໍາລັບຂໍ້ມູນທີ່ແຍກອອກໂດຍອົງກອນ
`pageSize` ແມ່ນຖືກຈຳກັດບໍ່ໃຫ້ເກີນ 200 ລາຍຊື່ສະເໝີ

---

## ການທົດສອບ

### ດໍາເນີນການທົດສອບທັງຝົດ

```bash
dotnet msbuild -t:Test
```

ດໍາເນີນໂຄງການສະເພາະ:

```bash
dotnet msbuild tests/DormFlow.UnitTests/DormFlow.UnitTests.csproj -t:Test
dotnet msbuild tests/DormFlow.ArchitectureTests/DormFlow.ArchitectureTests.csproj -t:Test
dotnet msbuild tests/DormFlow.IntegrationTests/DormFlow.IntegrationTests.csproj -t:Test
```

> **ເປັນຫຍັງເຈົ້າບໍ່ໃຊ້ມັນ? `dotnet test`**
> ການທົດສອບການນໍາໃຊ້ໂຄງການ xUnit v3 ສຸດ Microsoft Testing Platform ອີງຕາມມາດຕະຖານຂອງ .NET 10
> ແຕ່ orchestrator ຂອງ `dotnet test` ໃນ SDK 10.0.400 ຍັງເຊື່ອມຕໍ່ກັບ Microsoft.Testing.Platform 2.3.3 ບໍ່ສໍາເລັດ
> ແລະລາຍງານວ່າ `Zero tests ran` ເຖິງແມ່ນວ່າຜູ້ທົດສອບເຮັດວຽກຕາມປົກກະຕິ
> ເປົ້າໝາຍ `-t:Test` ໂທຫາການປະຕິບັດດຽວກັນໂດຍຜ່ານ MSBuild ແລະສາມາດເຮັດວຽກໄດ້ຢ່າງຖືກຕ້ອງ
> ເມື່ອໃດ SDK ສະບັບຕໍ່ໄປແກ້ໄຂບັນຫານີ້ `dotnet test` ມັນສາມາດນໍາໃຊ້ໄດ້ທັນທີໂດຍບໍ່ຕ້ອງແກ້ໄຂໂຄງການ

ນອກນັ້ນທ່ານຍັງສາມາດດໍາເນີນການທົດສອບໄຟລ໌ປະຕິບັດໄດ້ໂດຍກົງ (ແຕ່ລະໂຄງການທົດສອບ compile ແມ່ນ executable):

```bash
dotnet run --project tests/DormFlow.UnitTests
```

### ຂອບເຂດຂອງການທົດສອບໃນຂັ້ນຕອນນີ້

| ໂຄງການ | ປະລິມານ | ກວມເອົາ |
| --- | --- | --- |
| `DormFlow.UnitTests` | 132 | ຮູບແບບ API Response, Pagination, ການແປງຜິດພາດ Validation, BaseEntity, Domain Exception, ເຫດຜົນຂອງການອະນຸຍາດ `ICurrentUser`, ວົງຈອນຊີວິດ Refresh Token, ລາຍຊື່ການອະນຸຍາດແລະການອະນຸຍາດເລີ່ມຕົ້ນສໍາລັບແຕ່ລະບົດບາດ, ກົດລະບຽບ Validation ຂອງ Auth ແລະລາຍການຫໍພັກ, ການສ້າງ/ແຮຊ Refresh Token, `TenantScope` (ເຫດຜົນເພື່ອປົກປ້ອງຂໍ້ມູນໃນທົ່ວອົງກອນ/ຫໍພັກແບ່ງປັນໂດຍທຸກໂມດູນ) |
| `DormFlow.ArchitectureTests` | 11 | ທິດທາງຂອງການເພິ່ງພາອາໄສລະຫວ່າງ Layer, ຕໍາແຜ່ງຂອງ `DbContext`, ສົນທິສັນຍາການຕັ້ງຊື່ |
| `DormFlow.IntegrationTests` | 107 | Health Check ທັງສາມເສັ້ນທາງ, ເອກະສານ OpenAPI, JWT Bearer Security Scheme, ເສັ້ນທາງທີ່ບໍ່ມີຢູ່ແທ້ໆ, Login ປະສົບຜົນສໍາເລັດ/ລົ້ມເຫຼວ, `/auth/me`, Refresh Token Rotation, ການກວດສອບການນໍາໃຊ້ Token ເຮັດຊ້ຳ, Logout, Change Password, Forgot Password, ລຳດັບຊັບສິນທັງໝົດ (Organization→Dormitory→Building→Floor→RoomType→Room), Rooms available/summary, ການປົກປ້ອງຂໍ້ມູນທົ່ວອົງກອນ, CRUD ຜູ້ໃຊ້ + ກໍານົດບົດບາດ + ມອບໝາຍ/ຖອນຫໍພັກ, ບົດບາດມາດຕະຖານປ່ຽນຊື່/ບໍ່ສາມາດລຶບໄດ້, Audit Log, ວົງຈອນຊີວິດການເຊົ່າເຕັມ (Tenant→Reservation→Contract→Activate→Terminate ພ້ອມກວດສອບສະຖານະຫ້ອງທຸກຈຸດ), ການຈອງຊໍ້າກັນ ແລະຜູ້ເຊົ່າບັນຊີດໍາຈະຕ້ອງຖືກປະຕິເສດ, ບັນທຶກຄ່າແມັດ (ດຶງເອົາຄ່າທີ່ຜ່ານມາໂດຍອັດຕະໂນມັດ, ຢ່າບັນທຶກຊ້ຳໆໃນເດືອນດຽວກັນ, ຢ່າຂຽນຄ່າຕ່ໍາກວ່າເວລາທີ່ຜ່ານມາ), ວົງຈອນຊີວິດການຮຽກເກັບເງິນ (MeterReading→Invoice→Payment ເສັ້ນຄົບຖ້ວນ, ການຈ່າຍເງິນບາງສ່ວນແລະການຄືນເງິນ, ຫ້າມອອກໃບບິນຄ່າຊໍ້າກັນໃນເດືອນດຽວກັນ, ຢ່າຍົກເລີກໃບບິນທີ່ຈ່າຍແລ້ວ, ຢ່າຢືນຢັນຍອດເງິນເກີນຍອດຄົງເຫຼືອ), ວົງຈອນຊີວິດຂອງວຽກງານບໍາລຸງຮັກສາ (Report→Assign→InProgress→Complete ມີຫ້ອງປ່ຽນສະຖານະອັດຕະໂນມັດ, ມອບໝາຍຄືນໃໝ່, ຫ້ອງມີສອງວຽກງານສ້ອມແປງ, ປະຕິເສດນັກວິຊາການທີ່ບໍ່ມີຄຸນສົມບັດ/ຫໍພັກນີ້ບໍ່ໄດ້ຖືກຮັກສາໄວ້, ຢ່າຕັ້ງສະຖານະ Assigned ກົງ, ຢ່າຂ້າມຂັ້ນຕອນ, ຫ້ອງທີ່ຖືກຄອບຄອງໂດຍຜູ້ເຊົ່າໄດ້ຖືກປະໄວ້ untouched), ການປົກປ້ອງຂໍ້ມູນທົ່ວອົງກອນ Owner
ໃນ List Endpoint (Rooms, Invoices, MaintenanceRequests) — Regression Test ຂອງຊ່ອງໂຫວ່ທີ່ໄດ້ພົບເຫັນແລະການແກ້ໄຂ,
ບົດລາຍງານສະຫຼຸບລາຍຮັບແລະລາຍງານໃບເກັບເງິນທີ່ຍັງຄ້າງຄາ (ຄິດໄລ່ຈໍານວນທີ່ຖືກຕ້ອງ, ກັ່ນຕອງຕາມຫໍພັກ, ສົ່ງອອກ CSV ບົດລາຍງານທັງສອງ),
ວົງຈອນຊີວິດການຝາກເງິນ (ໄດ້ເງິນ→ສິ້ນສຸດສັນຍາ→ເງິນຄືນເຕັມ/ບາງ/ປະຖິ້ມທັງຝົດ, ບໍ່ຍອມຮັບອີກເທື່ອຜຶ່ງ, ຢ່າກັບຄືນກ່ອນທີ່ສັນຍາຈະສິ້ນສຸດລົງ,
ຢ່າສົ່ງຄືນຫຼາຍກ່ວາຈໍານວນທັງຝົດ, ບັງຄັບໃຫ້ລະບຸເຫດຜົນສໍາລັບການບໍ່ຄົບຖ້ວນໃນຄືນທີ່ຜ່ານມາ, ຢ່າກັບຄືນມາອີກ), ການອອກໃບຮັບເງິນ (ລາຍການການຈ່າຍເງິນເທົ່ານັ້ນ
ຢືນຢັນ, ຫ້າມອອກໃໝ່, ຢ່າອອກລາຍການທີ່ບໍ່ໄດ້ຮັບການຢືນຢັນຫຼືຄືນເງິນ), ບັນທຶກຄ່າໃຊ້ຈ່າຍ
(ກັ່ນຕອງຕາມໝວດໝູ່ ແລະຊ່ວງວັນທີ, ປະຕິເສດໝວດໝູ່ທີ່ບໍ່ຖືກຕ້ອງ), ວົງຈອນຊີວິດ parcel (ໄດ້ຮັບບັນທຶກ→ຮັບເຄື່ອງ,
ບໍ່ຍອມຮັບອີກເທື່ອຜຶ່ງ), ວົງຈອນຊີວິດຂອງນັກທ່ອງທ່ຽວ (ເຂົ້າ→ອອກ, ຫ້າມອອກໃໝ່), ວົງຈອນຊີວິດການລົງທະບຽນບ່ອນຈອດລົດ (ລົງທະບຽນ→ສິ້ນສຸດ,
ຫ້າມໃສ່ປ້າຍທະບຽນລົດຫຼາຍຄັນສຳລັບພາຫະນະດຽວກັນ, ທ່ານສາມາດລົງທະບຽນຄືນໃຝ່ໄດ້ຫຼັງຈາກຕົ້ນສະບັບໄດ້ສິ້ນສຸດລົງ), ວົງຈອນຊີວິດປະກາດ
(ສ້າງ (ແຕ້ມຮູບ)→ເຜີຍແຜ່→ເອົາເຂົ້າໄປໃນບ່ອນເກັບຮັກສາ, ຫ້າມເຜີຍແຜ່ໃໝ່, ຢ່າເກັບຮັກສາມັນໄວ້ໃນບ່ອນເກັບມ້ຽນກ່ອນທີ່ຈະເຜີຍແຜ່), ການແຈ້ງເຕືອນ
(ສົ່ງໃຫ້ຜູ້ໃຊ້→ປາກົດຢູ່ໃນລາຍຊື່ຜູ້ຮັບ, ໝາຍຂອງເຈົ້າວ່າອ່ານແລ້ວ, ຢ່າອ່ານໃນນາມຂອງຄົນອື່ນ,
ຢ່າສົ່ງທົ່ວອົງກອນ), ການຕັ້ງຄ່າອົງກອນ (ກໍ່ສ້າງຄືນໃໝ່, ແກ້ໄຂຄ່າຕົ້ນສະບັບ, ບໍ່ສ້າງຊ້ໍາກັນ, ລຶບ ແລະສືບຕໍ່ອ່ານ 404,
ລະຫັດດຽວກັນສາມາດຖືກສ້າງອີກເທື່ອຜຶ່ງຫຼັງຈາກການລຶບ, ແຍກຂໍ້ມູນຕາມອົງກອນ), Background Job (ການຈອງໝົດອາຍຸດຽວນີ້ ExpiresAt ແລ້ວປ່ອຍຫ້ອງຄືນ,
ຢ່າແຕະຕ້ອງຈອງທີ່ຍັງບໍ່ທັນໝົດອາຍຸເທື່ອ, ຢ່າແຕະຕ້ອງຫ້ອງທີ່ຖືກປ່ຽນເປັນສະຖານະອື່ນ, ໝາຍໃບແຈ້ງໜີ້ວ່າໝົດກຳນົດ
Overdue ພ້ອມຄິດໄລ່ຄ່າປັບໃໝຄົງທີ່ ແລະສ່ວນຮ້ອຍ, ຢ່າແຕະຕ້ອງໃບບິນຄ່າທີ່ຍັງບໍ່ທັນຮອດກຳນົດ, Rerun ບໍ່ໄດ້ເພີ່ມການປັບໃໝຊໍ້າກັນ) |

Integration Test ໃຊ້ **PostgreSQL ຄວາມຈິງ** ຜ່ານ Testcontainers ຢ່າໃຊ້ InMemory Database
ເນື່ອງຈາກວ່າພຶດຕິກໍາທີ່ສໍາຄັນຈໍານວນຫຼາຍເຊັ່ນ: Global Query Filter, `timestamptz`, `jsonb`
ແລະ Optimistic Concurrency ກັບຖັນ `xmin` ບໍ່ມີຢູ່ໃນ InMemory Provider

> Integration Test ຕ້ອງການ Docker ທີ່ເຮັດວຽກຖ້າຫາກວ່າ Docker ບໍ່ເຮັດວຽກ. ການທົດສອບນີ້ຈະລົ້ມເຫຼວໃນເວລາສ້າງ container

---

## Entity Framework Core ແລະ Migration

ມີ Migration ທໍາອິດ (`InitialCreate`) ກວມເອົາຕາຕະລາງຂອງ ASP.NET Core Identity
(`users`, `roles`, `user_roles`, `user_claims`, `user_logins`, `user_tokens`, `role_claims`)
ແລະຕາຕະລາງທຸລະກິດຂອງຂັ້ນຕອນນີ້ (`organizations`, `permissions`, `role_permissions`, `refresh_tokens`, `audit_logs`)

ຕິດຕັ້ງເຄື່ອງມື (ຜຶ່ງຄັ້ງຕໍ່ອຸປະກອນ):

```bash
dotnet tool install --global dotnet-ef
```

ຄໍາສັ່ງທີ່ໃຊ້ໃນການຄຸ້ມຄອງ Migration:

```bash
# ສ້າງ Migration ໃໝ່
dotnet ef migrations add ຊື່Migration \
  --project src/DormFlow.Infrastructure \
  --startup-project src/DormFlow.Infrastructure

# ປັບຖານຂໍ້ມູນໃຫ້ກົງກັນ Migration ລ່າສຸດ
dotnet ef database update \
  --project src/DormFlow.Infrastructure \
  --startup-project src/DormFlow.Infrastructure
```

> **ເປັນຫຍັງ? `--startup-project` ຊີ້ໄປ `DormFlow.Infrastructure` ບໍ່ `DormFlow.Api`**
> ເຄື່ອງມື `dotnet ef` ຕ້ອງການໃຫ້ Startup Project ການອ້າງອີງແພັກເກດ `Microsoft.EntityFrameworkCore.Design` ໂດຍກົງ
> ເຊິ່ງ `DormFlow.Infrastructure` ມີຢູ່ແລ້ວ (ຈໍາເປັນສໍາລັບ `DormFlowDbContextFactory`)
> ສ່ວນ `DormFlow.Api` ຊຸດນີ້ບໍ່ໄດ້ຖືກລວມເອົາເພາະວ່າເຄື່ອງມືບໍ່ຄວນຮູ້ຈັກ Migration ໂດຍກົງ
> `DormFlowDbContextFactory` ອ່ານ Connection String ຈາກ environment variable `ConnectionStrings__DefaultConnection`
> ເມື່ອບໍ່ໄດ້ລະບຸ, ຄ່າເລີ່ມຕົ້ນຂອງ docker compose ໃນເຄື່ອງພັດທະນາແທນ

> **ຂໍ້ຄວນລະວັງໃນເວລາສ້າງ Migration ດ້ວຍຕົວທ່ານເອງ**
> `DormFlowDbContextFactory` ຕາຕະລາງປະຫວັດສາດຕ້ອງມີຊື່ Migration ຄືກັນ `DormFlowDbContext.MigrationsHistoryTableName`
> ກົງກັບວ່າ `DependencyInjection.AddPersistence` ໃຊ້ມັນສະເໝີເມື່ອແອັບກຳລັງເຮັດວຽກຢູ່ (ໃນປັດຈຸບັນ, ທັງສອງຈຸດແມ່ນຖືກກໍານົດ)
> ຖ້າຄ່າບໍ່ກົງກັນ `dotnet ef` ມັນຈະສ້າງຕາຕະລາງປະຫວັດສາດທີ່ມີຊື່ທີ່ແຕກຕ່າງກັນກ່ວາສິ່ງທີ່ app ກໍາລັງຊອກຫາ
> ອັນນີ້ເຮັດໃຫ້ແອັບຯພະຍາຍາມຊໍ້າກັນຕາຕະລາງທີ່ມີຢູ່ແລ້ວໃນຕອນເລີ່ມຕົ້ນ ແລະລົ້ມເຫຼວໃນທັນທີ

### ຂໍ້ຕົກລົງຖານຂໍ້ມູນ

| ຫົວຂໍ້ | ມູນຄ່າທີ່ໃຊ້ |
| --- | --- |
| ການຕັ້ງຊື່ຕາຕະລາງແລະຖັນ | `snake_case` ຜ່ານ `UseSnakeCaseNamingConvention()` |
| Primary Key | `uuid` (UUID v7 ຈັດຮຽງຕາມເວລາເພື່ອຫຼຸດຜ່ອນການແຕກແຍກ index) |
| ວັນທີແລະເວລາ | `timestamptz` ຜ່ານ `DateTimeOffset` ຮັກສາມັນເປັນ UTC ສະເໝີ |
| Optimistic Concurrency | ຖັນລະບົບ `xmin` ຂອງ PostgreSQL ແຜນທີ່ກັບ property `Version` |
| Soft Delete | ຖັນ `is_deleted` + Global Query Filter ໃສ່ມັນອັດຕະໂນມັດທຸກຄັ້ງ Entity |
| Audit | `created_at`, `created_by`, `updated_at`, `updated_by`, `deleted_at`, `deleted_by` ຕື່ມໂດຍ Interceptor |
| Index | `is_deleted` ແລະ `(organization_id, dormitory_id)` ຖືກສ້າງຂື້ນໂດຍອັດຕະໂນມັດ |
| ໂຕະ Migration History | `__ef_migrations_history` |
| ຄວາມລະອຽດຂອງ `decimal` | `numeric(18,2)` ເປັນຄ່າເລີ່ມຕົ້ນ |

ຂໍ້ຕົກລົງເຫຼົ່ານີ້ແມ່ນໄດ້ຮັບການຈັດຕັ້ງໃນສະຖານທີ່ສໍາລັບທຸກຄົນ Entity ອັດຕະໂນມັດຢູ່ທີ່
`src/DormFlow.Infrastructure/Persistence/Extensions/ModelBuilderExtensions.cs`
ບໍ່ຈໍາເປັນຕ້ອງຂຽນອີກເທື່ອຜຶ່ງໃນແຕ່ລະຄັ້ງ `IEntityTypeConfiguration`

---

## Background Jobs

ໃຊ້ **Hangfire** ພ້ອມ **Hangfire.PostgreSql** ແມ່ນ Job Storage ສຸດ PostgreSQL ຄືກັນກັບຖານຂໍ້ມູນຫຼັກ
(ບໍ່ຈໍາເປັນຕ້ອງເພີ່ມ Redis ຫຼືການບໍລິການແຍກຕ່າງຫາກ) Hangfire ສ້າງ Schema ຂອງຕົນເອງ (`hangfire`) ໃນຖານຂໍ້ມູນດຽວກັນອັດຕະໂນມັດ
ໃນຕອນເລີ່ມຕົ້ນຂອງລະບົບ, ແຍກອອກຈາກ Schema `public` ທີ່ EF Core Migration ການຄຸ້ມຄອງ, ບໍ່ຈໍາເປັນຕ້ອງດໍາເນີນການ Migration ເພີ່ມສໍາລັບພາກນີ້

### Recurring Job ທີ່ມີຢູ່ແລ້ວ

| Job | ຕັ້ງເວລາ | ເຈົ້າເຮັດຫຍັງຢູ່? |
| --- | --- | --- |
| `expire-reservations` | ທຸກໆ 15 ນາທີ (`*/15 * * * *`) | ການຈອງໝົດອາຍຸແລ້ວ `Pending` ແຕ່ຢູ່ໃນທັງຝົດ `ExpiresAt` ແລ້ວ (`Reservation.Status → Expired`) ປ່ອຍຄືນຫ້ອງ `Available` ຖ້າຍັງຢູ່ໃນສະຖານະ `Reserved` |
| `mark-overdue-invoices` | ມື້ລະເທື່ອ 01:00 UTC | ໝາຍໃບແຈ້ງໜີ້ຢູ່ `Status` ແມ່ນ `Issued`/`PartiallyPaid` ແຕ່ຢູ່ໃນທັງຝົດ `DueDate` ຫຼັງຈາກນັ້ນ, ໃຫ້ມັນເປັນ `Overdue` ຄຽງຄູ່ກັບການຄິດໄລ່ຄ່າປັບໃໝຈ່າຍຊ້າຕາມການກຳນົດຂອງຫໍພັກ (`LateFeeType`/`LateFeeAmount`) ພຽງແຕ່ຄັ້ງດຽວໃນເວລາທີ່ມີການປ່ຽນແປງສະຖານະ |

ເຫດຜົນທາງທຸລະກິດຂອງທັງສອງ Job ຢູ່ໃນ Use Case ຕົ້ນສະບັບ (`IReservationService.ExpirePastDueReservationsAsync`,
`IInvoiceService.MarkOverdueInvoicesAsync`) ບໍ່ໄດ້ແຍກອອກເປັນຫ້ອງຮຽນ "Job" ແຍກຕ່າງຫາກ — Hangfire ໂທ Interface ເຫຼົ່ານີ້
ກົງຜ່ານ Dependency Injection ເຮັດໃຫ້ມັນເປັນໄປໄດ້ທີ່ຈະທົດສອບໃນລັກສະນະດຽວກັນ Use Case ທຸກສິ່ງທຸກຢ່າງອື່ນ: ທັງສອງວິທີການເຮັດວຽກໃນທົ່ວອົງກອນ
ໂດຍບໍ່ມີການຜ່ານ `TenantScope` ເນື່ອງຈາກວ່າມັນເປັນການເຮັດວຽກຂອງລະບົບຕົວມັນເອງ. ມັນບໍ່ແມ່ນຄໍາສັ່ງທີ່ເອີ້ນໂດຍຜູ້ໃຊ້ໃດໆ (`AuditLog` ບັນທຶກແລ້ວ
ຈະມີ `UserId` ແມ່ນ `null` ມັນເປັນສັນຍານວ່າລະບົບໄດ້ສ້າງທຸລະກໍານີ້)

### Dashboard ແລະການຕັ້ງຄ່າ

- ໜ້າ Dashboard ທີ່ `/hangfire` ສະພາບແວດລ້ອມເປີດເທົ່ານັ້ນທີ່ບໍ່ມີ Production ຄືກັນກັບເອກະສານ OpenAPI/Scalar
  (ໃນ Production ຕ້ອງໄດ້ຮັບການປົກປ້ອງສະເຝີໂດຍການຢືນຢັນຕົວຕົນຂອງທ່ານກ່ອນທີ່ຈະເປີດໃຊ້. ລະບົບການອະນຸຍາດຂອງແອັບຯບໍ່ໄດ້ເຊື່ອມຕໍ່ກັບໃນເວລານີ້)
- ຄົນພິການ Background Job ທັງໝົດໄດ້ຜ່ານການຕັ້ງຄ່າແລ້ວ `BackgroundJobs__Enabled` (ຄ່າເລີ່ມຕົ້ນ `true`) ໂດຍບໍ່ມີການ deploy ໃໝ່
- ຂ້າມການລົງທະບຽນ Hangfire ທັງໝົດ (ທັງສອງ Storage ແລະ Server) ໃນສະພາບແວດລ້ອມ Testing ສະເຝີ, ບໍ່ວ່າການຕັ້ງຄ່າແມ່ນຫຍັງ
  ເນື່ອງຈາກວ່າ Integration Test ໃຊ້ຖານຂໍ້ມູນຊົ່ວຄາວຂອງຕົນເອງໂດຍຜ່ານ Testcontainers ແລະ Hangfire.PostgreSql ຕິດຕັ້ງ Schema
  ຂອງທ່ານເອງທັນທີທີ່ລົງທະບຽນ (ຢ່າລໍຖ້າຈົນກ່ວາ Server ເລີ່ມເຮັດວຽກ) ຖ້າອະນຸຍາດໃຫ້ເຮັດວຽກຂະຜານກັບການກະກຽມຖານຂໍ້ມູນການທົດສອບ
  EF Core ມັນເຮັດໃຫ້ຄວາມຜິດພາດຂອງການສົມມຸດວ່າຖານຂໍ້ມູນມີຕາຕະລາງແລ້ວແລະຂ້າມການສ້າງຕາຕະລາງຂອງແອັບພລິເຄຊັນທັງຝົດ (ພົບແມງໄມ້ທີ່ແທ້ຈິງໃນລະຫວ່າງການພັດທະນາ
  Feature ນີ້ໄດ້ຖືກແກ້ໄຂໂດຍການຍ້າຍເງື່ອນໄຂໃນທົ່ວສະພາບແວດລ້ອມ Testing ໄປທີ່ຈຸດລົງທະບຽນ Storage ມັນບໍ່ແມ່ນພຽງແຕ່ຈຸດເລີ່ມຕົ້ນ Server)

---

## ສົນທິສັນຍາການເຂົ້າລະຫັດ

- `Nullable` ເປີດທຸກໂຄງການ
- `TreatWarningsAsErrors` ພຽງແຕ່ໂຄງການເປີດຢູ່ໃນ `src/` ການທົດສອບໂຄງການບໍ່ເປີດ
- ວິທີການເຮັດ I/O ຕ້ອງເປັນ `async` ແລະໄດ້ຮັບ `CancellationToken` ສະເໝີ
- ຢ່າກັບຄືນ Entity ອອກໄປ Controller ການນໍາໃຊ້ DTO ຫຼື Response Contract ເທົ່ານັ້ນ
- ຢ່າໃສ່ Business Logic ໃນ Controller
- ທຸກໆ Query ຜູ້ທີ່ອ່ານຂໍ້ມູນທຸລະກິດຍັງຕ້ອງການກັ່ນຕອງມັນ `organization_id` ແລະ `dormitory_id`
- ຢ່າບັນທຶກລະຫັດຜ່ານ, Access Token ຫຼື Refresh Token ລົງ Log
- Log ບໍ່ໄດ້ບັນທຶກ Request Body ຫຼື Response Body ເນື່ອງຈາກວ່າເສັ້ນທາງເຊັ່ນ login ມີລະຫັດຜ່ານຢູ່ໃນເນື້ອຫາ
- ສະບັບຊຸດແມ່ນຢູ່ທີ່ `Directory.Packages.props` ບ່ອນດຽວ, ຢ່າໃສ່ມັນ `Version` ໃນ `.csproj`

### Logging

Serilog ຂຽນ Log ລົງ Console ແລະໄຟລ໌ `logs/dormflow-<ວັນທີ>.log` (ເກັບກໍາ 14 ມື້)
ຄໍາຮ້ອງສະຝັກທຸກແມ່ນໄດ້ສະຫຼຸບເຂົ້າໄປໃນແຖວດຽວກັບ `TraceId`, `RemoteIp`, `UserAgent`
ແລະໃນເວລາທີ່ຜູ້ໃຊ້ໄດ້ຢັ້ງຢືນຕົວຕົນຂອງເຂົາເຈົ້າ, ຈະມີ `UserId`, `OrganizationId`, `DormitoryIds` ເພີ່ມ
ຮ້ອງຂໍໃຫ້ `/health` ໄດ້ຖືກຫຼຸດລົງເປັນ `Verbose` ເພື່ອບໍ່ໃຫ້ກວມເອົາເຖິງ Log ມີຄວາມໝາຍ

---

## ຂອບເຂດຂອງຂັ້ນຕອນ 1

### ສຳເລັດແລ້ວ

| # | ລາຍການ | ທີ່ຢູ່ |
| --- | --- | --- |
| 1 | Solution ແລະໂຄງສ້າງໂຄງການ 8 ໂຄງການ | `DormFlow.sln` |
| 2 | Project References ຕາມທິດທາງ Clean Architecture | `src/**/*.csproj` |
| 3 | NuGet Packages ຈໍາເປັນ | `Directory.Packages.props` |
| 4 | `Directory.Build.props` (root, src, tests) | `Directory.Build.props` |
| 5 | `Directory.Packages.props` (Central Package Management) | `Directory.Packages.props` |
| 6 | `.editorconfig` | `.editorconfig` |
| 7 | `appsettings.json` ແລະ `appsettings.Development.json` | `src/DormFlow.Api/` |
| 8 | ການເຊື່ອມຕໍ່ PostgreSQL + snake_case + retry + soft delete + audit | `src/DormFlow.Infrastructure/` |
| 9 | Serilog ພ້ອມ request logging ແລະເພີ່ມຂໍ້ມູນບໍລິບົດ | `src/DormFlow.Api/Configuration/SerilogConfiguration.cs` |
| 10 | OpenAPI ພ້ອມ JWT Bearer, XML Comments ແລະ Scalar | `src/DormFlow.Api/Configuration/OpenApiConfiguration.cs` |
| 11 | Health Check ສາມເສັ້ນທາງ | `src/DormFlow.Api/Configuration/HealthCheckConfiguration.cs` |
| 12 | Global Exception Middleware | `src/DormFlow.Api/Middleware/` |
| 13 | API Response Model + Pagination Contract | `src/DormFlow.Contracts/Common/` |
| 14 | `BaseEntity` ແລະ `TenantEntity` | `src/DormFlow.Domain/Common/` |
| 15 | Dockerfile ແລະ docker-compose.yml | `src/DormFlow.Api/Dockerfile`, `docker-compose.yml` |
| 16 | README | ໄຟລ໌ນີ້ |
| 17 | Build ຜ່ານທັງສອງ Solution ແລະການທົດສອບທັງຝົດຜ່ານ | — |

ລາຍການເພີ່ມເຕີມຢູ່ນອກບັນຊີລາຍຊື່ເນື່ອງຈາກວ່າມັນເປັນສິ່ງຈໍາເປັນເພື່ອເຮັດໃຫ້ລະບົບພື້ນຖານເຮັດວຽກ:
CORS ທີ່ອ່ານຄ່າຈາກ configuration ແລະປະຕິເສດທີ່ຈະເລີ່ມຕົ້ນຖ້າຫາກວ່າມັນບໍ່ໄດ້ຖືກຕັ້ງຢູ່ໃນ Production,
Rate Limiting, API Versioning, `ICurrentUser`, Forwarded Headers ແລະ Architecture Test

### ສຳເລັດແລ້ວ (ຂັ້ນຕອນ 2 — Identity, JWT, Refresh Token, Roles & Permissions)

| # | ລາຍການ | ທີ່ຢູ່ |
| --- | --- | --- |
| 1 | ASP.NET Core Identity (`ApplicationUser`, `ApplicationRole`) | `src/DormFlow.Infrastructure/Identity/` |
| 2 | JWT Access Token (HMAC-SHA256, 15 ນາທີ, ກວດ Issuer/Audience/Signature/Expiration) | `src/DormFlow.Infrastructure/Authentication/JwtAccessTokenGenerator.cs` |
| 3 | Refresh Token ບ່ອນຈັດເກັບຂໍ້ມູນຄ່າ Hash ພ້ອມແລ້ວ Token Rotation ແລະກວດຫາການນໍາໃຊ້ຄືນ | `src/DormFlow.Infrastructure/Authentication/RefreshTokenService.cs` |
| 4 | Roles ແລະ Permissions (7 ບົດບາດ, ສິດທີ່ລະບຸໄວ້ແມ່ນຄົບຖ້ວນສົມບູນສໍາລັບທຸກໂມດູນ) | `src/DormFlow.Application/Common/Authorization/` |
| 5 | Permission-Based Authorization ເຄື່ອນໄຫວ (`[RequirePermission("...")]`) | `src/DormFlow.Api/Authorization/` |
| 6 | Audit Log ສໍາເລັດ 15 ປະເພດເຫດການທີ່ລະບຸໄວ້ຈະຖືກເກັບໄວ້ old/new values ແມ່ນ jsonb | `src/DormFlow.Domain/Enums/AuditAction.cs`, `src/DormFlow.Infrastructure/Persistence/AuditLogWriter.cs` |
| 7 | Endpoint ສໍາເລັດການຢັ້ງຢືນຕົວຕົນ 7 ເສັ້ນທາງທີ່ໄດ້ລະບຸໄວ້ | `src/DormFlow.Api/Controllers/V1/AuthController.cs` |
| 8 | Lock Account ເມື່ອໃສ່ລະຫັດຜ່ານຜິດຢ່າງສົມບູນ 5 ເທື່ອ (ລັອກ 15 ນາທີ) | `src/DormFlow.Infrastructure/DependencyInjection.cs` |
| 9 | Seed ຂໍ້ມູນເລີ່ມຕົ້ນ (ສິດ, ບົດບາດ, ຜູ້ບໍລິຫານຈາກ `SEED_ADMIN_*`) | `src/DormFlow.Infrastructure/Persistence/DatabaseSeeder.cs` |
| 10 | Migration ທໍາອິດ (`InitialCreate`) | `src/DormFlow.Infrastructure/Migrations/` |
| 11 | Integration Test ທີ່ສົມບູນແບບ Login/Refresh/Logout/Change Password | `tests/DormFlow.IntegrationTests/Endpoints/AuthEndpointTests.cs` |
| 12 | ການທົດສອບທີ່ແທ້ຈິງກັບ PostgreSQL ຜ່ານ docker compose (ບໍ່ພຽງແຕ່ Testcontainers) | ກວດເບິ່ງຕົວທ່ານເອງໃນລະຫວ່າງການພັດທະນາ |

ສະຜັບສະຜູນ Two-Factor Authentication ຢູ່ໃນລະດັບໂຄງສ້າງແລ້ວ (`AuthenticationService` ກວດເບິ່ງສະຖານະ
`RequiresTwoFactor` ແລະປະຕິເສດການເຂົ້າສູ່ລະບົບ) ແຕ່ຍັງບໍ່ທັນມີເທື່ອ Endpoint ສໍາລັບການຢືນຢັນລະຫັດ 2FA ຄວາມຈິງ
ເນື່ອງຈາກວ່າ ASP.NET Core Identity ສະຜັບສະຜູນ 2FA ຜ່ານ Authenticator App ທັນທີທີ່ມັນຖືກເປີດໃຊ້
ແຕ່ຕ້ອງມີ UI/Endpoint ໄດ້ຮັບລະຫັດເພີ່ມເຕີມທີ່ຢູ່ນອກຂອບເຂດຂອງຂັ້ນຕອນນີ້

### ສຳເລັດແລ້ວ (ຂັ້ນຕອນ 3 — Organizations, Dormitories, Buildings, Floors, Room Types, Rooms, Users, Roles/Permissions, Audit Logs)

| # | ລາຍການ | ທີ່ຢູ່ |
| --- | --- | --- |
| 1 | Organizations CRUD (ສ້າງ/ລຶບອອກເທົ່ານັ້ນ SuperAdmin, Owner ສັງເກດເບິ່ງ/ສາມາດແກ້ໄຂໄດ້ໂດຍອົງກອນຂອງເຈົ້າເທົ່ານັ້ນ) | `src/DormFlow.Application/Features/Organizations/` |
| 2 | Dormitories CRUD ພ້ອມ billing day / payment due day / late fee | `src/DormFlow.Application/Features/Dormitories/` |
| 3 | Buildings, Floors, Room Types CRUD (ກວດສອບຄວາມເປັນເຈົ້າຂອງ Entity ແມ່ຂອງທຸກຫ້ອງຮຽນ) | `src/DormFlow.Application/Features/{Buildings,Floors,RoomTypes}/` |
| 4 | Rooms CRUD ພ້ອມ `GET /rooms/available` ແລະ `GET /rooms/summary` | `src/DormFlow.Application/Features/Rooms/` |
| 5 | Users CRUD, ກໍານົດບົດບາດ (`POST /users/{id}/roles`), ມອບໝາຍ/ຖອນຫໍພັກ | `src/DormFlow.Application/Features/Users/` |
| 6 | Roles CRUD (ບົດບາດມາດຕະຖານປ່ຽນຊື່/ບໍ່ສາມາດລຶບໄດ້, ແຕ່ການອະນຸຍາດສາມາດປັບໄດ້) ແລະ Permissions (ອ່ານເທົ່ານັ້ນ) | `src/DormFlow.Application/Features/{Roles,Permissions}/` |
| 7 | Audit Logs Endpoint ສໍາເລັດດ້ວຍຕົວກອງ (ອົງກອນ, ຫໍພັກ, ຜູ້ໃຊ້, ປະເພດເຫດການ, ໄລຍະເວລາ) | `src/DormFlow.Application/Features/AuditLogs/` |
| 8 | `TenantScope` — ເຫດຜົນສູນກາງເພື່ອປົກປ້ອງຂໍ້ມູນໃນທົ່ວອົງກອນ/ຫໍພັກແບ່ງປັນກັບທຸກໂມດູນຂ້າງເທິງ | `src/DormFlow.Application/Common/Authorization/TenantScope.cs` |
| 9 | Controller ສໍາເລັດ 9 ໂມດູນພ້ອມ `[RequirePermission("...")]` ທຸກໆ Endpoint | `src/DormFlow.Api/Controllers/V1/` |

ແຕ່ລະໂມດູນແບ່ງອອກເປັນ Repository Port (Application) + Repository Implementation (Infrastructure)
ປະຕິບັດຕາມຄໍາແນະນໍາດຽວກັນກັບ `IDormitoryRepository`/`DormitoryRepository` ດັ່ງນັ້ນຊັ້ນ Application
ເຮັດວຽກກັບ Domain Entity ໂດຍບໍ່ມີການອ້າງອີງ EF Core ໂດຍກົງ — ກະລຸນາຢືນຢັນ Architecture Test ແລ້ວ

**ຫຼັກການແຍກຂໍ້ມູນຕາມອົງກອນແລະຫໍພັກທີ່ໃຊ້ຕົວຈິງໃນລະຫັດ**

- ສ້າງຂໍ້ມູນໃຝ່: SuperAdmin ຕ້ອງການ `organizationId` ຕົວເອງ, ຜູ້ໃຊ້ອື່ນໆຖືກບັງຄັບໃຫ້ບັງຄັບໃຫ້ອົງກອນຂອງພວກເຂົາສະເຝີ
  ໂດຍບໍ່ຄໍານຶງເຖິງມູນຄ່າສິ່ງທີ່ຄໍາຮ້ອງຂໍກໍານົດ (`TenantScope.ResolveOrganizationIdForCreate`)
- ອ່ານ/ແກ້ໄຂ/ລຶບຂໍ້ມູນທີ່ມີຢູ່ແລ້ວ: ຖ້າອົງກອນຂອງຂໍ້ມູນບໍ່ກົງກັບຄໍາຕອບຂອງຜູ້ໃຊ້ 404 ແທນ 403 ເພື່ອບໍ່ໃຫ້ເດົາວ່າ
  ຂໍ້ມູນນັ້ນມີຢູ່ໃນອົງກອນອື່ນ (`TenantScope.EnsureOrganizationAccess` / `EnsureDormitoryAccess`)
- List Endpoint: SuperAdmin ແລະ Owner ທ່ານສາມາດເບິ່ງມັນຢູ່ໃນທຸກໆຫໍພັກໃນອົງກອນຂອງເຈົ້າ. ສໍາລັບບົດບາດອື່ນໆ (DormManager, Staff ແລະອື່ນໆ)
  ພຽງແຕ່ໄດ້ເຫັນຫໍພັກທີ່ເຂົາເຈົ້າໄດ້ຖືກມອບຝາຍ (`TenantScope.GetDormitoryRestrictionForListing`) **ແລະ**
  ທຸກໆ List Endpoint ຕ້ອງໂທຫາ `TenantScope.GetOrganizationRestrictionForListing` ຈັບຄູ່ກັນສະເໝີເພື່ອກັ່ນຕອງ
  ຄືກັນ `OrganizationId` ຄືກັນ (ເບິ່ງຫົວຂໍ້ [ການແກ້ໄຂຊ່ອງໂຫວ່ໃນການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນ](#ການແກ້ໄຂຊ່ອງໂຫວ່ໃນການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນ-owner-ໃນ-list-endpoint))
- Building ຕ້ອງຢູ່ໃນ Dormitory ລະບຸ, Floor ຕ້ອງຢູ່ໃນ Building ຄືກັນກັບຫ້ອງ,
  RoomType ຕ້ອງຢູ່ໃນ Dormitory ຄືກັນກັບອາຄານທີ່ເລືອກ — ກວດເບິ່ງກ່ອນທີ່ຈະບັນທຶກທຸກຄັ້ງ Application layer

### ການແກ້ໄຂຊ່ອງໂຫວ່ໃນການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນ (Owner) ໃນ List Endpoint

**ພົບໃນລະຫວ່າງການພັດທະນາໂມດູນ Reports ຂອງ Phase 2 (2026-08-24) ແລະແກ້ໄຂທັນທີກ່ອນທີ່ຈະສືບຕໍ່ເຮັດວຽກ:**
`TenantScope.GetDormitoryRestrictionForListing` ຟື້ນຟູ `null` ໃຫ້ຜູ້ໃຊ້ມີບົດບາດ Owner ດ້ວຍຄວາມຕັ້ງໃຈນັ້ນ
"ບໍ່ຈໍາເປັນຕ້ອງຈໍາກັດຫໍພັກເພາະວ່າ Owner ທ່ານສາມາດເບິ່ງມັນຢູ່ໃນທຸກໆຫໍພັກໃນອົງກອນຂອງເຈົ້າ" ແຕ່ Repository ຂອງໂມດູນທີ່ສະກັດຂໍ້ມູນ
ອີງຕາມຫໍພັກ (Buildings, Floors, RoomTypes, Rooms, Reservations, Contracts, MeterReadings, Invoices,
Payments, MaintenanceRequests — ທັງໝົດ 10 ໂມດູນທັງສອງຈາກ Phase 1 ແລະ Phase 2) ບໍ່ມີການກັ່ນຕອງຄືກັນ `OrganizationId`
ຍິ່ງໄປກວ່ານັ້ນ, ຜົນໄດ້ຮັບແມ່ນ Owner ເອີ້ນວ່າ List Endpoint ໂດຍບໍ່ມີການລະບຸ `dormitoryId` (ເຊັ່ນ `GET /api/v1/rooms`,
`GET /api/v1/invoices`) ທ່ານຈະເຫັນຂໍ້ມູນກ່ຽວກັບ**ທຸກໆອົງກອນໃນລະບົບ** ບໍ່ພຽງແຕ່ອົງກອນຂອງທ່ານເອງ — ລວມທັງຂໍ້ມູນທາງດ້ານການເງິນ
ແລະຂໍ້ມູນສ່ວນຕົວຂອງລູກຄ້າອື່ນໆໃນເວທີ SaaS ຮູບແບບຫຼາຍອົງກອນນີ້ (ການອ່ານແຕ່ລະລາຍການເປັນສ່ວນບຸກຄົນ `GET /{id}`
ບໍ່ໄດ້ຮັບຜົນກະທົບເພາະວ່າ `TenantScope.EnsureDormitoryAccess` ກວດ `OrganizationId` ແລ້ວຮ່ວມກັບຫໍພັກ
ບົດບາດນອກເໜືອໄປຈາກ SuperAdmin/Owner ບໍ່ໄດ້ຮັບຜົນກະທົບທັງເພາະວ່າ `DormitoryIds` ຂອງບົດບາດເຫຼົ່ານັ້ນ
ໂດຍທໍາມະຊາດຈໍາກັດຢູ່ໃນອົງກອນຂອງຕົນເອງໂດຍການມອບຝາຍຂອງຫໍພັກ)

ແກ້ໄຂໂດຍການເພີ່ມວິທີການ `TenantScope.GetOrganizationRestrictionForListing(currentUser)` (ຟື້ນຟູ `null`
ເທົ່ານັ້ນ SuperAdmin ເທົ່ານັ້ນ) ຫຼັງຈາກນັ້ນ, ເພີ່ມພາກສະຜາມ `OrganizationId` ໃຫ້ `*ListFilter` ຂອງທັງສອງ 10 ໂມດູນພ້ອມທີ່ຈະເພີ່ມເງື່ອນໄຂ
ການກັ່ນຕອງໃນ Repository ແຕ່ລະຄົນຂອງພວກເຂົາ — ຮຽກຮ້ອງຮ່ວມກັບ `GetDormitoryRestrictionForListing` ສະເຝີໄປໃນທຸກ Use Case ທີ່ດຶງລາຍການ
ຂໍ້ມູນແຍກໂດຍຫໍພັກແມ່ນມີຢູ່ Regression Test ຢັ້ງຢືນຢູ່
`tests/DormFlow.IntegrationTests/Endpoints/CrossOrganizationIsolationEndpointTests.cs` (ກວມເອົາທັງສອງ
ໂມດູນ Phase 1 ມັກ Rooms ແລະໂມດູນ Phase 2 ມັກ Invoices/MaintenanceRequests)

### ຍັງບໍ່ແລ້ວ

Endpoint ຢືນຢັນລະຫັດ Two-Factor Authentication (ໂຄງປະກອບການສະຜັບສະຜູນແມ່ນມີ, ແຕ່ຍັງບໍ່ທັນ UI/Endpoint ເອົາລະຫັດ)

Integration Test ຂອງໂມດູນໃຝ່ໃນຂັ້ນຕອນ 3 ມັນສົມບູນເຊັ່ນກັນ (ເບິ່ງຕາຕະລາງຂ້າງເທິງ) ລວມທັງກໍລະນີທີ່ພົບແມງໄມ້ຕົວຈິງ
ພາຍໃຕ້ການພັດທະນາ: `RoleRepository.SetPermissionsAsync` ເປີດທຸລະກໍາຕົວທ່ານເອງໂດຍກົງເຊັ່ນດຽວກັນ `BeginTransactionAsync`
ເຊິ່ງໃຊ້ຮ່ວມກັບ `EnableRetryOnFailure` ຂອງ Npgsql ເຮັດບໍ່ໄດ້ (Retrying Execution Strategy ບໍ່ຮອງຮັບທຸລະກຳ
ວ່າຜູ້ໂທເປີດຕົນເອງ) ຈະຕ້ອງກວມເອົາເຊັ່ນດຽວກັນ `Database.CreateExecutionStrategy().ExecuteAsync(...)` ແທນ
Integration Test ສາມາດຈັບແມງໄມ້ນີ້ຈາກຮອບທໍາອິດແລ່ນກັບ PostgreSQL ຄວາມຈິງ

---

## Phase 2 — ໂມດູນເສັ້ນ Contracts

ເອກະສານຕົ້ນສະບັບລະບຸລາຍລະອຽດ Entity/Endpoint/ກົດລະບຽບ Validation ຂອງ Phase 1 ສົມບູນ
ແຕ່ສໍາລັບ Phase 2 ພຽງແຕ່ລະບຸຊື່ໂມດູນ (Reservations, Contracts, CheckIns, CheckOuts, Meters,
Billing, Invoices, Payments, Receipts, Deposits, Expenses, Maintenance, Parcels, Visitors,
Parking, Announcements, Notifications, Reports, Settings) ໂດຍບໍ່ມີລາຍລະອຽດທຸລະກິດ
ເລີ່ມຕົ້ນຈາກສາຍ **Tenants → Reservations → Contracts → Meter Readings → Billing/Invoices/Payments** ທໍາອິດ
ເນື່ອງຈາກວ່າ Billing ທັງຝົດຕ້ອງອີງໃສ່ການມີສັນຍາເຊົ່າ Active ແລະມີມູນຄ່າແມັດພື້ນຖານສໍາລັບຮອບບິນນັ້ນ
ຕິດຕາມດ້ວຍ **Maintenance** ເນື່ອງຈາກວ່າມັນເປັນໂມດູນດຽວທີ່ຍັງເຫຼືອຢູ່ໃນ Phase 2 ມີສິດໄດ້ຮັບ (`maintenance.*`) ແລະບົດບາດ
`Technician` ມັນໄດ້ຖືກຕັ້ງໄວ້ຕັ້ງແຕ່ນັ້ນມາ Phase 1 ອັນນີ້ບໍ່ຄືກັບສ່ວນທີ່ເຫຼືອຂອງໂມດູນທີ່ບໍ່ມີການອະນຸຍາດໃດໆທີ່ຕັ້ງໄວ້ເລີຍ
ຫຼັງຈາກນັ້ນ, ສ້າງ Reports, Deposits, Receipts, Expenses, Parcels, Visitors, Parking, Announcements ແລະ
Notifications ຕາມລໍາດັບດ້ວຍການສົມມຸດຕິຖານທີ່ສົມເຫດສົມຜົນ (ເບິ່ງຫົວຂໍ້ຂ້າງລຸ່ມນີ້) ແລະປິດ Phase 2 ຄືກັນ **Settings**
ເຊິ່ງມີສັນຍານດຽວກັນກັບ Maintenance ເປັນສິດ `settings.manage` ແລະໃຫ້ສິດນີ້ Owner
ມັນໄດ້ຖືກຕັ້ງໄວ້ຕັ້ງແຕ່ນັ້ນມາ Phase 1 (ຢູ່ໃນປະເພດ Administration) ພຽງແຕ່ບໍ່ມີມັນເທື່ອ Entity/Endpoint ມາສະຜັບສະຜູນແທ້ໆ

### ສຳເລັດແລ້ວ

| # | ລາຍການ | ທີ່ຢູ່ |
| --- | --- | --- |
| 1 | Tenants CRUD (ລະດັບອົງກອນ, ບໍ່ໄດ້ຜູກມັດຢ່າງຖາວອນກັບຫໍພັກໃດນຶ່ງ, ບໍ່ມີ Endpoint ລຶບ) | `src/DormFlow.Application/Features/Tenants/` |
| 2 | Reservations — ຈອງຫ້ອງ (ຫ້ອງປ່ຽນເປັນ Reserved ທັນທີ) ແລະຍົກເລີກການຈອງ | `src/DormFlow.Application/Features/Reservations/` |
| 3 | Contracts — ສ້າງ (ໂດຍຜ່ານການຈອງຫຼືສັນຍາໂດຍກົງ), ເປີດໃຊ້ (ເຊັກອິນ), ສິ້ນສຸດ (ກວດເບິ່ງ) | `src/DormFlow.Application/Features/Contracts/` |
| 4 | ສິດ `reservations.view/create/cancel` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 5 | Migration `AddLeasingModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 6 | Integration Test ວົງຈອນຊີວິດການເຊົ່າເຕັມ + ໃນກໍລະນີຂອງການປະຕິເສດ (ບໍ່ມີຫ້ອງຫວ່າງ, ຜູ້ເຊົ່າບັນຊີດຳ) | `tests/DormFlow.IntegrationTests/Endpoints/LeasingLifecycleEndpointTests.cs` |
| 7 | Meter Readings — ຂຽນຄ່າເຄື່ອງວັດແທກນໍ້າ/ໄຟຟ້າປະຈໍາເດືອນອັດຕະໂນມັດດຶງມູນຄ່າທີ່ຜ່ານມາເພື່ອຕໍ່ອາຍຸ (View/Create ເທົ່ານັ້ນ) | `src/DormFlow.Application/Features/MeterReadings/` |
| 8 | Migration `AddMeterReadings` | `src/DormFlow.Infrastructure/Migrations/` |
| 9 | Integration Test ບັນທຶກຄ່າແມັດ (ຄ່າເລີ່ມຕົ້ນ, ຢ່າງຕໍ່ເນື່ອງຕະຫຼອດເດືອນ, ປ້ອງກັນບໍ່ໃຫ້ການລົງທະບຽນຊ້ໍາຊ້ອນ, ປ້ອງກັນການເກັບຄ່າທໍານຽມ retroactive) | `tests/DormFlow.IntegrationTests/Endpoints/MeterReadingsEndpointTests.cs` |
| 10 | ເພີ່ມ `WaterRatePerUnit`/`ElectricityRatePerUnit` ໃຫ້ Dormitory (ຫຼັງຈາກພາກສະຜາມ Billing ຕົ້ນສະບັບຂອງ Phase 1) | `src/DormFlow.Domain/Entities/Dormitory.cs` |
| 11 | Invoices — ສ້າງຂຶ້ນຕາມສັນຍາ+ຄ່າແມັດ (Draft), ອອກໃຫ້ຜູ້ເຊົ່າ (Issued), ຍົກເລີກ | `src/DormFlow.Application/Features/Invoices/` |
| 12 | Payments — ບັນທຶກ (Pending), ຢືນຢັນ (Confirmed → ຫັກຈໍານວນໃບບິນ), ເງິນຄືນ (Refunded → ສົ່ງຄືນຍອດເງິນ) | `src/DormFlow.Application/Features/Payments/` |
| 13 | Migration `AddBillingModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 14 | Integration Test ຮອບວຽນໃບບິນເຕັມ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/BillingLifecycleEndpointTests.cs` |
| 15 | Maintenance — ລາຍງານການສ້ອມແປງ, ມອບໝາຍ/ມອບໝາຍນັກວິຊາການຄືນໃໝ່, ປ່ຽນສະຖານະຕາມລຳດັບທີ່ກຳນົດໄວ້ | `src/DormFlow.Application/Features/Maintenance/` |
| 16 | ເພີ່ມ `HasRoleForDormitoryAsync` ໃຫ້ `IUserAdministrationRepository` (ຢືນຢັນວ່າເຈົ້າແມ່ນຊ່າງຊ່າງຢູ່ຫໍພັກນີ້ແທ້ໆ) | `src/DormFlow.Infrastructure/Identity/UserAdministrationRepository.cs` |
| 17 | Migration `AddMaintenanceModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 18 | Integration Test ວົງຈອນຊີວິດການບໍາລຸງຮັກສາຢ່າງເຕັມທີ່ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/MaintenanceEndpointTests.cs` |
| 19 | Reports — ສະຫຼຸບລາຍຮັບ (View/Export) ແລະໃບເກັບເງິນທີ່ຍັງຄ້າງຄາ (View/Export) ແມ່ນ CSV | `src/DormFlow.Application/Features/Reports/` |
| 20 | ການແກ້ໄຂຊ່ອງຫວ່າງໃນການຮົ່ວໄຫຼຂອງຂໍ້ມູນໃນທົ່ວອົງກອນ Owner ໃນ List Endpoint 10 ໂມດູນ (ພົບເຫັນໃນລະຫວ່າງການພັດທະນາ Reports) | `src/DormFlow.Application/Common/Authorization/TenantScope.cs` |
| 21 | Integration Test ບົດລາຍງານລາຍຮັບ/ໃບແຈ້ງໜີ້ເກີນກຳນົດ + Regression Test ການປົກປ້ອງຂໍ້ມູນທົ່ວອົງກອນ | `tests/DormFlow.IntegrationTests/Endpoints/{Reports,CrossOrganizationIsolation}EndpointTests.cs` |
| 22 | ສິດ `deposits.view/collect/refund` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 23 | Deposits — ຮັບເງິນຝາກ (Held), ເງິນຄືນ (ເຕັມ/ບາງ/ເສຍຫາຍທັງໝົດ, ຄັ້ງຕໍ່ລາຍການ) | `src/DormFlow.Application/Features/Deposits/` |
| 24 | Migration `AddDepositsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 25 | Integration Test ວົງຈອນຊີວິດເງິນຝາກເຕັມ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/DepositsEndpointTests.cs` |
| 26 | ສິດ `receipts.view/issue` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 27 | Receipts — ອອກໃບຮັບເງິນຈາກທຸລະກໍາການຈ່າຍເງິນທີ່ຢືນຢັນ (ບໍ່ມີ Endpoint ແກ້ໄຂ/ລຶບ) | `src/DormFlow.Application/Features/Receipts/` |
| 28 | Migration `AddReceiptsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 29 | Integration Test ການອອກໃບຮັບເງິນ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/ReceiptsEndpointTests.cs` |
| 30 | ສິດ `expenses.view/create` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 31 | Expenses — ບັນທຶກຄ່າໃຊ້ຈ່າຍໃນການດໍາເນີນງານ ແຍກອອກຈາກດ້ານລາຍຮັບຢ່າງສົມບູນ | `src/DormFlow.Application/Features/Expenses/` |
| 32 | Migration `AddExpensesModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 33 | Integration Test ການບັນທຶກແລະການຊອກຫາຄ່າໃຊ້ຈ່າຍ | `tests/DormFlow.IntegrationTests/Endpoints/ExpensesEndpointTests.cs` |
| 34 | ສິດ `parcels.view/log/pickup` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 35 | Parcels — ບັນທຶກຊຸດທີ່ມາຮອດແລ້ວ (Received), ບັນທຶກຂອງການຮັບການພັດທະນາ (PickedUp) | `src/DormFlow.Application/Features/Parcels/` |
| 36 | Migration `AddParcelsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 37 | Integration Test ວົງຈອນຊີວິດ parcel + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/ParcelsEndpointTests.cs` |
| 38 | ສິດ `visitors.view/checkin/checkout` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 39 | Visitors — ບັນທຶກການເຂົ້າ (CheckedIn), ອອກຈາກບັນທຶກ (CheckedOut) | `src/DormFlow.Application/Features/Visitors/` |
| 40 | Migration `AddVisitorsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 41 | Integration Test ວົງຈອນຊີວິດຂອງນັກທ່ອງທ່ຽວ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/VisitorsEndpointTests.cs` |
| 42 | ສິດ `parking.view/register/release` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 43 | Parking — ການລົງທະບຽນບ່ອນຈອດລົດ (Active), ສິ້ນສຸດການລົງທະບຽນ (Ended) | `src/DormFlow.Application/Features/Parking/` |
| 44 | Migration `AddParkingModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 45 | Integration Test ວົງຈອນຊີວິດການລົງທະບຽນບ່ອນຈອດລົດ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/ParkingEndpointTests.cs` |
| 46 | ສິດ `announcements.view/create/publish` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 47 | Announcements — ສ້າງການປະກາດ (Draft), ເຜີຍແຜ່ (Published), ເອົາເຂົ້າໄປໃນບ່ອນເກັບຮັກສາ (Archived) | `src/DormFlow.Application/Features/Announcements/` |
| 48 | Migration `AddAnnouncementsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 49 | Integration Test ວົງຈອນຊີວິດປະກາດ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/AnnouncementsEndpointTests.cs` |
| 50 | ສິດ `notifications.send` ເພີ່ມໃໝ່ (ບໍ່ມີຢູ່ໃນເອກະສານຕົ້ນສະບັບ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 51 | Notifications — ສົ່ງໃຫ້ຜູ້ໃຊ້ຄົນດຽວ, ສັງເກດເບິ່ງ/ໝາຍການແຈ້ງເຕືອນຂອງຕົນເອງວ່າອ່ານແລ້ວ (ບໍ່ຈໍາເປັນຕ້ອງມີການອະນຸຍາດສະເພາະ) | `src/DormFlow.Application/Features/Notifications/` |
| 52 | Migration `AddNotificationsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 53 | Integration Test ວົງຈອນຊີວິດການແຈ້ງເຕືອນ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/NotificationsEndpointTests.cs` |
| 54 | Settings — ການຕັ້ງຄ່າອົງກອນ Key/Value ເອກະລາດ (GET/PUT upsert/DELETE) ໃຊ້ສິດຂອງເຈົ້າ `settings.manage` ຕົ້ນສະບັບຂອງ Phase 1 | `src/DormFlow.Application/Features/Settings/` |
| 55 | Migration `AddSettingsModule` (unique index ສຸດ `organization_id`+`key` ການກັ່ນຕອງດ້ວຍ `is_deleted = false`) | `src/DormFlow.Infrastructure/Migrations/` |
| 56 | Integration Test ການຈັດການການຕັ້ງຄ່າ + ໃນກໍລະນີຂອງການປະຕິເສດ | `tests/DormFlow.IntegrationTests/Endpoints/SettingsEndpointTests.cs` |

### ສົມມຸດຕິຖານທຸລະກິດທີ່ໃຊ້ (ຍັງບໍ່ໄດ້ຢືນຢັນກັບເຈົ້າຂອງລະບົບເທື່ອ)

ເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸກົດລະບຽບທຸລະກິດສໍາລັບໂມດູນເຫຼົ່ານີ້. ດັ່ງນັ້ນ, ການສົມມຸດຕິຖານທີ່ສົມເຫດສົມຜົນຕໍ່ໄປນີ້ຖືກນໍາໃຊ້
**ຄວນຈະທົບທວນຄືນກ່ອນທີ່ຈະໃຊ້ຕົວຈິງ**:

- **ບໍ່ມີ Entity ແຍກຕ່າງຫາກສໍາລັບ CheckIn/CheckOut** — ລວມເປັນການກະທໍາໃນສັນຍາແທນ
  (`POST /contracts/{id}/activate` = ເຊັກອິນ, `POST /contracts/{id}/terminate` = ກວດເບິ່ງ)
  ຖ້າທ່ານຕ້ອງການບັນທຶກລາຍລະອຽດຂອງການກວດກາຫ້ອງຫຼືຮູບພາບໃນເວລາເຂົ້າ-ຕ້ອງໄດ້ຮັບການແຍກອອກເປັນ Entity ໃໝ່ຕໍ່ມາ
- **ສະຖານະຫ້ອງມີການປ່ຽນແປງອັດຕະໂນມັດຕາມວົງຈອນຊີວິດ**: ຈອງ/ເຮັດສັນຍາ (Draft) → `Reserved`,
  ເປີດໃຊ້ສັນຍາ (ເຊັກອິນ) → `Occupied`, ສິ້ນສຸດສັນຍາ (ກວດເບິ່ງ) → `Cleaning`
  (ເຈົ້າຜ້າທີ່ຕ້ອງປ່ຽນເປັນ `Available` ຂ້ອຍຜ່ານມັນເອງ Endpoint ຂອງຫ້ອງຫຼັງຈາກທໍາຄວາມສະອາດສໍາເລັດ)
- **ໝາຍເລກສັນຍາ** ແບບສຸ່ມ `CT-{yyyyMMdd}-{6 ຕົວອັກສອນແບບສຸ່ມ}` ຢ່າໃຊ້ຕົວເລກແລ່ນຢ່າງຕໍ່ເນື່ອງ
  ເນື່ອງຈາກວ່າຍັງບໍ່ທັນມີຂໍ້ກໍານົດກ່ຽວກັບຮູບແບບຕົວເລກສັນຍາທີ່ຕ້ອງອ້າງອີງໃສ່ລະບົບບັນຊີພາຍນອກ
- **ຜູ້ເຊົ່າບັນຊີດຳ (`Blacklisted`)** ບໍ່ສາມາດຈອງຫ້ອງ ຫຼືເຮັດສັນຍາໃໝ່ໄດ້. ແຕ່ສັນຍາຕົ້ນສະບັບ Active ທີ່ມີຢູ່ແລ້ວຈະບໍ່ຖືກຍົກເລີກໂດຍອັດຕະໂນມັດ
- **ຫ້ອງຜຶ່ງມີສັນຍາທີ່ຍັງບໍ່ທັນສໍາເລັດ (Draft ຫຼື Active) ມີພຽງສຳເນົາອັນດຽວເທົ່ານັ້ນ**
- **ເຊົ່າ/ເງິນຝາກໃນສັນຍາແມ່ນຄັດລອກຈາກຫ້ອງໃນເວລາທີ່ສັນຍາໄດ້ຖືກເຊັນ** ມັນບໍ່ປ່ຽນແປງຕາມຄ່າເຊົ່າຫ້ອງທີ່ປັບປຸງຕໍ່ມາ
- **ການຈອງແມ່ນຖືກຕ້ອງເປັນເວລາຈໍາກັດ** (`HoldDays`, ຄ່າເລີ່ມຕົ້ນ 3 ມື້ສູງສຸດ 30 ມື້) ແລະໃນປັດຈຸບັນມີ Background Job
  (Hangfire, ເບິ່ງຫົວຂໍ້ [Background Jobs](#background-jobs)) ລໍຖ້າການຈອງໝົດອາຍຸ `ExpiresAt` ແລະຫຼັງຈາກນັ້ນອັດຕະໂນມັດ
  ທຸກໆ 15 ນາທີ (`Reservation.Status → Expired`, ປ່ອຍຫ້ອງຄືນຖ້າມັນຍັງຢູ່ໃນສະຖານະ `Reserved`) ບໍ່ຈໍາເປັນຕ້ອງຍົກເລີກດ້ວຍມື
  `POST /reservations/{id}/cancel` ອີກຕໍ່ໄປ (ຍັງບໍ່ທັນ Endpoint ນີ້ແມ່ນສໍາລັບການຍົກເລີກກ່ອນວັນກໍານົດ)
- **ບໍ່ມີ Entity ແມ່ຂອງ "ແມັດ" ແຍກ** — `MeterReading` ຜູກມັດກັບຫ້ອງ+ປະເພດແມັດ+ເດືອນທີ່ແນ່ນອນ
  ເນື່ອງຈາກວ່າສິດທີ່ມີຢູ່ແລ້ວ (`meter-readings.view/create` ເທົ່ານັ້ນ, ບໍ່ CRUD ແມັດ) ສະແດງໃຫ້ເຫັນວ່າມັນບໍ່ມີຈຸດປະສົງ
  ການລົງທະບຽນແມັດແຍກຕ່າງຫາກຖ້າຫາກວ່າຫ້ອງຕົວຈິງມີການປ່ຽນແປງແມັດ (ແມັດຫັກ/ປ່ຽນອັນໃໝ່) ທ່ານຕ້ອງຂຽນຄ່າເລີ່ມຕົ້ນໃຝ່ດ້ວຍຕົນເອງ
  ລະບົບບໍ່ສະຜັບສະຜູນການປັບຄ່າເຄື່ອງວັດແທກເປັນຄ່າຕ່ໍາກວ່າຄ່າທີ່ຜ່ານມາ
- **ມູນຄ່າແມັດແມ່ນການແກ້ໄຂບັນທຶກປະຫວັດສາດ/ບໍ່ສາມາດລຶບໄດ້** ຖ້າເຈົ້າເຮັດຜິດ, ເຈົ້າຕ້ອງເຮັດລາຍການໃໝ່ສໍາລັບເດືອນຕໍ່ໄປ ແລະແກ້ໄຂແທນ
- **ອັດຕານໍ້າ/ຄ່າໄຟຟ້າຕໍ່ຜ່ວຍແມ່ນເກັບຮັກສາໄວ້ໃນລະດັບຫໍພັກ** (`Dormitory.WaterRatePerUnit`/`ElectricityRatePerUnit`)
  ຫຼັງຈາກພາກສະຜາມການຕັ້ງຄ່າຕົ້ນສະບັບຂອງ Phase 1 (`BillingDay`, `PaymentDueDay`, `LateFeeType/Amount`)
  ເນື່ອງຈາກວ່າມັນເປັນມູນຄ່າທີ່ຄວນຈະຄືກັນສໍາລັບຫໍພັກທັງຝົດ. ບໍ່ແມ່ນຕໍ່ຫ້ອງ — ຖ້າບາງຫໍພັກຕ້ອງການອັດຕາທີ່ແຕກຕ່າງກັນຕາມປະເພດຫ້ອງ
  ຕ້ອງຍ້າຍໄປບ່ອນເກັບມ້ຽນ `RoomType` ແທນທີ່ຕໍ່ມາ
- **ບໍ່ມີ Entity ແຍກຕ່າງຫາກສໍາລັບ "ລາຍການ" ໃນໃບເກັບເງິນ (Invoice Line Item)** — ເຊົ່າ/ໃບບິນຄ່ານໍ້າ/ໃບບິນຄ່າໄຟຟ້າ/ດີ
  ເກັບຮັກສາເປັນພາກສະຜາມໂດຍກົງກ່ຽວກັບ `Invoice` ເນື່ອງຈາກວ່າຈໍານວນປະເພດຄ່າໃຊ້ຈ່າຍແມ່ນຖືກແກ້ໄຂໃນຂອບເຂດນີ້. ຖ້າຄ່າໃຊ້ຈ່າຍອື່ນໆຕ້ອງໄດ້ຮັບການສະຜັບສະຜູນ
  ຈໍານວນເງິນແມ່ນບໍ່ໄດ້ຮູ້ລ່ວງຜ້າ (ຕົວຢ່າງ, ການປັບໃໝຈາກຜູ້ເຊົ່າທີ່ທໍາລາຍລາຍການ) ຕ້ອງແຍກອອກເປັນຕາຕະລາງ Line Item ຕໍ່ມາ
- **ໃບເກັບເງິນສາມາດອອກໄດ້ຈາກສັນຍາເທົ່ານັ້ນ Active ເທົ່ານັ້ນ** ແລະສາມາດອອກເອກະສານຜຶ່ງຕໍ່ເດືອນຕໍ່ສັນຍາ
  ໃບບິນຄ່ານໍ້າ/ໃບເກັບຄ່າໄຟຟ້າແມ່ນຄິດໄລ່ຈາກຄ່າແມັດຫ້ອງສໍາລັບເດືອນນັ້ນ. ຖ້າບໍ່ມີຄ່າແມັດສໍາລັບເດືອນນັ້ນ, ມັນຈະຖືກຄິດໄລ່ 0
  (ມັນບໍ່ຈໍາເປັນສະເຝີທີ່ຈະຂຽນລົງແມັດກ່ອນທີ່ຈະອອກໃບບິນຄ່າ. ເພາະບາງຫໍພັກອາດຈະລວມຄ່ານ້ຳ-ມີໄຟໃຫ້ເຊົ່າແລ້ວ)
- **ວັນທີຊໍາລະແມ່ນຄິດໄລ່ຈາກ `Dormitory.PaymentDueDay`** ໂດຍການຊອກຫາວັນທີທີ່ກົງກັບຕົວເລກທີ່ຢູ່ໃນທີ່ຢູ່ *ກັບຄືນ*
  ວັນທີໃບບິນທີ່ໃກ້ທີ່ສຸດ (ຖ້າເລກທີ່ກຳນົດໄດ້ຜ່ານໄປແລ້ວໃນເດືອນທີ່ໃບບິນຖືກອອກຈະຖືກເລື່ອນໄປໃນເດືອນໜ້າ)
  ແລະບໍ່ມີການຄິດໄລ່ການລົງໂທດການຈ່າຍເງິນຊ້າ (`LateFeeAmount`) ອັດຕະໂນມັດເມື່ອເກີນກໍານົດເນື່ອງຈາກວ່າຕ້ອງມີ Background Job
  ທີ່ຍັງບໍ່ທັນໄດ້ຕິດຕັ້ງ (ດຽວກັນໃຊ້ກັບຄວາມອ່ອນແອ HoldDays ຂອງ Reservations) — ສະຖານະ `Overdue` ທີ່ກໍານົດໄວ້ໃນລະບົບ
  ແຕ່ມັນຍັງບໍ່ທັນມີຈຸດໃດໃນການຕັ້ງຄ່າອັດຕະໂນມັດ
- **ການຈ່າຍເງິນຕ້ອງໄດ້ຮັບການກວດສອບກ່ອນທີ່ຈະສາມາດຖືກນໍາໃຊ້ກັບໃບບິນຄ່າ** (`Pending` → `Confirmed`) ສິດການບັນທຶກແຍກຕ່າງຫາກ
  (`payments.create`) ຢືນຢັນການອະນຸຍາດອອກ (`payments.approve`) ອີງຕາມ Phase 1 ການອະນຸຍາດໄດ້ຖືກກໍານົດ
  ເພື່ອໃຫ້ມີຈຸດກວດກາກ່ອນເງິນເຂົ້າລະບົບຕົວຈິງ. ໂດຍສະເພາະໃນກໍລະນີຂອງການໂອນ/QR ຈະຕ້ອງປຽບທຽບກັບໃບຝາກເງິນຂອງທະນາຄານ
  ບໍ່ມີສະຖານະ "ປະຕິເສດ" ແຍກ (ບໍ່ມີສິດ `payments.reject` ໃນເອກະສານຕົ້ນສະບັບ) ລາຍການທີ່ບັນທຶກໄວ້ອອກບໍ່ຖືກຕ້ອງເປັນ
  `Pending` ສາມາດຖືໄດ້ໂດຍບໍ່ມີຜົນກະທົບຕໍ່ຍອດເງິນໃນບັນຊີລາຍການ
- **ບໍ່ສາມາດຢືນຢັນຈຳນວນການຈ່າຍເງິນເກີນຍອດເງິນໃນໃບບິນ** ປົກປ້ອງ Overpayment ແຕ່ຍັງບໍ່ໄດ້ສະຜັບສະຜູນ "ສິນເຊື່ອທີ່ຍັງເຫຼືອ"
  ໂອນໄປໃຊ້ໃນໃບບິນຂອງເດືອນຕໍ່ໄປ (ເງິນເກີນໃດໆຕ້ອງຖືກສົ່ງຄືນເປັນລາຍການແຍກຕ່າງຫາກ. ຫຼືເພີ່ມການອອກແບບໃນພາຍຫຼັງ)
- **ໃບເກັບເງິນສາມາດຖືກຍົກເລີກເທົ່ານັ້ນທີ່ບໍ່ໄດ້ຮັບການຈ່າຍເງິນ** (`PaidAmount == 0`) ຖ້າຈ່າຍເງິນແລ້ວ, ເງິນຕ້ອງຖືກສົ່ງຄືນ
  ລາຍການທີ່ໄດ້ຮັບການຢືນຢັນໃນເມື່ອກ່ອນດັ່ງນັ້ນບັນຊີລາຍການສາມາດຍົກເລີກໄດ້
- **ບໍ່ມີສິດ "ສ້າງບົດລາຍງານການສ້ອມແປງ" ແຍກ** ສິດເດີມຂອງ Phase 1 ມີພຽງແຕ່ `maintenance.view/assign/update-status`
  ດັ່ງນັ້ນ, ມັນຖືກຕີຄວາມວ່າການລາຍງານການສ້ອມແປງແມ່ນກໍານົດສະຖານະເລີ່ມຕົ້ນ (`Reported`) ໃຊ້ສິດຂອງເຈົ້າ `maintenance.update-status` ດຽວກັນ
  ເຮັດ Staff ແລະ Technician (ຜູ້ທີ່ບໍ່ມີສິດ `assign`) ທ່ານສາມາດລາຍງານການສ້ອມແປງດ້ວຍຕົນເອງ RolePermissionDefaults
  ມອບສິດພິເສດນີ້ໃຫ້ທັງສອງບົດບາດ — ຜູ້ເຊົ່າຍັງບໍ່ສາມາດລາຍງານການສ້ອມແປງດ້ວຍຕົນເອງໄດ້ (ບົດບາດ Tenant ບໍ່ມີສິດໃນໂມດູນນີ້ເລີຍ. ເຊັ່ນດຽວກັນກັບທຸກໂມດູນອື່ນໆ)
- **ການແຕ່ງຕັ້ງນັກວິຊາການ (`POST /assign`) ແຍກອອກຈາກການຫັນປ່ຽນຂອງລັດທົ່ວໄປ (`PATCH /status`) ຕັ້ງໃຈ**
  ເນື່ອງຈາກວ່າມັນຕ້ອງກວດເບິ່ງວ່າຜູ້ໃຊ້ທີ່ລະບຸໄວ້ມີບົດບາດ `Technician` ແລະຕົວຈິງແລ້ວໄດ້ຖືກມອບໝາຍໃຫ້ເບິ່ງແຍງຫໍພັກນັ້ນ (ຜ່ານ `UserDormitory`)
  ກ່ອນທີ່ຈະບັນທຶກ — `PATCH /status` ປະຕິເສດທີ່ຈະຕັ້ງສະຖານະເປັນ `Assigned` ໂດຍກົງຢູ່ສະ ເໝີ
- **ອະນຸຍາດໃຫ້ມອບໝາຍຄືນໃໝ່ (ປ່ຽນຊ່າງໃໝ່) ແມ່ນແລ້ວ, ຕາບໃດທີ່ວຽກບໍ່ໄດ້ປິດ** (`Reported`/`Assigned`/`InProgress`)
  ບໍ່ຈໍາກັດພຽງແຕ່ຄັ້ງທໍາອິດໃນການສ້າງວຽກເຮັດງານທໍາ. ເພາະວຽກຕົວຈິງມັກຈະຕ້ອງປ່ຽນຊ່າງຊ່າງເມື່ອຄົນດຽວກັນຫຍຸ້ງກັບວຽກອື່ນ
- **ຄໍາສັ່ງຂອງການຫັນປ່ຽນຂອງລັດທີ່ອະນຸຍາດໃຫ້ຖືກແກ້ໄຂ**: `Reported→Cancelled`, `Assigned→InProgress|Cancelled`,
  `InProgress→Completed|Cancelled` ພຽງແຕ່ຂ້າມຂັ້ນຕອນ (ເຊັ່ນ `Reported→Completed`) ເຮັດບໍ່ໄດ້
- **ສະຖານະຫ້ອງຈະປ່ຽນອັດຕະໂນມັດຄືກັບໂມດູນ Reservations/Contracts**: ແຕ່ງຕັ້ງນັກວິຊາການຄັ້ງທໍາອິດກັບຫ້ອງຫວ່າງ
  (`Available`) → ປ່ຽນເປັນ `Maintenance`; ວຽກງານປິດ (`Completed`/`Cancelled`) ຫຼັງຈາກນັ້ນ, ຫ້ອງຈະກັບຄືນໄປບ່ອນ `Available`
  ພຽງແຕ່ໃນເວລາທີ່ບໍ່ມີວຽກສ້ອມແປງອື່ນໆທີ່ຍັງຄ້າງຢູ່ໃນຫ້ອງດຽວກັນ — ຫ້ອງທີ່ບໍ່ຢູ່ໃນສະຖານະ `Available`/`Maintenance`
  ໃນເວລານັ້ນ (ເຊັ່ນ `Occupied`) ຈະບໍ່ຖືກແຕະຕ້ອງເລີຍ ເພາະວ່າຫ້ອງເຊົ່າແລ້ວ, ບໍ່ຈໍາເປັນຕ້ອງປ້ອງກັນການຈອງຊ້ຳອີກ
- **`RoomService.ChangeStatusAsync` (Endpoint ທົ່ວໄປຂອງຫ້ອງ) ບໍ່ມີການກວດສອບສະຖານະ** ຜູ້ໃຊ້ທີ່ມີສິດ
  `rooms.change-status` ດັ່ງນັ້ນ, ສະຖານະຂອງຫ້ອງທີ່ມີວຽກສ້ອມແປງທີ່ຍັງຄ້າງໄດ້ຖືກປ່ຽນກັບຄືນໄປບ່ອນ `Available` ເຈົ້າສາມາດເຮັດມັນໄດ້ໂດຍກົງ
  ການຫັນປ່ຽນສະຖານະອັດຕະໂນມັດຂອງໂມດູນນີ້ແມ່ນພຽງແຕ່ເພື່ອຊ່ວຍ. ມັນບໍ່ແມ່ນກົດລະບຽບລະດັບຖານຂໍ້ມູນ — ທີ່ຍອມຮັບໄດ້ໃນຂອບເຂດນີ້
- **Reports ພຽງແຕ່ເລືອກທີ່ຈະເຮັດມັນ 2 ລາຍງານກ່ອນ** (ສະຫຼຸບລາຍຮັບ ແລະ ໃບແຈ້ງໜີ້ຄ້າງຈ່າຍ) ເນື່ອງຈາກວ່າເຫຼົ່ານີ້ແມ່ນສອງບົດລາຍງານທີ່ສ້າງຂຶ້ນໃນຂໍ້ມູນ
  Invoice/Payment ທີ່ມີຢູ່ແລ້ວທັນທີໂດຍບໍ່ມີການມີການອອກແບບ Entity ໃໝ່ — ບໍ່ມີບົດລາຍງານສະຫຼຸບກ່ຽວກັບການພັກເຊົາ (Occupancy)
  ເນື່ອງຈາກວ່າມັນຊໍ້າຊ້ອນກັບ `GET /rooms/summary` ທີ່ມີຢູ່ແລ້ວ (ແຕ່ລະຄົນມີສິດແຕກຕ່າງກັນ `rooms.view` ບໍ່ `reports.view`)
  ແລະບໍ່ມີບົດລາຍງານສະຫຼຸບວຽກງານບໍາລຸງຮັກສາຫຼືບົດລາຍງານອື່ນໆ
- **ສະຖານະ `Overdue` ໃບແຈ້ງໜີ້ຖືກຕັ້ງອັດຕະໂນມັດໂດຍ Background Job ປະຈໍາວັນ** (ເບິ່ງຫົວຂໍ້
  [Background Jobs](#background-jobs)) ແຕ່ບົດລາຍງານທັງສອງຍັງຖືກຄິດໄລ່ "ເກີນກຳນົດ" ສົດໆຈາກການປຽບທຽບ `DueDate` ກັບ
  ວັນທີປັດຈຸບັນໃນເວລາໂທແມ່ນຄືກັນກັບກ່ອນໜ້ານີ້. ແທນທີ່ຈະອ່ານຈາກ `Invoice.Status == Overdue` ໂດຍກົງເພື່ອໃຫ້ຜົນໄດ້ຮັບທີ່ຖືກຕ້ອງສະເຝີ
  ເຖິງແມ່ນວ່າ Job ບໍ່ດົນມານີ້, ມັນບໍ່ໄດ້ດໍາເນີນການນັບຕັ້ງແຕ່ບັນຊີລາຍການຝົດອາຍຸ (Job ດໍາເນີນການພຽງແຕ່ຜຶ່ງຄັ້ງຕໍ່ມື້ມີເວລາທີ່ສະຖານະຍັງບໍ່ທັນໄດ້ປັບປຸງ)
- **ສົ່ງອອກ CSV ຈໍາກັດບໍ່ເກີນ 5,000 ແຖວຕໍ່ຄໍາຮ້ອງຂໍ** (`ReportRepository.MaxExportRows`) ປ້ອງກັນການສະກັດເອົາຂໍ້ມູນຈໍານວນຫຼວງຫຼາຍ
  ຫຼາຍເກີນໄປໃນຜຶ່ງຄໍາຮ້ອງຂໍຖ້າຫາກວ່າທ່ານຕ້ອງການຫຼາຍກ່ວານັ້ນ, ທ່ານຕ້ອງການກັ່ນຕອງມັນເຊັ່ນດຽວກັນ `dormitoryId`/ແຄບໄລຍະເວລາ ຫຼືແບ່ງອອກເປັນຫຼາຍຮອບ
- **Deposits ແຍກອອກຈາກໂມດູນ Payments ສົມບູນ** ຢ່າໃຊ້ Invoice/Payment tied ກັບເງິນຝາກຈໍາເລີຍເນື່ອງຈາກວ່າມັນເປັນເງິນໄຫຼ
  ແຕ່ລະອັນ (ຮັບປະກັນເງິນທີ່ຕ້ອງສົ່ງຄືນ ບໍ່ແມ່ນລາຍຮັບຕາມຮອບບິນ) ແລະບໍ່ມີສິດ `deposits.*` ຢູ່ໃນເອກະສານຕົ້ນສະບັບ
  ດັ່ງນັ້ນຈຶ່ງເພີ່ມສິດໃຝ່ຕາມຮູບແບບດຽວກັນກັບທີ່ໄດ້ເຮັດຜ່ານມາ `reservations.*` ຢູ່ໃນເສັ້ນ Contracts
- **ສັນຍາເຊົ່າຜຶ່ງໄດ້ຮັບພຽງແຕ່ຜຶ່ງເງິນຝາກ** ແລະສາມາດໄດ້ຮັບພຽງແຕ່ໃນຂະນະທີ່ສັນຍາຍັງບໍ່ທັນສິ້ນສຸດ (`Draft`/`Active`)
  ສຳເນົາເລີ່ມຕົ້ນຈາກ `Contract.DepositAmount` ແຕ່ມັນສາມາດປັບເປັນຈໍານວນອື່ນໄດ້ເມື່ອໄດ້ຮັບຕົວຈິງ
- **ເງິນຝາກແມ່ນສາມາດຄືນເງິນໄດ້ພຽງແຕ່ຫຼັງຈາກສັນຍາສິ້ນສຸດລົງ (`Terminated`) ເທົ່ານັ້ນ** ແລະສາມາດສົ່ງຄືນໄດ້ພຽງແຕ່ຄັ້ງດຽວຕໍ່ລາຍການ
  (ບໍ່ຮອງຮັບການກັບຄືນມາໃນຫຼາຍໄລຍະເຊັ່ນ Payment) — ຖ້າຈໍານວນເງິນເຕັມບໍ່ຖືກສົ່ງຄືນ, ເຫດຜົນສໍາລັບການຫັກອອກຕ້ອງໄດ້ຮັບການລະບຸໄວ້ສະເຝີ
  ກັບຄືນຫາ 0 ຝາຍຄວາມວ່າ forfeiting ເງິນຝາກທັງຝົດ (`Forfeited`)
- **Receipts ສາມາດອອກໄດ້ຈາກທຸລະກໍາການຈ່າຍເງິນທີ່ຢືນຢັນ (`Payment.Status == Confirmed`)** ແລະສາມາດອອກໄດ້ຄັ້ງດຽວເທົ່ານັ້ນ
  ຕໍ່ລາຍການການຈ່າຍເງິນ (unique index ສຸດ `PaymentId`) — ຖ້າລາຍການຖືກສົ່ງຄືນຫຼັງຈາກອອກໃບຮັບເງິນ
  ໄປ, ໃບຮັບເງິນຕົ້ນສະບັບຈະຍັງຄົງເປັນຫຼັກຖານປະຫວັດສາດ. ບໍ່ໄດ້ຍົກເລີກອັດຕະໂນມັດ (ລະບົບຍັງບໍ່ທັນມີ Endpoint ຍົກເລີກ/ອອກໃໝ່
  ໃບຮັບເງິນຖືກພິຈາລະນາຢູ່ນອກຂອບເຂດຂອງ Phase 2 ເສັ້ນນີ້)
- **ຈຳນວນໃບຮັບເງິນທີ່ສຳເນົາມາຈາກ `Payment.Amount` ເມື່ອອອກໃບຮັບເງິນ (Snapshot)** ບໍ່ແມ່ນການອ້າງອີງສົດເພື່ອສະໜອງເອກະສານ
  ເຊິ່ງຍັງຄົງຄົງທີ່ເຖິງແມ່ນວ່າຂໍ້ມູນຕົ້ນສະບັບຈະຖືກດັດແກ້ຕໍ່ມາ — ມັນຄ້າຍຄືກັບຫຼັກການດຽວກັນ `Contract.MonthlyRent`
  ສຳເນົາມາຈາກ `Room.MonthlyRent` ເມື່ອເຮັດສັນຍາ
- **Expenses ມັນເປັນປື້ມບັນທຶກຄ່າໃຊ້ຈ່າຍອັນບໍລິສຸດ. ບໍ່ມີສະຖານະຫຼື Workflow ອະນຸມັດ** ແຕກຕ່າງຈາກ Invoices/Payments
  ເຊິ່ງແມ່ນດ້ານລາຍຮັບທີ່ມີ Draft/Issued/Confirmed ກະລຸນາກວດເບິ່ງເພາະວ່າບໍ່ມີຂໍ້ກໍານົດສໍາລັບການອະນຸມັດຄ່າໃຊ້ຈ່າຍ
  ຈາກເອກະສານຕົ້ນສະບັບ — ຖ້າທ່ານຕ້ອງການ Workflow ການອະນຸມັດຕໍ່ມາ (ເຊັ່ນວ່າຕ້ອງຜ່ານ Owner ກ່ອນທີ່ຈະບັນທຶກຕົວຈິງ) ຕ້ອງການອອກແບບເພີ່ມເຕີມ
- **ປະເພດຄ່າໃຊ້ຈ່າຍແມ່ນຖືກກໍານົດເປັນ Enum ຄົງທີ່ 6 ມູນຄ່າ** (Utilities, Maintenance, Salaries, Supplies, Marketing,
  Other) ແທນທີ່ຈະໃຫ້ຜູ້ໃຊ້ໃນການກໍານົດປະເພດຂອງຕົນເອງໄດ້ຢ່າງເສລີ, ການຈັດກຸ່ມ/ສ້າງບົດລາຍງານສະຫຼຸບໄດ້ຢ່າງງ່າຍດາຍໃນອະນາຄົດ — ຖ້າມັນບໍ່ພຽງພໍ
  ທ່ານຕ້ອງເພີ່ມມູນຄ່າໃຝ່ໃນ Enum (ມັນບໍ່ແມ່ນຕາຕະລາງປະເພດແຍກຕ່າງຫາກ. ເນື່ອງຈາກວ່າມັນເຫັນວ່າຈໍານວນປະເພດແມ່ນຄົງທີ່ແລະບໍ່ປ່ຽນແປງເລື້ອຍໆ)
- **ບໍ່ມີສິດ `expenses.*` ໃຫ້ Staff/Technician** ເນື່ອງຈາກວ່າມັນຖືກເບິ່ງເປັນຂໍ້ມູນທາງດ້ານການເງິນພາຍໃນທີ່ບໍ່ຈໍາເປັນຕ້ອງໃຫ້
  ພະນັກງານເຄົາເຕີມີການເຂົ້າເຖິງທີ່ແຕກຕ່າງກັນ Payments ທີ່ Staff ຍັງມີສິດ View ແມ່ນແລ້ວ (ເນື່ອງຈາກວ່າມັນກ່ຽວຂ້ອງໂດຍກົງກັບຜູ້ເຊົ່າ)
- **Parcels ຜູກມັດກັບຫ້ອງ (`RoomId`) ສະເຝີ, ສໍາລັບຜູ້ເຊົ່າ (`TenantId`) ມັນເປັນທາງເລືອກ** ເພາະພະນັກງານເຄົາເຕີ
  ມັນມັກຈະເປັນໄປໄດ້ທີ່ຈະບັນທຶກຊຸດທີ່ມາຮອດກ່ອນ, ຢືນຢັນວ່າຜູ້ເຊົ່າຢູ່ໃນຫ້ອງໃດ (ຫ້ອງໜຶ່ງອາດມີຜູ້ເຊົ່າຫຼາຍຄົນ)
  ແລະຫ້ອງແມ່ນຜ່ວຍງານທີ່ຈໍາເປັນສໍາລັບຂອບເຂດຂອງອົງກອນ/ຫໍພັກ (`Tenant` ບໍ່ຜູກມັດກັບຫໍພັກຖາວອນເຊັ່ນ `Room`)
- **ຜູ້ຮັບຕົວຈິງຂອງຊຸດແມ່ນບັນທຶກໄວ້ພາຍໃຕ້ຊື່ເອກະລາດ (`PickedUpByName`) ບໍ່ແມ່ນການອ້າງອີງ `TenantId`** ເນື່ອງຈາກວ່າໃນການປະຕິບັດ
  ບຸກຄົນທີ່ມາຮັບເອົາອາດຈະບໍ່ແມ່ນຜູ້ເຊົ່າໂດຍກົງ (ເຊັ່ນ: ເພື່ອນຮ່ວມຫ້ອງ ຫຼືຍາດພີ່ນ້ອງມາຮັບແທນ) — ບໍ່ມີການຢັ້ງຢືນຕົວຕົນທີ່ເຄັ່ງຄັດອີກຕໍ່ໄປ
- **ສິດ `parcels.*` ໃຫ້ Staff ເຕັມ** (View/Log/Pickup) ແຕກຕ່າງຈາກ Deposits/Receipts/Expenses ວ່າບໍ່ໃຫ້
  Staff ທັງຝົດ, ເນື່ອງຈາກວ່າ parcels ແມ່ນວຽກງານປະຈໍາວັນໂດຍກົງຢູ່ທາງຜ້າຂອງ counter ໄດ້. ບໍ່ແມ່ນຂໍ້ມູນທາງດ້ານການເງິນພາຍໃນ — ໃຫ້ Accountant/Technician
  ບໍ່ມີສິດໃດໆເພາະວ່າພວກເຂົາບໍ່ກ່ຽວຂ້ອງກັບການເຮັດວຽກຂອງສອງບົດບາດນີ້
- **Visitors ອອກແບບໃນລັກສະນະດຽວກັນກັບ Parcels ໃນທຸກໆດ້ານ** (ສະເຝີ tied ກັບຫ້ອງຜູ້ເຊົ່າເປັນທາງເລືອກ, ສິດທີ່ໄດ້ຮັບ
  Owner/DormManager/Staff ການບໍລິການເຕັມທີ່ຄືກັບການເຮັດວຽກຢູ່ເຄົາເຕີ) ເນື່ອງຈາກວ່າພວກເຂົາເຈົ້າແມ່ນກໍລະນີການນໍາໃຊ້ທີ່ຄ້າຍຄືກັນຫຼາຍ
  (ບັນທຶກຄົນເຂົ້າມາ-ອອກໃຫ້ໂດຍຄົນທີ່ບໍ່ແມ່ນຜູ້ເຊົ່າເອງ) ພຽງແຕ່ແຕກຕ່າງກັນ Visitors ບໍ່ມີສະຖານະ "ສູນເສຍ/ຍົກເລີກ" ເພາະວ່າຂ້ອຍເຂົ້າ-ມີພຽງແຕ່ສອງລັດເທົ່ານັ້ນ
- **ສາມາດເກັບກຳເລກບັດປະຈຳຕົວຂອງຜູ້ເຂົ້າຊົມ (`IdNumber`) ມັນເປັນທາງເລືອກ** ສໍາລັບຫໍພັກທີ່ຕ້ອງການບັນທຶກ
  ເພື່ອຄວາມປອດໄພຕາມລະບຽບການພາຍໃນ, ແຕ່ບໍ່ຈໍາເປັນເພາະວ່າເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸກົດລະບຽບໃນພື້ນທີ່ນີ້
  (ການພິຈາລະນາຄວນໄດ້ຮັບການຮັກສາຂໍ້ມູນສ່ວນຕົວໃຫ້ສອດຄ່ອງກັບ PDPA ກ່ອນທີ່ຈະຖ່າຍທອດສົດຖ້າຊ່ອງຂໍ້ມູນນີ້ຖືກເປີດໃຊ້)
- **Parking ຜູ້ເຊົ່າຕ້ອງຖືກລະບຸສະເໝີ (`TenantId` ບັງຄັບ) ແຕກຕ່າງຈາກ Parcels/Visitors ວ່າຜູ້ເຊົ່າມີທາງເລືອກ**
  ເນື່ອງຈາກວ່າໃນການປະຕິບັດລົດຕ້ອງມີເຈົ້າຂອງທີ່ກໍານົດຕົວສະເຝີ. ບໍ່ຄືກັບ parcel/ນັກທ່ອງທ່ຽວທີ່ອາດຈະຍັງບໍ່ທັນຮູ້
  ມັນເປັນທີ່ຊັດເຈນເມື່ອມັນຖືກບັນທຶກຄັ້ງທໍາອິດ — ຫ້ອງຍັງຖືກເກັບຮັກສາໄວ້ສໍາລັບຂອບເຂດຂອງອົງກອນ/ຫໍພັກຕາມປົກກະຕິ
- **ບໍ່ມີ Entity ແມ່ຂອງ "ບ່ອນຈອດລົດ" ແຍກ** (`ParkingSpotLabel` ຂໍ້ຄວາມແມ່ນບໍ່ເສຍຄ່າ ບໍ່ໄດ້ຜູກມັດກັບລະບົບການຈັດການ
  ສະລັອດຕິງບ່ອນຈອດລົດຈໍາກັດຫຼືການຈອງລ່ວງຜ້າ) ເນື່ອງຈາກວ່າບໍ່ມີການສະຜອງໃນເລື່ອງນີ້ຈາກເອກະສານຕົ້ນສະບັບ. ຖ້າທ່ານຕ້ອງການຈັດການ
  ບ່ອນຈອດລົດທີ່ເປັນລະບົບ (ຕົວຢ່າງ, ການຈໍາກັດຈໍານວນຊ່ອງທາງ, ປ້ອງກັນການປະທະກັນ) ຕ້ອງໄດ້ຮັບການອອກແບບຕໍ່ມາ
- **ປົກປ້ອງພຽງແຕ່ການລົງທະບຽນຍານພາຫະນະດຽວກັນຍ້ອນວ່າມັນຍັງລົງທະບຽນ Active ຊ້ອນສອງລາຍການ** ມັນບໍ່ໄດ້ປົກປ້ອງຜູ້ເຊົ່າດຽວກັນ
  ລົງທະບຽນລົດຫຼາຍຄັນໃນເວລາດຽວກັນ (ໃນຄວາມເປັນຈິງ, ອາດຈະມີຫຼາຍກວ່າຜຶ່ງລົດ) ແລະບໍ່ມີຂອບເຂດຈໍາກັດກ່ຽວກັບຈໍານວນລົດຕໍ່ຫ້ອງ/ຕໍ່ຜູ້ເຊົ່າ
- **Announcement ຜູກມັດຢູ່ຫໍພັກໜຶ່ງສະເໝີ (`DormitoryId` ບັງຄັບ)** ບໍ່ຮອງຮັບການປະກາດວິສາຫະກິດພ້ອມກັນ
  ທຸກໆຫໍພັກໃນອົງກອນ ເນື່ອງຈາກວ່າເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸລາຍລະອຽດຂອງໂມດູນນີ້ເລີຍ. ຖ້າທ່ານຕ້ອງການປະກາດໃນລະດັບບໍລິສັດ
  ຕ້ອງການອອກແບບເພີ່ມເຕີມ (ຕົວຢ່າງ, ສ້າງການປະກາດແຍກຕ່າງຫາກສໍາລັບແຕ່ລະຫໍພັກຫຼືເພີ່ມ `DormitoryId` ທາງເລືອກຕໍ່ມາ)
- **ວົງຈອນຊີວິດມີສາມສະຖານະຄົງທີ່**: `Draft` (ກໍ່ສ້າງຄືນໃໝ່) → `Published` (ເຜີຍແຜ່ຊຸດ `PublishedAt`) →
  `Archived` (ເອົາເຂົ້າໄປໃນບ່ອນເກັບຮັກສາ) ຍ່າງໄປໃນທິດທາງດຽວ, ບໍ່ມີ Endpoint ແກ້ໄຂເນື້ອຫາຫຼືກັບຄືນຈາກ `Published` ໄປ `Draft`
  ເນື່ອງຈາກວ່າເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸລາຍລະອຽດຂອງການປັບປຸງການປະກາດທີ່ໄດ້ພິມອອກແລ້ວ
- **ບໍ່ມີສິດ `announcements.archive` ແຍກ** ໃຊ້ສິດຂອງເຈົ້າ `announcements.publish` ດຽວກັນກວມເອົາທັງສອງ
  ການພິມເຜີຍແຜ່ ແລະເກັບຂໍ້ມູນ ເນື່ອງຈາກທັງສອງແມ່ນການປ່ຽນແປງສະຖານະທີ່ເຮັດໂດຍອົງກອນຈັດພິມດຽວກັນສະເໝີ
  ໃນການປະຕິບັດ, ແຕກຕ່າງຈາກ Parcels/Visitors ທີ່ແຍກສິດ Log/Pickup ຫຼື CheckIn/CheckOut ເນື່ອງຈາກວ່າໃນກໍລະນີດັ່ງກ່າວມັນມັກຈະເຮັດໂດຍ
  ພະນັກງານທີ່ແຕກຕ່າງກັນກ່ຽວກັບການປ່ຽນແປງທີ່ແຕກຕ່າງກັນ
- **ສິດ `announcements.*` ໃຫ້ Owner/DormManager ເຕັມແຕ່ Staff ຂ້າພະເຈົ້າພຽງແຕ່ສາມາດ View** ແຕກຕ່າງຈາກ
  Parcels/Visitors/Parking ທີ່ໃຫ້ Staff ຢ່າງເຕັມທີ່ເນື່ອງຈາກວ່າການປະກາດເປັນການສື່ສານຢ່າງເປັນທາງການຈາກການຄຸ້ມຄອງ
  ມັນບໍ່ແມ່ນວຽກປະຈຳວັນຢູ່ໜ້າເຄົາເຕີ — Accountant/Technician ບໍ່ມີສິດໃດໆເພາະວ່າສອງບົດບາດນີ້ບໍ່ກ່ຽວຂ້ອງກັບການເຮັດວຽກ
- **Notification ບໍ່ໄດ້ສືບທອດມາຈາກ `TenantEntity`** ບໍ່ເຝືອນກັບທຸກໂມດູນທີ່ຜ່ານມາຂອງ Phase 2 ເນື່ອງຈາກວ່າມັນຖືກຜູກມັດກັບຜູ້ໃຊ້ຜູ້ຮັບ
  ໂດຍກົງ (`RecipientUserId`) ມັນບໍ່ແມ່ນຂໍ້ມູນທີ່ຕ້ອງໄດ້ຮັບການກັ່ນຕອງຜ່ານຫໍພັກ List Endpoint ທົ່ວໄປ — ໃຊ້ຄໍາແນະນໍາດຽວກັນກັບ
  `AuditLog` ທີ່ບໍ່ໄດ້ໃຊ້ `TenantScope` ເນື່ອງຈາກວ່າມັນບໍ່ແມ່ນຊັບພະຍາກອນທີ່ຖືກແບ່ງປັນໂດຍຜູ້ໃຊ້ຫຼາຍຄົນໃນທົ່ວຂອບເຂດຂອງອົງກອນ/ຫໍພັກ
- **ການເບິ່ງ ແລະໝາຍການແຈ້ງເຕືອນຕົນເອງວ່າອ່ານແລ້ວແມ່ນມີຢູ່ສະເໝີສຳລັບຜູ້ໃຊ້ທີ່ເຂົ້າສູ່ລະບົບທັງໝົດ** ບໍ່ໄດ້ຜູກມັດກັບ
  ສິດໃດ? (`[Authorize]` ຄືກັນກັບ `GET /auth/me`) ເນື່ອງຈາກວ່າມັນເປັນຂໍ້ມູນຂອງຕົນເອງໂດຍກົງ ມັນບໍ່ແມ່ນຂໍ້ມູນທີ່ຈະຕ້ອງໄດ້ກວດສອບ
  ບົດບາດ — ມີພຽງແຕ່ສິດ `notifications.send` ການອະນຸຍາດດຽວທີ່ຄວບຄຸມວ່າໃຜສາມາດສົ່ງການແຈ້ງເຕືອນໃຫ້ຄົນອື່ນ (Owner/DormManager)
- **ການແຈ້ງເຕືອນສາມາດຖືກສົ່ງໄປຫາຜູ້ໃຊ້ຢູ່ໃນອົງກອນດຽວກັນເທົ່ານັ້ນ** (ຍົກເວັ້ນ SuperAdmin ສາມາດສົ່ງຜ່ານອົງກອນ) ກວດເບິ່ງຈາກ
  `OrganizationId` ຂອງຜູ້ຮັບທຽບກັບຜູ້ສົ່ງ ຖ້າພວກເຂົາບໍ່ກົງກັນ, ຕອບ 404 ເພື່ອບໍ່ໃຫ້ຄາດເດົາວ່າມີຜູ້ໃຊ້ລະຫັດນັ້ນແທ້ໆ
  ອົງກອນອື່ນໆປະຕິບັດຕາມຫຼັກການດຽວກັນກັບ `TenantScope.EnsureOrganizationAccess`
- **ບໍ່ມີການແຈ້ງເຕືອນອັດຕະໂນມັດຈາກເຫດການອື່ນໆໃນລະບົບ** (ສໍາລັບຕົວຢ່າງ, ອອກໃບແຈ້ງຜີ້ແລະແຈ້ງໃຫ້ຜູ້ເຊົ່າ, ວຽກງານສ້ອມແປງແມ່ນສໍາເລັດ
  ແຈ້ງຜູ້ໃຫ້ຂໍ້ມູນ) — ໂມດູນນີ້ແມ່ນພຽງແຕ່ກົນໄກການສົ່ງຕໍ່/ຮັບການແຈ້ງເຕືອນດ້ວຍມືເທົ່ານັ້ນ ການເຊື່ອມຕໍ່ອັດຕະໂນມັດກັບເຫດການ
  ໂມດູນອື່ນໆຕ້ອງໄດ້ຮັບການອອກແບບຕໍ່ມາ (ເຊັ່ນ Domain Event ຫຼື Background Job)
- **ຜູ້ຮັບແມ່ນ User ຂອງລະບົບ (ພະນັກງານ/ຜູ້ດູແລ) ເທົ່ານັ້ນ, ບໍ່ Tenant** ຍ້ອນບົດບາດ Tenant ຍັງບໍ່ທັນ Endpoint
  ຫຼືກົນໄກການຢັ້ງຢືນຕົວຕົນແຍກຕ່າງຫາກສໍາລັບປະຕູຜູ້ເຊົ່າໃນຂອບເຂດນີ້ (ເບິ່ງຫົວຂໍ້ Tenants ເທິງ)
- **Settings ຮັກສາມັນເປັນຄູ່ Key/Value ເອກະລາດກັບອົງກອນ** (`Setting.Key`/`Setting.Value` ມັນເປັນຂໍ້ຄວາມຟຣີ, ມັນບໍ່ແມ່ນ
  ຖັນຄົງທີ່ຕັ້ງແຕ່ລະຄັ້ງ) ເນື່ອງຈາກວ່າເອກະສານຕົ້ນສະບັບບໍ່ໄດ້ລະບຸບັນຊີລາຍຊື່ຂອງການຕັ້ງຄ່າທີ່ກໍານົດໄວ້ທັງຝົດ. ມີພຽງແຕ່ສິດ
  `settings.manage` ທີ່ໄດ້ລະບຸໄວ້ຕັ້ງແຕ່ນັ້ນມາ Phase 1 ບ່ອນຈັດເກັບຂໍ້ມູນຟຣີອະນຸຍາດໃຫ້ທ່ານເພີ່ມການຕັ້ງຄ່າໃໝ່ໃນອະນາຄົດໄດ້ໂດຍບໍ່ຈໍາເປັນຕ້ອງ
  Migration ທຸກຄັ້ງ, ໃນການແລກປ່ຽນສໍາລັບການບໍ່ມີ Type Safety ຫຼື Validation ສະເພາະແຕ່ລະການຕັ້ງຄ່າ (ຝັ່ງ Client ເຈົ້າຕ້ອງຮູ້ຈັກຕົວເອງ
  ລະຫັດໃດຄວນມີຮູບແບບມູນຄ່າໃດ?) — ມັນບໍ່ທັບຊ້ອນກັບການຕັ້ງຄ່າທີ່ມີໂຄງສ້າງທີ່ຊັດເຈນ, ເຊັ່ນ: ການຕັ້ງຄ່າການເອີ້ນເກັບເງິນຂອງ
  `Dormitory` (`BillingDay`, `PaymentDueDay`, `LateFeeType/Amount`, `WaterRatePerUnit`/`ElectricityRatePerUnit`)
- **Settings ມັນເປັນລະດັບບໍລິສັດເທົ່ານັ້ນ. ບໍ່ແມ່ນລະດັບຫໍພັກ** (`Setting` ສືບທອດມາຈາກ `BaseEntity` ບໍ່ `TenantEntity`
  ມັກ `Tenant`) ເພາະສິດ `settings.manage` ຕົ້ນສະບັບຂອງ Phase 1 ຢູ່ໃນປະເພດ Administration ລະດັບວິສາຫະກິດ
  ບໍ່ມີສັນຍານທີ່ຈະຈຳເປັນຕ້ອງແຍກຄ່າທຳນຽມຂອງແຕ່ລະຫໍພັກ. ຖ້າຫາກວ່າທ່ານຕ້ອງການທີ່ຈະສ້າງຕັ້ງພຽງແຕ່ຫໍພັກໃນອະນາຄົດພາກສະຜາມຄວນຈະໄດ້ຮັບການເພີ່ມເຂົ້າໄປໃນ `Dormitory`
  ໂດຍກົງຄືກັບການຕັ້ງຄ່າໃບບິນທີ່ມີຢູ່ກ່ອນແລ້ວ ໂມດູນນີ້ບໍ່ໄດ້ຖືກຂະຫຍາຍອອກ
- **ສ້າງແລະແກ້ໄຂການນໍາໃຊ້ `PUT` ດຽວກັນ (Upsert) ແທນທີ່ຈະແຍກ `POST`/`PUT`** ເນື່ອງຈາກວ່າກຸນແຈແມ່ນຕົວຕົນຂອງການຕັ້ງຄ່າ
  ແລ້ວ (ບໍ່ມີ ID ການແບ່ງປັນທີ່ຕ້ອງຖືກສ້າງຂຶ້ນກ່ອນທີ່ຈະສາມາດແກ້ໄຂໄດ້) — ໂທ `PUT` ບໍ່ວ່າທ່ານເຮັດຊ້ຳກະແຈອັນດຽວກັນຫຼາຍປານໃດ, ທ່ານກໍໄດ້ຮັບຜົນຄືກັນສະເໝີ
  (Idempotent) ສອດຄ້ອງກັບຄວາມຝາຍຂອງ HTTP `PUT` ຫຼາຍກວ່າ `POST`
- **Unique Index ສຸດ `(organization_id, key)` ການກັ່ນຕອງດ້ວຍ `is_deleted = false`** (ບໍ່ Unique Index indifferent)
  ປະຕິບັດຕາມຮູບແບບດຽວກັນກັບທັງຝົດ Unique Index ຂອງ Entity ໃຊ້ Soft Delete ໃນລະບົບ (ເຊັ່ນ `Deposit.ContractId`,
  `Receipt.PaymentId`) ນີ້ອະນຸຍາດໃຫ້ທ່ານສ້າງການຕັ້ງຄ່າລະຫັດດຽວກັນຄືນໃຝ່ຫຼັງຈາກລຶບລາຍການຕົ້ນສະບັບ — ຖ້າລືມໃສ່ Filter
  ມັນບໍ່ສາມາດຖືກລຶບໄດ້ຢ່າງແທ້ຈິງເພາະວ່າແຖວຕົ້ນສະບັບຍັງຢູ່ໃນຖານຂໍ້ມູນ (Soft Delete) ແລະ collided ກັບ Unique Index ທັນທີທີ່ສ້າງໃຝ່

### ຍັງບໍ່ແລ້ວ

ບໍ່ມີສ່ວນຜຶ່ງຂອງ Phase 2 ຍັງເຫຼືອ — ໂມດູນທັງຝົດທີ່ມີຊື່ໃນເອກະສານຕົ້ນສະບັບແມ່ນຖືກສ້າງຂຶ້ນຢ່າງເຕັມສ່ວນ
ລາຍການທັງໝົດທີ່ຍັງບໍ່ທັນສຳເລັດໃນລະບົບແມ່ນ: Endpoint ຢືນຢັນລະຫັດ Two-Factor Authentication ຂອງ Phase 1
(ເບິ່ງຫົວຂໍ້ [ຂອບເຂດຂອງຂັ້ນຕອນ 1](#ຂອບເຂດຂອງຂັ້ນຕອນ-1)) ແລະທົບທວນຄືນສົມມຸດຕິຖານທຸລະກິດທັງຝົດ
Phase 2 ກັບເຈົ້າຂອງລະບົບຕົວຈິງກ່ອນທີ່ຈະໃຊ້ມັນ (ເບິ່ງບັນຊີລາຍຊື່ເຕັມຢູ່ໃນພາກຂ້າງເທິງ)
