using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Model.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exchanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    ExchangeId = table.Column<int>(nullable: false),
                    PrivateKey = table.Column<string>(nullable: true),
                    PublicKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiatBalances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeId = table.Column<Guid>(nullable: false),
                    Invested = table.Column<decimal>(nullable: false),
                    Payout = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiatBalances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    ExchangeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BuyAmount = table.Column<decimal>(nullable: false),
                    BuyCurrency = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    ExchangeId = table.Column<Guid>(nullable: false),
                    FeeAmount = table.Column<decimal>(nullable: false),
                    FeeCurrency = table.Column<string>(nullable: true),
                    FromAddress = table.Column<string>(nullable: true),
                    InAmount = table.Column<decimal>(nullable: false),
                    InCurrency = table.Column<string>(nullable: true),
                    OutAmount = table.Column<decimal>(nullable: false),
                    OutCurrency = table.Column<string>(nullable: true),
                    Rate = table.Column<decimal>(nullable: false),
                    SellAmount = table.Column<decimal>(nullable: false),
                    SellCurrency = table.Column<string>(nullable: true),
                    ToAddress = table.Column<string>(nullable: true),
                    TransactionHash = table.Column<string>(nullable: true),
                    TransactionKey = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exchanges");

            migrationBuilder.DropTable(
                name: "FiatBalances");

            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
