using System;
using EasyScheduler.Models;

namespace EasyScheduler.Processor.States
{
    public class SucceededState : IState
    {
        public const string StateName = "Succeeded";

        public SucceededState()
        {
            ExpiresAfter = TimeSpan.FromHours(1);
        }

        public SucceededState(int expireAfterSeconds)
        {
            ExpiresAfter = TimeSpan.FromSeconds(expireAfterSeconds);
        }

        public TimeSpan? ExpiresAfter { get; }

        public string Name => StateName;

        public void Apply(PublishedMessage message, IStorageTransaction transaction)
        {
        }

        public void Apply(ReceivedMessage message, IStorageTransaction transaction)
        {
        }
    }
}
