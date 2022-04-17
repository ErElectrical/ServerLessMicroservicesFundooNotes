using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserRegistration.Model;
using RepositoryLayer.Interface;
using Microsoft.Azure.Cosmos;

namespace UserServices.AzureFunctions
{
    public  class UserRegistration
    {
        private IUserRL userRL;

        public UserRegistration(IUserRL userRL)
        {
            this.userRL = userRL;
        }

        [FunctionName("UserRegistration")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger UserRegistration function processed a request.");

            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<UserDetails>(requestBody);

                var result = await this.userRL.createUser(data);
                return new OkObjectResult(result.Resource);

            }
            catch (CosmosException cosmosException)
            {


                log.LogError("Creating item failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to create item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");


            }
            

        }
    }
}
