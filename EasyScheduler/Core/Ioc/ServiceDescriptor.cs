using System;

namespace EasyScheduler.Core.Ioc
{
    public class ServiceDescriptor
    {
        public ServiceDescriptor(Type implementationType, Type serviceType)
        {
            ImplementationType = implementationType;
            ServiceType = serviceType;
        }

        public Type ImplementationType { get; set; }

        public Type ServiceType { get; set; }

        public static ServiceDescriptor Singleton<TService, TImplementation>()
        {
            var instance = new ServiceDescriptor(typeof(TImplementation), typeof(TService));
            return instance;
        }

    }
}
