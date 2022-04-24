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

        Task<string> GetLabelByNoteId(string noteId,string userId);
    }
}
