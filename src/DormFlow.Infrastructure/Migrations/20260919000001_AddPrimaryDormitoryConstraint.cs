using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DormFlow.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddPrimaryDormitoryConstraint : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_user_dormitories_one_primary_per_user",
            table: "user_dormitories",
            column: "user_id",
            unique: true,
            filter: "is_primary = true");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_user_dormitories_one_primary_per_user",
            table: "user_dormitories");
    }
}
