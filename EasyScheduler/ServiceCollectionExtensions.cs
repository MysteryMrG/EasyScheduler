using System;
using System.Threading;
using EasyScheduler;
using EasyScheduler.Processor;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///Add Scheduler Injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="actions"></param>
        public static void AddScheduler(this IServiceCollection services, Action<SchedulerOptions> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, ProcessingServer>());

            var options = new SchedulerOptions();
            actions(options);
            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }
            services.AddSingleton(options);


            //Bootstrap
            services.BuildServiceProvider().GetService<IBootstrapper>().BootstrapAsync(new CancellationToken());

        }
    }
}