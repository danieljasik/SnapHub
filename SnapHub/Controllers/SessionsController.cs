using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnapHub.Data;
using SnapHub.Models;

namespace SnapHub.Controllers
{
    public class SessionsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: Sessions
        [Authorize]
        public async Task<IActionResult> Index()
        {
              return _context.Session != null ? 
                          View(await _context.Session.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Session'  is null.");
        }

        public async Task<IActionResult> SelectSession()
        {
            return View();
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            /*
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }

            // Rozdziel identyfikator sesji i datę
            var parts = sessionCode.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0], out var sessionId) || !DateTime.TryParseExact(parts[1], "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sessionDate))
            {
                return RedirectToAction("Index");
            }
            */
            // Sprawdź, czy sesja istnieje w bazie danych
            var session = _context.Session.Find(id);

            if (session == null)
            {
                return RedirectToAction("Index");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", id.ToString());

            // Sprawdź, czy folder istnieje
            if (Directory.Exists(uploadsFolder))
            {
                // Pobierz listę plików w folderze
                var photoFiles = Directory.GetFiles(uploadsFolder).Select(Path.GetFileName).ToList();

                // Przekierowanie do widoku sesji wraz z listą plików
                var viewModel = new SessionViewModel
                {
                    Session = session,
                    PhotoFiles = photoFiles
                };

                return View(viewModel);
            }
            else
            {
                // Obsługa błędów, jeśli folder nie istnieje
                return RedirectToAction("Index");
            }
        }

        // GET: Sessions/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,CreatedDate,Photos")] Session session, List<IFormFile> photos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();

                var sessionId = session.Id;
                Console.WriteLine(sessionId);

                var sessionFolder = Path.Combine(_webHostEnvironment.WebRootPath,"uploads", sessionId.ToString());

                Console.WriteLine(sessionFolder);
                
                if (!Directory.Exists(sessionFolder))
                {
                    Directory.CreateDirectory(sessionFolder);
                }

                if (photos != null && photos.Count > 0)
                {
                    Console.WriteLine("Są zdjęcia");
                }
                else
                {
                    Console.WriteLine("Ni ma zdjęć");
                }

                foreach (var photoFile in photos)
                {
                    if (photoFile.Length > 0)
                    {
                        var photoFileName = Path.GetFileName(photoFile.FileName);
                        Console.WriteLine(photoFileName);
                        var photoFilePath = Path.Combine(sessionFolder, photoFileName);

                        Console.WriteLine(photoFilePath);

                        using (var fileStream = new FileStream(photoFilePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(fileStream);
                        }

                        // Tworzenie nowego obiektu Photo i przypisanie do sesji
                        var newPhoto = new Photo
                        {
                            Id = sessionId,
                            FileName = photoFileName,
                            SessionId = sessionId,
                            // Dodaj inne właściwości zdjęcia, które są wymagane
                        };

                        

                        // Dodawanie zdjęcia do bazy danych
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Sessions/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }

            var session = await _context.Session.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Data,CreatedDate")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // GET: Sessions/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }

            var session = await _context.Session
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Session == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Session'  is null.");
            }
            var session = await _context.Session.FindAsync(id);
            if (session != null)
            {
                _context.Session.Remove(session);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
          return (_context.Session?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
