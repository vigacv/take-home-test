using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fundo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "Id", "Amount", "ApplicantName", "CurrentBalance", "Status" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0000-0000-000000000001"), 1000.00m, "Maria Silva", 1000.00m, "Active" },
                    { new Guid("a1b2c3d4-0002-0000-0000-000000000002"), 2500.00m, "João Santos", 0.00m, "Paid" },
                    { new Guid("a1b2c3d4-0003-0000-0000-000000000003"), 5000.00m, "Ana Oliveira", 5000.00m, "Active" },
                    { new Guid("a1b2c3d4-0004-0000-0000-000000000004"), 750.00m, "Carlos Pereira", 0.00m, "Paid" },
                    { new Guid("a1b2c3d4-0005-0000-0000-000000000005"), 3200.00m, "Fernanda Lima", 1600.00m, "Active" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Loans");
        }
    }
}
