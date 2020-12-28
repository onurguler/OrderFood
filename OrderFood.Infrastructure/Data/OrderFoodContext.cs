using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFood.Domain;

namespace OrderFood.Infrastructure.Data
{
    public class OrderFoodContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            optionsBuilder.UseSqlite("Data Source=orderfood.db");
        }
    }
}
