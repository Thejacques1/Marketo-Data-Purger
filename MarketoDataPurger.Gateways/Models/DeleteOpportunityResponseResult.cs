using System;
using System.Collections.Generic;

namespace MarketoDataPurger.Gateways.Models
{
    public class DeleteOpportunityResponseResult
    {
        public int id { get; set; }

        public int seq { get; set; }

        public Guid marketoGuid { get; set; }

        public string status { get; set; }

        public IEnumerable<DeleteOpportunityResponseResultReason> reasons { get; set; }
    }
}
