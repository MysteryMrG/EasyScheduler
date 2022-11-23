using System;
using EasyScheduler.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace EasyScheduler.SqlServer
{
    internal class SqlServerOptionsExtension : IOptionsExtension
    {
        private readonly Action<SqlServerOptions> _configure;

        public SqlServerOptionsExtension(Action<SqlServerOptions> configure)
        {
            _configure = configure;
        }
        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IStorage, SqlServerStorage>();

            //Startup and Middleware
            services.AddTransient<IBootstrapper, SqlServerBootstrapper>();

            AddSqlServerOptions(services);
        }
        private void AddSqlServerOptions(IServiceCollection services)
        {
            var sqlServerOptions = new SqlServerOptions();

            _configure(sqlServerOptions);

            services.AddSingleton(sqlServerOptions);
        }
    }
}
