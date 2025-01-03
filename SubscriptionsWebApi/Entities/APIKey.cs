using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SubscriptionsWebApi.Enums;
using System.Security.Policy;

namespace SubscriptionsWebApi.Entities
{
  public class APIKey
  {
    public int Id { get; set; }
    public string Key { get; set; }
    public KeyType KeyType {get; set;}

    public bool IsActive { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public List<DomainRestriction> DomainRestrictions { get; set; }
    public List<IPRestriction> IPRestrictions { get; set; }
  }
}
