using EasyScheduler.Core.Ioc;
using System;
using EasyScheduler;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        internal static IServiceCollection ServiceCollection;
        public static void AddScheduler(this IServiceCollection services, Action<SchedulerOptions> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            ServiceCollection = services;

            //Options and extension service
            var options = new SchedulerOptions();
            actions(new SchedulerOptions());
            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }

        }
    }
}
