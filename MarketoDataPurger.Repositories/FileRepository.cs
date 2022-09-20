using System.Collections.Generic;
using MarketoDataPurger.Repositories.Models;
using System.IO;
using System.Linq;
using System.Text;

namespace MarketoDataPurger.Repositories
{
    public class FileRepository : IFileRepository
    {
        public IEnumerable<MarketoLead> ReadDuplicateLeadsFromCSV()
        {
            List<MarketoLead> readLeads = File.ReadAllLines("C:\\Users\\jacques.marais\\Documents\\Possible_Duplicates.csv")
                .Skip(1)
                .Select(v => MarketoLead.Map(v))
                .ToList();

            return readLeads;
        }

        public IEnumerable<MarketoScvLeadId> ReadStaleLeadsFromCsv()
        {
            //List<MarketoScvLeadId> readLeads = File.ReadAllLines("C:\\Users\\Jacques.Marais\\Desktop\\PossibleStaleLeadList.csv")
            //    //.Skip(1)
            //    .Select(v => MarketoScvLeadId.Map(v))
            //    .ToList();

            List<MarketoScvLeadId> readLeads = new List<MarketoScvLeadId>();

            IEnumerable<string> lines = File.ReadLines("C:\\Users\\jacques.marais\\Documents\\PossibleStaleLeadList.csv");

            foreach (string line in lines)
            {
                MarketoScvLeadId readLead = MarketoScvLeadId.Map(line);
                readLeads.Add(readLead);
            }

            return readLeads;
        }

        public void WriteStaleLeadsToCsv(IEnumerable<MarketoScvLeadId> staleLeads)
        {
            var csv = new StringBuilder();

            foreach (MarketoScvLeadId marketoScvLeadId in staleLeads)
            {

                var newLine = string.Format("{0},{1}", marketoScvLeadId.ScvDatabaseId, marketoScvLeadId.MarketoLeadId);
                csv.AppendLine(newLine);
            }

            //after your loop
            File.WriteAllText("C:\\Users\\Jacques.Marais\\Desktop\\Stale_Leads.csv", csv.ToString());
        }
    }
}