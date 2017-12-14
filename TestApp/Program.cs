using System;
using CryptoManager.Models;
using Microsoft.EntityFrameworkCore;
using Model.DbModels;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var options = new DbContextOptions<CryptoContext>();

            var ctx = new CryptoContext();
            
            ctx.Transactions.Add(new CryptoTransaction());
            ctx.SaveChanges();
        }
    }
}
