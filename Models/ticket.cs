using Microsoft.Identity.Client;
using TicketTracking.Enums;

namespace TicketTracking.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ticketType Type { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Acik;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
   
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}