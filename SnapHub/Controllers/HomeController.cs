using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnapHub.Data;
using SnapHub.Models;
using System.Diagnostics;

namespace SnapHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var portfolio = _context.Portfolio.Find(1);

            if (portfolio == null)
            {
                return View();
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