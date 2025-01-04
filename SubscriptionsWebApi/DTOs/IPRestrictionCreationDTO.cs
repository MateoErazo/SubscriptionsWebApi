using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
  public class IPRestrictionCreationDTO
  {
    [Required]
    public int APIKeyId { get; set; }
    [Required]
    public string IP { get; set; }
  }
}
