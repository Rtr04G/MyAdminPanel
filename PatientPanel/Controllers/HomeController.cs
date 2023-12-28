using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using PatientPanel.Models;
using System.Web.WebPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.Eventing.Reader;

namespace PatientPanel.Controllers
{
    [Authorize(Roles = "Patient")]
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

        public IActionResult IndexAsync()
        {

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Recording()
        {
            var user = await _userManager.GetUserAsync(User);
            string PatId = user.Id.ToString();
            ViewData["PatId"] = PatId;
            List<AdminUser> users = _context.AdminUsers.AsQueryable().ToList();
            List<Category> categories = _context.Categories.AsQueryable().ToList();
            List<Specialization> specializations = _context.Specializations.AsQueryable().ToList();
            List<AdminUser> doctors = new List<AdminUser>();
            foreach (AdminUser us in users)
            {
                if (await _userManager.IsInRoleAsync(us, "Doctor"))
                {
                    if (_context.Records.AsQueryable().Where(r => r.PatientId == PatId && r.DoctorId == us.Id).FirstOrDefault() == null) {
                        doctors.Add(us);
                    }
                }
            }
            var model = (doctors, categories, specializations);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Recording(string DocId, string PatId)
        {
            ViewData["PatId"] = _userManager.GetUserAsync(User).Id;
            List<Record> records = _context.Records.AsQueryable().Where(r => r.DoctorId==DocId).ToList();
            DateTime date = DateTime.Now.AddHours(29-DateTime.Now.Hour);
            while(true)
            {
                if (_context.Records.AsQueryable().Where(r => r.DoctorId == DocId && r.DateX == date).FirstOrDefault() == null)
                {
                    break;
                }
                if (date.Hour > 20)
                {
                    date = date.AddHours(12);
                }
                else
                {
                    date = date.AddHours(1);
                }
            }
            Record newRec = new Record() { 
                    DoctorId= DocId,
                    PatientId= PatId,
                    DateX= date,
            };
            _context.Records.Add(newRec);
            await _context.SaveChangesAsync();
            return Redirect("Recording") ;
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