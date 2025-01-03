namespace SubscriptionsWebApi.DTOs
{
  public class LimitRequestsConfigurationDTO
  {
    public int FreeRequestsPerDay { get; set; }
    public string[] WhitelistRoutes { get; set; }
  }
}
