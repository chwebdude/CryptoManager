using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Model.Migrations
{
    public partial class SimplyfiedCreditcardBuys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeWithWallet",
                table: "Transactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TradeWithWallet",
                table: "Transactions",
                nullable: false,
                defaultValue: false);
        }
    }
}
