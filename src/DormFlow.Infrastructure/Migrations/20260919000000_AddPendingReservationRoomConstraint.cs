using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DormFlow.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPendingReservationRoomConstraint : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_reservations_one_pending_per_room",
            table: "reservations",
            column: "room_id",
            unique: true,
            filter: "status = 'Pending' AND is_deleted = false");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_reservations_one_pending_per_room",
            table: "reservations");
    }
}
