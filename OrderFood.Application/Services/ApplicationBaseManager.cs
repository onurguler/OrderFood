using OrderFood.Application.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFood.Application.Services
{
    public class ApplicationBaseManager
    {
        public ApplicationBaseManager(ProductManager productManager)
        {
            ProductManager = productManager;
        }

        public ProductManager ProductManager { get; set; }
    }
}
