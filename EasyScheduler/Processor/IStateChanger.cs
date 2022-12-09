using EasyScheduler.Models;
using EasyScheduler.Processor.States;

namespace EasyScheduler.Processor
{
    public interface IStateChanger
    {
        void ChangeState(PublishedMessage message, IState state, IStorageTransaction transaction);

        void ChangeState(ReceivedMessage message, IState state, IStorageTransaction transaction);
    }
}
