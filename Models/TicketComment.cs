using System.ComponentModel.DataAnnotations;

namespace TicketTracking.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yorum içeriği boş bırakılamaz.")]
        public string CommentText { get; set; } = string.Empty;

        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Bilet İlişkisi
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        // Kullanıcı İlişkisi
        public int CommentedByUserId { get; set; }
        public User? CommentedByUser { get; set; }
    }
}