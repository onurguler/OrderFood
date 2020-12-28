using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain;

namespace OrderFood.Infrastructure.Data
{
    public static class SeedDatabase
    {

        public static void Seed()
        {
            var context = new OrderFoodContext();

            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context.Products.Count() == 0)
                {
                    SeedProducts(context);
                }

                context.SaveChanges();
            }
        }

        public static void SeedProducts(OrderFoodContext context)
        {
            var products = new Product[]
            {
                new Product
                {
                    Title = "Hamburger Menü",
                    Subtitle = "Patates kızartması ve seçeceğiniz bir içecek ile.",
                    Price = 19.99m,
                    Description = "Nefis hamburger menü.",
                },
                new Product
                {
                    Title = "Dana Steak",
                    Subtitle = "Salata ve meze ile.",
                    Price = 47.99m,
                    Description = "Barbeküde pişmiş steak.",
                },
                new Product
                {
                    Title = "Izgara Köfte Menü",
                    Subtitle = "Salata ve meze ile.",
                    Price = 29.99m,
                    Description = "Nefis ızgara köfte.",
                },
                new Product
                {
                    Title = "Şefin Tavası",
                    Subtitle = "Salata ve ekmek ile.",
                    Price = 24.99m,
                    Description = "Nefis terbiyeli tavuk, mantar, soğan, domates ve leziz salata bütünleşmesi.",
                },
            };

            context.Products.AddRange(products);
        }
    }
}
