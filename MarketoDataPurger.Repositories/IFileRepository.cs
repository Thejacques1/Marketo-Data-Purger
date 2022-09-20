using System.Collections.Generic;
using MarketoDataPurger.Repositories.Models;

namespace MarketoDataPurger.Repositories
{
    public interface IFileRepository
    {
        IEnumerable<MarketoLead> ReadDuplicateLeadsFromCSV();
        IEnumerable<MarketoScvLeadId> ReadStaleLeadsFromCsv();
        void WriteStaleLeadsToCsv(IEnumerable<MarketoScvLeadId> staleLeads);
    }
}