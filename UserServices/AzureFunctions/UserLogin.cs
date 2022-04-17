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
    public  class UserLogin
    {
        private IUserRL userRL;

        public UserLogin(IUserRL userRL)
        {
            this.userRL = userRL;
        }

        [FunctionName("UserLogin")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<LoginDetails>(requestBody);
                var result = this.userRL.UserLogin(data);
                if(result == true)
                {
                    return new OkObjectResult(result.Resource);
                }
                return new BadRequestResult();
            }
            catch (CosmosException cosmosException)
            {

                log.LogError("Login failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to Login. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }


        }
    }
}
