using System.Linq;
using MarketoDataPurger.Gateways.Models;
using MarketoDataPurger.Services.Models;

namespace MarketoDataPurger.Services.Mappers
{
    public static class DeleteOpportunityRequestMapper
    {
        public static DeleteOpportunityRequest Map(this DeleteOpportunityRequestDto deleteOpportunityRequestDto)
        {
            return new DeleteOpportunityRequest()
            {
                deleteBy = deleteOpportunityRequestDto.DeleteBy,
                input = deleteOpportunityRequestDto.Input.Select(d => d.Map())
            };
        }
    }
}
