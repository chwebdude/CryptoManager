using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace CryptoManager.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var nlogConfig = Path.Combine(rootPath, "NLog.config");

            var logger = NLogBuilder.ConfigureNLog(nlogConfig).GetCurrentClassLogger();

            try
            {
                logger.Debug("Start Webhost");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                //NLog: catch setup errors
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();  // NLog: Setup NLog for Dependency injection;
    }
}
