namespace EasyScheduler.CustomerException
{
    public class BrokerConnectionException : System.Exception
    {
        public BrokerConnectionException(System.Exception innerException)
            : base("Broker Unreachable", innerException)
        {

        }
    }
}
