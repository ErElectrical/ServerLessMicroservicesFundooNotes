using CommonLayer.NotesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface INoteRL
    {
        Task<NoteDetails> CreateNote(NoteDetails details,string userId);

        Task<List<NoteDetails>> GetAllNotes();

        Task<List<NoteDetails>> GetAllFundooNotesById(string id,string email);

        Task<NoteDetails> UpdateNote(NoteUpdation update, string userId, string noteId);
    }

}
