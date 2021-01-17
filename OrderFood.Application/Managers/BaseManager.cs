using Microsoft.AspNetCore.Hosting;
using OrderFood.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFood.Application.Managers
{
    public class BaseManager
    {
        public BaseManager(IServiceProvider provider, IHostingEnvironment hostingEnvironment)
        {
            ServiceProvider = provider;
            UnitOfWork = (IUnitOfWork)provider.GetService(typeof(IUnitOfWork));
            HostingEnvironment = hostingEnvironment;
        }

        public IServiceProvider ServiceProvider { get; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }
    }
}
