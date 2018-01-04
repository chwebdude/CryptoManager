using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Model.Migrations
{
    public partial class FiatToBuys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BuyFiatAmount",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BuyFiatRate",
                table: "Transactions",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyFiatAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BuyFiatRate",
                table: "Transactions");
        }
    }
}
