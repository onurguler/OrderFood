using OrderFood.Application.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFood.Application.Services
{
    public class ApplicationBaseManager
    {
        public ApplicationBaseManager(ProductManager productManager, CategoryManager categoryManager)
        {
            ProductManager = productManager;
            CategoryManager = categoryManager;
        }

        public ProductManager ProductManager { get; }
        public CategoryManager CategoryManager { get; }
    }
}
