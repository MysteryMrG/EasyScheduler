using EasyScheduler.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EasyScheduler.Abstractions;
using EasyScheduler.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EasyScheduler.RabbitMQ
{
    internal sealed class RabbitMQConsumerClient : IConsumerClient
    {
        private readonly IConnectionChannelPool _connectionChannelPool;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private IModel _channel;
        private readonly IContentSerializer _serializer;

        private IConnection _connection;
        private ulong _deliveryTag;

        public RabbitMQConsumerClient(string queueName,
            IConnectionChannelPool connectionChannelPool,
            RabbitMQOptions options, IContentSerializer serializer)
        {
            _queueName = queueName;
            _connectionChannelPool = connectionChannelPool;
            _rabbitMQOptions = options;
            _exchangeName = connectionChannelPool.Exchange;
            _serializer = serializer;
            InitClient();
        }

        public event EventHandler<MessageContext> OnMessageReceived;

        public event EventHandler<LogMessageEventArgs> OnLog;

        public string ServersAddress => _rabbitMQOptions.HostName;

        //绑定队列
        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            foreach (var topic in topics)
            {
                _channel.QueueBind(_queueName, _exchangeName, topic);
            }
        }

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnConsumerReceived;
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(_queueName, false, consumer);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.WaitHandle.WaitOne(timeout);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public void Commit()
        {
            //确认消息
            _channel.BasicAck(_deliveryTag, false);
        }

        public void Reject()
        {
            //重新入队列
            _channel.BasicReject(_deliveryTag, true);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        private void InitClient()
        {
            _connection = _connectionChannelPool.GetConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                _exchangeName,
                RabbitMQOptions.ExchangeType,
                true);

            var arguments = new Dictionary<string, object>
            {
                {"x-message-ttl", _rabbitMQOptions.QueueMessageExpires}
            };
            _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
        }

        #region events

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
            var args = new LogMessageEventArgs
            {
                LogType = MqLogType.ConsumerCancelled,
                Reason = e.ConsumerTags
            };
            OnLog?.Invoke(sender, args);
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            var args = new LogMessageEventArgs
            {
                LogType = MqLogType.ConsumerUnregistered,
                Reason = e.ConsumerTags
            };
            OnLog?.Invoke(sender, args);
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            var args = new LogMessageEventArgs
            {
                LogType = MqLogType.ConsumerRegistered,
                Reason = e.ConsumerTags
            };
            OnLog?.Invoke(sender, args);
        }

        private void OnConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var headers = new Dictionary<string, string>();

            if (e.BasicProperties.Headers != null)
            {
                foreach (var header in e.BasicProperties.Headers)
                {
                    if (header.Value is byte[] val)
                    {
                        headers.Add(header.Key, Encoding.UTF8.GetString(val));
                    }
                    else
                    {
                        headers.Add(header.Key, header.Value?.ToString());
                    }
                }
            }
            headers.Add(Models.Headers.Group, _queueName);


            var bodyStr = Encoding.UTF8.GetString(e.Body.ToArray());
            var content = new RQMessage { Headers = headers };
            try
            {
                var data = _serializer.DeSerialize<dynamic>(bodyStr);
                content.Value = data;
            }
            catch (Exception)
            {
                content.Value = bodyStr;
            }
            _deliveryTag = e.DeliveryTag;
            var message = new MessageContext
            {
                Group = _queueName,
                Name = e.RoutingKey,
                Content = _serializer.Serialize(content),
            };
            OnMessageReceived?.Invoke(sender, message);
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            var args = new LogMessageEventArgs
            {
                LogType = MqLogType.ConsumerShutdown,
                ReasonText = e.ReplyText
            };
            OnLog?.Invoke(sender, args);
        }

        #endregion
    }
}
