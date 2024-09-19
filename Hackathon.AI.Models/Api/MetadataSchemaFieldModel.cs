using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    public class MetadataSchemaFieldModel
    {
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Filterable { get; set; }
        public string Type { get; set; }
    }
}
