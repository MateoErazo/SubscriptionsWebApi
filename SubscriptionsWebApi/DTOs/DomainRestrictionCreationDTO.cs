using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace SubscriptionsWebApi.DTOs
{
  public class DomainRestrictionCreationDTO
  {
    [Required]
    public int APIKeyId { get; set; }
    [Required]
    public string Domain {  get; set; }
  }
}
