using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CommonLayer.Model;
using RepositoryLayer.Interface;
using Microsoft.Azure.Cosmos;

namespace UserMicroservices.AzureFunctions
{
    public  class ResetPassword
    {
        private IUserRL userRL;

        public ResetPassword(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        [FunctionName("ResetPassword")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<ResetPassWordDetails>(requestBody);

                var result = this.userRL.ResetPassword(data);
                if (data != null)
                {
                    return new OkObjectResult(result);

                }
                return new BadRequestResult();
            }
            catch (CosmosException cosmosException)
            {

                log.LogError(" Reset password failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to proced for Reset password Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }


        }
    }
}
