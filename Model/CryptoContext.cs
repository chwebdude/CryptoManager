using System;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=crypto.db");
        }

        public DbSet<CryptoTransaction> Transactions { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Exchange> Exchanges{ get; set; }
    }
}
