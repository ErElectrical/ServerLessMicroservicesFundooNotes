using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Model
{
    public class LabelDetails
    {
        [JsonProperty(PropertyName = "id", ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public string LabelId { get; set; } = " ";

        public string LabelName { get; set; } = " ";

        public string userId { get; set; } = " ";

        public string noteId { get; set; } = " ";

    }
}
