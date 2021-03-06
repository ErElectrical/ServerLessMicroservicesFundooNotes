using CommonLayer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Model;

namespace CommonLayer.NotesModel
{
    public class NoteDetails
    {
        [JsonProperty(PropertyName = "id", ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]

        public string NoteId { get; set; } = " ";

        public string Title { get; set; } = " ";

        public string Description { get; set; } = " ";

        public DateTime? Reminder { get; set; }

        public string Colour { get; set; } = " ";

        public string Image { get; set; } = " ";

        public bool IsTrash { get; set; }

        public bool IsArchieve { get; set; }

        public bool IsPinned { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string userId { get; set; } = " ";

        public string Email { get; set; } = " ";
    }
}
