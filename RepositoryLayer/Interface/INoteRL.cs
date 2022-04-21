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
        Task<NoteDetails> CreateNote(NoteDetails details);

        Task<List<NoteDetails>> GetAllNotes();
    }
}
