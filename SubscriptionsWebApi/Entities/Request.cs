using System.Security.Policy;

namespace SubscriptionsWebApi.Entities
{
  public class Request
  {
    public int Id { get; set; }
    public int APIKeyId { get; set; }
    public DateTime RequestDate { get; set; }
    public APIKey APIKey { get; set; }
  }
}
