using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class VisionFilters
    {
        [JsonPropertyName("stringFilters")]
        public List<VisionStringFilter> StringFilters { get; set; }

        [JsonPropertyName("featureFilters")]
        public List<string> FeatureFilters { get; set; }
    }

    public class VisionQueryModel
    {
        [JsonPropertyName("queryText")]
        public string QueryText { get; set; }

        [JsonPropertyName("filters")]
        public VisionFilters Filters { get; set; }
    }

    public class VisionStringFilter
    {
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }

        [JsonPropertyName("values")]
        public List<string> Values { get; set; }
    }


}
