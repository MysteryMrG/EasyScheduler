using System;

namespace EasyScheduler.Core.Ioc
{
    public static partial class ServiceCollectionExtensions
    {
        internal static IServiceCollection ServiceCollection;

        /// <summary>
        /// For Framework Injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="actions"></param>
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
