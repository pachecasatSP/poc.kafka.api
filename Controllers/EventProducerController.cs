﻿using api.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventProducerController : ControllerBase
	{
		private readonly ILogger<EventProducerController> _logger;
		private readonly IConfiguration _config;
		private readonly ProducerConfig _producerConfig;
		private readonly string _topic;
		private readonly string _filePath;
		private readonly string _fileName;

		public EventProducerController(ILogger<EventProducerController> logger, IConfiguration config)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_producerConfig = new ProducerConfig
			{
				BootstrapServers = _config.GetSection("Kafka:ServerAndPort").Value,
				Partitioner = Partitioner.Random
			};
			_topic = _config.GetSection("Kafka:GenericTopic").Value;
			_filePath = _config.GetSection("Kafka:OutputFileFolder").Value;
			_fileName= _config.GetSection("Kafka:FileName").Value;
		}

		public async Task<IActionResult> Post([FromBody] EventProducerRequest value)
		{
			try
			{
				var companyQuantity = value.CompanyQuantity < 1 ? 1 : value.CompanyQuantity;
				var csv = new StringBuilder();
				csv.AppendLine("Company;Event");
				var aggregatedCsv = new StringBuilder();
				aggregatedCsv.AppendLine("Company;Qtd");
				var eventPack = new EventPack
				{
					Id = Guid.NewGuid(),
					Events = new List<Event>()
				};

				for (var i = 0; i < value.EventQuantity; i++)
				{
					var company = $"company{new Random().Next(1, companyQuantity)}";
					var @event = $"EventMessage{i}";
					eventPack.Events.Add(new Event
					{
						CompanyKey = company,
						EventMessage = @event
					});
					csv.AppendLine($"{company};{@event}");
				}

				foreach (var item in eventPack.Events.GroupBy(e => e.CompanyKey).Select(g => $"{g.Key};{g.Count()}"))
					aggregatedCsv.AppendLine(item);

				using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
				{
					var msg = new Message<Null, string>
					{
						Value = Newtonsoft.Json.JsonConvert.SerializeObject(eventPack)
					};
					var result = await producer.ProduceAsync(_topic, msg);

					System.IO.Directory.CreateDirectory(_filePath);
					await System.IO.File.WriteAllTextAsync($"{_filePath}{_fileName}", csv.ToString());
					await System.IO.File.WriteAllTextAsync($"{_filePath}aggregated_{_fileName}", aggregatedCsv.ToString());

					return Created("", result);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);

				throw;
			}
		}
	}
}
