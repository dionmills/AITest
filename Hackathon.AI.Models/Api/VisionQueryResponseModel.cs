using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    public class VisionQueryResponseModel
    {
        [JsonPropertyName("documentId")]
        public string DocumentId { get; set; }

        [JsonPropertyName("documentKind")]
        public string DocumentKind { get; set; }

        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("best")]
        public string Best { get; set; }

        [JsonPropertyName("relevance")]
        public double Relevance { get; set; }
    }
}
