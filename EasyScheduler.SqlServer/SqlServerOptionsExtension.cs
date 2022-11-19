using EasyScheduler.Core.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

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
            throw new NotImplementedException();
        }
    }
}
