using System;

namespace MarketoDataPurger.Repositories.Models
{
    public class MarketoScvLeadId

    {
        public string ScvDatabaseId { get; set; }
        public int MarketoLeadId { get; set; }


        public static MarketoScvLeadId Map(string csvLine)
        {
            string[] values = csvLine.Split(',');
            MarketoScvLeadId marketoLead = new MarketoScvLeadId();
            marketoLead.ScvDatabaseId = Convert.ToString(values[0]);

            int marketoLeadId;
            bool marketoLeadIdParsed = Int32.TryParse(values[1], out marketoLeadId);
            if (marketoLeadIdParsed)
            {
                marketoLead.MarketoLeadId = Convert.ToInt32(values[1]);
            }
            else
            {
                throw new Exception($"Unxpected record found in file with ScvDatabaseId {marketoLead.ScvDatabaseId}");
            }

            return marketoLead;
        }
    }
}