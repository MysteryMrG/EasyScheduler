namespace EasyScheduler
{
    /// <summary>
    /// Encapsulates an error from the operate subsystem.
    /// </summary>
    public class OperateError
    {
        /// <summary>
        /// Gets or sets ths code for this error.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description for this error.
        /// </summary>
        public string Description { get; set; }
    }
}
