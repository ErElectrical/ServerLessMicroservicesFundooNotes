using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.DataBaseConfiguration
{
    public class config
    {
        private readonly IConfiguration _toolsetting;

        private Database database ;

        private Container container;

        private CosmosClient cosmosClient;




        public config(IConfiguration _toolsetting)
        {
            this._toolsetting = _toolsetting;
        }

        public  async  void Appconfiguration()
        {
            var databaseName = this._toolsetting["Values:DataBaseId"];
            var containerName = this._toolsetting["Values:ContainerId"];
            //CosmosClient client = new DocumentClient(new Uri(this._toolsetting["Values:DocDbEndpoint"]), this._toolsetting["Values:DocDbMasterKey"]);

            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerName,"/Id");
            
        }
    }
}