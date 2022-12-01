using System;
using System.Collections.Generic;

namespace EasyScheduler.Models
{
    /// <summary>
    /// 消息内容
    /// </summary>
    public class RQMessage
    {
        public RQMessage()
        { }
        public RQMessage(IDictionary<string, string> headers, object value)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Value = value;
        }
        public IDictionary<string, string> Headers { get; set; }
        public object Value { get; set; }
    }
}
