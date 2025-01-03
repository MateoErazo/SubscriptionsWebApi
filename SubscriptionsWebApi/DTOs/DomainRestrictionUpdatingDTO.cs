using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
  public class DomainRestrictionUpdatingDTO
  {
    [Required]
    public string Domain {  get; set; }
  }
}
