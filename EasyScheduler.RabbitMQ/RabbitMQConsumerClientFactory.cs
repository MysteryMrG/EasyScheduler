using System;
using EasyScheduler.Abstractions;
using EasyScheduler.CustomerException;
using EasyScheduler.Interface;

namespace EasyScheduler.RabbitMQ
{
    internal sealed class RabbitMQConsumerClientFactory : IConsumerClientFactory
    {
        private readonly IConnectionChannelPool _connectionChannelPool;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly IContentSerializer _serializer;

        public RabbitMQConsumerClientFactory(RabbitMQOptions rabbitMQOptions, IConnectionChannelPool channelPool, IContentSerializer serializer)
        {
            _rabbitMQOptions = rabbitMQOptions;
            _connectionChannelPool = channelPool;
            _serializer = serializer;
        }

        public IConsumerClient Create(string groupId)
        {
            try
            {
                return new RabbitMQConsumerClient(groupId, _connectionChannelPool, _rabbitMQOptions, _serializer);
            }
            catch (Exception e)
            {
                throw new BrokerConnectionException(e);
            }
        }
    }
}
