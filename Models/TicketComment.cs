namespace TicketTracking.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }
        public int CommentedByUserId { get; set; }
        public User? CommentedByUser { get; set; }
    }
}