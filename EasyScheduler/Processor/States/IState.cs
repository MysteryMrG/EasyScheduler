using System;
using EasyScheduler.Models;

namespace EasyScheduler.Processor.States
{
    public interface IState
    {
        TimeSpan? ExpiresAfter { get; }

        string Name { get; }

        void Apply(PublishedMessage message, IStorageTransaction transaction);

        void Apply(ReceivedMessage message, IStorageTransaction transaction);
    }
}
