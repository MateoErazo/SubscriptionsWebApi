using SubscriptionsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class AuthorDTO: Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
