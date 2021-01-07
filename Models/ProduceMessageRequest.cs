using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ProduceMessageRequest
    {
        public ProduceMessageRequest()
        {
            Messages = new List<MessageDetails>();
        }
        public List<MessageDetails> Messages { get; set; }
    }

    public class MessageDetails
    {
        public string Message { get; set; }
        public string Topic { get; set; }
    }
}
