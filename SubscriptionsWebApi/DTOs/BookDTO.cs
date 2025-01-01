using SubscriptionsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
