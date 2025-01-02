namespace SubscriptionsWebApi.DTOs
{
  public class UpdateAPIKeyDTO
  {
    public int KeyId { get; set; }
    public bool UpdateKey { get; set; }
    public bool IsActive { get; set; }
  }
}
