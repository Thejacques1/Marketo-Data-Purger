using System;

namespace MarketoDataPurger.Repositories.Models
{
    public class MarketoLead
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Company { get; set; }
        public string EmailAddress { get; set; }
        public string DateOfBirth { get; set; }
        public string State { get; set; }
        public string Suburb { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Salutation { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string ScvDatabaseIdLead { get; set; }

        public static MarketoLead Map(string csvLine)
        {
            string[] values = csvLine.Split(',');
            MarketoLead marketoLead = new MarketoLead();
            marketoLead.Id = Convert.ToInt32(values[0]);
            marketoLead.LastName = Convert.ToString(values[1]);
            marketoLead.FirstName = Convert.ToString(values[2]);
            marketoLead.Company = Convert.ToString(values[3]);
            marketoLead.EmailAddress = Convert.ToString(values[4]);
            marketoLead.DateOfBirth = Convert.ToString(values[5]);
            marketoLead.State = Convert.ToString(values[6]);
            marketoLead.Suburb = Convert.ToString(values[7]);
            marketoLead.Address = Convert.ToString(values[8]);
            marketoLead.City = Convert.ToString(values[9]);
            marketoLead.PostalCode = Convert.ToString(values[10]);
            marketoLead.Salutation = Convert.ToString(values[11]);
            marketoLead.MobilePhoneNumber = Convert.ToString(values[12]);
            marketoLead.ScvDatabaseIdLead = Convert.ToString(values[13]);

            return marketoLead;
        }
    }
}