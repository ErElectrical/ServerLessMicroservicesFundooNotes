using CommonLayer.Model;
using CommonLayer.NotesModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using RepositoryLayer.Authorisation;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

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

        public async Task<bool> ChangeColour(string colour, string userId, string noteId)
        {
            if (userId == null || noteId == null)
            {
                throw new Exception("please pass userId and noteId compulsary ");
            }
            if(colour == null)
            {
                throw new Exception("Please pass colour to change ");
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
                    itembody.Colour = colour;
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

        public async Task<NoteDetails> GetAllNoteByNoteId(string noteId, string email)
        {
            List<NoteDetails> notes = new List<NoteDetails>();
            try
            {
                if (noteId == null)
                {
                    throw new NullReferenceException();
                }

                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                NoteDetails note = await container.ReadItemAsync<NoteDetails>(noteId, new PartitionKey(noteId));

                return note;
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

        public async Task<bool> IsArchieve(string userId, string noteId)
        {
            if (userId == null || noteId == null)
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
                if( document != null)
                {
                    ItemResponse<NoteDetails> response = await container.ReadItemAsync<NoteDetails>(document.NoteId, new PartitionKey(document.NoteId));
                    var itembody = response.Resource;
                    if (document.IsArchieve == true)
                    {
                        itembody.IsArchieve = false;
                    }
                    if (document.IsArchieve == false)
                    {
                        itembody.IsArchieve = true;
                    }
                    response = await container.ReplaceItemAsync<NoteDetails>(itembody, itembody.NoteId, new PartitionKey(itembody.NoteId));

                    return true;
                }
                return false;

            }
            catch (Exception ex)
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

        public async Task<bool> UploadImage(IFormFile file, string noteId, string userId)
        {
            if (userId == null || noteId == null)
            {
                throw new Exception("please pass userId and noteId compulsary ");
            }
            if(file == null)
            {
                throw new Exception("Please upload Image correctly ");
            }

            try
            {
                var CloudinaryData = new CloudinaryDotNet.Cloudinary(new Account
                {
                    ApiKey = "489788912754161",
                    ApiSecret = "SgcAbpkm2vEoafLV5OhtOt_PJiU",
                    Cloud = "dwwotohwm"
                });

                Stream s = file.OpenReadStream();

                var imageuploadparams = new ImageUploadParams()
                {
                    File = new FileDescription("Test", s)
                };

                var Result = CloudinaryData.Upload(imageuploadparams);

                var container = this._cosmosClient.GetContainer("FundooNotesNoteDb", "NoteDetails");
                var document = container.GetItemLinqQueryable<NoteDetails>(true)
                               .Where(b => b.userId == userId && b.NoteId == noteId)
                               .AsEnumerable()
                               .FirstOrDefault();
                if(document != null)
                {
                    ItemResponse<NoteDetails> response = await container.ReadItemAsync<NoteDetails>(document.NoteId, new PartitionKey(document.NoteId));
                    var itembody = response.Resource;
                    itembody.Image = Result.Url.ToString();
                    response = await container.ReplaceItemAsync<NoteDetails>(itembody, itembody.NoteId, new PartitionKey(itembody.NoteId));
                    return true;

                }
                return false;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
