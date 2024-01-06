using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnapHub.Data;
using SnapHub.Data.Migrations;
using SnapHub.Models;
using static NuGet.Packaging.PackagingConstants;

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

            var session = _context.Session.Find(id);

            // Pobierz listę plików z folderu wwwroot/uploads/Portfolio
            var sessionFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", id.ToString());

            var photoFiles = Directory.GetFiles(sessionFolder)
                .Select(filePath => Path.GetFileName(filePath))
                .ToList();

            // Przekazanie listy plików do widoku
            ViewBag.PhotoFiles = photoFiles;

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Data,CreatedDate,Photos")] Session session, List<string> photoFileNames, List<IFormFile> photos)
        {
            var sessionFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", id.ToString());

            if (id != session.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(session);
                await _context.SaveChangesAsync();

                var sessionId = session.Id;
                //Console.WriteLine(sessionId);

                //Console.WriteLine(sessionFolder);

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
                        //Console.WriteLine(photoFileName);
                        var photoFilePath = Path.Combine(sessionFolder, photoFileName);

                        //Console.WriteLine(photoFilePath);

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
                if (photoFileNames != null && photoFileNames.Any())
                {
                    foreach (var photoFileName in photoFileNames)
                    {
                        var photoToDelete = session.Photos.FirstOrDefault(p => p.FileName == photoFileName);

                        if (photoFileName != null)
                        {
                            session.Photos.Remove(photoToDelete);
                            // Usuń plik z dysku
                            var filePath = Path.Combine(sessionFolder, photoFileName);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();

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
            var sessionFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", id.ToString());

            if (_context.Session == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Session'  is null.");
            }
            var session = await _context.Session.FindAsync(id);
            if (session != null)
            {
                _context.Session.Remove(session);
            }

            string[] files = Directory.GetFiles(sessionFolder);
            foreach (string file in files)
            {
                System.IO.File.SetAttributes(file, FileAttributes.Normal);
                System.IO.File.Delete(file);
            }

            if (System.IO.Directory.Exists(sessionFolder))
            {
                System.IO.Directory.Delete(sessionFolder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DownloadAllPhotos(int id)
        {
            // Logika pobierania wszystkich zdjęć
            Console.WriteLine(id.ToString());
            // Przykładowy kod: Pobierz wszystkie pliki z folderu
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", id.ToString());
            var photoFiles = Directory.GetFiles(folderPath);

            // Tworzenie archiwum ZIP
            var zipPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "Zdjęcia z sesji.zip");
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }
            
            ZipFile.CreateFromDirectory(folderPath, zipPath);

            // Pobierz plik ZIP
            return PhysicalFile(zipPath, "application/zip", "Zdjęcia z sesji.zip");
        }

        private bool SessionExists(int id)
        {
          return (_context.Session?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
