using System;

namespace MarketoDataPurger.Gateways.Exceptions
{
    public class MarketoMaxRateLimitException : Exception
    {
        public MarketoMaxRateLimitException()
        {
        }

        public MarketoMaxRateLimitException(string message)
            : base(message)
        {
        }

        public MarketoMaxRateLimitException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}