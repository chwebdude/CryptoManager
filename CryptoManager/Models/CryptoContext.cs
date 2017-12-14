using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoManager.Models.DataModel;
using Microsoft.EntityFrameworkCore;

namespace CryptoManager.Models
{
    public class CryptoContext : DbContext
    {
        public CryptoContext(DbContextOptions<CryptoContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=crypto.db");
        }

        public DbSet<CryptoTransaction> CryptoTransactions { get; set; }
    }
}
