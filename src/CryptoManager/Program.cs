using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace CryptoManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var nlogConfig = Path.Combine(rootPath, "NLog.config");
            NLogBuilder.ConfigureNLog(nlogConfig);
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                //NLog: catch setup errors
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog() // NLog: setup NLog for Dependency injection
                .Build();
    }
}
