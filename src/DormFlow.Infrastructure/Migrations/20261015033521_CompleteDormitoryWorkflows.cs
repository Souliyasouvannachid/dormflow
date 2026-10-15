using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DormFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteDormitoryWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "confirmed_at",
                table: "reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "actual_cost",
                table: "maintenance_requests",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "maintenance_requests",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "General");

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "maintenance_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "maintenance_request_id",
                table: "expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "co_tenant_ids",
                table: "contracts",
                type: "uuid[]",
                nullable: false,
                defaultValue: Array.Empty<Guid>());

            migrationBuilder.AddColumn<decimal>(
                name: "electricity_rate_per_unit",
                table: "contracts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "water_rate_per_unit",
                table: "contracts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "announcements",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.AddColumn<string[]>(
                name: "target_roles",
                table: "announcements",
                type: "text[]",
                nullable: false,
                defaultValue: Array.Empty<string>());

            migrationBuilder.Sql("""
                UPDATE contracts c SET water_rate_per_unit = d.water_rate_per_unit,
                    electricity_rate_per_unit = d.electricity_rate_per_unit
                FROM dormitories d WHERE c.dormitory_id = d.id;
                """);

            migrationBuilder.CreateTable(
                name: "billing_periods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    month = table.Column<DateOnly>(type: "date", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dormitory_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_billing_periods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contract_renewals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    previous_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    new_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    previous_rent = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    new_rent = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dormitory_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_renewals", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_renewals_contracts_contract_id",
                        column: x => x.contract_id,
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invoice_charges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dormitory_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_charges", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_charges_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    maintenance_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dormitory_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_events_maintenance_requests_maintenance_request",
                        column: x => x.maintenance_request_id,
                        principalTable: "maintenance_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_requests_tenant_id",
                table: "maintenance_requests",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_expenses_maintenance_request_id",
                table: "expenses",
                column: "maintenance_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_billing_periods_dormitory_id_month",
                table: "billing_periods",
                columns: new[] { "dormitory_id", "month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_billing_periods_is_deleted",
                table: "billing_periods",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_billing_periods_organization_id_dormitory_id",
                table: "billing_periods",
                columns: new[] { "organization_id", "dormitory_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contract_renewals_contract_id",
                table: "contract_renewals",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_renewals_is_deleted",
                table: "contract_renewals",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_contract_renewals_organization_id_dormitory_id",
                table: "contract_renewals",
                columns: new[] { "organization_id", "dormitory_id" });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_charges_invoice_id",
                table: "invoice_charges",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_charges_is_deleted",
                table: "invoice_charges",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_invoice_charges_organization_id_dormitory_id",
                table: "invoice_charges",
                columns: new[] { "organization_id", "dormitory_id" });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_events_is_deleted",
                table: "maintenance_events",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_events_maintenance_request_id",
                table: "maintenance_events",
                column: "maintenance_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_events_organization_id_dormitory_id",
                table: "maintenance_events",
                columns: new[] { "organization_id", "dormitory_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_maintenance_requests_maintenance_request_id",
                table: "expenses",
                column: "maintenance_request_id",
                principalTable: "maintenance_requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_maintenance_requests_tenants_tenant_id",
                table: "maintenance_requests",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expenses_maintenance_requests_maintenance_request_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_maintenance_requests_tenants_tenant_id",
                table: "maintenance_requests");

            migrationBuilder.DropTable(
                name: "billing_periods");

            migrationBuilder.DropTable(
                name: "contract_renewals");

            migrationBuilder.DropTable(
                name: "invoice_charges");

            migrationBuilder.DropTable(
                name: "maintenance_events");

            migrationBuilder.DropIndex(
                name: "ix_maintenance_requests_tenant_id",
                table: "maintenance_requests");

            migrationBuilder.DropIndex(
                name: "ix_expenses_maintenance_request_id",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "confirmed_at",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "actual_cost",
                table: "maintenance_requests");

            migrationBuilder.DropColumn(
                name: "category",
                table: "maintenance_requests");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "maintenance_requests");

            migrationBuilder.DropColumn(
                name: "maintenance_request_id",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "co_tenant_ids",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "electricity_rate_per_unit",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "water_rate_per_unit",
                table: "contracts");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "target_roles",
                table: "announcements");
        }
    }
}
