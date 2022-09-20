using System.Collections.Generic;

namespace MarketoDataPurger.Gateways.Models
{
    public class FindLeadResponse
    {
        public IEnumerable<GenericErrorResponse> Errors { get; set; }
        public bool MoreResults { get; set; }
        public string NextPageToken { get; set; }
        public string RequestId { get; set; }
        public IEnumerable<FindLeadResponseResult> Result { get; set; }
        public bool Success { get; set; }
        public IEnumerable<GenericErrorResponse> Warnings { get; set; }
    }
}