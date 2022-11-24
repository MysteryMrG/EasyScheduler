using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using EasyScheduler.Interface;
using Microsoft.Data.SqlClient;

namespace EasyScheduler.SqlServer
{
    public class SqlServerStorage : IStorage
    {
        private readonly IDbConnection _existingConnection = null;
        private readonly SqlServerOptions _options;

        public SqlServerStorage(SqlServerOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var sql = CreateDbTablesScript(_options.Schema);

            using (var connection = new SqlConnection(_options.ConnectionString))
            {
                await connection.ExecuteAsync(sql);
            }
        }

        /// <summary>
        /// Create Db Tables Script
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        protected virtual string CreateDbTablesScript(string schema)
        {
            var batchSql =
                $@"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{schema}')
BEGIN
	EXEC('CREATE SCHEMA [{schema}]')
END;

IF OBJECT_ID(N'[{schema}].[SchedulerJob]',N'U') IS NULL
BEGIN
CREATE TABLE [{schema}].[SchedulerJob](
	[Id] [bigint] NOT NULL,
	[JobId] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[JobType] [int] NOT NULL,
	[Interval] [int] NOT NULL,
	[Parameter] [nvarchar](1200) NULL,	
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime]  NULL,
	[InUse] [int] NOT NULL,
 CONSTRAINT [PK_{schema}.SchedulerJob] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
END;

"

                ;
            return batchSql;
        }

        internal T UseConnection<T>(Func<IDbConnection, T> func)
        {
            IDbConnection connection = null;

            try
            {
                connection = CreateAndOpenConnection();
                return func(connection);
            }
            finally
            {
                ReleaseConnection(connection);
            }
        }

        internal IDbConnection CreateAndOpenConnection()
        {
            var connection = _existingConnection ?? new SqlConnection(_options.ConnectionString);

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }

        internal bool IsExistingConnection(IDbConnection connection)
        {
            return connection != null && ReferenceEquals(connection, _existingConnection);
        }

        internal void ReleaseConnection(IDbConnection connection)
        {
            if (connection != null && !IsExistingConnection(connection))
            {
                connection.Dispose();
            }
        }
    }
}
