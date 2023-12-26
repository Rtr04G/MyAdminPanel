using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAdminPanel.Models;
using PagedList;
using System;
using System.Diagnostics;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;


namespace MyAdminPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment, AppDbContext context, ILogger<HomeController> logger)
        {
            _environment = environment;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string sortOrder, string searchString, DateTime? startDate, DateTime? endDate)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["AuthorSortParam"] = sortOrder == "author" ? "author_desc" : "author";

            ViewData["CurrentFilter"] = searchString;

            IQueryable<Document> documents = _context.Documents.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(d => d.CreatedBy.Contains(searchString) || d.Title.Contains(searchString));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                documents = documents.Where(d => d.CreationDate >= startDate && d.CreationDate <= endDate);
            }

            documents = sortOrder switch
            {
                "title_desc" => documents.OrderByDescending(d => d.Title),
                "date" => documents.OrderBy(d => d.CreationDate),
                "date_desc" => documents.OrderByDescending(d => d.CreationDate),
                "author" => documents.OrderBy(d => d.CreatedBy),
                "author_desc" => documents.OrderByDescending(d => d.CreatedBy),
                _ => documents.OrderBy(d => d.Title),
            };

            var viewModel = new DocumentIndexViewModel
            {
                Documents = documents.ToList(),
                CurrentFilter = searchString,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(FileUploadModel fileModel)
        {
            if (fileModel.File != null && fileModel.File.Length > 0)
            {

                var fileName = Path.GetFileName(fileModel.File.FileName);
                var filePath = Path.Combine("C:\\FileCatalog\\", fileName);


                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Delete(filePath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fileModel.File.CopyTo(stream);
                }


                var existingDocument = _context.Documents.SingleOrDefault(d => d.Title == Path.GetFileNameWithoutExtension(fileName));

                if (existingDocument != null)
                {

                    existingDocument.CreationDate = DateTime.Now;

                }
                else
                {

                    var newDocument = new Document
                    {
                        Title = Path.GetFileNameWithoutExtension(fileName),
                        CreatedBy = "Пользователь",
                        CreationDate = DateTime.Now,
                        FilePath = filePath
                    };

                    _context.Documents.Add(newDocument);
                }

                _context.SaveChanges();

                return RedirectToAction("Add");
            }

            ModelState.AddModelError("File", "Пожалуйста, выберите файл.");
            return View();
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var document = _context.Documents.Find(id);

            if (document == null)
            {
                return NotFound();
            }

            var fileStream = System.IO.File.OpenRead(document.FilePath);
            return File(fileStream, "application/octet-stream", document.Title + ".docx");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var document = _context.Documents.Find(id);

            if (document == null)
            {
                return NotFound();
            }

            System.IO.File.Delete(document.FilePath);
            _context.Documents.Remove(document);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult ChangeFileName(int fileId, string newTitle)
        {
            try
            {
                var document = _context.Documents.Find(fileId);

                if (document != null)
                {
                    string oldFilePath = document.FilePath;
                    document.FilePath = document.FilePath.Replace(document.Title, newTitle);
                    string newFilePath = document.FilePath;
                    document.Title = newTitle;
                    System.IO.File.Move(oldFilePath, newFilePath);
                    _context.SaveChanges();

                    return Redirect("Index");
                }

                return Json(new { success = false, errorMessage = "Document not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}