using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    public class VideoMetadata
    {
        [JsonPropertyName("cameraId")]
        public string CameraId { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }

    public class VideoRequestModel
    {
        [JsonPropertyName("videos")]
        public List<VideoModel> Videos { get; set; }
    }

    public class VideoModel
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("documentId")]
        public string DocumentId { get; set; }

        [JsonPropertyName("documentUrl")]
        public string DocumentUrl { get; set; }

        [JsonPropertyName("metadata")]
        public VideoMetadata Metadata { get; set; }
    }


}
