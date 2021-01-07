using Newtonsoft.Json;

namespace api.Models
{
	public class EventProducerRequest
	{
		[JsonProperty("companyQuantity")]
		public int CompanyQuantity { get; set; }
		[JsonProperty("eventQuantity")]
		public int EventQuantity { get; set; }
	}
}
