using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasyScheduler.RabbitMQ
{
    internal sealed class RabbitMQOptionsExtension : IOptionsExtension
    {
        private readonly Action<RabbitMQOptions> _configure;

        public RabbitMQOptionsExtension(Action<RabbitMQOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            //services.AddSingleton<CapMessageQueueMakerService>();

            var options = new RabbitMQOptions();
            _configure?.Invoke(options);
            services.AddSingleton(options);

            //services.AddSingleton<IConsumerClientFactory, RabbitMQConsumerClientFactory>();
            //services.AddSingleton<IConnectionChannelPool, ConnectionChannelPool>();
            //services.AddSingleton<IPublishExecutor, RabbitMQPublishMessageSender>();
            //services.AddSingleton<IPublishMessageSender, RabbitMQPublishMessageSender>();
        }
    }
}
