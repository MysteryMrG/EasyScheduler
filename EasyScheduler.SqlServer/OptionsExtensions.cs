using EasyScheduler;
using EasyScheduler.SqlServer;
using System;
using System.Data.Entity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class OptionsExtensions
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

        public static SchedulerOptions UseEntityFramework<TContext>(this SchedulerOptions options)
            where TContext : DbContext
        {
            return options.UseEntityFramework<TContext>(opt => { });
        }

        public static SchedulerOptions UseEntityFramework<TContext>(this SchedulerOptions options, Action<EfOptions> configure)
            where TContext : DbContext
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            options.RegisterExtension(new SqlServerOptionsExtension(x =>
            {
                configure(x);
                x.DbContextType = typeof(TContext);
            }));

            return options;
        }
    }
}
