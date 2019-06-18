using System;

namespace MarketoDataPurger.Gateways.Exceptions
{
    public class MarketoConcurrentAccessLimitException : Exception
    {
        public MarketoConcurrentAccessLimitException()
        {
        }

        public MarketoConcurrentAccessLimitException(string message)
            : base(message)
        {
        }

        public MarketoConcurrentAccessLimitException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}