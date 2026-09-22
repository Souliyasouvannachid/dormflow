using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DormFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomFloorPlanHotspot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "floor_plan_height",
                table: "rooms",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "floor_plan_width",
                table: "rooms",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "floor_plan_x",
                table: "rooms",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "floor_plan_y",
                table: "rooms",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "floor_plan_height",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "floor_plan_width",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "floor_plan_x",
                table: "rooms");

            migrationBuilder.DropColumn(
                name: "floor_plan_y",
                table: "rooms");
        }
    }
}
