using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiShield.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyDailyUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeyDailyUsage",
                columns: table => new
                {
                    KeyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyDailyUsage", x => new { x.KeyId, x.UsageDate });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyDailyUsage");
        }
    }
}
