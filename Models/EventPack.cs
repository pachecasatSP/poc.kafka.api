using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace api.Models
{
	public class EventPack
	{
		[JsonProperty("id")]
		public Guid Id { get; set; }
		[JsonProperty("events")]
		public List<Event> Events { get; set; }
	}
}
