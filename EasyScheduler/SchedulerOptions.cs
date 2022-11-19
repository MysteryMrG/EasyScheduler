using System;
using System.Collections.Generic;

namespace EasyScheduler
{
    public class SchedulerOptions
    {
        /// <summary>
        /// Default Completed job expiration time span, in seconds.
        /// </summary>
        public const int DefaultCompletedJobExpirationAfter = 24 * 3600;

        /// <summary>
        /// Failed job retry waiting interval.
        /// </summary>
        public const int DefaultFailedJobWaitingInterval = 60;

        /// <summary>
        /// Failed job retry count.
        /// </summary>
        public const int DefaultFailedRetryCount = 50;

        /// <summary>
        /// Failed job retry, defalut false
        /// </summary>
        public const bool DefalutFailedRetry = false;

        public SchedulerOptions()
        {

        }

        internal IList<IOptionsExtension> Extensions { get; }

        /// <summary>
        ///  Failed job retry, defalut false
        /// </summary>
        public bool FailedRetry { get; set; }

        /// <summary>
        /// After time span of due, then the message will be deleted at due time.
        /// Default is 24*3600 seconds.
        /// </summary>
        public int CompletedJobExpiredAfter { get; set; }

        /// <summary>
        /// Failed job polling delay time.
        /// Default is 60 seconds.
        /// </summary>
        public int FailedRetryInterval { get; set; }

        /// <summary>
        /// We’ll invoke this call-back with job type,name,content when retry failed  equals <see cref="FailedRetryCount"/> times.
        /// </summary>
        public Action<string, string> FailedThresholdCallback { get; set; }

        /// <summary>
        /// The number of job retries, the retry will stop when the threshold is reached.
        /// Default is 50 times.
        /// </summary>
        public int FailedRetryCount { get; set; }

        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(IOptionsExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extensions.Add(extension);
        }
    }
}