using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketTracking.Data;
using TicketTracking.Models;

namespace TicketTracking.Controllers
{
    [Authorize]
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
                TotalProjects = await _context.Projects.CountAsync(),
                TotalTickets = await _context.Tickets.CountAsync(),
                OpenTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Acik),
                ResolvedTickets = await _context.Tickets.CountAsync(t => t.Status == TicketStatus.Cozuldu),
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
    }
}