using Autofac;
using System;

namespace EasyScheduler.Core.Ioc
{
    public interface IServiceScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; set; }

    }

    public class ServiceScope : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; set; }

        private ILifetimeScope _scope;
        public ServiceScope(ILifetimeScope scope)
        {

            _scope = scope;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
