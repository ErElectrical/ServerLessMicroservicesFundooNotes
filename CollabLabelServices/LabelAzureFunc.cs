using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RepositoryLayer.Interface;
using RepositoryLayer.Authorisation;
using Microsoft.Azure.Cosmos;
using CommonLayer.Model;

namespace CollabLabelServices
{
    public class LabelAzureFunc
    {
        private ILabelRL labelRL;

        private readonly ITokenServices jwt;

        public LabelAzureFunc(ILabelRL labelRL, ITokenServices jwt)
        {
            this.labelRL = labelRL;
            this.jwt = jwt;
        }

        [FunctionName("createLabel")]
        public async Task<IActionResult> createLabel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note/createLabel/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger createLabel function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<LabelRequest>(requestBody);
                var result = this.labelRL.createLabel(authresponse.UserId, noteId, data);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("createLabel  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to createLabel item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("GetLabelByNoteId")]
        public async Task<IActionResult> GetLabelByNoteId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note/GetLabelByNoteId/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger GetLabelByNoteId function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var result = this.labelRL.GetLabelByNoteId(noteId, authresponse.UserId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("GetLabelByNoteId  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to GetLabelByNoteId item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("RemoveLabel")]
        public async Task<IActionResult> RemoveLabel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "note/RemoveLabel/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger RemoveLabel function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var result = this.labelRL.RemoveLabel(noteId, authresponse.UserId);
                return new OkResult();
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("GetLabelByNoteId  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to GetLabelByNoteId item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("UpdateLabel")]
        public async Task<IActionResult> UpdateLabel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/UpdateLabel/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger UpdateLabel function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<LabelRequest>(requestBody);
                var result = this.labelRL.UpdateLabel(authresponse.UserId, noteId, data);
                return new OkResult();

            }
            catch (CosmosException cosmosException)
            {
                log.LogError("UpdateLabel  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to UpdateLabel item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }


        }
    }
}