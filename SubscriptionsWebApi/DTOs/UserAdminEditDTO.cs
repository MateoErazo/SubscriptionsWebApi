using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class UserAdminEditDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
