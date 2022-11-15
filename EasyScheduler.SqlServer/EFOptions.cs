using System;

namespace EasyScheduler.SqlServer
{
    public class EfOptions
    {
        public const string DefaultSchema = "Es";

        /// <summary>
        /// Gets or sets the schema to use when creating database objects.
        /// </summary>
        public string Schema { get; set; } = DefaultSchema;

        /// <summary>
        /// EF dbcontext type.
        /// </summary>
        internal Type DbContextType { get; set; }
    }
}
