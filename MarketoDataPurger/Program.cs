using System;
using System.IO;
using System.Threading.Tasks;
using MarketoDataPurger.Gateways;
using MarketoDataPurger.Repositories;
using MarketoDataPurger.Services;
using MarketoDataPurger.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JsonSettings = MarketoDataPurger.Settings.Settings;

namespace MarketoDataPurger
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();
            JsonSettings mySettingsConfig = new JsonSettings();
            configuration.Bind(mySettingsConfig);

            DisplaySettings(mySettingsConfig);
            SetupDependencies(mySettingsConfig);
            await InitiatePurge(mySettingsConfig);

            Console.ReadKey();
        }

        private static void DisplaySettings(JsonSettings mySettingsConfig)
        {
            Console.WriteLine("Database Connection string: " + mySettingsConfig.DatabaseConnectionString);
            Console.WriteLine("Marketo Instance: " + mySettingsConfig.Marketo.InstanceUrl);
            Console.WriteLine("Marketo ClientId: " + mySettingsConfig.Marketo.ClientId);
            Console.WriteLine("Marketo Secret: " + mySettingsConfig.Marketo.Secret);
        }

        private static void SetupDependencies(JsonSettings mySettingsConfig)
        {
            _serviceProvider = new ServiceCollection()
                .AddSingleton<IDatabaseRepository>(dbr => new DatabaseRepository(mySettingsConfig.DatabaseConnectionString))
                .AddSingleton<IMarketoGateway>(mkg => new MarketoGateway(mySettingsConfig.Marketo.InstanceUrl, mySettingsConfig.Marketo.ClientId, mySettingsConfig.Marketo.Secret))
                .AddSingleton<IMarketoPurgingService, MarketoPurgingService>()
                .BuildServiceProvider();
        }

        private static async Task InitiatePurge(JsonSettings mySettingsConfig)
        {
            IMarketoPurgingService marketoPurgingService = _serviceProvider.GetService<IMarketoPurgingService>();

            bool marketoTestResult = false;

            Console.WriteLine("Ready to test Marketo connection? Y/N");
            string marketoTestQuestionAnswer = Console.ReadLine();
            if (marketoTestQuestionAnswer.Equals("Y", StringComparison.InvariantCultureIgnoreCase) || marketoTestQuestionAnswer.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
            {
                marketoTestResult = marketoPurgingService.TestMarketoConnection();

                Console.WriteLine(
                    marketoTestResult
                        ? "Marketo Connection successful and token has been retrieved"
                        : "Marketo Connection failed. Please fix configuration and try again.");

            }
            if (marketoTestResult)
            {
                bool databaseTestResult = false;

                Console.WriteLine("Ready to test SQL Database connection? Y/N");
                string sqlDatabaseTestQuestionAnswer = Console.ReadLine();
                if (sqlDatabaseTestQuestionAnswer.Equals("Y", StringComparison.InvariantCultureIgnoreCase) || sqlDatabaseTestQuestionAnswer.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    DatabaseTestDto dbTestResult = await marketoPurgingService.TestDatabaseConnection();
                    databaseTestResult = dbTestResult.Success;
                    Console.WriteLine(
                        dbTestResult.Success
                            ? "Database Connection successful, " + dbTestResult.RecordsRetrieved + " records retrieved."
                            : "Database Connection failed with error " + dbTestResult.Error + ". Please fix configuration and try again.");
                }

                if (databaseTestResult)
                {
                    Console.WriteLine(
                        "Would you like to proceed to delete all Opportunities and OpportunityRoles? Y/N");
                    string purgeQuestionAnswer = Console.ReadLine();
                    if (purgeQuestionAnswer.Equals("Y", StringComparison.InvariantCultureIgnoreCase) || purgeQuestionAnswer.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await marketoPurgingService.Purge();

                        Console.WriteLine("Purge has been completed.");
                    }
                }
            }
        }
    }
}
