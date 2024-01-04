using System;
using System.Collections.Generic;
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
    public class PortfoliosController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public PortfoliosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Portfolios
        [Authorize]
        public async Task<IActionResult> Index()
        {
              return _context.Portfolio != null ? 
                          View(await _context.Portfolio.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Portfolio'  is null.");
        }

        // GET: Portfolios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var portfolio = _context.Portfolio.Find(id);

            if (portfolio == null)
            {
                return RedirectToAction("Index");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "Portfolio");

            // Sprawdź, czy folder istnieje
            if (Directory.Exists(uploadsFolder))
            {
                // Pobierz listę plików w folderze
                var photoFiles = Directory.GetFiles(uploadsFolder).Select(Path.GetFileName).ToList();

                Console.WriteLine(photoFiles.Count);

                // Przekierowanie do widoku sesji wraz z listą plików
                var viewModel = new PortfolioViewModel
                {
                    Portfolio = portfolio,
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

        // GET: Portfolios/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Portfolios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Photos")] Portfolio portfolio, List<IFormFile> photos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(portfolio);
                await _context.SaveChangesAsync();

                var portfolioId = portfolio.Id;
                //Console.WriteLine(sessionId);

                var portfolioFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "Portfolio");

                //Console.WriteLine(sessionFolder);

                if (!Directory.Exists(portfolioFolder))
                {
                    Directory.CreateDirectory(portfolioFolder);
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
                        var photoFilePath = Path.Combine(portfolioFolder, photoFileName);

                        Console.WriteLine(photoFilePath);

                        using (var fileStream = new FileStream(photoFilePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(fileStream);
                        }

                        // Tworzenie nowego obiektu Photo i przypisanie do sesji
                        var newPhoto = new Photo
                        {
                            Id = portfolioId,
                            FileName = photoFileName,
                            SessionId = portfolioId,
                            // Dodaj inne właściwości zdjęcia, które są wymagane
                        };



                        // Dodawanie zdjęcia do bazy danych
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(portfolio);
        }

        // GET: Portfolios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            // Pobierz listę plików z folderu wwwroot/uploads/Portfolio
            var portfolioFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "Portfolio");

            var photoFiles = Directory.GetFiles(portfolioFolder)
                .Select(filePath => Path.GetFileName(filePath))
                .ToList();

            // Przekazanie listy plików do widoku
            ViewBag.PhotoFiles = photoFiles;

            // Pobierz pozostałe informacje o portfolio
            var portfolio = _context.Portfolio.Find(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // POST: Portfolios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Photos")] Portfolio portfolio, List<string> photoFileNames, List<IFormFile> photos)
        {
            var portfolioFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "Portfolio");


            if (id != portfolio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(portfolio);
                await _context.SaveChangesAsync();

                var portfolioId = portfolio.Id;
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
                        Console.WriteLine(photoFileName);
                        var photoFilePath = Path.Combine(portfolioFolder, photoFileName);

                        Console.WriteLine(photoFilePath);

                        using (var fileStream = new FileStream(photoFilePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(fileStream);
                        }

                        // Tworzenie nowego obiektu Photo i przypisanie do sesji
                        var newPhoto = new Photo
                        {
                            Id = portfolioId,
                            FileName = photoFileName,
                            SessionId = portfolioId,
                            // Dodaj inne właściwości zdjęcia, które są wymagane
                        };



                        // Dodawanie zdjęcia do bazy danych
                    }
                }
                if (photoFileNames != null && photoFileNames.Any())
                {
                    foreach (var photoFileName in photoFileNames)
                    {
                        var photoToDelete = portfolio.Photos.FirstOrDefault(p => p.FileName == photoFileName);

                        if (photoFileName != null)
                        {
                            portfolio.Photos.Remove(photoToDelete);
                            // Usuń plik z dysku
                            var filePath = Path.Combine(portfolioFolder, photoFileName);
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

            return View(portfolio);
        }

        // GET: Portfolios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Portfolio == null)
            {
                return NotFound();
            }

            var portfolio = await _context.Portfolio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (portfolio == null)
            {
                return NotFound();
            }

            return View(portfolio);
        }

        // POST: Portfolios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Portfolio == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Portfolio'  is null.");
            }
            var portfolio = await _context.Portfolio.FindAsync(id);
            if (portfolio != null)
            {
                _context.Portfolio.Remove(portfolio);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PortfolioExists(int id)
        {
          return (_context.Portfolio?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
