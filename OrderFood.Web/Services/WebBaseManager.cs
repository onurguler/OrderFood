using OrderFood.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFood.Web.Services
{
    public class WebBaseManager
    {
        public WebBaseManager(ApplicationBaseManager applicationBaseManager)
        {
            ApplicationBaseManager = applicationBaseManager;
        }

        public ApplicationBaseManager ApplicationBaseManager { get; }
    }
}
