using MarketoDataPurger.Gateways.Models;
using MarketoDataPurger.Services.Models;

namespace MarketoDataPurger.Services.Mappers
{
    public static class DeleteOpportunityMapper
    {
        public static DeleteOpportunity Map(this DeleteOpportunityDto deleteOpportunityDto)
        {
            return new DeleteOpportunity()
            {
                marketoguid = deleteOpportunityDto.Id
            };
        }
    }
}
