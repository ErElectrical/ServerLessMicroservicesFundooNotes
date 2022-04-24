using CommonLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface ICollabRL
    {
        public Task<CollabratorDetails> CreateCollabrator(string userId,string noteId, CollabRequest collab);

        public Task<List<string>> GetAllNotesByUserId(string userId, string noteId);
    }
}
