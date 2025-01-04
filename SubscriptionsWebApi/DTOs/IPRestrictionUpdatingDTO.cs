using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
  public class IPRestrictionUpdatingDTO
  {
    [Required]
    public string IP { get; set; }
  }
}
