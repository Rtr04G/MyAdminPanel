using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DoctorPanel.Models;
using System.Web.WebPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace DoctorPanel.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<AdminUser> _userManager;

        public HomeController(UserManager<AdminUser> userManager, IWebHostEnvironment environment, AppDbContext context, ILogger<HomeController> logger)
        {
            _environment = environment;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync(string sortOrderName, string sortOrderDate , string startDate, string endDate)
        {
            ViewData["Title"] = "Home";
            ViewData["SortOrderName"] = sortOrderName;
            ViewData["SortOrderDate"] = sortOrderDate;
            IQueryable <Record> records = _context.Records.AsQueryable();
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
            records = records.Where(r=>r.DoctorId==userId);
            List<AdminUser> usersDB = _context.AdminUsers.AsQueryable().ToList();
            List<RecordIndexViewModel> recordsOfThisUser = new List<RecordIndexViewModel>(); 
            foreach (var record in records)
            {
                RecordIndexViewModel myrecord = new RecordIndexViewModel();
                myrecord.SurName = usersDB.FirstOrDefault(u => u.Id == record.PatientId).SurName;
                myrecord.MiddleName = usersDB.FirstOrDefault(u => u.Id == record.PatientId).MiddleName;
                myrecord.Adderss = usersDB.FirstOrDefault(u => u.Id == record.PatientId).Address;
                myrecord.Date = record.DateX;
                myrecord.Id = record.PatientId;
                recordsOfThisUser.Add(myrecord);
            };

            if (startDate != null && endDate != null)
            {
                recordsOfThisUser = recordsOfThisUser.Where(d => d.Date >= DateTime.Parse(startDate) && d.Date <= DateTime.Parse(endDate)).ToList();
            }

            switch (sortOrderName)
            {
                case "asc":
                    recordsOfThisUser = recordsOfThisUser.OrderBy(r => r.SurName).ToList();
                    break;
                case "desc":
                    recordsOfThisUser = recordsOfThisUser.OrderByDescending(r => r.SurName).ToList();
                    break;

            }

            switch (sortOrderDate)
            {
                case "asc":
                    recordsOfThisUser = recordsOfThisUser.OrderBy(r => r.Date).ToList();
                    break;
                case "desc":
                    recordsOfThisUser = recordsOfThisUser.OrderByDescending(r => r.Date).ToList();
                    break;

            }

            return View(recordsOfThisUser);
        }

        [HttpGet]
        public IActionResult Patient(string Id, string searchStr)
        {
            searchStr = searchStr ?? "";
            ViewData["Search"] = searchStr;
            AdminUser patient = _context.AdminUsers.AsQueryable().Where(r => r.Id == Id).FirstOrDefault();
            List<Document> docs = _context.Documents.AsQueryable().Where(r => r.Title.Contains(searchStr)).ToList();
            List<PatientDocument>Pdoc = _context.PatientDocuments.AsQueryable().Where(p => p.PatientId==Id).ToList();
            var model = (patient, docs, Pdoc);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_Patient", model);
            }

            return View(model);
        }


        [HttpPost]
        public async Task<RedirectResult> Patient(int DocId, string PatId, string docName)
        {
            Document Doc = _context.Documents.AsQueryable().Where(r => r.Id == DocId).FirstOrDefault();
            AdminUser patient = _context.AdminUsers.AsQueryable().Where(r => r.Id == PatId).FirstOrDefault();
            string DPath = "C:\\FileCatalog\\Patients\\" + patient.SurName + " " + patient.Name + " " + patient.MiddleName + " " + PatId;
            string FPath2 = DPath + "\\" + docName + ".docx";
            if (Directory.Exists(DPath))
            {
                if(System.IO.File.Exists(FPath2))
                {
                    System.IO.File.Delete(FPath2);
                }
                System.IO.File.Copy(Doc.FilePath, FPath2,true);
            }
            else
            {
                Directory.CreateDirectory(DPath);
                System.IO.File.Copy(Doc.FilePath, FPath2,true);
            }
            PatientDocument pDocument = _context.PatientDocuments.AsQueryable().Where(p=>p.Title == docName).FirstOrDefault();
            if (pDocument != null)
            {
                _context.PatientDocuments.Remove(pDocument);
            }
            AdminUser user = await _userManager.GetUserAsync(HttpContext.User);
            PatientDocument pDoc = new PatientDocument
            {
                Title = docName,
                CreatedBy = user.Name +" "+user.MiddleName+" "+user.SurName,
                CreationDate = DateTime.Now,
                FilePath = FPath2,
                PatientId = PatId
            };
            _context.PatientDocuments.Add(pDoc);
            _context.SaveChanges();
            return Redirect("Patient?Id="+PatId+"&seatchString=''");
        }


        [HttpPost]
        public IActionResult ChangePatientFileName(int fileId, string newTitle, string PatId)
        {

            try
            {
                var document = _context.PatientDocuments.Find(fileId);

                if (document != null)
                {
                    string oldFilePath = document.FilePath;
                    document.FilePath = document.FilePath.Replace(document.Title, newTitle);
                    string newFilePath = document.FilePath;
                    document.Title = newTitle;
                    System.IO.File.Move(oldFilePath, newFilePath);
                    _context.SaveChanges();

                    return Redirect("Patient?Id=" + PatId + "&seatchString=''");
                }

                return Json(new { success = false, errorMessage = "Document not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var document = _context.PatientDocuments.Find(id);

            if (document == null)
            {
                return NotFound();
            }

            var fileStream = System.IO.File.OpenRead(document.FilePath);
            return File(fileStream, "application/octet-stream", document.Title + ".docx");
        }

        [HttpGet]
        public IActionResult Delete(int id, string PatId)
        {
            var document = _context.PatientDocuments.Find(id);

            if (document == null)
            {
                return NotFound();
            }

            System.IO.File.Delete(document.FilePath);
            _context.PatientDocuments.Remove(document);
            _context.SaveChanges();

            return Redirect("/Home/Patient?Id=" + PatId);
        }

        [HttpGet]
        public IActionResult Add(string PatId)
        {
            ViewData["id"] = PatId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(FileUploadModel fileModel, string PatId)
        {
            ViewData["id"] = PatId;
            if (fileModel.File != null && fileModel.File.Length > 0)
            {
                var patient = _context.AdminUsers.AsQueryable().FirstOrDefault(x => x.Id == PatId);

                string fPath = "C:\\FileCatalog\\Patients\\" + patient.SurName + " " + patient.Name + " " + patient.MiddleName + " " + PatId;
                var fileName = Path.GetFileName(fileModel.File.FileName);
                var filePath = Path.Combine(fPath, fileName);


                if (System.IO.File.Exists(filePath))
                {

                    System.IO.File.Delete(filePath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fileModel.File.CopyTo(stream);
                }

                AdminUser user = await _userManager.GetUserAsync(HttpContext.User);
                var newDocument = new PatientDocument
                {
                    Title = Path.GetFileNameWithoutExtension(fileName),
                    CreatedBy = user.Name + " " + user.MiddleName + " " + user.SurName,
                    CreationDate = DateTime.Now,
                    FilePath = filePath,
                    PatientId= patient.Id
                };

                var oldDocument = _context.PatientDocuments.AsQueryable().Where(d => d.Title == newDocument.Title).FirstOrDefault();
                if (oldDocument != null)
                {
                    _context.PatientDocuments.Remove(oldDocument);
                }

                _context.PatientDocuments.Add(newDocument);

                _context.SaveChanges();

                return RedirectToAction("Add");
            }

            ModelState.AddModelError("File", "Пожалуйста, выберите файл.");
            return View();
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