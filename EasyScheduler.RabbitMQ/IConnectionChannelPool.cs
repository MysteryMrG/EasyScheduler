using RabbitMQ.Client;

namespace EasyScheduler.RabbitMQ
{
    public interface IConnectionChannelPool
    {
        string HostAddress { get; }

        string Exchange { get; }

        IConnection GetConnection();

        IModel Rent();

        bool Return(IModel context);
    }
}
