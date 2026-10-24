using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketTracking.Data;
using TicketTracking.Models;

namespace TicketTracking.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. READ & FILTER: Biletleri listeleme ve filtreleme
        // GET: /Ticket
        public async Task<IActionResult> Index(string? type, string? priority, TicketStatus? status, int? projectId)
        {
            var query = _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .AsQueryable();

            // 1. Tür Filtresi (Hata, Talep, Soru)
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(t => t.Type == type);
            }

            // 2. Öncelik Filtresi (Dusuk, Orta, Yuksek)
            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(t => t.Priority == priority);
            }

            // 3. Durum Filtresi (Acik, Kapatildi)
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            // 4. Proje Filtresi
            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Seçimlerin sayfada seçili kalması ve proje listesinin yüklenmesi
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "Name", projectId);
            ViewBag.SelectedType = type;
            ViewBag.SelectedPriority = priority;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedProjectId = projectId;

            return View(tickets);
        }  
        // 2. DETAILS: Bilet detayı ve yorumlar
        // GET: /Ticket/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .Include(t => t.TicketComments)
                    .ThenInclude(c => c.CommentedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            ticket.TicketComments = ticket.TicketComments
                .OrderByDescending(c => c.CommentedAt)
                .ToList();

            return View(ticket);
        }

        // 3. CREATE (Form)
        // GET: /Ticket/Create
        public IActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }

        // 3. CREATE (Kaydet)
        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(currentUserId))
            {
                ticket.CreatedByUserId = int.Parse(currentUserId);
                ticket.CreatedAt = DateTime.UtcNow;
            }

            ModelState.Remove(nameof(ticket.Project));
            ModelState.Remove(nameof(ticket.CreatedByUser));
            ModelState.Remove(nameof(ticket.CreatedByUserId));
            ModelState.Remove(nameof(ticket.TicketComments));

            if (ModelState.IsValid)
            {
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // 4. UPDATE (Form)
        // GET: /Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return NotFound();

            // AŞAMA 18: Yetki Kontrolü (Yalnızca kendi biletini açabilir)
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (ticket.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok! Yalnızca kendi açtığınız talepleri düzenleyebilirsiniz.";
                return RedirectToAction(nameof(Index));
            }

            // AŞAMA 16: Kapatılmış bilet tekrar düzenlenemez
            if (ticket.Status == TicketStatus.Kapatildi)
            {
                TempData["ErrorMessage"] = "Kapatılmış bir talep tekrar düzenlenemez.";
                return RedirectToAction(nameof(Details), new { id = ticket.Id });
            }

            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // 4. UPDATE (Kaydet)
        // POST: /Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket model)
        {
            if (id != model.Id) return NotFound();

            var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTicket == null) return NotFound();

            // AŞAMA 18: POST seviyesinde yetki kontrolü
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (existingTicket.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Yetkisiz işlem! Yalnızca kendi açtığınız talepleri güncelleyebilirsiniz.";
                return RedirectToAction(nameof(Index));
            }

            // AŞAMA 16: Kapatılmış bilet güncellenemez
            if (existingTicket.Status == TicketStatus.Kapatildi)
            {
                TempData["ErrorMessage"] = "Kapatılmış bir talep güncellenemez.";
                return RedirectToAction(nameof(Details), new { id = existingTicket.Id });
            }

            ModelState.Remove(nameof(model.Project));
            ModelState.Remove(nameof(model.CreatedByUser));
            ModelState.Remove(nameof(model.TicketComments));

            if (ModelState.IsValid)
            {
                existingTicket.Title = model.Title;
                existingTicket.Description = model.Description;
                existingTicket.Priority = model.Priority;
                existingTicket.Type = model.Type;
                existingTicket.Status = model.Status;
                existingTicket.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = existingTicket.Id });
            }

            ViewBag.ProjectId = new SelectList(_context.Projects, "Id", "Name", model.ProjectId);
            return View(model);
        }

        // 5. CLOSE (Talebi Kapat)
        // POST: /Ticket/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();

            // AŞAMA 18: Yetki Kontrolü
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (ticket.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Yalnızca kendi açtığınız talepleri kapatabilirsiniz.";
                return RedirectToAction(nameof(Index));
            }

            ticket.Status = TicketStatus.Kapatildi;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = ticket.Id });
        }

        // 6. DELETE (Onay Ekranı)
        // GET: /Ticket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.Tickets
                .Include(t => t.Project)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            // AŞAMA 18: Yetki Kontrolü
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (ticket.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Yalnızca kendi açtığınız talepleri silebilirsiniz.";
                return RedirectToAction(nameof(Index));
            }

            return View(ticket);
        }

        // 6. DELETE (Sil)
        // POST: /Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                // AŞAMA 18: Yetki Kontrolü
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (ticket.CreatedByUserId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Yalnızca kendi açtığınız talepleri silebilirsiniz.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 7. COMMENT (Yorum Ekleme)
        // POST: /Ticket/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int ticketId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return Challenge();

            var comment = new TicketComment
            {
                TicketId = ticketId,
                CommentedByUserId = int.Parse(currentUserId),
                CommentText = commentText.Trim(),
                CommentedAt = DateTime.UtcNow
            };

            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }
    }
}