using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            // Create a scope for the Host Builder
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Create the DB connection service
            // Apply the migration
            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();

                // This is our Seed class
                await Seed.SeedData(context);
            }
            catch (Exception ex)
            {
                // Log out any errors if migration fails
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during database migration");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
