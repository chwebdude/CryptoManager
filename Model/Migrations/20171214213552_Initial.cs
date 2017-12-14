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
                    InAdress = table.Column<string>(nullable: true),
                    InAmount = table.Column<decimal>(nullable: false),
                    InCurrency = table.Column<string>(nullable: true),
                    OutAdress = table.Column<string>(nullable: true),
                    OutAmount = table.Column<decimal>(nullable: false),
                    OutCurrency = table.Column<string>(nullable: true),
                    SellAmount = table.Column<decimal>(nullable: false),
                    SellCurrency = table.Column<string>(nullable: true),
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
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
