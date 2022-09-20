using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketoDataPurger.Gateways;
using MarketoDataPurger.Gateways.Models;
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
        private readonly IFileRepository _fileRepository;
        int noOfscvLeadExistsTries;

        public MarketoPurgingService(IDatabaseRepository databaseRepository, IMarketoGateway marketoGateway, IFileRepository fileRepository)
        {
            _databaseRepository = databaseRepository;
            _marketoGateway = marketoGateway;
            _fileRepository = fileRepository;
        }

        public async Task DeduplicateLeads()
        {
            int taskCount = 0;
            int recordCounter = 0;
            int winningLead = 0;
            int duplicateLeadId = 0;
            noOfscvLeadExistsTries = 0;

            Console.WriteLine("Loading Leads from file. Please wait...");
            IEnumerable<MarketoLead> marketoFullDuplicateLeads = _fileRepository.ReadDuplicateLeadsFromCSV();
            List<MarketoLead> marketoFullDuplicateLeadsList = marketoFullDuplicateLeads.ToList();

            List<MarketoLead> scvLeads =
                marketoFullDuplicateLeadsList.Where(x => !string.IsNullOrEmpty(x.ScvDatabaseIdLead)).ToList();

            foreach (MarketoLead scvLead in scvLeads)
            {
                MarketoLead bolCreatedLead = marketoFullDuplicateLeadsList
                    .FirstOrDefault(x => x.EmailAddress.ToLowerInvariant().Equals(scvLead.EmailAddress.ToLowerInvariant()) && string.IsNullOrEmpty(x.ScvDatabaseIdLead));

                if (bolCreatedLead != null)
                {
                    bool scvLeadExists;
                    bool bolLeadExists;

                    Console.WriteLine("Checking if SCV Lead exists in DB...");
                    scvLeadExists = await LeadExistsInDb(scvLead.Id);
                    noOfscvLeadExistsTries = 0;

                    Console.WriteLine("Checking if BOL Lead exists in DB...");
                    bolLeadExists = await LeadExistsInDb(bolCreatedLead.Id);
                    noOfscvLeadExistsTries = 0;

                    if (scvLeadExists && !bolLeadExists)
                    {
                        Console.WriteLine($"SCV Lead with MarketoLeadID {scvLead.Id} found in DB and being used as winner...");
                        winningLead = scvLead.Id;
                        duplicateLeadId = bolCreatedLead.Id;
                    }
                    else if(!scvLeadExists && bolLeadExists)
                    {
                        winningLead = bolCreatedLead.Id;
                        duplicateLeadId = scvLead.Id;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue; 
                }

                List<Task> apiTasks = new List<Task>();

                apiTasks.Add(_marketoGateway.MergeLeads(winningLead, duplicateLeadId));
                taskCount++;
                recordCounter+=2;

                if (taskCount >= 10)
                {
                    if (taskCount % 10 == 0)
                    {
                        Console.WriteLine("De-duplicating leads - Task count: {0}, RecordCount: {1}", taskCount, recordCounter);
                        await Task.WhenAll(apiTasks);
                        Console.WriteLine("De-duplicating leads - Sleeping for {0} seconds on count: {1}", 21, taskCount);
                        await Task.Delay(21000);
                    }
                }
                else
                {
                    await Task.WhenAll(apiTasks);
                }
            }
        }

        private async Task<bool> LeadExistsInDb(int leadId)
        {
            bool leadExists = false;
            try
            {
                noOfscvLeadExistsTries++;
                leadExists = await _databaseRepository.CustomerExistsForMarketoLeadId(leadId);
            }
            catch (Exception)
            {
                if (noOfscvLeadExistsTries <= 10)
                {
                    leadExists = await LeadExistsInDb(leadId);
                }
                else
                {
                    throw;
                }
            }

            return leadExists;
        }

        public async Task FindStaleLeads()
        {
            int recordCounter = 0;
            Console.WriteLine("Loading Lead Id's from file. Please wait...");
            IEnumerable<MarketoScvLeadId> marketoLeadIds = _fileRepository.ReadStaleLeadsFromCsv();
            List<MarketoScvLeadId> deletedCustomers = new List<MarketoScvLeadId>();

            foreach (MarketoScvLeadId marketoScvLeadId in marketoLeadIds)
            {
                recordCounter++;
                FindLeadResponse findLeadResponse = await _marketoGateway.FindLead(marketoScvLeadId.MarketoLeadId);
                if (findLeadResponse.Success && !findLeadResponse.Result.Any())
                {
                    deletedCustomers.Add(marketoScvLeadId);
                }

                if (recordCounter % 10 == 0)
                {
                    Console.WriteLine("Find Stale leads RecordCount: {0}", recordCounter);
                }
            }

            if (deletedCustomers.Any())
            {
                Console.WriteLine("Find Stale leads - Writing output to CSV file");
                _fileRepository.WriteStaleLeadsToCsv(deletedCustomers);
            }
            else
            {
                Console.WriteLine("Find Stale leads - No leads found to write to CSV file");
            }
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
