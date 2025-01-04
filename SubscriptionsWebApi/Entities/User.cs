using Microsoft.AspNetCore.Identity;

namespace SubscriptionsWebApi.Entities
{
  public class User:IdentityUser
  {
    public bool NonPayingUser { get; set; }
  }
}
