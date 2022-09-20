using System.Threading.Tasks;
using MarketoDataPurger.Services.Models;

namespace MarketoDataPurger.Services
{
    public interface IMarketoPurgingService
    {
        Task Purge();

        Task DeduplicateLeads();
        Task FindStaleLeads();

        bool TestMarketoConnection();

        Task<DatabaseTestDto> TestDatabaseConnection();
    }
}