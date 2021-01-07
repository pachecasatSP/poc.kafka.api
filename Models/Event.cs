using Newtonsoft.Json;

namespace api.Models
{
	public class Event
	{
		[JsonProperty("company")]
		public string Company { get; set; }
	}
}
