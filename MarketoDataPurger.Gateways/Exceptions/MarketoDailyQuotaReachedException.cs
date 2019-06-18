using System;

namespace MarketoDataPurger.Gateways.Exceptions
{
    public class MarketoDailyQuotaReachedException : Exception
    {
        public MarketoDailyQuotaReachedException()
        {
        }

        public MarketoDailyQuotaReachedException(string message)
            : base(message)
        {
        }

        public MarketoDailyQuotaReachedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}