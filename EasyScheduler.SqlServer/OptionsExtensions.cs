using EasyScheduler;
using EasyScheduler.SqlServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OptionsExtensions
    {
        public static SchedulerOptions UseSqlServer(this SchedulerOptions options, string connectionString)
        {
            return options.UseSqlServer(opt => { opt.ConnectionString = connectionString; });
        }

        public static SchedulerOptions UseSqlServer(this SchedulerOptions options, Action<SqlServerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new SqlServerOptionsExtension(configure));

            return options;
        }

    }
}
