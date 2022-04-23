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
using CommonLayer.NotesModel;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Authorisation;

namespace NoteMicroservices.NotesAzureFunc
{
    public class NotesServices
    {
        private INoteRL noteRL;

        private readonly ITokenServices jwt;

        public NotesServices(INoteRL noteRL, ITokenServices jwt)
        {
            this.noteRL = noteRL;
            this.jwt = jwt;
        }
        [FunctionName("CreateNotes")]
        public async Task<IActionResult> CreateNotes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Createnotes function processed a request.");


            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<NoteDetails>(requestBody);

                var result = await this.noteRL.CreateNote(data, authresponse.UserId);
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {


                log.LogError("Creating item failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to create item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");


            }

        }

        [FunctionName("GetAllNotes")]
        public async Task<IActionResult> GetAllNotes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Getallnotes function processed a request.");


            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                var result = await this.noteRL.GetAllNotes();
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {


                log.LogError("Getallnotes  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to create item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");


            }
        }

        [FunctionName("GetNotesByUserId")]

        public async Task<IActionResult> GetNotesByUserId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger GetAllNotesById function processed a request.");

            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var result = this.noteRL.GetAllNotesByUserId(authresponse.UserId, authresponse.Email);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("GetAllNotesByUserId  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to Get item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("UpdateNote")]
        public async Task<IActionResult> UpdateNote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note/Update/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger UpdateNote function processed a request.");

            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<NoteUpdation>(requestBody);
                var result = this.noteRL.UpdateNote(data, authresponse.UserId, noteId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("UpdateNote  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to UpdateNote item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("DeleteNote")]
        public async Task<IActionResult> DeleteNote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "note/Delete/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger DeleteNote function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var result = this.noteRL.DeleteNote(noteId);
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {
                log.LogError("DeleteNote  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to DeleteNote item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }
        }

        [FunctionName("IsPinned")]
        public async Task<IActionResult> IsPinned(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/IsPinned/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger IsPinned function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var result = this.noteRL.IsPinned(authresponse.UserId, noteId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("IsPinned  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to IsPinned item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("IsTrashed")]
        public async Task<IActionResult> IsTrash(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/IsTrashed/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger IsTrash function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var result = this.noteRL.IsTrash(authresponse.UserId, noteId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("IsTrashed  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to IsTrashed item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("IsArchieved")]
        public async Task<IActionResult> IsArchieve(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/IsArchieved/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger IsArchieve function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var result = this.noteRL.IsArchieve(authresponse.UserId, noteId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("IsArchieved  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to IsArchieved item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("UploadImage")]

        public async Task<IActionResult> UploadImage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/UploadImage/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger UploadImage function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }

                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var file = req.Form.Files["file"];

                var result = this.noteRL.UploadImage(file, noteId, authresponse.UserId);
                return new OkObjectResult(result);
            }
            catch (CosmosException cosmosException)
            {
                log.LogError("UploadImage  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to UploadImage item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }

        [FunctionName("ChangeColour")]

        public async Task<IActionResult> ChangeColour(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/ChangeColour/{noteId}")] HttpRequest req,
            ILogger log, string noteId)
        {
            log.LogInformation("C# HTTP trigger ChangeColour function processed a request.");
            try
            {
                var authresponse = this.jwt.ValidateToken(req);
                if (!authresponse.IsValid)
                {
                    return new UnauthorizedResult();
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                var result = this.noteRL.ChangeColour(data, authresponse.UserId, noteId);
                return new OkObjectResult(result);

            }
            catch (CosmosException cosmosException)
            {
                log.LogError("ChangeColour  failed with error {0}", cosmosException.ToString());
                return new BadRequestObjectResult($"Failed to ChangeColour item. Cosmos Status Code {cosmosException.StatusCode}, Sub Status Code {cosmosException.SubStatusCode}: {cosmosException.Message}.");
            }

        }
    }
}