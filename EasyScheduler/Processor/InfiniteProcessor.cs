using System;
using System.Threading.Tasks;

namespace EasyScheduler.Processor
{
    public class InfiniteProcessor : IProcessor
    {
        private readonly IProcessor _inner;
        public InfiniteProcessor(IProcessor inner)
        {
            _inner = inner;
        }

        public async Task ProcessAsync(ProcessingContext context)
        {
            while (!context.IsStopping)
            {
                try
                {
                    await _inner.ProcessAsync(context);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }
    }
}
