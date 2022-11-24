using System;
using System.Threading.Tasks;
using Dapper;
using EasyScheduler.Processor;
using Microsoft.Data.SqlClient;

namespace EasyScheduler.SqlServer
{
    public class SqlServerCollectProcessor : ICollectProcessor
    {
        private const int MaxBatch = 1000;

        private static readonly string[] Tables =
        {
            "Published", "Received"
        };

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
        private readonly SqlServerOptions _options;
        private readonly TimeSpan _waitingInterval = TimeSpan.FromHours(1);

        public SqlServerCollectProcessor(SqlServerOptions sqlServerOptions)
        {
            _options = sqlServerOptions;
        }
        public async Task ProcessAsync(ProcessingContext context)
        {
            foreach (var table in Tables)
            {
                int removedCount;
                do
                {
                    using (var connection = new SqlConnection(_options.ConnectionString))
                    {
                        removedCount = connection.Execute($@"
                        Insert Into [{_options.Schema}].[{table}His] Select  TOP (@count) * FROM [{_options.Schema}].[{table}] WITH (readpast)  WHERE ExpiresAt < @now;  
                        DELETE TOP (@count)
                        FROM [{_options.Schema}].[{table}] WITH (readpast)
                        WHERE ExpiresAt < @now;", new { now = DateTime.Now, count = MaxBatch });
                    }
                    if (removedCount == 0)
                    {
                        continue;
                    }
                    await context.WaitAsync(_delay);
                    context.ThrowIfStopping();
                } while (removedCount != 0);
            }

            await context.WaitAsync(_waitingInterval);
        }
    }
}
