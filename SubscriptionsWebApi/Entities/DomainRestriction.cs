namespace SubscriptionsWebApi.Entities
{
  public class DomainRestriction
  {
    public int Id { get; set; }
    public int APIKeyId { get; set; }
    public string Domain { get; set; }
    public APIKey APIKey { get; set; }
  }
}
