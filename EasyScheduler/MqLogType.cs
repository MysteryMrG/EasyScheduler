using System;
using System.Collections.Generic;
using System.Text;

namespace EasyScheduler
{
    public enum MqLogType
    {
        //RabbitMQ
        ConsumerCancelled,
        ConsumerRegistered,
        ConsumerUnregistered,
        ConsumerShutdown,

        //Kafka
        ConsumeError,
        ServerConnError,

        //AzureServiceBus
        ExceptionReceived
    }

    public class LogMessageEventArgs : EventArgs
    {
        public string[] Reason { get; set; }

        public string ReasonText { get; set; }

        public MqLogType LogType { get; set; }
    }
}
