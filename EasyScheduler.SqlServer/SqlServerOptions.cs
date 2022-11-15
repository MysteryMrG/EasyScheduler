namespace EasyScheduler.SqlServer
{
    public class SqlServerOptions : EfOptions
    {
        /// <summary>
        /// Gets or sets the database's connection string that will be used to store database entities.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
