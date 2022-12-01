using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using RabbitMQ.Client;

namespace EasyScheduler.RabbitMQ
{
    public class ConnectionChannelPool : IConnectionChannelPool, IDisposable
    {
        private const int DefaultPoolSize = 15;
        private readonly Func<IConnection> _connectionActivator;
        private readonly ConcurrentQueue<IModel> _pool;
        private IConnection _connection;
        private static readonly object s_lock = new object();

        private int _count;
        private int _maxSize;

        public ConnectionChannelPool(RabbitMQOptions options)
        {
            _maxSize = DefaultPoolSize;
            _pool = new ConcurrentQueue<IModel>();
            _connectionActivator = CreateConnection(options);

            HostAddress = options.HostName + ":" + options.Port;

            Exchange = options.ExchangeName;
        }

        IModel IConnectionChannelPool.Rent()
        {
            lock (s_lock)
            {
                while (_count > _maxSize)
                {
                    Thread.SpinWait(1);
                }
                return Rent();
            }
        }

        bool IConnectionChannelPool.Return(IModel connection)
        {
            return Return(connection);
        }

        public string HostAddress { get; }

        public string Exchange { get; }

        public IConnection GetConnection()
        {
            if (_connection != null && _connection.IsOpen)
            {
                return _connection;
            }

            _connection = _connectionActivator();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            return _connection;
        }

        public void Dispose()
        {
            _maxSize = 0;

            while (_pool.TryDequeue(out var context))
            {
                context.Dispose();
            }
        }

        private static Func<IConnection> CreateConnection(RabbitMQOptions options)
        {
            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name.ToLower();
            
            var factory = new ConnectionFactory
            {
                UserName = options.UserName,
                Port = options.Port,
                Password = options.Password,
                VirtualHost = options.VirtualHost
            };

            if (options.HostName.Contains(","))
            {
                options.ConnectionFactoryOptions?.Invoke(factory);
                return () => factory.CreateConnection(
                    options.HostName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries), serviceName);
            }

            factory.HostName = options.HostName;
            options.ConnectionFactoryOptions?.Invoke(factory);
            return () => factory.CreateConnection(serviceName);
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        public virtual IModel Rent()
        {
            if (_pool.TryDequeue(out var model))
            {
                return model;
            }

            model = GetConnection().CreateModel();

            return model;
        }

        public virtual bool Return(IModel connection)
        {
            if (Interlocked.Increment(ref _count) <= _maxSize)
            {
                _pool.Enqueue(connection);

                return true;
            }

            Interlocked.Decrement(ref _count);

            Debug.Assert(_maxSize == 0 || _pool.Count <= _maxSize);

            return false;
        }
    }
}
