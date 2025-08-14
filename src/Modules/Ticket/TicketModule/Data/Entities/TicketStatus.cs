using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TicketModule.Data.Entities
{
  //[JsonConverter(typeof(StringEnumConverter))]
  public enum TicketStatus
  {
    Pending,
    Answered,
    Closed
  }
}
