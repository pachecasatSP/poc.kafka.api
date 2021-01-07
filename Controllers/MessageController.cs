using api.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private ILogger<MessageController> _logger;
        private IConfiguration _config;

        public string KafkaServer { get; }

        private ProducerConfig _producerConfig;

        public MessageController(ILogger<MessageController> logger,
                                 IConfiguration config)
        {
            _logger = logger;
            _config = config;
            KafkaServer = _config.GetSection("Kafka:ServerAndPort").Value;
            _producerConfig = new ProducerConfig { BootstrapServers = KafkaServer, Partitioner = Partitioner.Random };
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProduceMessageRequest request)
        {
            await ProduceMessage(request);
            return Created("", request);
        }

        private async Task ProduceMessage(ProduceMessageRequest request)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                foreach(var m in request.Messages)
                {
                    await producer.ProduceAsync(m.Topic, new Message<Null, string> { Value = m.Message });
                }
                
            };
        }
    }
}
