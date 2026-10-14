using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketTracking.Data;
using TicketTracking.Models;

namespace TicketTracking.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar bilet işlemlerine erişebilir
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. READ: Tüm biletleri ilişkili Proje ve Kullanıcı bilgileriyle listeleme
        // GET: /Ticket
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // 2. CREATE (Form): Proje seçim listesiyle boş form
        // GET: /Ticket/Create
        public IActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }

        // 2. CREATE (Kaydet): Giriş yapan kullanıcı ID'sini otomatik atar
        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
    // Oturum açan kullanıcının ID'sini alıyoruz
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(currentUserId))
            {
                ticket.CreatedByUserId = int.Parse(currentUserId);
                ticket.CreatedAt = DateTime.UtcNow;
            }

            // Formdan gelmeyen ilişkisel ve otomatik alanların validasyon hatalarını siliyoruz:
            ModelState.Remove(nameof(ticket.Project));
            ModelState.Remove(nameof(ticket.CreatedByUser));
            ModelState.Remove(nameof(ticket.CreatedByUserId));
            ModelState.Remove("TicketComments");
            ModelState.Remove("Comments");

             if (ModelState.IsValid)
            {
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Eğer bir alan hala hata veriyorsa formu dropdown verisiyle birlikte geri bas
            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }
        // 3. UPDATE: Düzenleme formu
        // GET: /Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // 3. UPDATE (Kaydet)
        // POST: /Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();

            ModelState.Remove(nameof(ticket.Project));
            ModelState.Remove(nameof(ticket.CreatedByUser));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Tickets.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tickets.Any(e => e.Id == ticket.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // 4. DELETE: Onay ekranı
        // GET: /Ticket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // 4. DELETE (Sil)
        // POST: /Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}