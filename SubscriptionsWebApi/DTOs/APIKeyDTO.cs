namespace SubscriptionsWebApi.DTOs
{
  public class APIKeyDTO
  {
    public int Id { get; set; }
    public string Key { get; set; }
    public string KeyType { get; set; }
    public bool IsActive { get; set; }
  }
}
