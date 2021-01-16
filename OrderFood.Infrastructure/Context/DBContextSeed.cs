using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Models;

namespace OrderFood.Infrastructure.Context
{
    public static class DBContextSeed
    {

        public static void Seed(DBContext context)
        {
            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context.Products.Count() == 0)
                {
                    SeedProducts(context);
                }

                context.SaveChanges();
            }
        }

        public static void SeedProducts(DBContext context)
        {
            var products = new Product[]
            {
                new Product
                {
                    Title = "Hamburger Menü",
                    Subtitle = "Patates kızartması ve seçeceğiniz bir içecek ile.",
                    Price = 19.99m,
                    Description = "Nefis hamburger menü.",
                    ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?ixid=MXwxMjA3fDB8MHxzZWFyY2h8Mnx8aGFtYnVyZ2VyfGVufDB8fDB8&ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=60",
                },
                new Product
                {
                    Title = "Dana Steak",
                    Subtitle = "Salata ve meze ile.",
                    Price = 47.99m,
                    Description = "Barbeküde pişmiş steak.",
                    ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?ixid=MXwxMjA3fDB8MHxzZWFyY2h8MTB8fHN0ZWFrfGVufDB8fDB8&ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=60",
                },
                new Product
                {
                    Title = "Izgara Köfte Menü",
                    Subtitle = "Salata ve meze ile.",
                    Price = 29.99m,
                    Description = "Nefis ızgara köfte.",
                    ImageUrl = "https://images.unsplash.com/photo-1529042410759-befb1204b468?ixlib=rb-1.2.1&ixid=MXwxMjA3fDB8MHxzZWFyY2h8MTV8fGZvb2R8ZW58MHx8MHw%3D&auto=format&fit=crop&w=800&q=60"
                },
                new Product
                {
                    Title = "Şefin Tavası",
                    Subtitle = "Salata ve ekmek ile.",
                    Price = 24.99m,
                    Description = "Nefis terbiyeli tavuk, mantar, soğan, domates ve leziz salata bütünleşmesi.",
                    ImageUrl = "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?ixid=MXwxMjA3fDB8MHxzZWFyY2h8MzZ8fGNoaWNrZW4lMjBtZWF0fGVufDB8fDB8&ixlib=rb-1.2.1&auto=format&fit=crop&w=800&q=60",
                },
            };

            context.Products.AddRange(products);
        }
    }
}
