using System.Collections.Generic;

namespace MarketoDataPurger.Gateways.Models
{
    public class DeleteOpportunityResponse
    {
        public string RequestId { get; set; }

        public bool Success { get; set; }

        public IEnumerable<DeleteOpportunityResponseResult> Result { get; set; }

        public IEnumerable<DeleteOpportunityResponseError> Errors { get; set; }
    }
}
