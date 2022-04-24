using CommonLayer.Model;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class CollabRL : ICollabRL
    {
        private readonly CosmosClient _cosmosClient;

        private IUserRL userRL;

        public CollabRL(CosmosClient _cosmosClient, IUserRL userRL)
        {
            this._cosmosClient = _cosmosClient;
            this.userRL = userRL;

        }

        public async Task<CollabratorDetails> CreateCollabrator(string userId, string noteId, CollabRequest collab)
        {
            if(collab.Email == null)
            {
                throw new Exception("Please pass Collabrator Email");
            }
            if (userId == null || noteId == null)
            {
                throw new NullReferenceException();
            }
            var checkUser = this.userRL.GetDetailsByEmailId(collab.Email);
            if(checkUser == null)
            {
                throw new Exception("Given EmailId not regisred with any account");
            }
            try
            {
                var collabrator = new CollabratorDetails()
                {
                    CollabId= Guid.NewGuid().ToString(),
                    CollabEmail=collab.Email,
                    noteId=noteId,
                    userId=userId
                };
                var container = this._cosmosClient.GetContainer("NoteCollabLabelDB", "CollabDetails");
                return await container.CreateItemAsync<CollabratorDetails>(collabrator, new PartitionKey(collabrator.CollabId));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
