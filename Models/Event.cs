using Newtonsoft.Json;

namespace api.Models
{
	public class Event
	{
		[JsonProperty("companyKey")]
		public string CompanyKey { get; set; }
		[JsonProperty("eventMessage")]
		public string EventMessage { get; set; }
	}
}
