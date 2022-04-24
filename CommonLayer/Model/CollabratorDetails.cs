using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class CollabratorDetails
    {

        [JsonProperty(PropertyName = "id", ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public string CollabId { get; set; } = " ";

        public string CollabEmail { get; set; } = " ";

        public string noteId { get; set; } = " ";

        public string userId { get; set; } = " ";
    }
}
