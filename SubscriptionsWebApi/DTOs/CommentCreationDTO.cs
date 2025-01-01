using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class CommentCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 2000)]
        public string Content { get; set; }
    }
}
