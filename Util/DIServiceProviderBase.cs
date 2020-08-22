using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;


namespace Utilities.Util
{
    public abstract class DIServiceProviderBase
    {
        protected readonly IServiceProvider ServiceProvider;

        protected DIServiceProviderBase()
        {
            var serviceCollection = new ServiceCollection();
            AddServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        protected abstract void AddServices(ServiceCollection serviceCollection);
    }
}