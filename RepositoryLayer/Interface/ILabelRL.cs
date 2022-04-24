using CommonLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface ILabelRL
    {
        Task<LabelDetails> createLabel(string userId, string noteId, LabelRequest label);

        Task<List<string>> GetLabelByNoteId(string noteId,string userId);

        Task<bool> UpdateLabel(string noteId, string userId, LabelRequest label);

        Task<bool> RemoveLabel(string noteId, string userId);
    }
}
