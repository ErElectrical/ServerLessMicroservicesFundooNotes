using CommonLayer.NotesModel;
using Microsoft.AspNetCore.Http;
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

        Task<List<NoteDetails>> GetAllNotesByUserId(string id, string email);

        Task<NoteDetails> UpdateNote(NoteUpdation update, string userId, string noteId);

        Task<bool> DeleteNote(string noteId);

        Task<bool> IsPinned(string userId, string noteId);

        Task<bool> IsTrash(string userId, string noteId);

        Task<bool> IsArchieve(string userId, string noteId);

        Task<bool> ChangeColour(string colour, string userId, string noteId);

        Task<bool> UploadImage(IFormFile file, string noteId, string userId);
    }

}
