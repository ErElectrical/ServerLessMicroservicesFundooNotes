using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer.Interface;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(UserMicroservices.Dbconfiguration.Startup))]
namespace UserMicroservices.Dbconfiguration
{
    public class Startup : FunctionsStartup
    {
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register the CosmosClient as a Singleton

            builder.Services.AddSingleton((s) => {

                var connectionString = configuration["Values:CosmosDBConnection"];
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("Please specify a valid connection string in the local.settings.json file or your Azure Functions Settings.");
                }
                if (connectionString != null)
                {
                    config con = new config(configuration);
                    con.Initialize();
                }

                CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(connectionString);
                return configurationBuilder
                        .Build();
            });

            builder.Services.AddTransient<IUserRL, UserRL>();
        }


    }
}
