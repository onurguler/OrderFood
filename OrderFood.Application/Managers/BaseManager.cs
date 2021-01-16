using OrderFood.Infrastructure.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFood.Application.Managers
{
    public class BaseManager
    {
        public BaseManager(IServiceProvider provider)
        {
            ServiceProvider = provider;
            UnitOfWork = (IUnitOfWork)provider.GetService(typeof(IUnitOfWork));
        }

        public IServiceProvider ServiceProvider { get; }
        public IUnitOfWork UnitOfWork { get; set; }
    }
}
