using System.Threading.Tasks;
using EasyScheduler.Models;

namespace EasyScheduler
{
    public interface IPublishMessageSender
    {
        Task<OperateResult> SendAsync(PublishedMessage message);
    }
}
