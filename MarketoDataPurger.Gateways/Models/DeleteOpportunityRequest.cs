using System.Collections.Generic;

namespace MarketoDataPurger.Gateways.Models
{
    public class DeleteOpportunityRequest
    {
        public string deleteBy { get; set; }
        public IEnumerable<DeleteOpportunity> input { get; set; }
    }
}
