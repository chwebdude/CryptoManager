using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundServices;
using CryptoManager.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Plugins;
using Swashbuckle.AspNetCore.Swagger;
using CryptoCompare = Plugins.MarketData.CryptoCompare;

namespace CryptoManager
{
    public class Startup
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var parent = Directory.GetParent(rootPath);
            var dataDir = Path.Combine(parent.FullName, "data");
            CryptoContext.DatabaseFile = Path.Combine(dataDir, "crypto.db");

            services.AddMvc();
            Directory.CreateDirectory(dataDir);
            var connection = "Data Source = '" + CryptoContext.DatabaseFile + "'";
            services.AddDbContext<CryptoContext>(options => options.UseSqlite(connection));

            // Add some Swag
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "Crypto Manager API",
                    Version = "v1"
                }));

            services.AddAutoMapper();

            services.AddHangfire(config => config.UseMemoryStorage());

            services.AddSingleton<IMarketData>(new Plugins.MarketData.CryptoCompare());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }



            // Migrate and seed the database during startup. Must be synchronous.
            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<CryptoContext>().Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // I'm using Serilog here, but use the logging solution of your choice.
                Logger.Error(ex, "Failed to migrate database");
            }


            app.UseStaticFiles();

            // Enable Hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crypto Manager API");
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            //BackgroundJob.Enqueue<Importer>(i => i.ImportAll());
            BackgroundJob.Enqueue<Calculator>(c => c.RecalculateAll());
            //BackgroundJob.Enqueue<Calculator>(c => c.CalculateFlow());

        }
    }
}
