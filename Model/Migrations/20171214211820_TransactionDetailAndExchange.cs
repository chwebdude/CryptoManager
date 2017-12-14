using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Model.Migrations
{
    public partial class TransactionDetailAndExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BuyCurrency",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Transactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ExchangeId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FeeCurrency",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "InCurrency",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OutAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OutCurrency",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SellAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SellCurrency",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionKey",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Transactions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Exchange",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PrivateKey = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true),
                    SupportsPrivateKey = table.Column<bool>(nullable: false),
                    SupportsPublicKey = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchange", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ExchangeId",
                table: "Transactions",
                column: "ExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Exchange_ExchangeId",
                table: "Transactions",
                column: "ExchangeId",
                principalTable: "Exchange",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Exchange_ExchangeId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Exchange");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ExchangeId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BuyAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BuyCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ExchangeId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FeeAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FeeCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OutAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OutCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SellAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SellCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionKey",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");
        }
    }
}
