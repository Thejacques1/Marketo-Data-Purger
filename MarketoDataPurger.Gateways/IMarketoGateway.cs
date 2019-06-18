using System.Threading.Tasks;
using MarketoDataPurger.Gateways.Models;

namespace MarketoDataPurger.Gateways
{
    public interface IMarketoGateway
    {
        Task<DeleteOpportunityResponse> DeleteOpportunity(DeleteOpportunityRequest deleteOpportunityRequest);

        Task<DeleteOpportunityResponse> DeleteOpportunityRole(DeleteOpportunityRequest deleteOpportunityRequest);

        string GetToken();
    }
}