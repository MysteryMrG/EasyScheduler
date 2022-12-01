﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace EasyScheduler.Interface
{
    /// <summary>
    /// Message queue consumer client
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        string ServersAddress { get; }

        /// <summary>
        /// Subscribe to a set of topics to the message queue
        /// </summary>
        /// <param name="topics"></param>
        void Subscribe(IEnumerable<string> topics);

        /// <summary>
        /// Start listening
        /// </summary>
        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        void Commit();

        /// <summary>
        /// Reject message and resumption
        /// </summary>
        void Reject();

        event EventHandler<MessageContext> OnMessageReceived;

        event EventHandler<LogMessageEventArgs> OnLog;
    }
}
