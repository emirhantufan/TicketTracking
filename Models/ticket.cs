using System.ComponentModel.DataAnnotations;

namespace TicketTracking.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        // Aşama 19: Başlık doğrulamaları
        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık maksimum 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;
        // Aşama 19: Açıklama doğrulamaları
        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(2000, ErrorMessage = "Açıklama maksimum 2000 karakter olabilir.")]
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
        public ICollection<TicketAttachment> TicketAttachments { get; set; } = new List<TicketAttachment>();

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