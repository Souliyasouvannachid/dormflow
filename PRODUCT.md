# DormFlow

## Product and users
Dormitory management for owners, managers, staff and technicians. The requested workflow is rooms → tenants → reservations → contracts → move in → meters → invoices → payments → receipts, with maintenance, parcels, expenses, announcements and reporting.

## Stack
User requested React + Vite, Bootstrap / React-Bootstrap, ApexCharts, ASP.NET Core .NET 10, EF Core and PostgreSQL. The user confirmed creating the frontend in `web`.

## Constraints
Use actual API data. Restrict data by organization and assigned dormitory. Permission checks on the server remain authoritative. Interface language is Thai. Currency is not confirmed; format amounts without an invented currency symbol. No payment gateway credentials were supplied; QR means a recorded payment method until a bank integration is configured.

## Open decisions
Production hosting, bank integration, and tenant-facing accounts are not specified.
