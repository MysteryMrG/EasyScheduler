using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyScheduler
{
    /// <summary>
    /// publish message excutor. The excutor sends the message to the message queue
    /// </summary>
    public interface IPublishExecutor
    {
        /// <summary>
        /// publish message to message queue.
        /// </summary>
        /// <param name="keyName">The message topic name.</param>
        /// <param name="content">The message content.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        Task<OperateResult> PublishAsync(string keyName, string content, IDictionary<string, object> headers);
    }
}
