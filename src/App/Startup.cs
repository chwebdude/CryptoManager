using System;
using System.IO;
using BackgroundServices;
using CryptoManager.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;

namespace CryptoManager.App
{
    public class Startup
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static string DataDir;


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var rootPath = Directory.GetCurrentDirectory();
            var parent = Directory.GetParent(rootPath);
            DataDir = Path.Combine(parent.FullName, "data");
            Directory.CreateDirectory(DataDir);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CryptoContext.DatabaseFile = Path.Combine(DataDir, "crypto.db");
            var connectionString = "Data Source = '" + CryptoContext.DatabaseFile + "'";
            services.AddDbContext<CryptoContext>(options => options.UseSqlite(connectionString));

            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoManager", Version = "v1" });
            });

            //services.AddAut

            services.AddHangfire(Configuration => Configuration.UseMemoryStorage());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "App v1"));
            }

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<CryptoContext>().Database.Migrate();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to migrate database");
                throw;
            }

            app.UseHttpsRedirection();

            // Enable Hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            BackgroundJob.Enqueue<Calculator>(c => c.RecalculateAll());
        }
    }
}
