using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon.AI.Models.Api
{
    public class CreateIndexModel
    {
        public MetadataSchemaModel MetadataSchema { get; set; }
        public IEnumerable<FeatureModel> Features { get; set; }
    }
}
