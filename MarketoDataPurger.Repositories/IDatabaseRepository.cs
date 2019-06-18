using System.Collections.Generic;
using System.Threading.Tasks;
using MarketoDataPurger.Repositories.Models;

namespace MarketoDataPurger.Repositories
{
    public interface IDatabaseRepository
    {
        Task<IEnumerable<MarketoOpportunity>> GetMarketoOpportunities();
    }
}