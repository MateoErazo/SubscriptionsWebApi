using SubscriptionsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class BookPatchDTO
    {
        [StringLength(50, ErrorMessage = "Sorry, the maximum lenght for the field {0} is {1}")]
        [FirstCapitalLetter]
        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }
    }
}
