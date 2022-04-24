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
    public class LabelRL : ILabelRL
    {
        private readonly CosmosClient _cosmosClient;

        private IUserRL userRL;

        public LabelRL(CosmosClient _cosmosClient, IUserRL userRL)
        {
            this._cosmosClient = _cosmosClient;
            this.userRL = userRL;

        }
        public async Task<LabelDetails> createLabel(string userId, string noteId, LabelRequest label)
        {
            if (label.LabelName == null)
            {
                throw new Exception("Please pass LabelName ");
            }
            if (userId == null || noteId == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                var labelDetails = new LabelDetails()
                {
                    LabelId = Guid.NewGuid().ToString(),
                    LabelName = label.LabelName,
                    noteId = noteId,
                    userId = userId
                };


                var container = this._cosmosClient.GetContainer("NoteCollabLabelDB", "LabelDetails");
                return await container.CreateItemAsync<LabelDetails>(labelDetails, new PartitionKey(labelDetails.LabelId));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<string>> GetLabelByNoteId(string noteId, string userId)
        {
            if (userId == null || noteId == null)
            {
                throw new NullReferenceException();
            }

            List<string> labelName = new List<string>();
            try
            {
                QueryDefinition sqldefn = new QueryDefinition(
                   "select * from c where c.userId = @userId and c.noteId = @noteId")
                   .WithParameter("@userId", userId)
                   .WithParameter("@noteId", noteId);

                var container = this._cosmosClient.GetContainer("NoteCollabLabelDB", "LabelDetails");
                using FeedIterator<LabelDetails> queryResultSetIterator = container.GetItemQueryIterator<LabelDetails>(sqldefn);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<LabelDetails> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var item in currentResultSet)
                    {
                        labelName.Add(item.LabelName);
                    }

                    return labelName;

                }
                return labelName;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
