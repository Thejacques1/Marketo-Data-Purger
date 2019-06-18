using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketoDataPurger.Gateways;
using MarketoDataPurger.Repositories;
using MarketoDataPurger.Repositories.Models;
using MarketoDataPurger.Services.Extensions;
using MarketoDataPurger.Services.Mappers;
using MarketoDataPurger.Services.Models;

namespace MarketoDataPurger.Services
{
    public class MarketoPurgingService : IMarketoPurgingService
    {
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IMarketoGateway _marketoGateway;
        public MarketoPurgingService(IDatabaseRepository databaseRepository, IMarketoGateway marketoGateway)
        {
            _databaseRepository = databaseRepository;
            _marketoGateway = marketoGateway;
        }
        public async Task Purge()
        {
            await PurgeOpportunityRoles();
            await PurgeOpportunities();
        }

        private async Task PurgeOpportunityRoles()
        {
            int taskCount = 0;
            int recordCounter = 0;
            Console.WriteLine("Loading Opportunities from Database. Please wait...");
            IEnumerable<MarketoOpportunity> marketoOpportunities = await _databaseRepository.GetMarketoOpportunities();

            Console.WriteLine("Starting Roles Purge");
            IEnumerable<MarketoOpportunity> opportunities = marketoOpportunities.ToList();
            Console.WriteLine("Roles to Purge: {0}", opportunities.Count());

            foreach (IEnumerable<MarketoOpportunity> opportunityBatch in opportunities.Chunk(300))
            {
                DeleteOpportunityRequestDto deleteOpportunityRequestDto = new DeleteOpportunityRequestDto()
                { DeleteBy = "idField", Input = new List<DeleteOpportunityDto>() };
                List<DeleteOpportunityDto> deleteOpportunityDtos = new List<DeleteOpportunityDto>();

                List<Task> apiTasks = new List<Task>();

                foreach (MarketoOpportunity opportunity in opportunityBatch)
                {
                    deleteOpportunityDtos.Add(new DeleteOpportunityDto()
                    { Id = opportunity.MarketoOpportunityRoleId });
                }

                deleteOpportunityRequestDto.Input = deleteOpportunityDtos;

                apiTasks.Add(_marketoGateway.DeleteOpportunityRole(deleteOpportunityRequestDto.Map()));
                taskCount++;
                recordCounter += 300;

                if (taskCount >= 10)
                {
                    if (taskCount % 10 == 0)
                    {
                        Console.WriteLine("Purging Roles - Task count: {0}, RecordCount: {1}", taskCount, recordCounter);
                        await Task.WhenAll(apiTasks);
                        Console.WriteLine("Purging Roles - Sleeping for {0} seconds on count: {1}", 21, taskCount);
                        await Task.Delay(21000);
                    }
                }
                else
                {
                    await Task.WhenAll(apiTasks);
                }
            }
        }

        private async Task PurgeOpportunities()
        {
            int taskCount = 0;
            int recordCounter = 0;
            Console.WriteLine("Loading Opportunities from Database. Please wait...");
            IEnumerable<MarketoOpportunity> marketoOpportunities = await _databaseRepository.GetMarketoOpportunities();

            Console.WriteLine("Starting Opportunities Purge");
            IEnumerable<MarketoOpportunity> opportunities = marketoOpportunities.ToList();
            Console.WriteLine("Opportunities to Purge: {0}", opportunities.Count());

            foreach (IEnumerable<MarketoOpportunity> opportunityBatch in opportunities.Chunk(300))
            {
                DeleteOpportunityRequestDto deleteOpportunityRequestDto = new DeleteOpportunityRequestDto()
                { DeleteBy = "idField", Input = new List<DeleteOpportunityDto>() };
                List<DeleteOpportunityDto> deleteOpportunityDtos = new List<DeleteOpportunityDto>();

                List<Task> apiTasks = new List<Task>();

                foreach (MarketoOpportunity opportunity in opportunityBatch)
                {
                    deleteOpportunityDtos.Add(new DeleteOpportunityDto()
                    { Id = opportunity.MarketoOpportunityId });
                }

                deleteOpportunityRequestDto.Input = deleteOpportunityDtos;

                apiTasks.Add(_marketoGateway.DeleteOpportunity(deleteOpportunityRequestDto.Map()));
                taskCount++;
                recordCounter += 300;

                if (taskCount >= 10)
                {
                    if (taskCount % 10 == 0)
                    {
                        Console.WriteLine("Purging Opportunities - Task count: {0}, RecordCount: {1}", taskCount, recordCounter);
                        await Task.WhenAll(apiTasks);
                        Console.WriteLine("Purging Opportunities - Sleeping for {0} seconds on count: {1}", 21, taskCount);
                        await Task.Delay(21000);
                    }
                }
                else
                {
                    await Task.WhenAll(apiTasks);
                }
            }
        }

        public bool TestMarketoConnection()
        {
            return _marketoGateway.GetToken().Length > 0;
        }

        public async Task<DatabaseTestDto> TestDatabaseConnection()
        {
            try
            {
                IEnumerable<MarketoOpportunity> result = await _databaseRepository.GetMarketoOpportunities();
                return new DatabaseTestDto()
                {
                    RecordsRetrieved = result.Count(), Success = true
                };
            }
            catch (Exception e)
            {
                return new DatabaseTestDto()
                {
                    RecordsRetrieved = 0,
                    Success = false,
                    Error = e.Message
                };
            }
        }
    }
}
