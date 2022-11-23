using System.Threading;
using System.Threading.Tasks;

namespace EasyScheduler
{
    public interface IBootstrapper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        Task BootstrapAsync(CancellationToken stoppingToken);
    }
}
