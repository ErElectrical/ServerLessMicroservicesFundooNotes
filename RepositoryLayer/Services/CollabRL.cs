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

        public async Task<List<string>> GetAllCollabByNoteId(string userId, string noteId)
        {
            List<string> CollabEmail = new List<string>();
            try
            {
                QueryDefinition sqldefn = new QueryDefinition(
                    "select * from c where c.userId = @userId and c.noteId = @noteId")
                    .WithParameter("@userId", userId)
                    .WithParameter("@noteId", noteId);


                var container = this._cosmosClient.GetContainer("NoteCollabLabelDB", "CollabDetails");
                using FeedIterator<CollabratorDetails> queryResultSetIterator = container.GetItemQueryIterator<CollabratorDetails>(sqldefn);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<CollabratorDetails> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var item in currentResultSet)
                    {
                        CollabEmail.Add(item.CollabEmail);
                    }

                    return CollabEmail;

                }
                return CollabEmail;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RemoveCollab(string noteId, string userId, string CollabEmail)
        {
            if(noteId == null || userId == null)
            {
                throw new NullReferenceException();
            }
            if(CollabEmail == null)
            {
                throw new Exception("Please pass the CollabEmail ");
            }
            try
            {
                var container = this._cosmosClient.GetContainer("NoteCollabLabelDB", "CollabDetails");
                var document = container.GetItemLinqQueryable<CollabratorDetails>(true)
                               .Where(b => b.userId == userId && b.noteId == noteId  && b.CollabEmail == CollabEmail)
                               .AsEnumerable()
                               .FirstOrDefault();
                if(document != null)
                {
                    using (ResponseMessage response = await container.DeleteItemStreamAsync(document.CollabId, new PartitionKey(document.CollabId))) 
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }

                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
