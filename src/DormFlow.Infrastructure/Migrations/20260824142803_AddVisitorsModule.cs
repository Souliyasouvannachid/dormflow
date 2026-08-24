using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DormFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "visitors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    visitor_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    visitor_phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    id_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    check_in_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    check_out_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dormitory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_visitors", x => x.id);
                    table.ForeignKey(
                        name: "fk_visitors_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_visitors_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_visitors_is_deleted",
                table: "visitors",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_visitors_organization_id_dormitory_id",
                table: "visitors",
                columns: new[] { "organization_id", "dormitory_id" });

            migrationBuilder.CreateIndex(
                name: "ix_visitors_room_id",
                table: "visitors",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_visitors_status",
                table: "visitors",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_visitors_tenant_id",
                table: "visitors",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "visitors");
        }
    }
}
