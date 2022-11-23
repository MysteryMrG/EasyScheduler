namespace EasyScheduler.SqlServer
{
    public class SqlServerOptions
    {
        public const string DefaultSchema = "Es";

        /// <summary>
        /// Gets or sets the schema to use when creating database objects.
        /// </summary>
        public string Schema { get; set; } = DefaultSchema;

        /// <summary>
        /// Gets or sets the database's connection string that will be used to store database entities.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
