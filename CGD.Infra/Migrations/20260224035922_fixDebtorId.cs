using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CGD.Infra.Migrations
{
    /// <inheritdoc />
    public partial class fixDebtorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DebtorId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_DebtorId",
                table: "Expenses",
                column: "DebtorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_users_DebtorId",
                table: "Expenses",
                column: "DebtorId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_users_DebtorId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_DebtorId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DebtorId",
                table: "Expenses");
        }
    }
}
