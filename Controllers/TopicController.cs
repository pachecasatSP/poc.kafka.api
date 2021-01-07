using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Context;
using api.Models;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("topic")]
    public class TopicController : ControllerBase
    {

        private readonly ILogger<TopicController> _logger;
        private readonly IConfiguration _config;

        public string KafkaServer { get; }
        private AdminClientConfig _adminClientConfig;
        private ProducerConfig _producerConfig;

        public TopicController(ILogger<TopicController> logger,
                                IConfiguration config)
        {
            _logger = logger;
            _config = config;
            KafkaServer = _config.GetSection("Kafka:ServerAndPort").Value;
            _adminClientConfig = new AdminClientConfig { BootstrapServers = KafkaServer };
            _producerConfig = new ProducerConfig { BootstrapServers = KafkaServer, Partitioner = Partitioner.Random };
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Ok");
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CreateTopicRequest request)
        {
            await CreateTopics(request);
            if (request.Quantity == 1)
                await ProduceMessage(request.Pattern, "New message to topic " + request.Pattern);

            return Created("", request);
        }

        private async Task ProduceMessage(string topicName, string message)
        {
            using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
            {
                await producer.ProduceAsync(topicName, new Message<Null, string> { Value = message });
            };
        }

        private async Task CreateTopics(CreateTopicRequest request)
        {
            List<TopicSpecification> specs = new List<TopicSpecification>();
            using (var adminClient = new AdminClientBuilder(_adminClientConfig).Build())
            {
                if (request.Quantity > 1)
                {
                    for (var i = 0; i < request.Quantity; i++)
                    {
                        var spec = new TopicSpecification()
                        {
                            Name = $"{request.Pattern}{i}",
                            NumPartitions = request.PartitionNumber,
                            ReplicationFactor = request.ReplicationFactor
                        };

                        specs.Add(spec);
                    }
                }
                else
                {
                    var spec = new TopicSpecification()
                    {
                        Name = $"{request.Pattern}",
                        NumPartitions = request.PartitionNumber,
                        ReplicationFactor = request.ReplicationFactor
                    };

                    specs.Add(spec);
                }
                await adminClient.CreateTopicsAsync(specs);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string topicName)
        {
            using (var adminClient = new AdminClientBuilder(_adminClientConfig).Build())
            {
                await adminClient.DeleteTopicsAsync(new List<string>() { topicName });
            }

            return Ok();
        }

    }
}
