using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Interface;
using EasyScheduler.Processor;

namespace EasyScheduler.SqlServer
{
    public class SqlServerBootstrapper : IBootstrapper
    {

        private IStorage Storage { get; }
        private IEnumerable<IProcessingServer> Processors { get; }

        public SqlServerBootstrapper(
            IStorage storage,
            IEnumerable<IProcessingServer> processors)
        {
            Storage = storage;
            Processors = processors;
        }


        public async Task BootstrapAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Storage.InitializeAsync(stoppingToken);
            }
            catch (Exception e)
            {
                // ignored
            }

            stoppingToken.Register(() =>
            {

                foreach (var item in Processors)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            });

            await BootstrapCoreAsync();
        }
        protected virtual Task BootstrapCoreAsync()
        {
            foreach (var item in Processors)
            {
                try
                {
                    item.Start();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return Task.FromResult(true);
        }

    }
}
