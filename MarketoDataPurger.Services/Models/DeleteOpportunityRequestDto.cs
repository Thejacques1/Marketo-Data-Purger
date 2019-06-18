using System.Collections.Generic;

namespace MarketoDataPurger.Services.Models
{
    public class DeleteOpportunityRequestDto
    {
        public string DeleteBy { get; set; }
        public IEnumerable<DeleteOpportunityDto> Input { get; set; }
    }
}
