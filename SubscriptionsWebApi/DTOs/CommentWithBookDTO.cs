namespace SubscriptionsWebApi.DTOs
{
    public class CommentWithBookDTO: CommentDTO
    {
        public BookDTO Book { get; set; }
    }
}
