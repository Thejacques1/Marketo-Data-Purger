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
            List<MarketoOpportunity> marketoOpportunities;

            using (IDbConnection con = Connection)
            {
                con.Open();

                string sqlQuery = "SELECT [MarketoOpportunityId], [MarketoOpportunityRoleId] FROM [schema].[Table] (nolock)";

                var result = await con.QueryAsync<MarketoOpportunity>(sqlQuery);
                marketoOpportunities = result.ToList();
            }

            return marketoOpportunities;

            ////Examples for testing:
            //MarketoOpportunity testMarketoOpportunity = new MarketoOpportunity()
            //{
            //    MarketoOpportunityId = new Guid("6EDF6399-0D7B-4CCF-8A9B-4AFC1DB559E9"),
            //    MarketoOpportunityRoleId = new Guid("4ED96573-B8D4-4234-AD88-7AE34013809E")
            //};

            //MarketoOpportunity testMarketoOpportunity2 = new MarketoOpportunity()
            //{
            //    MarketoOpportunityId = new Guid("E96DEAC5-4A4E-4B92-8480-1E76AA45A336"),
            //    MarketoOpportunityRoleId = new Guid("D27F0046-D681-4D11-A09F-8CA5BAD741B8")
            //};

            //return new List<MarketoOpportunity>() { testMarketoOpportunity, testMarketoOpportunity2 };
        }
    }
}
