using System.Threading.Tasks;
using MarketoDataPurger.Gateways.Models;

namespace MarketoDataPurger.Gateways
{
    public interface IMarketoGateway
    {
        Task<DeleteOpportunityResponse> DeleteOpportunity(DeleteOpportunityRequest deleteOpportunityRequest);

        Task<DeleteOpportunityResponse> DeleteOpportunityRole(DeleteOpportunityRequest deleteOpportunityRequest);

        Task<MergeLeadsResponse> MergeLeads(int winningLead, int duplicateLead);
        Task<FindLeadResponse> FindLead(int marketoLeadId);

        string GetToken();
    }
}