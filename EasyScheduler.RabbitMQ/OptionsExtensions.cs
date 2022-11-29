using System;
using EasyScheduler;
using EasyScheduler.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OptionsExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static SchedulerOptions UseRabbitMQ(this SchedulerOptions options, string hostName)
        {
            return options.UseRabbitMQ(opt => { opt.HostName = hostName; });
        }

        // ReSharper disable once InconsistentNaming
        public static SchedulerOptions UseRabbitMQ(this SchedulerOptions options, Action<RabbitMQOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new RabbitMQOptionsExtension(configure));

            return options;
        }

    }
}
