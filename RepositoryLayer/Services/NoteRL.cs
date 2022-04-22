using CommonLayer.Model;
using CommonLayer.NotesModel;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Authorisation;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;

namespace RepositoryLayer.Services
{
    public class NoteRL : INoteRL
    {
        private readonly CosmosClient _cosmosClient;

        private readonly ITokenServices jwt;

        private IUserRL userRL;

        public NoteRL(CosmosClient _cosmosClient, ITokenServices jwt, IUserRL userRL)
        {
            this._cosmosClient = _cosmosClient;
            this.jwt = jwt;
            this.userRL = userRL;
        }
        public async Task<NoteDetails> CreateNote(NoteDetails details, string userId)
        {
            if (details == null)
            {
                throw new NullReferenceException();
            }

            UserDetails res = this.userRL.GetDetailsById(userId);
            
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
                    userId = res.Id,
                    Email = res.Email
                    
                    

                    
                };

                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                return await container.CreateItemAsync<NoteDetails>(notes, new PartitionKey(notes.NoteId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteNote(string noteId)
        {
            if(noteId == null )
            {
                throw new NullReferenceException();
            }
            try
            {
                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");

                using (ResponseMessage response =  await container.DeleteItemStreamAsync(noteId, new PartitionKey(noteId)))
                {
                    if(response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                }
                return false;

                   


            }
            catch(CosmosException ex )
            {
                throw new Exception(ex.Message);
            }

             
        }

        public async Task<List<NoteDetails>> GetAllFundooNotesByUserId(string id, string email)
        {
            List<NoteDetails> notes = new List<NoteDetails>();
            try
            {
                QueryDefinition sqldefn = new QueryDefinition(
                    "select * from c where c.userId = @id and c.Email = @email")
                    .WithParameter("@id", id)
                    .WithParameter("@email", email);

                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                using FeedIterator<NoteDetails> queryResultSetIterator = container.GetItemQueryIterator<NoteDetails>(sqldefn);


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
            catch(Exception ex)
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

        public async Task<bool> IsPinned(string userId, string noteId)
        {
            if(userId == null || noteId == null)
            {
                throw new Exception("please pass userId and noteId compulsary ");
            }
            try
            {
                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                var document = container.GetItemLinqQueryable<NoteDetails>(true)
                               .Where(b => b.userId == userId && b.NoteId == noteId)
                               .AsEnumerable()
                               .FirstOrDefault();
                if(document != null)
                {
                    ItemResponse<NoteDetails> response = await container.ReadItemAsync<NoteDetails>(document.NoteId, new PartitionKey(document.NoteId));
                    var itembody = response.Resource;

                    if(document.IsPinned == true)
                    {
                        itembody.IsPinned = false;
                    }
                    if(document.IsPinned == false)
                    {
                        itembody.IsPinned = true;
                    }
                    response = await container.ReplaceItemAsync<NoteDetails>(itembody, itembody.NoteId, new PartitionKey(itembody.NoteId));

                    return true;

                }
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsTrash(string userId, string noteId)
        {
           if(userId == null || noteId == null)
           {
                throw new Exception("please pass userId and noteId compulsary ");
           }
           try
           {
                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                var document = container.GetItemLinqQueryable<NoteDetails>(true)
                               .Where(b => b.userId == userId && b.NoteId == noteId)
                               .AsEnumerable()
                               .FirstOrDefault();
                if (document != null)
                {
                    ItemResponse<NoteDetails> response = await container.ReadItemAsync<NoteDetails>(document.NoteId, new PartitionKey(document.NoteId));
                    var itembody = response.Resource;

                    if (document.IsTrash == true)
                    {
                        itembody.IsTrash = false;
                    }
                    if (document.IsTrash == false)
                    {
                        itembody.IsTrash = true;
                    }
                    response = await container.ReplaceItemAsync<NoteDetails>(itembody, itembody.NoteId, new PartitionKey(itembody.NoteId));

                    return true;

                }
                return false;
            }
           catch(Exception ex)
           {
                throw new Exception(ex.Message);
           }
        }

        public async Task<NoteDetails> UpdateNote(NoteUpdation update, string userId, string noteId)
        {
            if (update == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                var document = container.GetItemLinqQueryable<NoteDetails>(true)
                               .Where(b => b.userId == userId && b.NoteId == noteId)
                               .AsEnumerable()
                               .FirstOrDefault();
                if(document != null)
                {
                    ItemResponse<NoteDetails> response = await  container.ReadItemAsync<NoteDetails>(document.NoteId, new PartitionKey(document.NoteId));
                    var itembody = response.Resource;
                    itembody.Title = update.Title;
                    itembody.Description = update.Description;
                    itembody.Image = update.Image;
                    itembody.Colour = update.Colour;
                    response = await container.ReplaceItemAsync<NoteDetails>(itembody, itembody.NoteId, new PartitionKey(itembody.NoteId));
                    return response.Resource;

                }
                throw new NullReferenceException();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
