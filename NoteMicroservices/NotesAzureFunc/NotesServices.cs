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

namespace NoteMicroservices.NotesAzureFunc
{
    public class NotesServices
    {
        private INoteRL noteRL;

        public NotesServices(INoteRL noteRL)
        {
            this.noteRL = noteRL;
        }
        [FunctionName("CreateNotes")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger Createnotes function processed a request.");


            try
            {


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject<NoteDetails>(requestBody);

                var result = await this.noteRL.CreateNote(data);
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
    }
}