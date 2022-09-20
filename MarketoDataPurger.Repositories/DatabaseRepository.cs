using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MarketoDataPurger.Repositories.Models;

namespace MarketoDataPurger.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly string _connectionString;
        public DatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public async Task<IEnumerable<MarketoOpportunity>> GetMarketoOpportunities()
        {
            //List<MarketoOpportunity> marketoOpportunities;

            //using (IDbConnection con = Connection)
            //{
            //    con.Open();

            //    string sqlQuery = "SELECT [MarketoOpportunityId], [MarketoOpportunityRoleId] FROM [schema].[Table] (nolock)";

            //    var result = await con.QueryAsync<MarketoOpportunity>(sqlQuery);
            //    marketoOpportunities = result.ToList();
            //}

            //return marketoOpportunities;

            ////Examples for testing:
            //MarketoOpportunity testMarketoOpportunity = new MarketoOpportunity()
            //{
            //    MarketoOpportunityId = new Guid("CC3D965E-4A58-4C99-B9EE-582643CF1EE0"),
            //    MarketoOpportunityRoleId = new Guid("97DF8D0B-AEFC-4555-A394-F105832F1AB9")
            //};

            //MarketoOpportunity testMarketoOpportunity2 = new MarketoOpportunity()
            //{
            //    MarketoOpportunityId = new Guid("03B3FB42-CB15-4990-AE48-E957B27C866A"),
            //    MarketoOpportunityRoleId = new Guid("0E30A432-DFCB-4C93-AF4B-3B957D330ED2")
            //};

            //return new List<MarketoOpportunity>() { testMarketoOpportunity, testMarketoOpportunity2 };
            return new List<MarketoOpportunity>() { };
        }

        public async Task<bool> CustomerExistsForMarketoLeadId(int marketoLeadId)
        {
            List<ScvDBCustomer> scvCustomers;

            using (IDbConnection con = Connection)
            {
                con.Open();

                string sqlQuery = $"SELECT [MarketoLeadId] FROM [AUS-PROD-DB-MI-SQLDB].[mkt].[MarketoLeadSCVCustomerMap] WITH (nolock) Where[MarketoLeadId] = {marketoLeadId}";

                var result = await con.QueryAsync<ScvDBCustomer>(sqlQuery);
                scvCustomers = result.ToList();
            }

            if (scvCustomers.Any())
            {
                return true;
            }

            return false;
        }
    }
}
