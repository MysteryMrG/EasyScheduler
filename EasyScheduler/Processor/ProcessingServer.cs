using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EasyScheduler.Processor
{
    public class ProcessingServer : IProcessingServer
    {
        private readonly CancellationTokenSource _cts;
        private readonly IServiceProvider _provider;

        private Task _compositeTask;
        private ProcessingContext _context;
        private bool _disposed;

        public ProcessingServer(IServiceProvider provider)
        {
            _provider = provider;
            _cts = new CancellationTokenSource();
        }

        public void Start()
        {
            _context = new ProcessingContext(_provider, _cts.Token);
            var processorTasks = GetProcessors()
                .Select(Infinite)
                .Select(p => p.ProcessAsync(_context));
            _compositeTask = Task.WhenAll(processorTasks);
        }

        public void Pulse()
        {
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            try
            {
                _disposed = true;

                _cts.Cancel();

                _compositeTask?.Wait((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private IProcessor Infinite(IProcessor inner)
        {
            return new InfiniteProcessor(inner);
        }

        private IProcessor[] GetProcessors()
        {
            var returnedProcessors = new List<IProcessor>
            {
                _provider.GetRequiredService<ICollectProcessor>()
            };
            return returnedProcessors.ToArray();
        }
    }
}
