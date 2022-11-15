using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyScheduler.Processor
{
    public interface IProcessor
    {
        Task ProcessAsync(ProcessingContext context);
    }
}
