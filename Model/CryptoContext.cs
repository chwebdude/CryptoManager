﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model.DbModels;

namespace CryptoManager.Models
{
    public class CryptoContext : DbContext
    {
        public CryptoContext(DbContextOptions<CryptoContext> options) : base(options) { }
        public static string DatabaseFile;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source='" + CryptoContext.DatabaseFile + "'");
        }

        public DbSet<CryptoTransaction> Transactions { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<FiatBalance> FiatBalances{ get; set; }
        public DbSet<FlowNode> FlowNodes { get; set; }
        public DbSet<FlowLink> FlowLinks { get; set; }
    }
}
