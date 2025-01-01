using SubscriptionsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class AuthorCreationDTO
    {
        [StringLength(maximumLength: 50, ErrorMessage = "Sorry, the maximum lenght for the field {0} is {1}")]
        [FirstCapitalLetter]
        public string Name { get; set; }
    }
}
