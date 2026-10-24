using TicketTracking.Models;

namespace TicketTracking.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int TotalProjects { get; set; }

        // Grafik için tür sayıları
        public int BugCount { get; set; }
        public int RequestCount { get; set; }
        public int QuestionCount { get; set; }

        // Son eklenen talepler
        public List<Ticket> RecentTickets { get; set; } = new List<Ticket>();
    }
}