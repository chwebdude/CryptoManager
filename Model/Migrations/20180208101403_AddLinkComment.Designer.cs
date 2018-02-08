﻿// <auto-generated />
using CryptoManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Model.Enums;
using System;

namespace Model.Migrations
{
    [DbContext(typeof(CryptoContext))]
    [Migration("20180208101403_AddLinkComment")]
    partial class AddLinkComment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Model.DbModels.CryptoTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("BuyAmount");

                    b.Property<string>("BuyCurrency");

                    b.Property<decimal>("BuyFiatAmount");

                    b.Property<decimal>("BuyFiatRate");

                    b.Property<string>("Comment");

                    b.Property<DateTime>("DateTime");

                    b.Property<Guid>("ExchangeId");

                    b.Property<decimal>("FeeAmount");

                    b.Property<string>("FeeCurrency");

                    b.Property<string>("FromAddress");

                    b.Property<decimal>("InAmount");

                    b.Property<string>("InCurrency");

                    b.Property<decimal>("OutAmount");

                    b.Property<string>("OutCurrency");

                    b.Property<decimal>("Rate");

                    b.Property<decimal>("SellAmount");

                    b.Property<string>("SellCurrency");

                    b.Property<string>("ToAddress");

                    b.Property<string>("TransactionHash");

                    b.Property<string>("TransactionKey");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Model.DbModels.Exchange", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<int>("ExchangeId");

                    b.Property<string>("Passphrase");

                    b.Property<string>("PrivateKey");

                    b.Property<string>("PublicKey");

                    b.HasKey("Id");

                    b.ToTable("Exchanges");
                });

            modelBuilder.Entity("Model.DbModels.FiatBalance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency");

                    b.Property<Guid>("ExchangeId");

                    b.Property<decimal>("Invested");

                    b.Property<decimal>("Payout");

                    b.HasKey("Id");

                    b.ToTable("FiatBalances");
                });

            modelBuilder.Entity("Model.DbModels.FlowLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Comment");

                    b.Property<string>("Currency");

                    b.Property<DateTime>("DateTime");

                    b.Property<Guid>("FlowNodeSource");

                    b.Property<Guid>("FlowNodeTarget");

                    b.HasKey("Id");

                    b.ToTable("FlowLinks");
                });

            modelBuilder.Entity("Model.DbModels.FlowNode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Comment");

                    b.Property<string>("Currency");

                    b.Property<DateTime>("DateTime");

                    b.Property<Guid>("ExchangeId");

                    b.Property<Guid>("TransactionId");

                    b.HasKey("Id");

                    b.ToTable("FlowNodes");
                });

            modelBuilder.Entity("Model.DbModels.Fund", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Currency");

                    b.Property<Guid>("ExchangeId");

                    b.HasKey("Id");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("Model.DbModels.Setting", b =>
                {
                    b.Property<int>("Key");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
