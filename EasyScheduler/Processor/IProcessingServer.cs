using System;

namespace EasyScheduler.Processor
{
    /// <summary>
    /// A process thread abstract of message process.
    /// </summary>
    public interface IProcessingServer : IDisposable
    {
        void Pulse();

        void Start();
    }
}
