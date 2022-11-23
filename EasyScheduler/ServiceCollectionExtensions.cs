using System;
using EasyScheduler;

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

        }
    }
}