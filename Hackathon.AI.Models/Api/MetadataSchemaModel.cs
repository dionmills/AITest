using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    public class MetadataSchemaModel
    {
        public string Language { get; set; }
        public List<MetadataSchemaFieldModel> Fields { get; set; }
    }
}
