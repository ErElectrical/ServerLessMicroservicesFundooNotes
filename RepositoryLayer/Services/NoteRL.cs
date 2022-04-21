using CommonLayer.NotesModel;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class NoteRL : INoteRL
    {
        private readonly CosmosClient _cosmosClient;

        public NoteRL(CosmosClient _cosmosClient)
        {
            this._cosmosClient = _cosmosClient;
        }
        public async Task<NoteDetails> CreateNote(NoteDetails details)
        {
            if (details == null)
            {
                throw new NullReferenceException();
            }

            try
            {
                var notes = new NoteDetails()
                {
                    NoteId = Guid.NewGuid().ToString(),
                    Title = details.Title,
                    Description = details.Description,
                    Colour = details.Colour,
                    Image = details.Image,
                    Reminder = details.Reminder,
                    IsArchieve = details.IsArchieve,
                    IsPinned = details.IsPinned,
                    IsTrash = details.IsTrash,
                    CreatedAt = DateTime.Now,

                    
                };

                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                return await container.CreateItemAsync<NoteDetails>(notes, new PartitionKey(notes.NoteId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<NoteDetails>> GetAllNotes()
        {
            try
            {
                var sqlQueryText = "SELECT * FROM c ";

                QueryDefinition sqldefn = new QueryDefinition(sqlQueryText);
                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                using FeedIterator<NoteDetails> queryResultSetIterator = container.GetItemQueryIterator<NoteDetails>(sqldefn);

                List<NoteDetails> notes = new List<NoteDetails>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<NoteDetails> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var item in currentResultSet)
                    {
                        notes.Add(item);
                    }

                    return notes;

                }
                return notes;
            }
            catch(CosmosException ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}
