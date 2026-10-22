namespace TicketTracking.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Hata";
        public string Priority { get; set; } = "Orta";
        
        // Durum alanı enum'ı kullanmaya devam eder:
        public TicketStatus Status { get; set; } = TicketStatus.Acik;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();

    } // <--- Ticket sınıfının BİTİŞ parantezi

    // Enum'ı sınıfın DIŞINA, doğrudan namespace altına koyuyoruz:
    public enum TicketStatus
    {
        Acik,
        Inceleniyor,
        Cozuldu,
        Kapatildi
    }
}