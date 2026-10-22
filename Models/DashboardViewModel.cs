namespace TicketTracking.Models
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public List<Ticket> RecentTickets { get; set; } = new List<Ticket>();
    }
}