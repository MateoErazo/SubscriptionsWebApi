using SubscriptionsWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionsWebApi.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
