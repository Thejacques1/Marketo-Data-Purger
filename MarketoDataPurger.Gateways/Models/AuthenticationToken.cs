using System;

namespace MarketoDataPurger.Gateways.Models
{
    public class AuthenticationToken
    {
        public string AccessToken { get; set; }

        public DateTime Expires { get; set; }

        public bool HasExpired()
        {
            return DateTime.Now >= Expires;
        }

        public bool HasToken()
        {
            return !string.IsNullOrEmpty(AccessToken);
        }
    }
}
