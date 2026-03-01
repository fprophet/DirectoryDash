using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Factories
{
    internal class ItemFactory
    {
        private IServiceProvider _serviceProvider;

        public ItemFactory(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>() => _serviceProvider.GetRequiredService<T>();
    }
}
