using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class CreateTopicRequest
    {
        public string Pattern { get; set; }

        public int Quantity { get; set; }

        public int PartitionNumber { get; set; }

        public short ReplicationFactor { get; set; }
    }
}
