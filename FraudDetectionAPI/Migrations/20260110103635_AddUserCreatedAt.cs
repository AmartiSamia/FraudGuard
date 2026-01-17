using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudDetectionAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FraudAlerts_TransactionId",
                table: "FraudAlerts");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_TransactionId",
                table: "FraudAlerts",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FraudAlerts_TransactionId",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_TransactionId",
                table: "FraudAlerts",
                column: "TransactionId");
        }
    }
}
