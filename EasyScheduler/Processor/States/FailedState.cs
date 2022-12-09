using System;
using EasyScheduler.Models;

namespace EasyScheduler.Processor.States
{
    public class FailedState : IState
    {
        public const string StateName = "Failed";

        public TimeSpan? ExpiresAfter => TimeSpan.FromDays(15);

        public string Name => StateName;

        public void Apply(PublishedMessage message, IStorageTransaction transaction)
        {
        }

        public void Apply(ReceivedMessage message, IStorageTransaction transaction)
        {
        }
    }
}
