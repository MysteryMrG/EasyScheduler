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

IF OBJECT_ID(N'[{schema}].[Received]',N'U') IS NULL
BEGIN
CREATE TABLE [{schema}].[Received](
	[Id] [bigint] NOT NULL,
    [Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Group] [nvarchar](200) NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
    [ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_{schema}.Received] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END;

IF OBJECT_ID(N'[{schema}].[Published]',N'U') IS NULL
BEGIN
CREATE TABLE [{schema}].[Published](
	[Id] [bigint] NOT NULL,
    [Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
    [ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_{schema}.Published] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END;


IF OBJECT_ID(N'[{schema}].[ReceivedHis]',N'U') IS NULL
BEGIN
CREATE TABLE [{schema}].[ReceivedHis](
	[Id] [bigint] NOT NULL,
    [Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Group] [nvarchar](200) NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
    [ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_{schema}.ReceivedHis] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END;

IF OBJECT_ID(N'[{schema}].[PublishedHis]',N'U') IS NULL
BEGIN
CREATE TABLE [{schema}].[PublishedHis](
	[Id] [bigint] NOT NULL,
    [Version] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Retries] [int] NOT NULL,
	[Added] [datetime2](7) NOT NULL,
    [ExpiresAt] [datetime2](7) NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_{schema}.PublishedHis] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END;"

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
