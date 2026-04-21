using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiShield.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeyDailyUsage",
                columns: table => new
                {
                    KeyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsageDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyDailyUsage", x => new { x.KeyId, x.UsageDate });
                });

            migrationBuilder.CreateTable(
                name: "ApiRequestLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApiKeyId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Route = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiRequestLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UX_ApiRequestLog_ApiKeyId_IdempotencyKey",
                table: "ApiRequestLog",
                columns: new[] { "ApiKeyId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyDailyUsage");

            migrationBuilder.DropTable(
                name: "ApiRequestLog");
        }
    }
}
