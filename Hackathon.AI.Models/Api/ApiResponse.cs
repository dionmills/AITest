using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("value")]
        public T Value { get; set; }
        [JsonPropertyName("nextLink")]
        public string NextLink { get; set; }
    }
}
