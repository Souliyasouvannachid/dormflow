# DormFlow — Dormitory Management System (Backend API)

ระบบบริหารจัดการหอพักและอพาร์ตเมนต์แบบหลายองค์กร (Multi-Organization SaaS)
สร้างด้วย ASP.NET Core Web API บน .NET 10 ตามแนวทาง **Modular Monolith + Clean Architecture**

> สถานะปัจจุบัน: **Phase 1 เสร็จสมบูรณ์ (ขั้นตอนที่ 1-3)**
> ครบทั้ง Identity, JWT, Refresh Token, Roles & Permissions และ Endpoint บริหารจัดการ
> Organizations, Dormitories, Buildings, Floors, Room Types, Rooms, Users, User-Dormitory
> Assignment, Roles/Permissions และ Audit Logs ตามที่ระบุไว้ใน Phase 1 ทั้งหมด
> ดูรายละเอียดได้ที่หัวข้อ [ขอบเขตของขั้นตอนที่ 1](#ขอบเขตของขั้นตอนที่-1)

---

## สารบัญ

- [เทคโนโลยีที่ใช้](#เทคโนโลยีที่ใช้)
- [โครงสร้าง Solution](#โครงสร้าง-solution)
- [ทิศทางการพึ่งพาระหว่าง Layer](#ทิศทางการพึ่งพาระหว่าง-layer)
- [สิ่งที่ต้องติดตั้งก่อน](#สิ่งที่ต้องติดตั้งก่อน)
- [เริ่มต้นใช้งานบนเครื่องนักพัฒนา](#เริ่มต้นใช้งานบนเครื่องนักพัฒนา)
- [รันด้วย Docker Compose](#รันด้วย-docker-compose)
- [การตั้งค่าและตัวแปรสภาพแวดล้อม](#การตั้งค่าและตัวแปรสภาพแวดล้อม)
- [Endpoint ที่ใช้งานได้ในขั้นตอนนี้](#endpoint-ที่ใช้งานได้ในขั้นตอนนี้)
- [รูปแบบ API Response](#รูปแบบ-api-response)
- [การทดสอบ](#การทดสอบ)
- [Entity Framework Core และ Migration](#entity-framework-core-และ-migration)
- [ข้อตกลงในการเขียนโค้ด](#ข้อตกลงในการเขียนโค้ด)
- [ขอบเขตของขั้นตอนที่ 1](#ขอบเขตของขั้นตอนที่-1)

---

## เทคโนโลยีที่ใช้

| ส่วน | เทคโนโลยี | เวอร์ชัน |
| --- | --- | --- |
| Runtime | .NET | 10.0 |
| Web | ASP.NET Core Web API | 10.0 |
| ORM | Entity Framework Core | 10.0 |
| ฐานข้อมูล | PostgreSQL (Npgsql provider) | 17 / 10.0 |
| การตั้งชื่อคอลัมน์ | EFCore.NamingConventions (snake_case) | 10.0 |
| Validation | FluentValidation | 12.1 |
| Logging | Serilog | 10.0 |
| เอกสาร API | Microsoft.AspNetCore.OpenApi + Scalar | 10.0 / 2.17 |
| API Versioning | Asp.Versioning | 10.2 |
| Test framework | xUnit v3 + FluentAssertions | 4.0 / 7.2 |
| Integration test | Testcontainers for PostgreSQL | 4.14 |
| Architecture test | NetArchTest.Rules | 1.3 |

> **หมายเหตุเรื่องสัญญาอนุญาต** — โปรเจกต์ตรึง FluentAssertions ไว้ที่ **7.2.2**
> ซึ่งเป็นเวอร์ชันสุดท้ายที่ใช้ Apache-2.0 ตั้งแต่ 8.x เป็นต้นไปต้องซื้อ license สำหรับการใช้งานเชิงพาณิชย์
> หากองค์กรซื้อ license แล้ว เปลี่ยนเวอร์ชันได้ที่ `Directory.Packages.props` ที่เดียว

---

## โครงสร้าง Solution

```
DormFlow.sln
├── global.json                  ตรึงเวอร์ชัน SDK และเปิดใช้ Microsoft Testing Platform
├── Directory.Build.props        ค่าตั้ง MSBuild ที่ใช้ร่วมกันทุกโปรเจกต์
├── Directory.Packages.props     Central Package Management (เวอร์ชันแพ็กเกจอยู่ที่นี่ที่เดียว)
├── .editorconfig                ข้อตกลงการจัดรูปแบบและการตั้งชื่อ
├── docker-compose.yml           dormflow-api + dormflow-postgres
├── .env.example                 ต้นแบบของไฟล์ .env
│
├── src/
│   ├── Directory.Build.props            เปิด TreatWarningsAsErrors เฉพาะโค้ด production
│   ├── DormFlow.Domain/                 Entity, Enum, Value Object, Domain Event, Business Rule
│   ├── DormFlow.Contracts/              Request / Response Model ที่ใช้ร่วมกับ Client
│   ├── DormFlow.Application/            Use Case, Command, Query, DTO, Interface, Validation
│   ├── DormFlow.Infrastructure/         EF Core, PostgreSQL, Repository, External Service
│   └── DormFlow.Api/                    Controller, Middleware, DI, OpenAPI, Health Check
│
└── tests/
    ├── Directory.Build.props
    ├── DormFlow.UnitTests/              ตรรกะล้วน ไม่แตะ I/O
    ├── DormFlow.IntegrationTests/       ยิงผ่าน HTTP จริงบน PostgreSQL ใน Testcontainers
    └── DormFlow.ArchitectureTests/      บังคับทิศทางการพึ่งพาระหว่าง Layer
```

---

## ทิศทางการพึ่งพาระหว่าง Layer

```
        Contracts  ◄──────────────┐
            ▲                     │
            │                     │
Domain ◄── Application ◄── Infrastructure ◄── Api
```

- **Domain** ไม่พึ่งพาอะไรเลย ไม่รู้จัก EF Core, ASP.NET Core หรือ Npgsql
- **Application** พึ่งพาได้เฉพาะ Domain และ Contracts
- **Infrastructure** พึ่งพา Application ได้ แต่ห้ามพึ่งพา Api
- **Api** เป็นชั้นนอกสุด พึ่งพาได้ทุกชั้น

กฎเหล่านี้ถูกบังคับด้วยเทสต์จริงใน `tests/DormFlow.ArchitectureTests`
การเผลอเพิ่ม `ProjectReference` ผิดทิศทางจะทำให้เทสต์ล้มเหลวทันที

---

## สิ่งที่ต้องติดตั้งก่อน

| เครื่องมือ | เวอร์ชัน | ใช้ทำอะไร |
| --- | --- | --- |
| .NET SDK | 10.0.400 ขึ้นไป | build และรันโปรเจกต์ |
| Docker Desktop / Docker Engine | ล่าสุด | รัน PostgreSQL, รัน compose และ Integration Test |
| PostgreSQL | 17 | ใช้ผ่าน Docker ไม่ต้องติดตั้งบนเครื่อง |

ตรวจเวอร์ชัน SDK:

```bash
dotnet --version   # ต้องได้ 10.0.400 ขึ้นไป
```

> `global.json` ตรึง SDK ไว้ที่ `10.0.400` แบบ `rollForward: latestFeature`
> ถ้าเครื่องมี SDK เก่ากว่านี้ คำสั่ง `dotnet` จะแจ้งให้อัปเดตก่อน

---

## เริ่มต้นใช้งานบนเครื่องนักพัฒนา

### 1. เตรียมฐานข้อมูล

```bash
cp .env.example .env
# แก้ POSTGRES_PASSWORD ในไฟล์ .env ก่อน
docker compose up -d dormflow-postgres
```

ตรวจว่าฐานข้อมูลพร้อมใช้งาน:

```bash
docker compose ps dormflow-postgres      # STATUS ต้องเป็น healthy
```

### 2. Restore และ Build

```bash
dotnet restore
dotnet build
```

### 3. รัน API

```bash
dotnet run --project src/DormFlow.Api
```

จะได้:

| ปลายทาง | URL |
| --- | --- |
| เอกสาร API แบบโต้ตอบ (Scalar) | http://localhost:5080/scalar/v1 |
| เอกสาร OpenAPI (JSON) | http://localhost:5080/openapi/v1.json |
| Health Check | http://localhost:5080/health |

Connection String ของสภาพแวดล้อม Development อยู่ใน `src/DormFlow.Api/appsettings.Development.json`
และชี้ไปที่ PostgreSQL ของ docker compose บนเครื่องอยู่แล้ว

---

## รันด้วย Docker Compose

```bash
cp .env.example .env
# แก้ POSTGRES_PASSWORD, ConnectionStrings__DefaultConnection และ Cors__AllowedOrigins__0

docker compose up -d --build
docker compose ps
docker compose logs -f dormflow-api
```

ตรวจสอบสุขภาพระบบ:

```bash
curl http://localhost:8080/health
curl http://localhost:8080/health/ready
curl http://localhost:8080/health/live
```

หยุดการทำงาน:

```bash
docker compose down            # หยุดคอนเทนเนอร์ แต่ยังเก็บข้อมูลไว้
docker compose down -v         # ลบ volume ของฐานข้อมูลทิ้งด้วย
```

**บริการใน compose**

| Service | Image | พอร์ต | Health Check |
| --- | --- | --- | --- |
| `dormflow-postgres` | `postgres:17-alpine` | `${POSTGRES_PORT:-5432}` | `pg_isready` |
| `dormflow-api` | build จาก `src/DormFlow.Api/Dockerfile` | `${API_HTTP_PORT:-8080}` | `curl /health/live` |

`dormflow-api` จะเริ่มทำงานก็ต่อเมื่อ `dormflow-postgres` รายงานสถานะ `healthy` แล้วเท่านั้น

> Health Check ของ API เรียกเฉพาะ `/health/live` โดยตั้งใจ
> เพื่อไม่ให้คอนเทนเนอร์ถูกรีสตาร์ตทิ้งเพียงเพราะฐานข้อมูลล่มไปชั่วครู่

---

## การตั้งค่าและตัวแปรสภาพแวดล้อม

ค่าตั้งทั้งหมดอ่านผ่าน configuration ปกติของ ASP.NET Core
กำหนดทับด้วย environment variable โดยใช้ `__` แทนการลงชั้น เช่น `ConnectionStrings__DefaultConnection`

| ตัวแปร | ค่าเริ่มต้น | คำอธิบาย |
| --- | --- | --- |
| `ConnectionStrings__DefaultConnection` | *(ไม่มี)* | **จำเป็น** ระบบจะไม่สตาร์ตถ้าไม่ได้กำหนด |
| `ASPNETCORE_ENVIRONMENT` | `Production` | `Development` เปิดเอกสาร OpenAPI และ Log ระดับ Debug |
| `Https__EnableRedirection` | `true` | ตั้งเป็น `false` เมื่อมี reverse proxy จัดการ TLS ให้แล้ว |
| `Cors__AllowedOrigins__0` … | *(ว่าง)* | **จำเป็นใน Production** ไม่กำหนดแล้วระบบจะไม่ยอมสตาร์ต |
| `RateLimiting__Enabled` | `true` | เปิดหรือปิด Rate Limiting |
| `RateLimiting__PermitLimit` | `100` | จำนวนคำขอต่อหนึ่งช่วงเวลา |
| `RateLimiting__WindowSeconds` | `60` | ความยาวของช่วงเวลา |
| `Database__CommandTimeoutSeconds` | `30` | เวลาสูงสุดของหนึ่งคำสั่ง SQL |
| `Database__MaxRetryCount` | `3` | จำนวนครั้งที่ลองเชื่อมต่อใหม่เมื่อเจอข้อผิดพลาดชั่วคราว |
| `Database__EnableSensitiveDataLogging` | `false` | **ห้ามเปิดใน Production** ทำให้ค่าพารามิเตอร์ SQL ปรากฏใน Log |
| `Database__AutoMigrate` | `false` | รัน Migration ที่ค้างอยู่อัตโนมัติตอนเริ่มระบบ เปิดเฉพาะเครื่องนักพัฒนา |
| `Database__SeedOnStartup` | `true` | สร้างสิทธิ์ บทบาท และบัญชีผู้ดูแลระบบเริ่มต้นตอนเริ่มระบบ (ทำงานได้หลายรอบโดยไม่สร้างซ้ำ) |
| `Jwt__SigningKey` | *(ไม่มี)* | **จำเป็น** ต้องยาวอย่างน้อย 32 ตัวอักษร ใช้เซ็นและตรวจ Access Token |
| `Jwt__Issuer` | `DormFlow` | ผู้ออกโทเคน |
| `Jwt__Audience` | `DormFlow.Client` | ผู้รับโทเคน |
| `Jwt__AccessTokenMinutes` | `15` | อายุของ Access Token |
| `Jwt__RefreshTokenDays` | `7` | อายุของ Refresh Token |
| `SEED_ADMIN_EMAIL` | *(ไม่มี)* | อีเมลผู้ดูแลระบบเริ่มต้น ไม่กำหนดแล้วระบบจะข้ามการสร้างบัญชีนี้ |
| `SEED_ADMIN_PASSWORD` | *(ไม่มี)* | รหัสผ่านผู้ดูแลระบบเริ่มต้น ต้องผ่านนโยบายรหัสผ่านเดียวกับผู้ใช้ทั่วไป |
| `SEED_ADMIN_FIRST_NAME` / `SEED_ADMIN_LAST_NAME` | `System` / `Administrator` | ชื่อของบัญชีผู้ดูแลระบบเริ่มต้น |

### หลักการเรื่องความลับ

- `appsettings.json` **ไม่มี** ค่าลับใด ๆ ช่อง Connection String และ `Jwt:SigningKey` ถูกเว้นว่างไว้โดยตั้งใจ
- `appsettings.Development.json` มีเฉพาะรหัสผ่านของ PostgreSQL และกุญแจ JWT สำหรับเครื่องนักพัฒนาเท่านั้น
  ค่าเหล่านี้ไม่ใช่ความลับจริงและตรงกับค่าใน compose ห้ามใช้ค่าเดียวกันใน Production เด็ดขาด
- ค่าลับจริงส่งผ่าน environment variable, Docker Secret หรือ Secret Manager เท่านั้น
- `.env` ถูกกันไว้ใน `.gitignore` และ `.dockerignore` แล้ว
- บัญชีผู้ดูแลระบบเริ่มต้นสร้างจาก `SEED_ADMIN_*` เท่านั้น ไม่มีการ Hardcode รหัสผ่านไว้ในโค้ดเด็ดขาด

---

## Endpoint ที่ใช้งานได้ในขั้นตอนนี้

| Method | Path | สิทธิ์ที่ต้องมี | คำอธิบาย |
| --- | --- | --- | --- |
| `GET` | `/health` | - | ผลการตรวจสอบทั้งหมด |
| `GET` | `/health/live` | - | ตรวจว่ากระบวนการยังทำงานอยู่ ไม่แตะฐานข้อมูล |
| `GET` | `/health/ready` | - | ตรวจว่าเชื่อมต่อ PostgreSQL ได้แล้วหรือยัง |
| `GET` | `/openapi/v1.json` | - | เอกสาร OpenAPI (เฉพาะสภาพแวดล้อมที่ไม่ใช่ Production) |
| `GET` | `/scalar/v1` | - | เอกสาร API แบบโต้ตอบ (เฉพาะสภาพแวดล้อมที่ไม่ใช่ Production) |
| `POST` | `/api/v1/auth/login` | ไม่ต้องยืนยันตัวตน | เข้าสู่ระบบด้วยอีเมลและรหัสผ่าน |
| `POST` | `/api/v1/auth/refresh-token` | ไม่ต้องยืนยันตัวตน | ต่ออายุ Access Token ด้วย Refresh Token (มี Token Rotation) |
| `POST` | `/api/v1/auth/logout` | ต้องเข้าสู่ระบบ | เพิกถอน Refresh Token ที่ส่งมา |
| `POST` | `/api/v1/auth/forgot-password` | ไม่ต้องยืนยันตัวตน | ขอลิงก์ตั้งรหัสผ่านใหม่ทางอีเมล |
| `POST` | `/api/v1/auth/reset-password` | ไม่ต้องยืนยันตัวตน | ตั้งรหัสผ่านใหม่ด้วยโทเคนที่ได้รับทางอีเมล |
| `POST` | `/api/v1/auth/change-password` | ต้องเข้าสู่ระบบ | เปลี่ยนรหัสผ่าน (เพิกถอนเซสชันเดิมทั้งหมด) |
| `GET` | `/api/v1/auth/me` | ต้องเข้าสู่ระบบ | ข้อมูลผู้ใช้ปัจจุบัน พร้อมบทบาทและสิทธิ์ทั้งหมด |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/organizations` | `organizations.*` | จัดการองค์กร (สร้าง/ลบทำได้เฉพาะ SuperAdmin) |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/dormitories` | `dormitories.*` | จัดการหอพัก |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/buildings` | `buildings.*` | จัดการอาคาร |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/floors` | `floors.*` | จัดการชั้นของอาคาร |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/room-types` | `room-types.*` | จัดการประเภทห้อง |
| `GET`/`POST`/`PUT`/`PATCH .../status`/`DELETE` | `/api/v1/rooms` | `rooms.*` | จัดการห้องพัก |
| `GET` | `/api/v1/rooms/available` | `rooms.view` | ห้องว่างที่พร้อมให้เช่า |
| `GET` | `/api/v1/rooms/summary` | `rooms.view` | สรุปจำนวนห้องแยกตามสถานะ |
| `GET`/`POST`/`PUT`/`PATCH .../status` | `/api/v1/users` | `users.manage` | จัดการบัญชีผู้ใช้ |
| `POST` | `/api/v1/users/{id}/roles` | `users.manage` | กำหนดบทบาทให้ผู้ใช้ (แทนที่บทบาทเดิมทั้งหมด) |
| `POST`/`DELETE` | `/api/v1/users/{id}/dormitories` | `users.manage` | มอบหมาย/ถอนหอพักที่ผู้ใช้ดูแล |
| `GET`/`POST`/`PUT`/`DELETE` | `/api/v1/roles` | `roles.manage` | จัดการบทบาท (บทบาทมาตรฐานเปลี่ยนชื่อ/ลบไม่ได้) |
| `POST` | `/api/v1/roles/{id}/permissions` | `roles.manage` | กำหนดสิทธิ์ให้บทบาท (แทนที่สิทธิ์เดิมทั้งหมด) |
| `GET` | `/api/v1/permissions` | `roles.manage` | รายการสิทธิ์ทั้งหมดของระบบ |
| `GET` | `/api/v1/audit-logs` | `audit-logs.view` | ประวัติเหตุการณ์ของระบบ เรียงล่าสุดก่อนเป็นค่าเริ่มต้น |

ทุก List Endpoint รองรับ `page`, `pageSize`, `search`, `sortBy`, `sortDirection` ร่วมกัน
และ Endpoint ที่ข้อมูลผูกกับองค์กร/หอพักรองรับตัวกรอง `organizationId`/`dormitoryId` เพิ่มเติม
ผู้ใช้ที่ไม่ใช่ SuperAdmin ถูกจำกัดให้เห็นเฉพาะข้อมูลในองค์กรของตนเองเสมอที่ชั้น Application
ไม่ว่าคำขอจะระบุตัวกรองเหล่านี้เป็นค่าใดก็ตาม

ตัวอย่างผลลัพธ์ของ `GET /health`:

```json
{
  "status": "Healthy",
  "totalDurationMs": 12.34,
  "checks": [
    { "name": "self", "status": "Healthy", "durationMs": 0.01, "description": "API ทำงานอยู่" },
    { "name": "postgresql", "status": "Healthy", "durationMs": 11.9, "description": null }
  ]
}
```

---

## รูปแบบ API Response

ทุก Endpoint ของ DormFlow ตอบกลับด้วยโครงสร้างเดียวกัน กำหนดไว้ที่
`src/DormFlow.Contracts/Common/ApiResponse.cs`

**สำเร็จ**

```json
{ "success": true, "message": "ดำเนินการสำเร็จ", "data": {} }
```

**สำเร็จ พร้อมข้อมูลแบ่งหน้า**

```json
{
  "success": true,
  "message": "ดึงข้อมูลสำเร็จ",
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

**ข้อมูลไม่ผ่านการตรวจสอบ (400)**

```json
{
  "success": false,
  "message": "ข้อมูลไม่ถูกต้อง",
  "errors": { "name": ["กรุณาระบุชื่อ"] },
  "traceId": "00-..."
}
```

**สถานะข้อผิดพลาดที่ระบบแปลงให้อัตโนมัติ**

| Exception | HTTP | ข้อความ |
| --- | --- | --- |
| `ValidationFailedException`, `FluentValidation.ValidationException` | 400 | ข้อมูลไม่ถูกต้อง |
| `UnauthorizedAccessException` | 401 | กรุณาเข้าสู่ระบบ |
| `ForbiddenAccessException` | 403 | คุณไม่มีสิทธิ์ทำรายการนี้ |
| `NotFoundException` | 404 | ไม่พบข้อมูลที่ต้องการ |
| `ConflictException`, `DomainException` | 409 | ข้อความจาก exception |
| `DbUpdateConcurrencyException` | 409 | ข้อมูลถูกแก้ไขโดยผู้ใช้อื่น |
| อื่น ๆ ทั้งหมด | 500 | เกิดข้อผิดพลาดภายในระบบ |

`traceId` ที่ส่งกลับไปตรงกับ `TraceId` ที่บันทึกไว้ใน Log ใช้ค้นหาต้นเหตุได้ทันที
ข้อความของ exception ที่ไม่คาดคิดจะปรากฏใน Response เฉพาะสภาพแวดล้อม Development เท่านั้น

พารามิเตอร์มาตรฐานของ List Endpoint ทุกตัว: `page`, `pageSize`, `search`, `sortBy`, `sortDirection`
บวกกับ `organizationId` และ `dormitoryId` สำหรับข้อมูลที่แยกตามองค์กร
`pageSize` ถูกจำกัดไม่ให้เกิน 200 รายการเสมอ

---

## การทดสอบ

### รันเทสต์ทั้งหมด

```bash
dotnet msbuild -t:Test
```

รันเฉพาะบางโปรเจกต์:

```bash
dotnet msbuild tests/DormFlow.UnitTests/DormFlow.UnitTests.csproj -t:Test
dotnet msbuild tests/DormFlow.ArchitectureTests/DormFlow.ArchitectureTests.csproj -t:Test
dotnet msbuild tests/DormFlow.IntegrationTests/DormFlow.IntegrationTests.csproj -t:Test
```

> **ทำไมไม่ใช้ `dotnet test`**
> โปรเจกต์เทสต์ใช้ xUnit v3 บน Microsoft Testing Platform ตามมาตรฐานของ .NET 10
> แต่ orchestrator ของ `dotnet test` ใน SDK 10.0.400 ยังเชื่อมต่อกับ Microsoft.Testing.Platform 2.3.3 ไม่สำเร็จ
> และรายงานว่า `Zero tests ran` ทั้งที่เทสต์ทำงานได้ปกติ
> เป้าหมาย `-t:Test` เรียกตัวรันเดียวกันผ่าน MSBuild และทำงานได้ถูกต้อง
> เมื่อ SDK รุ่นถัดไปแก้ปัญหานี้แล้ว `dotnet test` จะใช้งานได้ทันทีโดยไม่ต้องแก้โปรเจกต์

รันไฟล์ปฏิบัติการของเทสต์โดยตรงก็ได้ (แต่ละโปรเจกต์เทสต์ compile เป็น executable):

```bash
dotnet run --project tests/DormFlow.UnitTests
```

### ขอบเขตของเทสต์ในขั้นตอนนี้

| โปรเจกต์ | จำนวน | ครอบคลุม |
| --- | --- | --- |
| `DormFlow.UnitTests` | 132 | รูปแบบ API Response, การแบ่งหน้า, การแปลงข้อผิดพลาด Validation, BaseEntity, Domain Exception, ตรรกะสิทธิ์ของ `ICurrentUser`, วงจรชีวิต Refresh Token, รายการสิทธิ์และสิทธิ์เริ่มต้นของแต่ละบทบาท, กฎ Validation ของ Auth และของหอพัก, การสร้าง/แฮช Refresh Token, `TenantScope` (ตรรกะกันข้อมูลข้ามองค์กร/หอพักที่ทุกโมดูลใช้ร่วมกัน) |
| `DormFlow.ArchitectureTests` | 11 | ทิศทางการพึ่งพาระหว่าง Layer, ตำแหน่งของ `DbContext`, ข้อตกลงการตั้งชื่อ |
| `DormFlow.IntegrationTests` | 33 | Health Check ทั้งสามเส้นทาง, เอกสาร OpenAPI, JWT Bearer Security Scheme, เส้นทางที่ไม่มีอยู่จริง, Login สำเร็จ/ล้มเหลว, `/auth/me`, Refresh Token Rotation, การตรวจจับการใช้ Token ซ้ำ, Logout, Change Password, Forgot Password, ลำดับชั้นทรัพย์สินทั้งสาย (Organization→Dormitory→Building→Floor→RoomType→Room), Rooms available/summary, การกันข้อมูลข้ามองค์กร (Owner เห็นหอพักองค์กรอื่นไม่ได้และสร้างข้ามองค์กรไม่ได้), CRUD ผู้ใช้ + กำหนดบทบาท + มอบหมาย/ถอนหอพัก, บทบาทมาตรฐานเปลี่ยนชื่อ/ลบไม่ได้, Audit Log ถูกบันทึกจริงและค้นหาเจอผ่าน Endpoint |

Integration Test ใช้ **PostgreSQL จริง** ผ่าน Testcontainers ไม่ใช้ InMemory Database
เพราะพฤติกรรมสำคัญหลายอย่าง เช่น Global Query Filter, `timestamptz`, `jsonb`
และ Optimistic Concurrency ด้วยคอลัมน์ `xmin` ไม่มีอยู่ใน InMemory Provider

> Integration Test ต้องการ Docker ที่ทำงานอยู่ ถ้า Docker ไม่ทำงาน เทสต์ชุดนี้จะล้มเหลวตั้งแต่ขั้นสร้างคอนเทนเนอร์

---

## Entity Framework Core และ Migration

มี Migration แรกแล้ว (`InitialCreate`) ครอบคลุมตารางของ ASP.NET Core Identity
(`users`, `roles`, `user_roles`, `user_claims`, `user_logins`, `user_tokens`, `role_claims`)
และตารางธุรกิจของขั้นตอนนี้ (`organizations`, `permissions`, `role_permissions`, `refresh_tokens`, `audit_logs`)

ติดตั้งเครื่องมือ (ครั้งเดียวต่อเครื่อง):

```bash
dotnet tool install --global dotnet-ef
```

คำสั่งที่ใช้จัดการ Migration:

```bash
# สร้าง Migration ใหม่
dotnet ef migrations add ชื่อMigration \
  --project src/DormFlow.Infrastructure \
  --startup-project src/DormFlow.Infrastructure

# ปรับฐานข้อมูลให้ตรงกับ Migration ล่าสุด
dotnet ef database update \
  --project src/DormFlow.Infrastructure \
  --startup-project src/DormFlow.Infrastructure
```

> **ทำไม `--startup-project` ชี้ไปที่ `DormFlow.Infrastructure` ไม่ใช่ `DormFlow.Api`**
> เครื่องมือ `dotnet ef` ต้องการให้ Startup Project อ้างอิงแพ็กเกจ `Microsoft.EntityFrameworkCore.Design` โดยตรง
> ซึ่ง `DormFlow.Infrastructure` มีอยู่แล้ว (จำเป็นสำหรับ `DormFlowDbContextFactory`)
> ส่วน `DormFlow.Api` ไม่มีแพ็กเกจนี้เพราะไม่ควรต้องรู้จักเครื่องมือ Migration โดยตรง
> `DormFlowDbContextFactory` อ่าน Connection String จาก environment variable `ConnectionStrings__DefaultConnection`
> เมื่อไม่ได้กำหนดจะใช้ค่าเริ่มต้นของ docker compose บนเครื่องนักพัฒนาแทน

> **ข้อควรระวังเมื่อสร้าง Migration ด้วยตัวเอง**
> `DormFlowDbContextFactory` ต้องตั้งชื่อตารางประวัติ Migration ด้วย `DormFlowDbContext.MigrationsHistoryTableName`
> ให้ตรงกับที่ `DependencyInjection.AddPersistence` ใช้ตอนแอปทำงานจริงเสมอ (ปัจจุบันตั้งไว้ให้แล้วทั้งสองจุด)
> ถ้าค่าไม่ตรงกัน `dotnet ef` จะสร้างตารางประวัติคนละชื่อกับที่แอปมองหา
> ทำให้แอปพยายามสร้างตารางที่มีอยู่แล้วซ้ำตอนเริ่มทำงานและล้มเหลวทันที

### ข้อตกลงของฐานข้อมูล

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| การตั้งชื่อตารางและคอลัมน์ | `snake_case` ผ่าน `UseSnakeCaseNamingConvention()` |
| Primary Key | `uuid` (UUID v7 เรียงตามเวลา ลดการแตกกระจายของ index) |
| วันเวลา | `timestamptz` ผ่าน `DateTimeOffset` เก็บเป็น UTC เสมอ |
| Optimistic Concurrency | คอลัมน์ระบบ `xmin` ของ PostgreSQL แม็ปเข้ากับ property `Version` |
| Soft Delete | คอลัมน์ `is_deleted` + Global Query Filter ใส่ให้อัตโนมัติทุก Entity |
| Audit | `created_at`, `created_by`, `updated_at`, `updated_by`, `deleted_at`, `deleted_by` เติมโดย Interceptor |
| Index | `is_deleted` และ `(organization_id, dormitory_id)` ถูกสร้างให้อัตโนมัติ |
| ตาราง Migration History | `__ef_migrations_history` |
| ความละเอียดของ `decimal` | `numeric(18,2)` เป็นค่าเริ่มต้น |

ข้อตกลงเหล่านี้ถูกใส่ให้ทุก Entity โดยอัตโนมัติที่
`src/DormFlow.Infrastructure/Persistence/Extensions/ModelBuilderExtensions.cs`
ไม่ต้องเขียนซ้ำในแต่ละ `IEntityTypeConfiguration`

---

## ข้อตกลงในการเขียนโค้ด

- `Nullable` เปิดทุกโปรเจกต์
- `TreatWarningsAsErrors` เปิดเฉพาะโปรเจกต์ใน `src/` โปรเจกต์เทสต์ไม่เปิด
- เมธอดที่ทำ I/O ต้องเป็น `async` และรับ `CancellationToken` เสมอ
- ห้ามคืน Entity ออกจาก Controller ให้ใช้ DTO หรือ Response Contract เท่านั้น
- ห้ามใส่ Business Logic ใน Controller
- ทุก Query ที่อ่านข้อมูลธุรกิจต้องกรองด้วย `organization_id` และ `dormitory_id`
- ห้ามบันทึกรหัสผ่าน, Access Token หรือ Refresh Token ลง Log
- Log ไม่บันทึก Request Body หรือ Response Body เลย เพราะเส้นทางอย่าง login มีรหัสผ่านอยู่ในเนื้อหา
- เวอร์ชันของแพ็กเกจอยู่ที่ `Directory.Packages.props` ที่เดียว ห้ามใส่ `Version` ใน `.csproj`

### Logging

Serilog เขียน Log ลง Console และไฟล์ `logs/dormflow-<วันที่>.log` (เก็บ 14 วัน)
ทุกคำขอถูกสรุปเป็นหนึ่งบรรทัดพร้อม `TraceId`, `RemoteIp`, `UserAgent`
และเมื่อผู้ใช้ยืนยันตัวตนแล้วจะมี `UserId`, `OrganizationId`, `DormitoryIds` เพิ่มเข้ามา
คำขอไปยัง `/health` ถูกลดระดับเป็น `Verbose` เพื่อไม่ให้กลบ Log ที่มีความหมาย

---

## ขอบเขตของขั้นตอนที่ 1

### ทำแล้ว

| # | รายการ | ที่อยู่ |
| --- | --- | --- |
| 1 | Solution และโครงสร้างโปรเจกต์ 8 โปรเจกต์ | `DormFlow.sln` |
| 2 | Project References ตามทิศทาง Clean Architecture | `src/**/*.csproj` |
| 3 | NuGet Packages ที่จำเป็น | `Directory.Packages.props` |
| 4 | `Directory.Build.props` (root, src, tests) | `Directory.Build.props` |
| 5 | `Directory.Packages.props` (Central Package Management) | `Directory.Packages.props` |
| 6 | `.editorconfig` | `.editorconfig` |
| 7 | `appsettings.json` และ `appsettings.Development.json` | `src/DormFlow.Api/` |
| 8 | การเชื่อมต่อ PostgreSQL + snake_case + retry + soft delete + audit | `src/DormFlow.Infrastructure/` |
| 9 | Serilog พร้อม request logging และการเติมข้อมูลบริบท | `src/DormFlow.Api/Configuration/SerilogConfiguration.cs` |
| 10 | OpenAPI พร้อม JWT Bearer, XML Comments และ Scalar | `src/DormFlow.Api/Configuration/OpenApiConfiguration.cs` |
| 11 | Health Check สามเส้นทาง | `src/DormFlow.Api/Configuration/HealthCheckConfiguration.cs` |
| 12 | Global Exception Middleware | `src/DormFlow.Api/Middleware/` |
| 13 | API Response Model + Pagination Contract | `src/DormFlow.Contracts/Common/` |
| 14 | `BaseEntity` และ `TenantEntity` | `src/DormFlow.Domain/Common/` |
| 15 | Dockerfile และ docker-compose.yml | `src/DormFlow.Api/Dockerfile`, `docker-compose.yml` |
| 16 | README | ไฟล์นี้ |
| 17 | Build ผ่านทั้ง Solution และเทสต์ผ่านทั้งหมด | — |

เพิ่มเติมนอกรายการ เพราะจำเป็นต่อการทำให้ระบบพื้นฐานทำงานได้จริง:
CORS ที่อ่านค่าจาก configuration และไม่ยอมสตาร์ตถ้าไม่ได้ตั้งค่าใน Production,
Rate Limiting, API Versioning, `ICurrentUser`, Forwarded Headers และ Architecture Test

### ทำแล้ว (ขั้นตอนที่ 2 — Identity, JWT, Refresh Token, Roles & Permissions)

| # | รายการ | ที่อยู่ |
| --- | --- | --- |
| 1 | ASP.NET Core Identity (`ApplicationUser`, `ApplicationRole`) | `src/DormFlow.Infrastructure/Identity/` |
| 2 | JWT Access Token (HMAC-SHA256, 15 นาที, ตรวจ Issuer/Audience/Signature/Expiration) | `src/DormFlow.Infrastructure/Authentication/JwtAccessTokenGenerator.cs` |
| 3 | Refresh Token แบบเก็บค่าแฮช พร้อม Token Rotation และตรวจจับการใช้ซ้ำ | `src/DormFlow.Infrastructure/Authentication/RefreshTokenService.cs` |
| 4 | Roles และ Permissions (7 บทบาท, สิทธิ์ตามที่ระบุไว้ครบทุกโมดูล) | `src/DormFlow.Application/Common/Authorization/` |
| 5 | Permission-Based Authorization แบบไดนามิก (`[RequirePermission("...")]`) | `src/DormFlow.Api/Authorization/` |
| 6 | Audit Log ครบ 15 ประเภทเหตุการณ์ตามที่ระบุไว้ เก็บ old/new values เป็น jsonb | `src/DormFlow.Domain/Enums/AuditAction.cs`, `src/DormFlow.Infrastructure/Persistence/AuditLogWriter.cs` |
| 7 | Endpoint การยืนยันตัวตนครบ 7 เส้นทางตามที่ระบุไว้ | `src/DormFlow.Api/Controllers/V1/AuthController.cs` |
| 8 | Lock Account เมื่อกรอกรหัสผ่านผิดครบ 5 ครั้ง (ล็อก 15 นาที) | `src/DormFlow.Infrastructure/DependencyInjection.cs` |
| 9 | Seed ข้อมูลเริ่มต้น (สิทธิ์, บทบาท, ผู้ดูแลระบบจาก `SEED_ADMIN_*`) | `src/DormFlow.Infrastructure/Persistence/DatabaseSeeder.cs` |
| 10 | Migration แรก (`InitialCreate`) | `src/DormFlow.Infrastructure/Migrations/` |
| 11 | Integration Test ครบวงจร Login/Refresh/Logout/Change Password | `tests/DormFlow.IntegrationTests/Endpoints/AuthEndpointTests.cs` |
| 12 | ทดสอบจริงกับ PostgreSQL ผ่าน docker compose (ไม่ใช่แค่ Testcontainers) | ตรวจด้วยตนเองระหว่างพัฒนา |

รองรับ Two-Factor Authentication ไว้แล้วในระดับโครงสร้าง (`AuthenticationService` ตรวจสถานะ
`RequiresTwoFactor` และปฏิเสธการเข้าสู่ระบบ) แต่ยังไม่มี Endpoint สำหรับยืนยันรหัส 2FA จริง
เพราะ ASP.NET Core Identity รองรับ 2FA ผ่าน Authenticator App ได้ทันทีที่เปิดใช้งาน
แต่ต้องมี UI/Endpoint รับรหัสเพิ่มซึ่งอยู่นอกขอบเขตของขั้นตอนนี้

### ทำแล้ว (ขั้นตอนที่ 3 — Organizations, Dormitories, Buildings, Floors, Room Types, Rooms, Users, Roles/Permissions, Audit Logs)

| # | รายการ | ที่อยู่ |
| --- | --- | --- |
| 1 | Organizations CRUD (สร้าง/ลบเฉพาะ SuperAdmin, Owner ดู/แก้ไขได้เฉพาะองค์กรตน) | `src/DormFlow.Application/Features/Organizations/` |
| 2 | Dormitories CRUD พร้อม billing day / payment due day / late fee | `src/DormFlow.Application/Features/Dormitories/` |
| 3 | Buildings, Floors, Room Types CRUD (ตรวจความเป็นเจ้าของของ Entity แม่ทุกชั้น) | `src/DormFlow.Application/Features/{Buildings,Floors,RoomTypes}/` |
| 4 | Rooms CRUD พร้อม `GET /rooms/available` และ `GET /rooms/summary` | `src/DormFlow.Application/Features/Rooms/` |
| 5 | Users CRUD, กำหนดบทบาท (`POST /users/{id}/roles`), มอบหมาย/ถอนหอพัก | `src/DormFlow.Application/Features/Users/` |
| 6 | Roles CRUD (บทบาทมาตรฐานเปลี่ยนชื่อ/ลบไม่ได้ แต่ปรับสิทธิ์ได้) และ Permissions (อ่านอย่างเดียว) | `src/DormFlow.Application/Features/{Roles,Permissions}/` |
| 7 | Audit Logs Endpoint พร้อมตัวกรองครบ (องค์กร, หอพัก, ผู้ใช้, ประเภทเหตุการณ์, ช่วงเวลา) | `src/DormFlow.Application/Features/AuditLogs/` |
| 8 | `TenantScope` — ตรรกะกลางกันข้อมูลข้ามองค์กร/หอพัก ใช้ร่วมกันทุกโมดูลข้างต้น | `src/DormFlow.Application/Common/Authorization/TenantScope.cs` |
| 9 | Controller ครบ 9 โมดูล พร้อม `[RequirePermission("...")]` ทุก Endpoint | `src/DormFlow.Api/Controllers/V1/` |

แต่ละโมดูลแบ่งเป็น Repository Port (Application) + Repository Implementation (Infrastructure)
ตามแนวทางเดียวกับ `IDormitoryRepository`/`DormitoryRepository` เพื่อให้ชั้น Application
ทำงานกับ Domain Entity ได้โดยไม่ต้องอ้างอิง EF Core โดยตรง — ยืนยันด้วย Architecture Test แล้ว

**หลักการแยกข้อมูลตามองค์กรและหอพักที่ใช้จริงในโค้ด**

- สร้างข้อมูลใหม่: SuperAdmin ต้องระบุ `organizationId` เอง ผู้ใช้อื่นถูกบังคับใช้องค์กรของตนเสมอ
  ไม่ว่าคำขอจะระบุค่าใดมาก็ตาม (`TenantScope.ResolveOrganizationIdForCreate`)
- อ่าน/แก้ไข/ลบข้อมูลที่มีอยู่: ถ้าองค์กรของข้อมูลไม่ตรงกับผู้ใช้ ตอบ 404 แทน 403 เพื่อไม่ให้เดาได้ว่า
  มีข้อมูลนั้นอยู่จริงในองค์กรอื่น (`TenantScope.EnsureOrganizationAccess` / `EnsureDormitoryAccess`)
- List Endpoint: SuperAdmin และ Owner เห็นได้ทุกหอพักในองค์กรของตน ส่วนบทบาทอื่น (DormManager, Staff ฯลฯ)
  เห็นเฉพาะหอพักที่ตนถูกมอบหมาย (`TenantScope.GetDormitoryRestrictionForListing`)
- Building ต้องอยู่ใน Dormitory ที่ระบุ, Floor ต้องอยู่ใน Building เดียวกับห้อง,
  RoomType ต้องอยู่ใน Dormitory เดียวกับอาคารที่เลือก — ตรวจก่อนบันทึกทุกครั้งที่ Application layer

### ยังไม่ทำ

Endpoint ยืนยันรหัส Two-Factor Authentication (โครงสร้างรองรับแล้ว แต่ยังไม่มี UI/Endpoint รับรหัส) ·
โมดูลของ Phase 2 ทั้งหมด (Reservations, Contracts, CheckIns/Outs, Meters, Billing, Invoices,
Payments, Receipts, Deposits, Expenses, Maintenance, Parcels, Visitors, Parking, Announcements,
Notifications, Reports, Settings)

Integration Test ของโมดูลใหม่ในขั้นตอนที่ 3 ครบแล้วเช่นกัน (ดูตารางด้านบน) รวมถึงกรณีที่พบบั๊กจริง
ระหว่างพัฒนา: `RoleRepository.SetPermissionsAsync` เปิดธุรกรรมเองตรง ๆ ด้วย `BeginTransactionAsync`
ซึ่งใช้ร่วมกับ `EnableRetryOnFailure` ของ Npgsql ไม่ได้ (Retrying Execution Strategy ไม่รองรับธุรกรรม
ที่ผู้เรียกเปิดเอง) ต้องครอบด้วย `Database.CreateExecutionStrategy().ExecuteAsync(...)` แทน
Integration Test เป็นตัวจับบั๊กนี้ได้ตั้งแต่รอบแรกที่รันกับ PostgreSQL จริง
