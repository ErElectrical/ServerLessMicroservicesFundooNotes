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
    public class CollabAzureFunc
    {
        private ICollabRL collabRL;
        private readonly ITokenServices jwt;

        public CollabAzureFunc(ICollabRL collabRL, ITokenServices jwt)
        {
            this.collabRL = collabRL;
            this.jwt = jwt;
        }

        [FunctionName("CreateCollabrator")]
        public async Task<IActionResult> CreateCollabrator(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CreateCollabrator/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger CreateCollabrator function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<CollabRequest>(requestBody);
                var result = this.collabRL.CreateCollabrator(authresponse.UserId, noteId, data);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("CreateCollabrator  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to CreateCollabrator item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("GetAllCollabByNoteId")]
        public async Task<IActionResult> GetAllCollabByNoteId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetAllCollabByNoteId/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger GetAllCollabByNoteId function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var result = this.collabRL.GetAllCollabByNoteId(authresponse.UserId, noteId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("GetAllCollabByNoteId  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to GetAllCollabByNoteId item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }


        }
        [FunctionName("RemoveCollab")]
        public async Task<IActionResult> RemoveCollab(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RemoveCollab/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger RemoveCollab function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<CollabRequest>(requestBody);
                var result = this.collabRL.RemoveCollab(noteId, authresponse.UserId, data);
                return new OkResult();
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("RemoveCollab  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to RemoveCollab item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }
