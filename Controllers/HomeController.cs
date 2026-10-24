using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketTracking.Data;
using TicketTracking.Models;
using TicketTracking.ViewModels;
using DashboardViewModel = TicketTracking.ViewModels.DashboardViewModel; // Belirsizliği çözen satır

namespace TicketTracking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalTickets = await _context.Tickets.CountAsync(),
                OpenTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Acik),
                ClosedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Kapatildi),
                TotalProjects = await _context.Projects.CountAsync(),

                BugCount = await _context.Tickets.CountAsync(t => t.Type == "Hata"),
                RequestCount = await _context.Tickets.CountAsync(t => t.Type == "Talep"),
                QuestionCount = await _context.Tickets.CountAsync(t => t.Type == "Soru"),

                RecentTickets = await _context.Tickets
                    .Include(t => t.Project)
                    .Include(t => t.CreatedByUser)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}