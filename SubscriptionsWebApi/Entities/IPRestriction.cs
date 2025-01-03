namespace SubscriptionsWebApi.Entities
{
  public class IPRestriction
  {
    public int Id { get; set; }
    public int APIKeyId { get; set; }
    public string IP {  get; set; }
    public APIKey APIKey { get; set; }
  }
}
