# DormFlow — Dormitory Management System (Backend API)

ระบบบริหารจัดการหอพักและอพาร์ตเมนต์แบบหลายองค์กร (Multi-Organization SaaS)
สร้างด้วย ASP.NET Core Web API บน .NET 10 ตามแนวทาง **Modular Monolith + Clean Architecture**

> สถานะปัจจุบัน: **Phase 1 เสร็จสมบูรณ์** และเริ่ม **Phase 2 แล้ว (สาย Contracts)**
> Phase 1 ครบทั้ง Identity, JWT, Refresh Token, Roles & Permissions และ Endpoint บริหารจัดการ
> Organizations, Dormitories, Buildings, Floors, Room Types, Rooms, Users, User-Dormitory
> Assignment, Roles/Permissions และ Audit Logs
> Phase 2 เพิ่ม Tenants, Reservations, Contracts (รวมเช็คอิน/เช็คเอาท์), Meter Readings,
> Invoices, Payments, Maintenance (งานแจ้งซ่อมบำรุง), Reports (รายงานรายได้/ใบแจ้งหนี้ค้างชำระ),
> Deposits (เงินมัดจำ), Receipts (ใบเสร็จรับเงิน), Expenses (ค่าใช้จ่ายในการดำเนินงาน),
> Parcels (พัสดุ), Visitors (ผู้มาเยือน), Parking (ที่จอดรถ), Announcements (ประกาศ) และ
> Notifications (การแจ้งเตือน) แล้ว
> ส่วนที่เหลือของ Phase 2 (Settings) ยังไม่ได้เริ่ม
> ระหว่างพัฒนา Reports ยังพบและแก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กรของบทบาท Owner ใน List Endpoint
> ของอีก 10 โมดูล ดูรายละเอียดที่หัวข้อ
> [แก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กร](#แก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กร-owner-ใน-list-endpoint)
> ดูรายละเอียดได้ที่หัวข้อ [ขอบเขตของขั้นตอนที่ 1](#ขอบเขตของขั้นตอนที่-1)
> และ [Phase 2 — โมดูลสาย Contracts](#phase-2--โมดูลสาย-contracts)

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
- [Phase 2 — โมดูลสาย Contracts](#phase-2--โมดูลสาย-contracts)

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
| `GET`/`POST`/`PUT`/`PATCH .../status` | `/api/v1/tenants` | `tenants.*` | จัดการข้อมูลผู้เช่า (ไม่มี DELETE ตามที่สเปกไม่ได้ระบุสิทธิ์ลบไว้) |
| `GET`/`POST` | `/api/v1/reservations` | `reservations.*` | จองห้องพัก (ห้องเปลี่ยนเป็น Reserved ทันที) |
| `POST` | `/api/v1/reservations/{id}/cancel` | `reservations.cancel` | ยกเลิกการจอง ห้องกลับเป็น Available ถ้ายังไม่ถูกใช้ทางอื่น |
| `GET`/`POST` | `/api/v1/contracts` | `contracts.*` | สร้างสัญญาเช่า (ผ่านการจองหรือทำสัญญาตรงก็ได้) |
| `POST` | `/api/v1/contracts/{id}/activate` | `contracts.activate` | เปิดใช้งานสัญญา = เช็คอิน ห้องเปลี่ยนเป็น Occupied |
| `POST` | `/api/v1/contracts/{id}/terminate` | `contracts.terminate` | สิ้นสุดสัญญา = เช็คเอาท์ ห้องเปลี่ยนเป็น Cleaning |
| `GET`/`POST` | `/api/v1/meter-readings` | `meter-readings.*` | จดค่ามิเตอร์น้ำ/ไฟรายเดือน (ไม่มี UPDATE/DELETE เป็นบันทึกเชิงประวัติ) |
| `GET`/`POST` | `/api/v1/invoices` | `invoices.*` | สร้างใบแจ้งหนี้ในสถานะร่าง คำนวณค่าน้ำ-ไฟจากค่ามิเตอร์อัตโนมัติ |
| `POST` | `/api/v1/invoices/{id}/issue` | `invoices.issue` | ออกใบแจ้งหนี้ให้ผู้เช่า (Draft → Issued) |
| `POST` | `/api/v1/invoices/{id}/cancel` | `invoices.cancel` | ยกเลิกใบแจ้งหนี้ (ทำได้เฉพาะที่ยังไม่มีการชำระเงินเข้ามา) |
| `GET`/`POST` | `/api/v1/payments` | `payments.*` | บันทึกรายการชำระเงินในสถานะรอยืนยัน ยังไม่มีผลต่อยอดใบแจ้งหนี้ |
| `POST` | `/api/v1/payments/{id}/confirm` | `payments.approve` | ยืนยันรายการชำระเงิน นำยอดไปหักใบแจ้งหนี้และปรับสถานะ |
| `POST` | `/api/v1/payments/{id}/refund` | `payments.refund` | คืนเงิน (เฉพาะรายการที่ยืนยันแล้ว) ยอดถูกหักออกจากใบแจ้งหนี้คืน |
| `GET`/`POST` | `/api/v1/maintenance-requests` | `maintenance.view` / `maintenance.update-status` | แจ้งซ่อมบำรุง (สร้างใช้สิทธิ์เดียวกับการเปลี่ยนสถานะ ไม่มีสิทธิ์ "create" แยก) |
| `POST` | `/api/v1/maintenance-requests/{id}/assign` | `maintenance.assign` | มอบหมาย/มอบหมายซ้ำช่างซ่อมบำรุง ห้องเปลี่ยนเป็น Maintenance ถ้ายังว่างอยู่ |
| `PATCH` | `/api/v1/maintenance-requests/{id}/status` | `maintenance.update-status` | เปลี่ยนสถานะตามลำดับที่กำหนด (ห้ามตั้งเป็น Assigned ตรง ๆ) ปิดงานแล้วห้องกลับเป็นว่างถ้าไม่มีงานอื่นค้าง |
| `GET` | `/api/v1/reports/revenue` | `reports.view` | สรุปยอดออกบิล/เก็บเงินได้/ค้างชำระในช่วงเวลาที่ระบุ |
| `GET` | `/api/v1/reports/revenue/export` | `reports.export` | ส่งออกรายงานสรุปรายได้เป็นไฟล์ CSV |
| `GET` | `/api/v1/reports/outstanding-invoices` | `reports.view` | รายการใบแจ้งหนี้ค้างชำระแบบแบ่งหน้า พร้อมยอดคงเหลือและจำนวนวันเกินกำหนด |
| `GET` | `/api/v1/reports/outstanding-invoices/export` | `reports.export` | ส่งออกรายงานใบแจ้งหนี้ค้างชำระเป็นไฟล์ CSV (สูงสุด 5,000 แถว) |
| `GET`/`POST` | `/api/v1/deposits` | `deposits.view` / `deposits.collect` | รับเงินมัดจำ (สัญญาหนึ่งฉบับรับได้ครั้งเดียว ต้องยังไม่สิ้นสุดสัญญา) |
| `POST` | `/api/v1/deposits/{id}/refund` | `deposits.refund` | คืนเงินมัดจำ (เต็ม/บางส่วน/ริบทั้งหมด) ทำได้เฉพาะสัญญาที่สิ้นสุดแล้ว และคืนได้ครั้งเดียวต่อรายการ |
| `GET`/`POST` | `/api/v1/receipts` | `receipts.view` / `receipts.issue` | ออกใบเสร็จรับเงิน (เฉพาะรายการชำระเงินที่ยืนยันแล้ว รายการละครั้งเดียว) |
| `GET`/`POST` | `/api/v1/expenses` | `expenses.view` / `expenses.create` | บันทึกค่าใช้จ่ายในการดำเนินงาน (ค่าน้ำ-ไฟส่วนกลาง, ซ่อมบำรุง, เงินเดือน ฯลฯ) |
| `GET`/`POST` | `/api/v1/parcels` | `parcels.view` / `parcels.log` | บันทึกพัสดุที่มาถึงหอพัก |
| `POST` | `/api/v1/parcels/{id}/pickup` | `parcels.pickup` | บันทึกการรับพัสดุ (ทำได้เฉพาะพัสดุที่ยังไม่ถูกรับ) |
| `GET`/`POST` | `/api/v1/visitors` | `visitors.view` / `visitors.checkin` | บันทึกการเข้าของผู้มาเยือน |
| `POST` | `/api/v1/visitors/{id}/checkout` | `visitors.checkout` | บันทึกการออกของผู้มาเยือน (ทำได้เฉพาะที่ยังไม่ออก) |
| `GET`/`POST` | `/api/v1/parking` | `parking.view` / `parking.register` | จดทะเบียนที่จอดรถ (ทะเบียนรถเดียวกันซ้อนกันไม่ได้ขณะยังไม่สิ้นสุด) |
| `POST` | `/api/v1/parking/{id}/release` | `parking.release` | สิ้นสุดการจดทะเบียนที่จอดรถ |
| `GET`/`POST` | `/api/v1/announcements` | `announcements.view` / `announcements.create` | สร้างประกาศ (สถานะร่างเสมอ) |
| `POST` | `/api/v1/announcements/{id}/publish` | `announcements.publish` | เผยแพร่ประกาศ (ทำได้เฉพาะประกาศที่ยังเป็นร่าง) |
| `POST` | `/api/v1/announcements/{id}/archive` | `announcements.publish` | เก็บประกาศเข้าคลัง (ทำได้เฉพาะประกาศที่เผยแพร่แล้ว) |
| `GET` | `/api/v1/notifications` | ต้องเข้าสู่ระบบ | ดูการแจ้งเตือนของตนเอง (ไม่ต้องมีสิทธิ์เฉพาะ เหมือน `/auth/me`) |
| `POST` | `/api/v1/notifications/{id}/read` | ต้องเข้าสู่ระบบ | ทำเครื่องหมายว่าอ่านการแจ้งเตือนของตนเองแล้ว |
| `POST` | `/api/v1/notifications` | `notifications.send` | ส่งการแจ้งเตือนถึงผู้ใช้หนึ่งคนในองค์กรเดียวกัน |

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
| `DormFlow.IntegrationTests` | 95 | Health Check ทั้งสามเส้นทาง, เอกสาร OpenAPI, JWT Bearer Security Scheme, เส้นทางที่ไม่มีอยู่จริง, Login สำเร็จ/ล้มเหลว, `/auth/me`, Refresh Token Rotation, การตรวจจับการใช้ Token ซ้ำ, Logout, Change Password, Forgot Password, ลำดับชั้นทรัพย์สินทั้งสาย (Organization→Dormitory→Building→Floor→RoomType→Room), Rooms available/summary, การกันข้อมูลข้ามองค์กร, CRUD ผู้ใช้ + กำหนดบทบาท + มอบหมาย/ถอนหอพัก, บทบาทมาตรฐานเปลี่ยนชื่อ/ลบไม่ได้, Audit Log, วงจรชีวิตการเช่าเต็มสาย (Tenant→Reservation→Contract→Activate→Terminate พร้อมตรวจสถานะห้องทุกจุด), การจองห้องซ้ำและผู้เช่าบัญชีดำต้องถูกปฏิเสธ, การจดค่ามิเตอร์ (ดึงค่าครั้งก่อนหน้าอัตโนมัติ, ห้ามจดซ้ำเดือนเดียวกัน, ห้ามจดค่าต่ำกว่าครั้งก่อน), วงจรชีวิตการออกบิล (MeterReading→Invoice→Payment ครบสาย, ชำระบางส่วนแล้วคืนเงิน, ห้ามออกบิลซ้ำเดือนเดียวกัน, ห้ามยกเลิกบิลที่ชำระแล้ว, ห้ามยืนยันยอดเกินยอดคงเหลือ), วงจรชีวิตงานแจ้งซ่อมบำรุง (Report→Assign→InProgress→Complete พร้อมห้องเปลี่ยนสถานะอัตโนมัติ, มอบหมายซ้ำ, ห้องมีงานซ่อมสองรายการ, ปฏิเสธช่างที่ไม่มีสิทธิ์/ไม่ได้ดูแลหอพักนี้, ห้ามตั้งสถานะ Assigned ตรง ๆ, ห้ามข้ามขั้นตอน, ห้องที่มีผู้เช่าไม่ถูกแตะต้อง), การกันข้อมูลข้ามองค์กรของ Owner
ใน List Endpoint (Rooms, Invoices, MaintenanceRequests) — Regression Test ของช่องโหว่ที่พบและแก้ไขแล้ว,
รายงานสรุปรายได้และรายงานใบแจ้งหนี้ค้างชำระ (คำนวณยอดถูกต้อง, กรองตามหอพัก, ส่งออก CSV ทั้งสองรายงาน),
วงจรชีวิตเงินมัดจำ (รับเงิน→สิ้นสุดสัญญา→คืนเต็ม/บางส่วน/ริบทั้งหมด, ห้ามรับซ้ำ, ห้ามคืนก่อนสัญญาสิ้นสุด,
ห้ามคืนเกินยอด, บังคับระบุเหตุผลเมื่อคืนไม่เต็ม, ห้ามคืนซ้ำ), การออกใบเสร็จรับเงิน (เฉพาะรายการชำระเงิน
ที่ยืนยันแล้ว, ห้ามออกซ้ำ, ห้ามออกให้รายการที่ยังไม่ยืนยันหรือถูกคืนเงินไปแล้ว), การบันทึกค่าใช้จ่าย
(กรองตามหมวดหมู่และช่วงวันที่, ปฏิเสธหมวดหมู่ที่ไม่ถูกต้อง), วงจรชีวิตพัสดุ (บันทึกรับ→รับพัสดุ,
ห้ามรับซ้ำ), วงจรชีวิตผู้มาเยือน (เข้า→ออก, ห้ามออกซ้ำ), วงจรชีวิตการจดทะเบียนที่จอดรถ (จดทะเบียน→สิ้นสุด,
ห้ามจดทะเบียนซ้อนทะเบียนรถเดียวกัน, จดทะเบียนซ้ำได้หลังรายการเดิมสิ้นสุดแล้ว), วงจรชีวิตประกาศ
(สร้าง (ร่าง)→เผยแพร่→เก็บเข้าคลัง, ห้ามเผยแพร่ซ้ำ, ห้ามเก็บเข้าคลังก่อนเผยแพร่), การแจ้งเตือน
(ส่งถึงผู้ใช้→ปรากฏในรายการของผู้รับ, ทำเครื่องหมายอ่านแล้วของตนเอง, ห้ามอ่านแทนผู้อื่น,
ห้ามส่งข้ามองค์กร) |

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
  เห็นเฉพาะหอพักที่ตนถูกมอบหมาย (`TenantScope.GetDormitoryRestrictionForListing`) **และ**
  ทุก List Endpoint ต้องเรียก `TenantScope.GetOrganizationRestrictionForListing` คู่กันเสมอเพื่อกรอง
  ด้วย `OrganizationId` ด้วย (ดูหัวข้อ [แก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กร](#แก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กร-owner-ใน-list-endpoint))
- Building ต้องอยู่ใน Dormitory ที่ระบุ, Floor ต้องอยู่ใน Building เดียวกับห้อง,
  RoomType ต้องอยู่ใน Dormitory เดียวกับอาคารที่เลือก — ตรวจก่อนบันทึกทุกครั้งที่ Application layer

### แก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กร (Owner) ใน List Endpoint

**พบระหว่างพัฒนาโมดูล Reports ของ Phase 2 (2026-08-24) และแก้ไขทันทีก่อนทำงานต่อ:**
`TenantScope.GetDormitoryRestrictionForListing` คืนค่า `null` ให้ผู้ใช้บทบาท Owner โดยมีเจตนาว่า
"ไม่ต้องจำกัดหอพัก เพราะ Owner เห็นได้ทุกหอพักในองค์กรของตนอยู่แล้ว" แต่ Repository ของโมดูลที่แยกข้อมูล
ตามหอพัก (Buildings, Floors, RoomTypes, Rooms, Reservations, Contracts, MeterReadings, Invoices,
Payments, MaintenanceRequests — รวม 10 โมดูล ทั้งจาก Phase 1 และ Phase 2) ไม่มีการกรองด้วย `OrganizationId`
เพิ่มเติมเลย ผลคือ Owner ที่เรียก List Endpoint โดยไม่ระบุ `dormitoryId` (เช่น `GET /api/v1/rooms`,
`GET /api/v1/invoices`) จะเห็นข้อมูลของ**ทุกองค์กรในระบบ** ไม่ใช่แค่ขององค์กรตนเอง — รวมถึงข้อมูลการเงิน
และข้อมูลส่วนบุคคลของลูกค้ารายอื่นบนแพลตฟอร์ม SaaS แบบหลายองค์กรนี้ (การอ่านทีละรายการด้วย `GET /{id}`
ไม่ได้รับผลกระทบ เพราะ `TenantScope.EnsureDormitoryAccess` ตรวจ `OrganizationId` ควบคู่กับหอพักอยู่แล้ว
บทบาทอื่นที่ไม่ใช่ SuperAdmin/Owner ก็ไม่ได้รับผลกระทบเช่นกัน เพราะ `DormitoryIds` ของบทบาทเหล่านั้น
ถูกจำกัดอยู่ในองค์กรตนเองอยู่แล้วโดยธรรมชาติจากการมอบหมายหอพัก)

แก้ไขด้วยการเพิ่มเมธอด `TenantScope.GetOrganizationRestrictionForListing(currentUser)` (คืนค่า `null`
เฉพาะ SuperAdmin เท่านั้น) แล้วเพิ่มฟิลด์ `OrganizationId` ให้ `*ListFilter` ของทั้ง 10 โมดูล พร้อมเพิ่มเงื่อนไข
กรองใน Repository ทุกตัว — เรียกคู่กับ `GetDormitoryRestrictionForListing` เสมอในทุก Use Case ที่ดึงรายการ
ข้อมูลที่แยกตามหอพัก มี Regression Test ยืนยันแล้วที่
`tests/DormFlow.IntegrationTests/Endpoints/CrossOrganizationIsolationEndpointTests.cs` (ครอบคลุมทั้ง
โมดูล Phase 1 อย่าง Rooms และโมดูล Phase 2 อย่าง Invoices/MaintenanceRequests)

### ยังไม่ทำ

Endpoint ยืนยันรหัส Two-Factor Authentication (โครงสร้างรองรับแล้ว แต่ยังไม่มี UI/Endpoint รับรหัส)

Integration Test ของโมดูลใหม่ในขั้นตอนที่ 3 ครบแล้วเช่นกัน (ดูตารางด้านบน) รวมถึงกรณีที่พบบั๊กจริง
ระหว่างพัฒนา: `RoleRepository.SetPermissionsAsync` เปิดธุรกรรมเองตรง ๆ ด้วย `BeginTransactionAsync`
ซึ่งใช้ร่วมกับ `EnableRetryOnFailure` ของ Npgsql ไม่ได้ (Retrying Execution Strategy ไม่รองรับธุรกรรม
ที่ผู้เรียกเปิดเอง) ต้องครอบด้วย `Database.CreateExecutionStrategy().ExecuteAsync(...)` แทน
Integration Test เป็นตัวจับบั๊กนี้ได้ตั้งแต่รอบแรกที่รันกับ PostgreSQL จริง

---

## Phase 2 — โมดูลสาย Contracts

เอกสารต้นฉบับระบุรายละเอียด Entity/Endpoint/กฎ Validation ของ Phase 1 ไว้ครบ
แต่สำหรับ Phase 2 ระบุไว้เพียงชื่อโมดูล (Reservations, Contracts, CheckIns, CheckOuts, Meters,
Billing, Invoices, Payments, Receipts, Deposits, Expenses, Maintenance, Parcels, Visitors,
Parking, Announcements, Notifications, Reports, Settings) โดยไม่มีรายละเอียดด้านธุรกิจ
เริ่มจากสาย **Tenants → Reservations → Contracts → Meter Readings → Billing/Invoices/Payments** ก่อน
เพราะ Billing ทั้งหมดต้องพึ่งพาว่ามีสัญญาเช่าที่ Active และมีค่ามิเตอร์ของรอบบิลนั้นอยู่แล้วเป็นพื้นฐาน
ตามด้วย **Maintenance** เพราะเป็นโมดูลเดียวที่เหลือใน Phase 2 ที่มีสิทธิ์ (`maintenance.*`) และบทบาท
`Technician` กำหนดไว้แล้วตั้งแต่ Phase 1 ต่างจากโมดูลอื่นที่เหลือซึ่งไม่มีสิทธิ์ใด ๆ ถูกกำหนดไว้เลย

### ทำแล้ว

| # | รายการ | ที่อยู่ |
| --- | --- | --- |
| 1 | Tenants CRUD (ระดับองค์กร ไม่ผูกกับหอพักใดหอพักหนึ่งถาวร ไม่มี Endpoint ลบ) | `src/DormFlow.Application/Features/Tenants/` |
| 2 | Reservations — จองห้อง (ห้องเปลี่ยนเป็น Reserved ทันที) และยกเลิกการจอง | `src/DormFlow.Application/Features/Reservations/` |
| 3 | Contracts — สร้าง (ผ่านการจองหรือทำสัญญาตรง), เปิดใช้งาน (เช็คอิน), สิ้นสุด (เช็คเอาท์) | `src/DormFlow.Application/Features/Contracts/` |
| 4 | สิทธิ์ `reservations.view/create/cancel` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 5 | Migration `AddLeasingModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 6 | Integration Test วงจรชีวิตการเช่าเต็มสาย + กรณีปฏิเสธ (ห้องไม่ว่าง, ผู้เช่าบัญชีดำ) | `tests/DormFlow.IntegrationTests/Endpoints/LeasingLifecycleEndpointTests.cs` |
| 7 | Meter Readings — จดค่ามิเตอร์น้ำ/ไฟรายเดือน ดึงค่าครั้งก่อนหน้ามาต่ออัตโนมัติ (View/Create เท่านั้น) | `src/DormFlow.Application/Features/MeterReadings/` |
| 8 | Migration `AddMeterReadings` | `src/DormFlow.Infrastructure/Migrations/` |
| 9 | Integration Test การจดค่ามิเตอร์ (ค่าเริ่มต้น, ต่อเนื่องข้ามเดือน, กันจดซ้ำ, กันค่าย้อนหลัง) | `tests/DormFlow.IntegrationTests/Endpoints/MeterReadingsEndpointTests.cs` |
| 10 | เพิ่ม `WaterRatePerUnit`/`ElectricityRatePerUnit` ให้ Dormitory (ต่อจากฟิลด์ Billing เดิมของ Phase 1) | `src/DormFlow.Domain/Entities/Dormitory.cs` |
| 11 | Invoices — สร้างจากสัญญา+ค่ามิเตอร์ (Draft), ออกให้ผู้เช่า (Issued), ยกเลิก | `src/DormFlow.Application/Features/Invoices/` |
| 12 | Payments — บันทึก (Pending), ยืนยัน (Confirmed → หักยอดบิล), คืนเงิน (Refunded → คืนยอด) | `src/DormFlow.Application/Features/Payments/` |
| 13 | Migration `AddBillingModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 14 | Integration Test วงจรชีวิตการออกบิลเต็มสาย + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/BillingLifecycleEndpointTests.cs` |
| 15 | Maintenance — แจ้งซ่อม, มอบหมาย/มอบหมายซ้ำช่าง, เปลี่ยนสถานะตามลำดับที่กำหนด | `src/DormFlow.Application/Features/Maintenance/` |
| 16 | เพิ่ม `HasRoleForDormitoryAsync` ให้ `IUserAdministrationRepository` (ตรวจว่าเป็นช่างของหอพักนี้จริง) | `src/DormFlow.Infrastructure/Identity/UserAdministrationRepository.cs` |
| 17 | Migration `AddMaintenanceModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 18 | Integration Test วงจรชีวิตงานซ่อมบำรุงเต็มสาย + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/MaintenanceEndpointTests.cs` |
| 19 | Reports — สรุปรายได้ (View/Export) และใบแจ้งหนี้ค้างชำระ (View/Export) เป็น CSV | `src/DormFlow.Application/Features/Reports/` |
| 20 | แก้ไขช่องโหว่ข้อมูลรั่วข้ามองค์กรของ Owner ใน List Endpoint 10 โมดูล (พบระหว่างพัฒนา Reports) | `src/DormFlow.Application/Common/Authorization/TenantScope.cs` |
| 21 | Integration Test รายงานรายได้/ใบแจ้งหนี้ค้างชำระ + Regression Test การกันข้อมูลข้ามองค์กร | `tests/DormFlow.IntegrationTests/Endpoints/{Reports,CrossOrganizationIsolation}EndpointTests.cs` |
| 22 | สิทธิ์ `deposits.view/collect/refund` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 23 | Deposits — รับเงินมัดจำ (Held), คืนเงิน (เต็ม/บางส่วน/ริบทั้งหมด ครั้งเดียวต่อรายการ) | `src/DormFlow.Application/Features/Deposits/` |
| 24 | Migration `AddDepositsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 25 | Integration Test วงจรชีวิตเงินมัดจำเต็มสาย + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/DepositsEndpointTests.cs` |
| 26 | สิทธิ์ `receipts.view/issue` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 27 | Receipts — ออกใบเสร็จรับเงินจากรายการชำระเงินที่ยืนยันแล้ว (ไม่มี Endpoint แก้ไข/ลบ) | `src/DormFlow.Application/Features/Receipts/` |
| 28 | Migration `AddReceiptsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 29 | Integration Test การออกใบเสร็จรับเงิน + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/ReceiptsEndpointTests.cs` |
| 30 | สิทธิ์ `expenses.view/create` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 31 | Expenses — บันทึกค่าใช้จ่ายในการดำเนินงาน แยกจากฝั่งรายรับโดยสิ้นเชิง | `src/DormFlow.Application/Features/Expenses/` |
| 32 | Migration `AddExpensesModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 33 | Integration Test การบันทึกและค้นหาค่าใช้จ่าย | `tests/DormFlow.IntegrationTests/Endpoints/ExpensesEndpointTests.cs` |
| 34 | สิทธิ์ `parcels.view/log/pickup` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 35 | Parcels — บันทึกพัสดุที่มาถึง (Received), บันทึกการรับพัสดุ (PickedUp) | `src/DormFlow.Application/Features/Parcels/` |
| 36 | Migration `AddParcelsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 37 | Integration Test วงจรชีวิตพัสดุ + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/ParcelsEndpointTests.cs` |
| 38 | สิทธิ์ `visitors.view/checkin/checkout` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 39 | Visitors — บันทึกการเข้า (CheckedIn), บันทึกการออก (CheckedOut) | `src/DormFlow.Application/Features/Visitors/` |
| 40 | Migration `AddVisitorsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 41 | Integration Test วงจรชีวิตผู้มาเยือน + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/VisitorsEndpointTests.cs` |
| 42 | สิทธิ์ `parking.view/register/release` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 43 | Parking — จดทะเบียนที่จอดรถ (Active), สิ้นสุดการจดทะเบียน (Ended) | `src/DormFlow.Application/Features/Parking/` |
| 44 | Migration `AddParkingModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 45 | Integration Test วงจรชีวิตการจดทะเบียนที่จอดรถ + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/ParkingEndpointTests.cs` |
| 46 | สิทธิ์ `announcements.view/create/publish` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 47 | Announcements — สร้างประกาศ (Draft), เผยแพร่ (Published), เก็บเข้าคลัง (Archived) | `src/DormFlow.Application/Features/Announcements/` |
| 48 | Migration `AddAnnouncementsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 49 | Integration Test วงจรชีวิตประกาศ + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/AnnouncementsEndpointTests.cs` |
| 50 | สิทธิ์ `notifications.send` เพิ่มใหม่ (ไม่มีในเอกสารต้นฉบับ) | `src/DormFlow.Application/Common/Authorization/PermissionCodes.cs` |
| 51 | Notifications — ส่งถึงผู้ใช้หนึ่งคน, ดู/ทำเครื่องหมายอ่านการแจ้งเตือนของตนเอง (ไม่ต้องมีสิทธิ์เฉพาะ) | `src/DormFlow.Application/Features/Notifications/` |
| 52 | Migration `AddNotificationsModule` | `src/DormFlow.Infrastructure/Migrations/` |
| 53 | Integration Test วงจรชีวิตการแจ้งเตือน + กรณีปฏิเสธ | `tests/DormFlow.IntegrationTests/Endpoints/NotificationsEndpointTests.cs` |

### สมมติฐานทางธุรกิจที่ใช้ (ยังไม่ได้ยืนยันกับเจ้าของระบบ)

เอกสารต้นฉบับไม่ได้ระบุกฎธุรกิจของโมดูลเหล่านี้ไว้ จึงใช้สมมติฐานที่สมเหตุสมผลต่อไปนี้
**ควรทบทวนก่อนใช้งานจริง**:

- **ไม่มี Entity แยกสำหรับ CheckIn/CheckOut** — รวมเป็นการกระทำบนสัญญาแทน
  (`POST /contracts/{id}/activate` = เช็คอิน, `POST /contracts/{id}/terminate` = เช็คเอาท์)
  ถ้าต้องการบันทึกรายละเอียดการตรวจสภาพห้องหรือรูปถ่ายตอนเข้า-ออก ต้องแยกเป็น Entity ใหม่ภายหลัง
- **สถานะห้องเปลี่ยนอัตโนมัติตามวงจรชีวิต**: จอง/ทำสัญญา (Draft) → `Reserved`,
  เปิดใช้งานสัญญา (เช็คอิน) → `Occupied`, สิ้นสุดสัญญา (เช็คเอาท์) → `Cleaning`
  (เจ้าหน้าที่ต้องเปลี่ยนเป็น `Available` เองผ่าน Endpoint ของห้องหลังทำความสะอาดเสร็จ)
- **เลขที่สัญญา** สุ่มเป็นรูปแบบ `CT-{yyyyMMdd}-{6 อักขระสุ่ม}` ไม่ใช้เลขรันต่อเนื่อง
  เพราะยังไม่มีข้อกำหนดเรื่องรูปแบบเลขที่สัญญาที่ต้องอ้างอิงระบบบัญชีภายนอก
- **ผู้เช่าที่ถูกขึ้นบัญชีดำ (`Blacklisted`)** จองห้องหรือทำสัญญาใหม่ไม่ได้ แต่สัญญาเดิมที่ Active อยู่แล้วไม่ถูกยกเลิกอัตโนมัติ
- **ห้องหนึ่งห้องมีสัญญาที่ยังไม่สิ้นสุด (Draft หรือ Active) ได้ครั้งละหนึ่งฉบับเท่านั้น**
- **ค่าเช่า/ค่ามัดจำของสัญญาคัดลอกมาจากห้องตอนทำสัญญา** ไม่เปลี่ยนตามค่าเช่าห้องที่แก้ไขในภายหลัง
- **การจองมีอายุจำกัด** (`HoldDays`, ค่าเริ่มต้น 3 วัน สูงสุด 30 วัน) แต่ยังไม่มี Background Job
  ที่คอยหมดอายุการจองอัตโนมัติเมื่อครบกำหนด (Hangfire/Quartz.NET ที่ระบุไว้ในเทคสแตกยังไม่ได้ติดตั้ง)
  ต้องยกเลิกด้วยมือผ่าน `POST /reservations/{id}/cancel` ไปก่อน
- **ไม่มี Entity แม่ของ "มิเตอร์" แยกต่างหาก** — `MeterReading` ผูกกับห้อง+ประเภทมิเตอร์+เดือนตรง ๆ
  เพราะสิทธิ์ที่มีอยู่เดิม (`meter-readings.view/create` เท่านั้น ไม่มี CRUD มิเตอร์) บ่งบอกว่าไม่ได้ตั้งใจให้มี
  ทะเบียนมิเตอร์แยกเป็นของตัวเอง ถ้าห้องเปลี่ยนมิเตอร์จริง (มิเตอร์เสีย/เปลี่ยนใหม่) ต้องจดค่าเริ่มต้นใหม่เอง
  ระบบไม่รองรับการรีเซ็ตค่ามิเตอร์กลับไปต่ำกว่าค่าครั้งก่อนหน้า
- **ค่ามิเตอร์เป็นบันทึกเชิงประวัติที่แก้ไข/ลบไม่ได้** จดผิดต้องจดรายการใหม่ของเดือนถัดไปแก้ไขแทน
- **อัตราค่าน้ำ/ค่าไฟต่อหน่วยเก็บไว้ที่ระดับหอพัก** (`Dormitory.WaterRatePerUnit`/`ElectricityRatePerUnit`)
  ต่อจากฟิลด์ตั้งค่าบิลเดิมของ Phase 1 (`BillingDay`, `PaymentDueDay`, `LateFeeType/Amount`)
  เพราะเป็นค่าที่ควรเท่ากันทั้งหอพัก ไม่ใช่รายห้อง — ถ้าบางหอพักต้องการอัตราต่างกันตามประเภทห้อง
  ต้องย้ายไปเก็บที่ `RoomType` แทนในภายหลัง
- **ไม่มี Entity แยกสำหรับ "รายการ" ในใบแจ้งหนี้ (Invoice Line Item)** — ค่าเช่า/ค่าน้ำ/ค่าไฟ/ค่าปรับ
  เก็บเป็นฟิลด์ตรง ๆ บน `Invoice` เพราะจำนวนประเภทค่าใช้จ่ายคงที่ในขอบเขตนี้ ถ้าต้องรองรับค่าใช้จ่ายอื่น
  ที่ไม่รู้จำนวนล่วงหน้า (เช่น ค่าปรับจากผู้เช่าทำของเสียหาย) ต้องแยกเป็นตาราง Line Item ในภายหลัง
- **ใบแจ้งหนี้ออกได้เฉพาะจากสัญญาที่ Active เท่านั้น** และออกได้เดือนละหนึ่งฉบับต่อสัญญา
  ค่าน้ำ/ค่าไฟคำนวณจากค่ามิเตอร์ของห้องในเดือนนั้น ถ้ายังไม่มีค่ามิเตอร์ของเดือนนั้นจะคิดเป็น 0
  (ไม่บังคับต้องจดมิเตอร์ก่อนออกบิลเสมอไป เพราะบางหอพักอาจรวมค่าน้ำ-ไฟในค่าเช่าอยู่แล้ว)
- **วันครบกำหนดชำระคำนวณจาก `Dormitory.PaymentDueDay`** โดยหาวันที่ตรงกับเลขวันนั้นที่อยู่ *หลัง*
  วันออกบิลที่ใกล้ที่สุด (ถ้าเลขวันครบกำหนดผ่านไปแล้วในเดือนที่ออกบิล จะเลื่อนไปเดือนถัดไป)
  และไม่มีการคำนวณค่าปรับชำระล่าช้า (`LateFeeAmount`) อัตโนมัติเมื่อเกินกำหนด เพราะต้องมี Background Job
  ที่ยังไม่ได้ติดตั้ง (เช่นเดียวกับช่องโหว่ HoldDays ของ Reservations) — สถานะ `Overdue` นิยามไว้ในระบบ
  แต่ยังไม่มีจุดใดตั้งค่าอัตโนมัติ
- **การชำระเงินต้องผ่านการยืนยันก่อนจึงมีผลกับยอดบิล** (`Pending` → `Confirmed`) แยกสิทธิ์บันทึก
  (`payments.create`) ออกจากสิทธิ์ยืนยัน (`payments.approve`) ตามที่ Phase 1 กำหนดสิทธิ์ไว้แล้ว
  เพื่อให้มีจุดตรวจสอบก่อนเงินเข้าระบบจริง โดยเฉพาะกรณีโอน/QR ที่ต้องเทียบกับใบนำฝากธนาคาร
  ไม่มีสถานะ "ปฏิเสธ" แยกต่างหาก (ไม่มีสิทธิ์ `payments.reject` ในเอกสารต้นฉบับ) รายการที่บันทึกผิดปล่อยเป็น
  `Pending` ค้างไว้ได้โดยไม่กระทบยอดบิล
- **ยืนยันยอดชำระเกินยอดคงเหลือของบิลไม่ได้** ป้องกัน Overpayment แต่ยังไม่รองรับ "เครดิตคงเหลือ"
  ย้ายไปใช้กับบิลเดือนถัดไป (ต้องคืนเงินส่วนเกินเป็นรายการแยก หรือออกแบบเพิ่มภายหลัง)
- **ยกเลิกใบแจ้งหนี้ได้เฉพาะที่ยังไม่มีการชำระเงินเข้ามา** (`PaidAmount == 0`) ถ้ามีการชำระแล้วต้องคืนเงิน
  รายการที่ยืนยันไว้ก่อน จึงจะยกเลิกบิลได้
- **ไม่มีสิทธิ์ "สร้างงานแจ้งซ่อม" แยกต่างหาก** สิทธิ์เดิมของ Phase 1 มีแค่ `maintenance.view/assign/update-status`
  จึงตีความว่าการแจ้งซ่อมคือการตั้งสถานะเริ่มต้น (`Reported`) ใช้สิทธิ์ `maintenance.update-status` เดียวกัน
  ทำให้ Staff และ Technician (ที่ไม่มีสิทธิ์ `assign`) แจ้งซ่อมเองได้ ตรงกับที่ RolePermissionDefaults
  มอบสิทธิ์นี้ให้ทั้งสองบทบาท — ผู้เช่ายังแจ้งซ่อมเองไม่ได้ (บทบาท Tenant ไม่มีสิทธิ์ในโมดูลนี้เลย เหมือนทุกโมดูลอื่น)
- **การมอบหมายช่าง (`POST /assign`) แยกจากการเปลี่ยนสถานะทั่วไป (`PATCH /status`) โดยตั้งใจ**
  เพราะต้องตรวจสอบว่าผู้ใช้ที่ระบุมีบทบาท `Technician` และได้รับมอบหมายให้ดูแลหอพักนั้นจริง (ผ่าน `UserDormitory`)
  ก่อนบันทึก — `PATCH /status` ปฏิเสธการตั้งสถานะเป็น `Assigned` โดยตรงเสมอ
- **อนุญาตให้มอบหมายซ้ำ (เปลี่ยนช่างคนใหม่) ได้ตราบใดที่งานยังไม่ปิด** (`Reported`/`Assigned`/`InProgress`)
  ไม่จำกัดแค่ตอนสร้างงานครั้งแรก เพราะงานจริงมักต้องสลับช่างเมื่อคนเดิมติดงานอื่น
- **ลำดับการเปลี่ยนสถานะที่อนุญาตกำหนดตายตัว**: `Reported→Cancelled`, `Assigned→InProgress|Cancelled`,
  `InProgress→Completed|Cancelled` เท่านั้น ข้ามขั้นตอน (เช่น `Reported→Completed`) ไม่ได้
- **สถานะห้องเปลี่ยนอัตโนมัติเช่นเดียวกับโมดูล Reservations/Contracts**: มอบหมายช่างครั้งแรกให้ห้องที่ว่างอยู่
  (`Available`) → เปลี่ยนเป็น `Maintenance`; ปิดงาน (`Completed`/`Cancelled`) แล้วห้องจะกลับเป็น `Available`
  ก็ต่อเมื่อไม่มีงานแจ้งซ่อมอื่นค้างอยู่ในห้องเดียวกัน — ห้องที่ไม่ได้อยู่ในสถานะ `Available`/`Maintenance`
  ตอนนั้น (เช่น `Occupied`) จะไม่ถูกแตะต้องเลย เพราะห้องมีผู้เช่าอยู่แล้วไม่จำเป็นต้องกันไม่ให้จองซ้ำ
- **`RoomService.ChangeStatusAsync` (Endpoint ทั่วไปของห้อง) ไม่มีการตรวจลำดับสถานะใด ๆ** ผู้ใช้ที่มีสิทธิ์
  `rooms.change-status` จึงยังเปลี่ยนสถานะห้องที่มีงานซ่อมค้างอยู่กลับเป็น `Available` เองได้โดยตรง
  การเปลี่ยนสถานะอัตโนมัติของโมดูลนี้เป็นเพียงการช่วยเหลือ ไม่ใช่ข้อบังคับระดับฐานข้อมูล — ยอมรับได้ในขอบเขตนี้
- **Reports เลือกทำแค่ 2 รายงานก่อน** (สรุปรายได้ และใบแจ้งหนี้ค้างชำระ) เพราะเป็นสองรายงานที่ต่อยอดจากข้อมูล
  Invoice/Payment ที่มีอยู่แล้วได้ทันทีโดยไม่ต้องออกแบบ Entity ใหม่ — ยังไม่มีรายงานสรุปการเข้าพัก (Occupancy)
  เพราะซ้ำซ้อนกับ `GET /rooms/summary` ที่มีอยู่แล้ว (คนละสิทธิ์กัน คือ `rooms.view` ไม่ใช่ `reports.view`)
  และยังไม่มีรายงานสรุปงานซ่อมบำรุงหรือรายงานอื่น ๆ
- **สถานะ `Overdue` ของใบแจ้งหนี้ยังไม่เคยถูกตั้งค่าอัตโนมัติที่ไหนเลย** (ตามที่บันทึกไว้ในหัวข้อ Billing ด้านบน)
  รายงานทั้งสองจึงคำนวณ "เกินกำหนด" สดจากการเทียบ `DueDate` กับวันที่ปัจจุบันตอนเรียก แทนที่จะอ่านจาก
  `Invoice.Status == Overdue` โดยตรง
- **ส่งออก CSV จำกัดไม่เกิน 5,000 แถวต่อคำขอ** (`ReportRepository.MaxExportRows`) ป้องกันการดึงข้อมูลจำนวนมาก
  เกินไปในคำขอเดียว ถ้าต้องการมากกว่านั้นต้องกรองด้วย `dormitoryId`/ช่วงเวลาให้แคบลง หรือแบ่งส่งออกหลายรอบ
- **Deposits แยกออกจากโมดูล Payments โดยสิ้นเชิง** ไม่ใช้ Invoice/Payment ผูกกับเงินมัดจำเลย เพราะเป็นกระแสเงิน
  คนละก้อนกัน (เงินค้ำประกันที่ต้องคืน ไม่ใช่รายรับตามรอบบิล) และไม่มีสิทธิ์ `deposits.*` อยู่ในเอกสารต้นฉบับ
  จึงเพิ่มสิทธิ์ใหม่ตามรูปแบบเดียวกับที่เคยทำกับ `reservations.*` ในสาย Contracts
- **สัญญาเช่าหนึ่งฉบับรับเงินมัดจำได้เพียงครั้งเดียว** และรับได้เฉพาะตอนสัญญายังไม่สิ้นสุด (`Draft`/`Active`)
  ค่าเริ่มต้นคัดลอกจาก `Contract.DepositAmount` แต่ปรับเป็นจำนวนอื่นได้ตอนรับจริง
- **คืนเงินมัดจำได้เฉพาะสัญญาที่สิ้นสุดแล้ว (`Terminated`) เท่านั้น** และคืนได้เพียงครั้งเดียวต่อรายการ
  (ไม่รองรับการคืนหลายงวดเหมือน Payment) — ถ้าคืนไม่เต็มจำนวนต้องระบุเหตุผลของส่วนที่หักไว้เสมอ
  คืนเป็น 0 หมายถึงริบเงินมัดจำทั้งหมด (`Forfeited`)
- **Receipts ออกได้เฉพาะจากรายการชำระเงินที่ยืนยันแล้ว (`Payment.Status == Confirmed`)** และออกได้ครั้งเดียว
  ต่อรายการชำระเงินหนึ่งรายการ (unique index บน `PaymentId`) — ถ้ารายการนั้นถูกคืนเงินภายหลังจากออกใบเสร็จ
  ไปแล้ว ใบเสร็จเดิมจะยังคงอยู่เป็นหลักฐานทางประวัติ ไม่ถูกยกเลิกอัตโนมัติ (ระบบยังไม่มี Endpoint ยกเลิก/ออกใหม่
  ใบเสร็จ ถือว่าอยู่นอกขอบเขตของ Phase 2 สายนี้)
- **จำนวนเงินในใบเสร็จคัดลอกจาก `Payment.Amount` ตอนออกใบเสร็จ (Snapshot)** ไม่ใช่การอ้างอิงสด เพื่อให้เอกสาร
  ที่ออกไปแล้วคงที่ตลอดไปแม้ข้อมูลต้นทางจะถูกแก้ไขภายหลัง — เหมือนหลักการเดียวกับที่ `Contract.MonthlyRent`
  คัดลอกจาก `Room.MonthlyRent` ตอนทำสัญญา
- **Expenses เป็นสมุดบันทึกฝั่งรายจ่ายล้วน ๆ ไม่มีสถานะหรือ Workflow อนุมัติ** ต่างจาก Invoices/Payments
  ที่เป็นฝั่งรายรับซึ่งมี Draft/Issued/Confirmed ให้ตรวจสอบ เพราะยังไม่มีข้อกำหนดเรื่องการอนุมัติค่าใช้จ่าย
  จากเอกสารต้นฉบับ — ถ้าต้องการ Workflow อนุมัติภายหลัง (เช่น ต้องผ่าน Owner ก่อนบันทึกจริง) ต้องออกแบบเพิ่ม
- **หมวดหมู่ค่าใช้จ่ายกำหนดเป็น Enum ตายตัว 6 ค่า** (Utilities, Maintenance, Salaries, Supplies, Marketing,
  Other) แทนที่จะให้ผู้ใช้กำหนดหมวดหมู่เองแบบอิสระ เพื่อให้จัดกลุ่ม/ทำรายงานสรุปได้ง่ายในอนาคต — ถ้าไม่พอ
  ต้องเพิ่มค่าใหม่ใน Enum (ไม่ใช่ตารางหมวดหมู่แยกต่างหาก เพราะมองว่าจำนวนหมวดหมู่คงที่ไม่บ่อยเปลี่ยน)
- **ไม่มีสิทธิ์ `expenses.*` ให้ Staff/Technician** เพราะมองว่าเป็นข้อมูลการเงินภายในที่ไม่จำเป็นต้องให้
  พนักงานหน้าเคาน์เตอร์เข้าถึง ต่างจาก Payments ที่ Staff ยังมีสิทธิ์ View ได้ (เพราะเกี่ยวข้องกับผู้เช่าโดยตรง)
- **Parcels ผูกกับห้อง (`RoomId`) เสมอ ส่วนผู้เช่า (`TenantId`) เป็นทางเลือก** เพราะเจ้าหน้าที่หน้าเคาน์เตอร์
  มักบันทึกพัสดุที่มาถึงก่อนยืนยันได้แน่ชัดว่าเป็นของผู้เช่าคนไหนในห้อง (ห้องอาจมีผู้เช่าร่วมหลายคน)
  และห้องคือหน่วยที่จำเป็นสำหรับขอบเขตองค์กร/หอพัก (`Tenant` ไม่ได้ผูกกับหอพักถาวรเหมือน `Room`)
- **ผู้มารับพัสดุจริงบันทึกเป็นชื่ออิสระ (`PickedUpByName`) ไม่ใช่การอ้างอิง `TenantId`** เพราะในทางปฏิบัติ
  คนที่มารับอาจไม่ใช่ผู้เช่าโดยตรง (เช่น เพื่อนร่วมห้อง หรือญาติมารับแทน) — ไม่ตรวจสอบตัวตนเข้มงวดกว่านี้
- **สิทธิ์ `parcels.*` มอบให้ Staff เต็มรูปแบบ** (View/Log/Pickup) ต่างจาก Deposits/Receipts/Expenses ที่ไม่ให้
  Staff เลย เพราะพัสดุเป็นงานหน้าเคาน์เตอร์ประจำวันโดยตรง ไม่ใช่ข้อมูลการเงินภายใน — ให้ Accountant/Technician
  ไม่มีสิทธิ์เลยเพราะไม่เกี่ยวข้องกับงานของทั้งสองบทบาทนี้
- **Visitors ออกแบบตามแบบเดียวกับ Parcels ทุกประการ** (ผูกกับห้องเสมอ ผู้เช่าเป็นทางเลือก, สิทธิ์มอบให้
  Owner/DormManager/Staff เต็มรูปแบบเหมือนงานหน้าเคาน์เตอร์) เพราะเป็นกรณีใช้งานที่คล้ายกันมาก
  (บันทึกคนเข้า-ออกที่ไม่ใช่ผู้เช่าเอง) ต่างกันแค่ Visitors ไม่มีสถานะ "ริบ/ยกเลิก" เพราะเข้า-ออกมีแค่สองสถานะ
- **เก็บเลขบัตรประจำตัวประชาชนของผู้มาเยือนได้ (`IdNumber`) เป็นทางเลือก** สำหรับหอพักที่ต้องการบันทึก
  เพื่อความปลอดภัยตามระเบียบภายใน แต่ไม่บังคับ เพราะเอกสารต้นฉบับไม่ได้กำหนดข้อบังคับด้านนี้ไว้
  (ควรพิจารณาเรื่องการเก็บรักษาข้อมูลส่วนบุคคลตาม PDPA ก่อนใช้งานจริงถ้าเปิดใช้ฟิลด์นี้)
- **Parking ต้องระบุผู้เช่าเสมอ (`TenantId` บังคับ) ต่างจาก Parcels/Visitors ที่ผู้เช่าเป็นทางเลือก**
  เพราะรถหนึ่งคันต้องมีเจ้าของที่ระบุตัวตนได้เสมอในทางปฏิบัติ ไม่เหมือนพัสดุ/ผู้มาเยือนที่อาจยังไม่ทราบ
  แน่ชัดตอนบันทึกครั้งแรก — ห้องยังคงเก็บไว้เพื่อขอบเขตองค์กร/หอพักเช่นเดิม
- **ไม่มี Entity แม่ของ "ช่องจอดรถ" แยกต่างหาก** (`ParkingSpotLabel` เป็นข้อความอิสระ ไม่ผูกกับระบบจัดการ
  ช่องจอดที่มีจำนวนจำกัดหรือการจองล่วงหน้า) เพราะไม่มีข้อกำหนดเรื่องนี้จากเอกสารต้นฉบับ ถ้าต้องการจัดการ
  ช่องจอดอย่างเป็นระบบ (เช่น จำกัดจำนวนช่อง, ป้องกันชนกัน) ต้องออกแบบเพิ่มภายหลัง
- **ป้องกันเฉพาะทะเบียนรถเดียวกันมีการจดทะเบียนที่ยัง Active ซ้อนกันสองรายการ** ไม่ได้ป้องกันผู้เช่าคนเดียวกัน
  จดทะเบียนรถหลายคันพร้อมกัน (อาจมีรถมากกว่าหนึ่งคันจริง ๆ) และไม่ได้จำกัดจำนวนคันต่อห้อง/ต่อผู้เช่า
- **Announcement ผูกกับหอพักเดียวเสมอ (`DormitoryId` บังคับ)** ไม่รองรับประกาศระดับองค์กรที่แสดงพร้อมกัน
  ทุกหอพักในองค์กร เพราะเอกสารต้นฉบับไม่ได้ระบุรายละเอียดของโมดูลนี้ไว้เลย ถ้าต้องการประกาศระดับองค์กร
  ต้องออกแบบเพิ่ม (เช่น สร้างประกาศแยกทีละหอพัก หรือเพิ่ม `DormitoryId` แบบไม่บังคับในภายหลัง)
- **วงจรชีวิตมีสามสถานะตายตัว**: `Draft` (สร้างใหม่) → `Published` (เผยแพร่ ตั้ง `PublishedAt`) →
  `Archived` (เก็บเข้าคลัง) เดินไปทางเดียว ไม่มี Endpoint แก้ไขเนื้อหาหรือย้อนกลับจาก `Published` ไป `Draft`
  เพราะเอกสารต้นฉบับไม่ได้กำหนดรายละเอียดเรื่องการแก้ไขประกาศที่เผยแพร่ไปแล้ว
- **ไม่มีสิทธิ์ `announcements.archive` แยกต่างหาก** ใช้สิทธิ์ `announcements.publish` เดียวกันครอบคลุมทั้ง
  การเผยแพร่และการเก็บเข้าคลัง เพราะทั้งสองเป็นการเปลี่ยนสถานะที่ทำโดยผู้มีอำนาจเผยแพร่ประกาศกลุ่มเดียวกันเสมอ
  ในทางปฏิบัติ ต่างจาก Parcels/Visitors ที่แยกสิทธิ์ Log/Pickup หรือ CheckIn/CheckOut เพราะกรณีนั้นมักทำโดย
  เจ้าหน้าที่คนละกะกัน
- **สิทธิ์ `announcements.*` มอบให้ Owner/DormManager เต็มรูปแบบ แต่ Staff ได้แค่ View** ต่างจาก
  Parcels/Visitors/Parking ที่ให้ Staff เต็มรูปแบบ เพราะประกาศเป็นการสื่อสารอย่างเป็นทางการจากฝ่ายบริหาร
  ไม่ใช่งานบันทึกประจำวันหน้าเคาน์เตอร์ — Accountant/Technician ไม่มีสิทธิ์เลยเพราะไม่เกี่ยวข้องกับงานทั้งสองบทบาทนี้
- **Notification ไม่สืบทอดจาก `TenantEntity`** ต่างจากทุกโมดูลก่อนหน้าของ Phase 2 เพราะผูกกับผู้ใช้ผู้รับ
  โดยตรง (`RecipientUserId`) ไม่ใช่ข้อมูลที่ต้องกรองด้วยหอพักแบบ List Endpoint ทั่วไป — ใช้แนวทางเดียวกับ
  `AuditLog` ที่ไม่ใช้ `TenantScope` เพราะไม่ใช่ทรัพยากรที่ผู้ใช้หลายคนแชร์เห็นร่วมกันตามขอบเขตองค์กร/หอพัก
- **การดูและทำเครื่องหมายอ่านแล้วของการแจ้งเตือนตนเองเปิดให้ผู้ใช้ที่เข้าสู่ระบบทุกคนทำได้เสมอ** ไม่ผูกกับ
  สิทธิ์ใด (`[Authorize]` เฉย ๆ เหมือน `GET /auth/me`) เพราะเป็นข้อมูลของตนเองโดยตรง ไม่ใช่ข้อมูลที่ต้องเช็ค
  บทบาท — มีเพียงสิทธิ์ `notifications.send` สิทธิ์เดียวที่คุมว่าใครส่งการแจ้งเตือนถึงคนอื่นได้ (Owner/DormManager)
- **ส่งการแจ้งเตือนได้เฉพาะถึงผู้ใช้ในองค์กรเดียวกันเท่านั้น** (ยกเว้น SuperAdmin ส่งข้ามองค์กรได้) ตรวจจาก
  `OrganizationId` ของผู้รับเทียบกับผู้ส่ง ถ้าไม่ตรงกันตอบ 404 เพื่อไม่ให้เดาได้ว่ามีผู้ใช้รหัสนั้นอยู่จริงใน
  องค์กรอื่น ตามหลักการเดียวกับ `TenantScope.EnsureOrganizationAccess`
- **ไม่มีการแจ้งเตือนอัตโนมัติจากเหตุการณ์อื่นในระบบ** (เช่น ออกใบแจ้งหนี้แล้วแจ้งเตือนผู้เช่า, งานซ่อมเสร็จแล้ว
  แจ้งเตือนผู้แจ้ง) — โมดูลนี้เป็นเพียงกลไกส่ง/รับการแจ้งเตือนด้วยมือเท่านั้น การเชื่อมต่ออัตโนมัติกับเหตุการณ์
  ของโมดูลอื่นต้องออกแบบเพิ่มภายหลัง (เช่น Domain Event หรือ Background Job)
- **ผู้รับเป็น User ของระบบ (พนักงาน/ผู้ดูแล) เท่านั้น ไม่ใช่ Tenant** เพราะบทบาท Tenant ยังไม่มี Endpoint
  หรือกลไกยืนยันตัวตนสำหรับพอร์ทัลผู้เช่าแยกต่างหากในขอบเขตนี้ (ดูหัวข้อ Tenants ด้านบน)

### ยังไม่ทำ (ส่วนที่เหลือของ Phase 2)

Settings — ยังไม่ได้เริ่ม และต้องการรายละเอียดกฎธุรกิจเพิ่มเติมก่อนเริ่มออกแบบ Entity ให้ตรงกับความต้องการจริง
