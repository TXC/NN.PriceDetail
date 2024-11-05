using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.IO;
using Shared;
using System;
using System.Reflection;
using System.Text;
using Web.Entities;
using System.Linq;

namespace Web.Data
{
    /// <summary>
    /// Context for the database.
    /// </summary>
    /// <param name="options"></param>
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        /// <seealso cref="DbSetProperty"/>
        public required DbSet<Price> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSeeding((ctx, _) => Seeder.Run(ctx, _, GetPrices))
                .UseAsyncSeeding((ctx, _, token)
                            => Seeder.RunAsync(ctx, _, GetPrices, token));
        }

        static Price[] GetPrices()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                           "Data/SeedData/price_detail.csv");

            var csvconfig = CsvConfiguration.FromAttributes<Price>();

            using StreamReader reader = new(path, Encoding.UTF8);
            using CsvReader csv = new(reader, csvconfig);

            var data = csv.GetRecords<Price>().ToArray();
            return data;
        }
    }
}
