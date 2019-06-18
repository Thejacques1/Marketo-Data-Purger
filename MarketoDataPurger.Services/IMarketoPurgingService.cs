using System.Threading.Tasks;
using MarketoDataPurger.Services.Models;

namespace MarketoDataPurger.Services
{
    public interface IMarketoPurgingService
    {
        Task Purge();

        bool TestMarketoConnection();

        Task<DatabaseTestDto> TestDatabaseConnection();
    }
}